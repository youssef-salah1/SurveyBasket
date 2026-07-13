using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using SurveyBasket.Core.Contracts.Answer;
using SurveyBasket.Core.Contracts.Commen;
using SurveyBasket.Core.Contracts.Question;
using System.Linq.Dynamic.Core;

namespace SurveyBasket.Services.Services;

public class QuestionService(
    ApplicationDbContext context,
    ILogger<QuestionService> logger,
    HybridCache hybridCache) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<QuestionService> _logger = logger;
    private readonly HybridCache _hybridCache = hybridCache;

    private const string _cachePrefix = "availableQuestions";

    public async Task<Result<PaginatedList<QuestionResponse>>> GetAllAsync(int pollId, RequestFilter requestFilter, CancellationToken cancellationToken = default)
    {
        var isPollExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

        if (isPollExists is false)
            return Result.Failure<PaginatedList<QuestionResponse>>(PollErrors.PollNotFound);


        var source = _context.Questions
            .Where(p => p.PollId == pollId && (string.IsNullOrEmpty(requestFilter.SearchValue) || p.Content.Contains(requestFilter.SearchValue)));

        if (!string.IsNullOrEmpty(requestFilter.SortColumn))
            source = source.OrderBy($"{requestFilter.SortColumn} {requestFilter.SortDirection}");

        var query = source.Include(q => q.Answers)
                        .Select(q => new QuestionResponse(
                            q.Id,
                            q.Content,
                            q.Answers.Select(a => new AnswerResponse(a.Id, a.Content))
                        ))
                        .AsNoTracking();


        var questions = await PaginatedList<QuestionResponse>.CreateAsync(query, requestFilter.PageNumber, requestFilter.PageSize, cancellationToken);

        return Result.Success(questions);
    }

    public async Task<Result<QuestionResponse>> GetAsync(int pollId, int questionId,
        CancellationToken cancellationToken = default)
    {
        var question = await _context.Questions
            .Where(q => q.PollId == pollId && q.Id == questionId)
            .Include(q => q.Answers)
            .ProjectToType<QuestionResponse>()
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        return question is null
            ? Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound)
            : Result.Success(question);
    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Cache Questions {Key}", pollId);

        var hasVoted = await _context.Votes.AnyAsync(v => v.PollId == pollId && v.UserId == userId, cancellationToken);

        if (hasVoted)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.UserAlreadyVoted);

        var isValid = await _context.Polls.AnyAsync(v =>
            v.Id == pollId && v.IsPublished && v.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)
            && v.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow));

        if (!isValid)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        string cacheKey = $"{_cachePrefix}-{pollId}";

        var questions = await _hybridCache.GetOrCreateAsync<IEnumerable<QuestionResponse>>(
            cacheKey,
            async cacheEntry => await _context.Questions
                .Where(v => v.PollId == pollId && v.IsActive)
                .Include(v => v.Answers)
                .Select(v => new QuestionResponse(
                    v.Id,
                    v.Content,
                    v.Answers.Where(x => x.IsActive).Select(x => new AnswerResponse(x.Id, x.Content)
                    ))
                )
                .AsNoTracking()
                .ToListAsync(cancellationToken)
        );

        return Result.Success(questions);
    }

    public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest questionRequest,
        CancellationToken cancellationToken = default)
    {
        var isPollExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

        if (isPollExists is false)
            return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

        var isQuestionExists = await _context.Questions
            .AnyAsync(q => q.Content == questionRequest.Content && q.PollId == pollId, cancellationToken);

        if (isQuestionExists)
            return Result.Failure<QuestionResponse>(QuestionErrors.QuestionDuplicated);

        var question = questionRequest.Adapt<Question>();

        question.PollId = pollId;

        await _context.Questions.AddAsync(question, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        string cacheKey = $"{_cachePrefix}-{pollId}";
        await _hybridCache.RemoveAsync(cacheKey, cancellationToken);
        _logger.LogInformation("Remove Cache Questions {Key}", pollId);

        return Result.Success(question.Adapt<QuestionResponse>());
    }

    public async Task<Result> UpdateAsync(int pollId, int questionId, QuestionRequest questionRequest,
        CancellationToken cancellationToken = default)
    {
        var isQuestionExists = await _context.Questions
            .AnyAsync(q => q.Id == questionId && q.PollId == pollId, cancellationToken);

        if (isQuestionExists is false)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        var isDuplicated = await _context.Questions
            .AnyAsync(q => q.Content == questionRequest.Content && q.Id != questionId && q.PollId == pollId);

        if (isDuplicated)
            return Result.Failure(QuestionErrors.QuestionDuplicated);

        var question = await _context.Questions
            .Include(q => q.Answers)
            .SingleOrDefaultAsync(q => q.Id == questionId && q.PollId == pollId, cancellationToken);

        question!.Content = questionRequest.Content;

        var currentAnswers = question.Answers.Select(a => a.Content).ToList();

        var newAnswers = questionRequest.Answers.Except(currentAnswers).ToList();

        foreach (var answer in newAnswers)
            question.Answers.Add(new Answer { Content = answer, QuestionId = questionId });

        foreach (var answer in question.Answers) answer.IsActive = questionRequest.Answers.Contains(answer.Content);

        await _context.SaveChangesAsync(cancellationToken);

        string cacheKey = $"{_cachePrefix}-{pollId}";
        await _hybridCache.RemoveAsync(cacheKey, cancellationToken);
        _logger.LogInformation("Remove Cache Questions {Key}", pollId);

        return Result.Success();
    }

    public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
    {
        var question =
            await _context.Questions.SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);

        question.IsActive = !question.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        string cacheKey = $"{_cachePrefix}-{pollId}";
        await _hybridCache.RemoveAsync(cacheKey, cancellationToken);

        return Result.Success();
    }
}
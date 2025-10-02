using Mapster;
using SurveyBasket.Core.Abstractions;
using SurveyBasket.Core.Contracts.Answer;
using SurveyBasket.Core.Contracts.Question;

namespace SurveyBasket.Services.Services;

public class QuestionService(ApplicationDbContext context) : IQuestionService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId,
        CancellationToken cancellationToken = default)
    {
        var isPollExists = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

        if (isPollExists is false)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var questions = await _context.Questions
            .Where(p => p.PollId == pollId)
            .Include(q => q.Answers)
            .Select(q => new QuestionResponse(
                q.Id,
                q.Content,
                q.Answers.Select(a => new AnswerResponse(a.Id, a.Content))
            ))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<QuestionResponse>>(questions);
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
        var hasVoted = await _context.Votes.AnyAsync(v => v.PollId == pollId && v.UserId == userId, cancellationToken);

        if (hasVoted)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.UserAlreadyVoted);

        var isValid = await _context.Polls.AnyAsync(v =>
            v.Id == pollId && v.IsPublished && v.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)
            && v.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow));

        if (!isValid)
            return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

        var questions = await _context.Questions
            .Where(v => v.PollId == pollId && v.IsActive)
            .Include(v => v.Answers)
            .Select(v => new QuestionResponse(
                v.Id,
                v.Content,
                v.Answers.Where(x => x.IsActive).Select(x => new AnswerResponse(x.Id, x.Content)
                ))
            )
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<QuestionResponse>>(questions);
    }

    public async Task<Result<QuestionResponse>> Add(int pollId, QuestionRequest questionRequest,
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

        return Result.Success(question.Adapt<QuestionResponse>());
    }

    public async Task<Result> Update(int pollId, int questionId, QuestionRequest questionRequest,
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

        return Result.Success();
    }
}
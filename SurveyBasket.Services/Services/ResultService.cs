using SurveyBasket.Core.Abstractions;
using SurveyBasket.Core.Contracts.Results;

namespace SurveyBasket.Services.Services;

public class ResultService(ApplicationDbContext context) : IResultService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<PollVotesResponse>> GetPollVoteAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var result = await _context.Polls
            .Where(p => p.Id == pollId)
            .Select(p => new PollVotesResponse(
                p.Title,
                p.StartsAt,
                p.EndsAt,
                p.Votes
                .Select(v => new VoteResponse(
                    $"{v.User.FirstName} {v.User.LastName}",
                    v.SubmittedIn,
                    v.Answers.Select(a => new QuestionAnswerResponse(
                        a.Question.Content,
                        a.Answer.Content
                    ))
                ))
            )).SingleOrDefaultAsync(cancellationToken);

        if (result is null)
            return Result.Failure<PollVotesResponse>(PollErrors.PollNotFound);

        return Result.Success(result);
    }

    public async Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default)
    {
        bool isExist = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

        if (!isExist)
            return Result.Failure<IEnumerable<VotesPerDayResponse>>(PollErrors.PollNotFound);

        var result = await _context.Votes
            .Where(p => p.PollId == pollId)
            .GroupBy(p => new { Date = DateOnly.FromDateTime(p.SubmittedIn) })
            .Select(v => new VotesPerDayResponse(
                v.Key.Date,
                v.Count()
            )).ToListAsync(cancellationToken);

        if (result is null)
            return Result.Failure<IEnumerable<VotesPerDayResponse>>(PollErrors.PollNotFound);

        return Result.Success<IEnumerable<VotesPerDayResponse>>(result);
    }

    public async Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestions(int pollId, CancellationToken cancellationToken = default)
    {
        var isExist = await _context.Polls.AnyAsync(p => p.Id == pollId, cancellationToken);

        if (!isExist)
            return Result.Failure<IEnumerable<VotesPerQuestionResponse>>(PollErrors.PollNotFound);

        var result = await _context.VoteAnswers
            .Where(a => a.Question.PollId == pollId)
            .Select(v => new VotesPerQuestionResponse(
                v.Question.Content,
                v.Question.VoteAnswers
                .GroupBy(x => new { AnswerId = x.Answer.Id, Contant = x.Answer.Content })
                .Select(g => new VotesPerAnswerResponse(
                    g.Key.Contant,
                    g.Count()
                ))
            )).ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<VotesPerQuestionResponse>>(result);
    }
}

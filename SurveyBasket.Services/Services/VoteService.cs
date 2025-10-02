using Mapster;
using SurveyBasket.Core.Abstractions;
using SurveyBasket.Core.Contracts.Vote;

namespace SurveyBasket.Services.Services;

public class VoteService(ApplicationDbContext context) : IVoteService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result> Add(int pollId, string userId, VoteRequest voteRequest,
        CancellationToken cancellationToken = default)
    {
        var hasVoted = await _context.Votes.AnyAsync(v => v.PollId == pollId && v.UserId == userId, cancellationToken);

        if (hasVoted)
            return Result.Failure(PollErrors.UserAlreadyVoted);

        if (!await _context.Polls.AnyAsync(v => v.Id == pollId && v.IsPublished &&
                                                v.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)
                                                && v.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow)))
            return Result.Failure(PollErrors.PollNotFound);

        var availableQuestions = await _context.Questions
            .Where(p => p.PollId == pollId && p.IsActive)
            .Include(p => p.Answers)
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        if (!voteRequest.Answers.Select(p => p.QuestionId).SequenceEqual(availableQuestions))
            return Result.Failure(VoteErrors.InvalidQuestions);

        var vote = new Vote
        {
            PollId = pollId,
            UserId = userId,
            Answers = voteRequest.Answers.Adapt<IEnumerable<VoteAnswer>>().ToList()
        };

        await _context.AddAsync(vote, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
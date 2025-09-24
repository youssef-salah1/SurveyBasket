using Mapster;
using SurveyBasket.Core.Abstractions;
using SurveyBasket.Core.Contracts.Polls;
using SurveyBasket.Core.Errors;

namespace SurveyBasket.Services.Services;

public class PollService(ApplicationDbContext context) : IPollService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var polls = await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);
        return polls.Adapt<IEnumerable<PollResponse>>();
    }

    public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken)
    {
        var poll = await _context.Polls.FindAsync(id, cancellationToken);
        return poll is null
            ? Result.Failure<PollResponse>(PollErrors.PollNotFound)
            : Result.Success(poll.Adapt<PollResponse>());
    }

    public async Task<PollResponse> AddAsync(PollRequest poll, CancellationToken cancellationToken)
    {
        var newPoll = await _context.AddAsync(poll.Adapt<Poll>(), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return newPoll.Entity.Adapt<PollResponse>();
    }

    public async Task<Result> UpdateAsync(int id, PollRequest poll, CancellationToken cancellationToken = default)
    {
        var updatedPoll = await _context.Polls.FindAsync(id, cancellationToken);

        if (updatedPoll is null || updatedPoll.Id != id)
            return Result.Failure(PollErrors.PollNotFound);

        updatedPoll.Title = poll.Title;
        updatedPoll.Summary = poll.Summary;
        updatedPoll.StartsAt = poll.StartsAt;
        updatedPoll.EndsAt = poll.EndsAt;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync(id, cancellationToken);

        if (poll is null)
            return Result.Failure(PollErrors.PollNotFound);

        _context.Remove(poll);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _context.Polls.FindAsync(id, cancellationToken);

        if (poll is null)
            return Result.Failure(PollErrors.PollNotFound);

        poll.IsPublished = !poll.IsPublished;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
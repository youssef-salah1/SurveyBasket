using Mapster;
using SurveyBasket.Core.Abstractions;
using SurveyBasket.Core.Contracts.Polls;

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

    public async Task<Result<PollResponse>> AddAsync(PollRequest poll, CancellationToken cancellationToken)
    {
        var isFound = await _context.Polls.AnyAsync(p => p.Title == poll.Title, cancellationToken);

        if (isFound)
            return Result.Failure<PollResponse>(PollErrors.PollTitleAlreadyExists);

        var newPoll = await _context.AddAsync(poll.Adapt<Poll>(), cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return newPoll.Entity.Adapt<Result<PollResponse>>();
    }

    public async Task<Result> UpdateAsync(int id, PollRequest poll, CancellationToken cancellationToken = default)
    {
        var updatedPoll = await _context.Polls.FindAsync(id, cancellationToken);

        if (updatedPoll is null)
            return Result.Failure(PollErrors.PollNotFound);

        var isFound = await _context.Polls.AnyAsync(p => p.Title == poll.Title && p.Id != id, cancellationToken);

        if (isFound)
            return Result.Failure<PollResponse>(PollErrors.PollTitleAlreadyExists);

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
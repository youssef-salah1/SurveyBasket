using Hangfire;
using Mapster;
using SurveyBasket.Core.Abstractions;
using SurveyBasket.Core.Contracts.Polls;

namespace SurveyBasket.Services.Services;

public class PollService(ApplicationDbContext context,
    INotificationService notificationService) : IPollService
{
    private readonly ApplicationDbContext _context = context;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var polls = await _context.Polls.AsNoTracking().ProjectToType<PollResponse>().ToListAsync(cancellationToken);
        return polls;
    }

    public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken)
    {
        var poll = await _context.Polls.FindAsync(id, cancellationToken);
        return poll is null
            ? Result.Failure<PollResponse>(PollErrors.PollNotFound)
            : Result.Success(poll.Adapt<PollResponse>());
    }

    public async Task<Result<IEnumerable<PollResponse>>> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        var result = await _context.Polls
            .Where(p => p.IsPublished && p.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)
                                      && p.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow))
            .ProjectToType<PollResponse>()
            .ToListAsync(cancellationToken);
        return Result.Success<IEnumerable<PollResponse>>(result);
    }

    public async Task<Result<PollResponse>> AddAsync(PollRequest poll, CancellationToken cancellationToken)
    {
        var isFound = await _context.Polls.AnyAsync(p => p.Title == poll.Title, cancellationToken);

        if (isFound)
            return Result.Failure<PollResponse>(PollErrors.PollTitleAlreadyExists);

        var newPoll = await _context.AddAsync(poll.Adapt<Poll>(), cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(newPoll.Entity.Adapt<PollResponse>());
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
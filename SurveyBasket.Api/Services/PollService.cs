using SurveyBasket.Api.Persistence;

namespace SurveyBasket.Api.Services;

public class PollService(ApplicationDbContext context) : IPollService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Poll> AddAsync(Poll poll, CancellationToken cancellationToken)
    {
        await _context.AddAsync(poll, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return poll;
    }

    public async Task<IEnumerable<Poll>> GetAllAsync(CancellationToken cancellationToken)
        => await _context.Polls.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<Poll?> GetAsync(int id, CancellationToken cancellationToken)
        => await _context.Polls.FindAsync(id, cancellationToken);

    public async Task<bool> UpdateAsync(int id, Poll poll, CancellationToken cancellationToken = default)
    {
        var findPoll = await GetAsync(id, cancellationToken);
        if (findPoll is null || findPoll.Id != id)
            return false;
        findPoll.Title = poll.Title;
        findPoll.Summary = poll.Summary;
        //findPoll.IsPublished = poll.IsPublished;
        findPoll.StartsAt = poll.StartsAt;
        findPoll.EndsAt = poll.EndsAt;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await GetAsync(id, cancellationToken);
        if (poll is null)
            return false;
        _context.Polls.Remove(poll);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> TogglePublishStatusAsync([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var poll = await GetAsync(id, cancellationToken);
        if (poll is null)
            return false;
        poll.IsPublished = !poll.IsPublished;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

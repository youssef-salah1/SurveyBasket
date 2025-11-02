using SurveyBasket.Core.Abstractions;
using SurveyBasket.Core.Contracts.Commen;
using SurveyBasket.Core.Contracts.Polls;

namespace SurveyBasket.Core.Service;

public interface IPollService
{
    Task<PaginatedList<PollResponse>> GetAllAsync(RequestFilter filter, CancellationToken cancellationToken = default);
    Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<PollResponse>>> GetCurrentAsync(RequestFilter filter, CancellationToken cancellationToken = default);
    Task<Result<PollResponse>> AddAsync(PollRequest poll, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(int id, PollRequest poll, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default);
}
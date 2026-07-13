using SurveyBasket.Core.Abstractions;
using SurveyBasket.Core.Contracts.Roles;

namespace SurveyBasket.Core.Service;

public interface IRoleSurvice
{
    Task<IEnumerable<RoleResponse>> GetAllAsyn(bool? includeDisabled = false, CancellationToken cancellationToken = default);
    Task<Result<RoleDetailsResponse>> GetAsync(string Id, CancellationToken cancellationToken = default);
    Task<Result<RoleDetailsResponse>> AddAsync(RoleRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(string Id, RoleRequest request, CancellationToken cancellationToken = default);
    Task<Result> ToggleStatusAsync(string id);
}

using SurveyBasket.Core.Abstractions;
using SurveyBasket.Core.Contracts.Users;

namespace SurveyBasket.Core.Service;

public interface IUserService
{
    Task<Result<UserProfileResponse>> GetProfileAsync(string userId);
    Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request);
    Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
}
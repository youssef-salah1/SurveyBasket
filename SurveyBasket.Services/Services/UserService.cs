using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using SurveyBasket.Core.Abstractions;
using SurveyBasket.Core.Abstractions.Consts;
using SurveyBasket.Core.Contracts.Roles;
using SurveyBasket.Core.Contracts.Users;
using SurveyBasket.Core.Entities;
using System.Data;
using System.Linq;

namespace SurveyBasket.Services.Services;

public class UserService(
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext context,
    IRoleSurvice roleSurvice,
    ILogger<UserService> logger) : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ApplicationDbContext _context = context;
    private readonly IRoleSurvice _roleSurvice = roleSurvice;
    private readonly ILogger<UserService> _logger = logger;

    public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    => await (from u in _context.Users
              join ur in _context.UserRoles
              on u.Id equals ur.UserId
              join r in _context.Roles
              on ur.RoleId equals r.Id into roles
              where !roles.Any(x => x.Name == DefaultRoles.Member)
              select new
              {
                  u.Id,
                  u.UserName,
                  u.Email,
                  u.FirstName,
                  u.LastName,
                  u.IsDisable,
                  Roles = roles.Select(x => x.Name!).ToList()
              })
               .GroupBy(u => new
               {
                   u.Id,
                   u.UserName,
                   u.Email,
                   u.FirstName,
                   u.LastName,
                   u.IsDisable
               })
                .Select(u => new UserResponse(

                   u.Key.Id,
                   u.Key.UserName,
                   u.Key.Email,
                   u.Key.FirstName,
                   u.Key.LastName,
                   u.Key.IsDisable,
                   u.SelectMany(x => x.Roles)
               )).ToListAsync(cancellationToken);
    public async Task<Result<UserResponse>> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByIdAsync(id) is not { } user)
            return Result.Failure<UserResponse>(UserErrors.UserNotFound);

        var roles = await _userManager.GetRolesAsync(user);

        var response = (user, roles).Adapt<UserResponse>();

        return Result.Success(response);
    }
    public async Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (await _userManager.Users.AnyAsync(u => u.NormalizedUserName == request.UserName.ToUpper(), cancellationToken))
            return Result.Failure<UserResponse>(UserErrors.DuplicatedUserName);

        if (await _userManager.Users.AnyAsync(u => u.NormalizedEmail == request.Email.Trim().ToUpper(), cancellationToken))
            return Result.Failure<UserResponse>(UserErrors.DuplicatedEmail);

        var validRoles = await _roleSurvice.GetAllAsyn(cancellationToken: cancellationToken);

        if (request.Roles.Except(validRoles.Select(r => r.Name)).Any())
            return Result.Failure<UserResponse>(UserErrors.InvalidRoles);

        var user = request.Adapt<ApplicationUser>();

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRolesAsync(user, request.Roles);

            var response = (user, request.Roles).Adapt<UserResponse>();

            _logger.LogInformation("User with Username : {Username} created succefily", user.UserName);

            return Result.Success(response);
        }

        _logger.LogInformation("User with Username : {Username} faild to create", user.UserName);

        var error = result.Errors.First();

        return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    public async Task<Result> UpdateAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByIdAsync(id) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);

        if (await _userManager.Users.AnyAsync(u => u.NormalizedEmail == request.Email.ToUpper() && u.Id != id, cancellationToken))
            return Result.Failure(UserErrors.DuplicatedEmail);

        if (await _userManager.Users.AnyAsync(u => u.NormalizedUserName == request.UserName.ToUpper() && u.Id != id, cancellationToken))
            return Result.Failure(UserErrors.DuplicatedUserName);

        var validRoles = await _roleSurvice.GetAllAsyn(cancellationToken: cancellationToken);

        if (request.Roles.Except(validRoles.Select(r => r.Name)).Any())
            return Result.Failure<UserResponse>(UserErrors.InvalidRoles);

        user = request.Adapt(user);

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            await _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .ExecuteDeleteAsync(cancellationToken);

            await _userManager.AddToRolesAsync(user, request.Roles);
            return Result.Success();
        }

        var error = result.Errors.First();

        return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    public async Task<Result> ToggleStatusAsync(string id)
    {
        if (await _userManager.FindByIdAsync(id) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);

        user.IsDisable = !user.IsDisable;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }
    public async Task<Result> UnlockAsync(string id)
    {
        if (await _userManager.FindByIdAsync(id) is not { } user)
            return Result.Failure(UserErrors.UserNotFound);

        var result = await _userManager.SetLockoutEndDateAsync(user , null);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }

    #region Profile Survices
    public async Task<Result<UserProfileResponse>> GetProfileAsync(string userId)
    {
        var user = await _userManager.Users.
            Where(u => u.Id == userId)
            .ProjectToType<UserProfileResponse>()
            .SingleAsync();

        return Result.Success(user);
    }
    public async Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request)
    {
        await _userManager.Users
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(setters =>
                setters
                .SetProperty(x => x.FirstName, request.FirstName)
                .SetProperty(x => x.LastName, request.LastName)
             );

        return Result.Success();
    }
    public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);

        var result = await _userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    #endregion
}

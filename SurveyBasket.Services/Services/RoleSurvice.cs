using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SurveyBasket.Core.Abstractions;
using SurveyBasket.Core.Abstractions.Consts;
using SurveyBasket.Core.Contracts.Roles;

namespace SurveyBasket.Services.Services;

public class RoleSurvice(RoleManager<ApplicationRole> roleManager,
    ApplicationDbContext context) : IRoleSurvice
{
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<RoleResponse>> GetAllAsyn(bool? includeDisabled = false, CancellationToken cancellationToken = default)
    {
        var roles = await _roleManager.Roles
            .Where(r => !r.IsDefualt && (!r.IsDeleted || (includeDisabled.HasValue && includeDisabled.Value)))
            .ProjectToType<RoleResponse>()
            .ToListAsync(cancellationToken);

        return roles;
    }

    public async Task<Result<RoleDetailsResponse>> GetAsync(string Id, CancellationToken cancellationToken = default)
    {
        if (await _roleManager.FindByIdAsync(Id) is not { } role)
            return Result.Failure<RoleDetailsResponse>(RoleErrors.RoleNotFound);

        var permissions = await _roleManager.GetClaimsAsync(role);

        var response = new RoleDetailsResponse(Id, role.Name!, role.IsDeleted, permissions.Select(x => x.Value));

        return Result.Success(response);
    }

    public async Task<Result<RoleDetailsResponse>> AddAsync(RoleRequest request , CancellationToken cancellationToken = default)
    {
        if (await _roleManager.RoleExistsAsync(request.Name))
            return Result.Failure<RoleDetailsResponse>(RoleErrors.DuplicateRole);

        var validPermissions = Permissions.GetAll();

        if(request.permissions.Except(validPermissions).Any())
            return Result.Failure<RoleDetailsResponse>(RoleErrors.InvalidPermissions);

        var role = new ApplicationRole
        {
            Name = request.Name,
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };

        var result = await _roleManager.CreateAsync(role);

        if (result.Succeeded)
        {
            var claims = request.permissions
                .Select(x => new IdentityRoleClaim<string>
                {
                    RoleId = role.Id,
                    ClaimType = Permissions.Type,
                    ClaimValue = x
                });

            await _context.AddRangeAsync(claims , cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(new RoleDetailsResponse(role.Id, role.Name, role.IsDeleted, request.permissions));
        }

        var error = result.Errors.First();

        return Result.Failure<RoleDetailsResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> UpdateAsync(string Id , RoleRequest request, CancellationToken cancellationToken = default)
    {
        if (await _context.Roles.AnyAsync(r => r.Id != Id && request.Name == r.Name , cancellationToken))
            return Result.Failure(RoleErrors.DuplicateRole);

        if (await _roleManager.FindByIdAsync(Id) is not { } role)
            return Result.Failure(RoleErrors.RoleNotFound);

        var validPermissions = Permissions.GetAll();

        if (request.permissions.Except(validPermissions).Any())
            return Result.Failure<RoleDetailsResponse>(RoleErrors.InvalidPermissions);

        var result = await _roleManager.SetRoleNameAsync(role , request.Name);

        if (result.Succeeded)
        {
            var permissionsName = await _context.RoleClaims
                .Where(rc => rc.RoleId == role.Id && rc.ClaimType == Permissions.Type)
                .Select(rc => rc.ClaimValue)
                .ToListAsync(cancellationToken);

            var newPermissions = request.permissions.Except(permissionsName)
                .Select(x => new IdentityRoleClaim<string>
                {
                    RoleId = role.Id,
                    ClaimType = Permissions.Type,
                    ClaimValue = x
                }).ToList();

            var deletedpermissions = permissionsName.Except(request.permissions);

            await _context.RoleClaims
                          .Where(rc => rc.RoleId == role.Id && deletedpermissions.Contains(rc.ClaimValue))
                          .ExecuteDeleteAsync(cancellationToken);

            await _context.RoleClaims.AddRangeAsync(newPermissions, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ToggleStatusAsync(string id)
    {
        if (await _roleManager.FindByIdAsync(id) is not { } role)
            return Result.Failure(RoleErrors.RoleNotFound);

        role.IsDeleted = !role.IsDeleted;

        var result = await _roleManager.UpdateAsync(role);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
}

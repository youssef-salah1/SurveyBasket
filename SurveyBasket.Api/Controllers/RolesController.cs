using SurveyBasket.Core.Abstractions;
using SurveyBasket.Core.Abstractions.Consts;
using SurveyBasket.Core.Authentication.Filters;
using SurveyBasket.Core.Contracts.Roles;

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RolesController(IRoleSurvice roleSurvice) : ControllerBase
{
    private readonly IRoleSurvice _roleSurvice = roleSurvice;

    [HttpGet("")]
    [HasPermission(Permissions.GetRoles)]
    public async Task<IActionResult> GetAll([FromQuery] bool includeDisabled, CancellationToken cancellationToken)
    {
        var roles = await _roleSurvice.GetAllAsyn(includeDisabled , cancellationToken);
        return Ok(roles);
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.GetRoles)]
    public async Task<IActionResult> Get([FromRoute] string id , CancellationToken cancellationToken)
    {
        var result = await _roleSurvice.GetAsync(id, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPost()]
    [HasPermission(Permissions.AddRoles)]
    public async Task<IActionResult> Add([FromBody] RoleRequest roleRequest , CancellationToken cancellationToken)
    {
        var result = await _roleSurvice.AddAsync(roleRequest, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { result.Value.Id }, result.Value)
            : result.ToProblem();
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdateRoles)]
    public async Task<IActionResult> Update([FromRoute] string id , RoleRequest roleRequest, CancellationToken cancellationToken)
    {
        var result = await _roleSurvice.UpdateAsync(id, roleRequest, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpPut("{id}/toggle-status")]
    [HasPermission(Permissions.UpdateRoles)]
    public async Task<IActionResult> ToggleStatus(string id , CancellationToken cancellationToken){
        var result = await _roleSurvice.ToggleStatusAsync(id);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}

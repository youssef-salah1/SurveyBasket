using SurveyBasket.Core.Abstractions;
using SurveyBasket.Core.Abstractions.Consts;
using SurveyBasket.Core.Authentication.Filters;
using SurveyBasket.Core.Contracts.Polls;

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;

    [HttpGet("")]
    [HasPermission(Permissions.GetPolls)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await _pollService.GetAllAsync(cancellationToken));
    }

    [HttpGet("{id}")]
    [HasPermission(Permissions.GetPolls)]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.GetAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpGet("current")]
    [Authorize(Roles = DefaultRoles.Member)]
    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken)
    {
        var result = await _pollService.GetCurrentAsync(cancellationToken);
        return Ok(result.Value);
    }

    [HttpPost("")]
    [HasPermission(Permissions.AddPolls)]
    public async Task<IActionResult> Add([FromBody] PollRequest pollRequest,
        CancellationToken cancellationToken)
    {
        var result = await _pollService.AddAsync(pollRequest, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdatePolls)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest pollRequest,
        CancellationToken cancellationToken)
    {
        var result = await _pollService.UpdateAsync(id, pollRequest, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.DeletePolls)]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.DeleteAsync(id, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }

    [HttpPut("{id}/togglePublish")]
    [HasPermission(Permissions.UpdatePolls)]
    public async Task<IActionResult> TogglePublishStatusAsync([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.TogglePublishStatusAsync(id, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
    }
}
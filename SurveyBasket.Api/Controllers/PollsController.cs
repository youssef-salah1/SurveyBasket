using SurveyBasket.Core.Abstractions;
using SurveyBasket.Core.Contracts.Polls;
using SurveyBasket.Core.Errors;

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PollsController(IPollService pollService) : ControllerBase
{
    private readonly IPollService _pollService = pollService;

    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _pollService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.GetAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem(StatusCodes.Status404NotFound);
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] PollRequest pollRequest,
        CancellationToken cancellationToken)
    {
        var result = await _pollService.AddAsync(pollRequest, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem(StatusCodes.Status409Conflict);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest pollRequest,
        CancellationToken cancellationToken)
    {
        var result = await _pollService.UpdateAsync(id, pollRequest, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem(result.Error.Code == PollErrors.PollTitleAlreadyExists.Code
                ? StatusCodes.Status409Conflict
                : StatusCodes.Status404NotFound);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.DeleteAsync(id, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem(StatusCodes.Status404NotFound);
    }

    [HttpPut("{id}/togglePublish")]
    public async Task<IActionResult> TogglePublishStatusAsync([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.TogglePublishStatusAsync(id, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem(StatusCodes.Status404NotFound);
    }
}
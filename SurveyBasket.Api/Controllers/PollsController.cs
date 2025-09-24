using SurveyBasket.Core.Contracts.Polls;

namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PollsController(IMapper mapper, IPollService pollService) : ControllerBase
{
    private readonly IMapper _mapper = mapper;
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
            : Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code,
                detail: result.Error.Message);
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] PollRequest pollRequest,
        CancellationToken cancellationToken)
    {
        var result = await _pollService.AddAsync(pollRequest, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest pollRequest,
        CancellationToken cancellationToken)
    {
        var result = await _pollService.UpdateAsync(id, pollRequest, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code,
                detail: result.Error.Message);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.DeleteAsync(id, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code,
                detail: result.Error.Message);
    }

    [HttpPut("{id}/togglePublish")]
    public async Task<IActionResult> TogglePublishStatusAsync([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _pollService.TogglePublishStatusAsync(id, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code,
                detail: result.Error.Message);
    }
}
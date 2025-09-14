namespace SurveyBasket.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PollsController(IMapper mapper, IPollService pollService) : ControllerBase
{
    private readonly IMapper _mapper = mapper;
    private readonly IPollService _pollService = pollService;

    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var polls = await _pollService.GetAllAsync(cancellationToken);
        return Ok(polls.Adapt<IEnumerable<PollResponse>>());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int id, CancellationToken cancellationToken)
    {
        var poll = await _pollService.GetAsync(id, cancellationToken);
        if (poll is null)
            return NotFound();
        return Ok(poll.Adapt<PollResponse>());
    }
    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] PollRequest pollRequest,
        CancellationToken cancellationToken)
    {
        var poll = _mapper.Map<Poll>(pollRequest);
        var createdPoll = await _pollService.AddAsync(poll, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = createdPoll.Id }, createdPoll.Adapt<PollResponse>());
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PollRequest pollRequest,
        CancellationToken cancellationToken)
    {
        var isUpdated = await _pollService.UpdateAsync(id, pollRequest.Adapt<Poll>(), cancellationToken);
        return isUpdated ? NoContent() : NotFound();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var isDeleted = await _pollService.DeleteAsync(id, cancellationToken);
        return isDeleted ? NoContent() : NotFound();
    }
    [HttpPut("{id}/togglePublish")]
    public async Task<IActionResult> TogglePublishStatusAsync([FromRoute] int id, CancellationToken cancellationToken)
    {
        bool isToggled = await _pollService.TogglePublishStatusAsync(id, cancellationToken);
        return isToggled ? NoContent() : NotFound();
    }
}

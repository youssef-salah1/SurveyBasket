using SurveyBasket.Core.Abstractions;
using SurveyBasket.Core.Contracts.Question;
using SurveyBasket.Core.Errors;

namespace SurveyBasket.Api.Controllers;

[Route("api/polls/{pollId}/[controller]")]
[ApiController]
[Authorize]
public class QuestionController(IQuestionService questionService) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;

    [HttpGet("")]
    public async Task<IActionResult> GetAll([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var result = await _questionService.GetAllAsync(pollId, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem(StatusCodes.Status404NotFound);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute] int pollId, [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var result = await _questionService.GetAsync(pollId, id, cancellationToken);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem(StatusCodes.Status404NotFound);
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromRoute] int pollId, [FromBody] QuestionRequest questionRequest,
        CancellationToken cancellationToken)
    {
        var result = await _questionService.Add(pollId, questionRequest, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { pollId, id = result.Value.Id }, result.Value)
            : result.ToProblem(result.Error.Equals(QuestionErrors.QuestionDuplicated)
                ? StatusCodes.Status409Conflict
                : StatusCodes.Status404NotFound
            );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int pollId, [FromRoute] int id,
        [FromBody] QuestionRequest questionRequest, CancellationToken cancellationToken)
    {
        var result = await _questionService.Update(pollId, id, questionRequest, cancellationToken);

        return result.IsSuccess
            ? NoContent()
            : result.ToProblem(result.Error.Equals(QuestionErrors.QuestionDuplicated)
                ? StatusCodes.Status409Conflict
                : StatusCodes.Status404NotFound
            );
    }

    [HttpPut("{id}/toggleStatus")]
    public async Task<IActionResult> ToggleStatus([FromRoute] int pollId, [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        var result = await _questionService.ToggleStatusAsync(pollId, id, cancellationToken);

        return result.IsSuccess ? NoContent() : result.ToProblem(StatusCodes.Status404NotFound);
    }
}
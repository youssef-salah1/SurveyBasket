using SurveyBasket.Core.Abstractions;
using SurveyBasket.Core.Contracts.Vote;
using SurveyBasket.Extensions;

namespace SurveyBasket.Api.Controllers;

[Route("api/polls/{pollId}/vote")]
[ApiController]
public class VotesController(IQuestionService questionService, IVoteService voteService) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;
    private readonly IVoteService _voteService = voteService;

    [HttpGet("current")]
    public async Task<IActionResult> Start([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        var result = await _questionService.GetAvailableAsync(pollId, User.GetUserId()!, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromRoute] int pollId, VoteRequest voteRequest,
        CancellationToken cancellationToken)
    {
        var result = await _voteService.Add(pollId, User.GetUserId()!, voteRequest, cancellationToken);

        return result.IsSuccess
            ? Created()
            : result.ToProblem();
    }
}
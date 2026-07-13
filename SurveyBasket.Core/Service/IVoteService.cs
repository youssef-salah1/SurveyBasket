using SurveyBasket.Core.Abstractions;
using SurveyBasket.Core.Contracts.Vote;

namespace SurveyBasket.Core.Service;

public interface IVoteService
{
    Task<Result> Add(int pollId, string userId, VoteRequest voteRequest, CancellationToken cancellationToken = default);
}
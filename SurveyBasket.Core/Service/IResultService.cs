using SurveyBasket.Core.Abstractions;
using SurveyBasket.Core.Contracts.Results;

namespace SurveyBasket.Core.Service;

public interface IResultService
{
    Task<Result<PollVotesResponse>> GetPollVoteAsync(int pollId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayAsync(int pollId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestions(int pollId, CancellationToken cancellationToken = default);
}

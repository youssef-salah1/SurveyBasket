using SurveyBasket.Core.Abstractions;
using SurveyBasket.Core.Contracts.Question;

namespace SurveyBasket.Core.Service;

public interface IQuestionService
{
    public Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId,
        CancellationToken cancellationToken = default);

    public Task<Result<QuestionResponse>> GetAsync(int pollId, int questionId,
        CancellationToken cancellationToken = default);

    public Task<Result<QuestionResponse>> Add(int pollId, QuestionRequest questionRequest,
        CancellationToken cancellationToken = default);

    public Task<Result> Update(int pollId, int questionId, QuestionRequest questionRequest,
        CancellationToken cancellationToken = default);

    public Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default);
}
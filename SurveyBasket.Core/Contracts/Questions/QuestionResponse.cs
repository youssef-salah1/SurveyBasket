using SurveyBasket.Core.Contracts.Answer;

namespace SurveyBasket.Core.Contracts.Question;

public record QuestionResponse(int Id, string Content, IEnumerable<AnswerResponse> Answers);
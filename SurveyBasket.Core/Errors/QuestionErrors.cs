using Microsoft.AspNetCore.Http;
using SurveyBasket.Core.Abstractions;

namespace SurveyBasket.Core.Errors;

public static class QuestionErrors
{
    public static readonly Error QuestionDuplicated =
        new("Question.Duplicated", "The question already exists in this poll.", StatusCodes.Status409Conflict);

    public static readonly Error QuestionNotFound =
        new("Question.NotFound", "The question was not found.", StatusCodes.Status404NotFound);
}
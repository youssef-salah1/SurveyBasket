using SurveyBasket.Core.Abstractions;

namespace SurveyBasket.Core.Errors;

public static class PollErrors
{
    public static Error PollNotFound => new("Poll.NotFound", "The specified poll was not found.");
}
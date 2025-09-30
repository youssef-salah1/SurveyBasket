using SurveyBasket.Core.Abstractions;

namespace SurveyBasket.Core.Errors;

public static class PollErrors
{
    public static readonly Error PollNotFound = new("Poll.NotFound", "The specified poll was not found.");

    public static readonly Error PollTitleAlreadyExists =
        new("Poll.TitleAlreadyExists", "The poll title already exists.");
}
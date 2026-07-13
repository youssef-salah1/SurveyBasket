using Microsoft.AspNetCore.Http;
using SurveyBasket.Core.Abstractions;

namespace SurveyBasket.Core.Errors;

public static class PollErrors
{
    public static readonly Error PollNotFound =
        new("Poll.NotFound", "The specified poll was not found.", StatusCodes.Status404NotFound);

    public static readonly Error PollTitleAlreadyExists = new("Poll.TitleAlreadyExists",
        "The poll title already exists.", StatusCodes.Status409Conflict);

    public static readonly Error UserAlreadyVoted = new("Poll.UserAlreadyVoted",
        "The user has already voted in this poll.", StatusCodes.Status409Conflict);
}
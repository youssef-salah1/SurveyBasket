using Microsoft.AspNetCore.Http;
using SurveyBasket.Core.Abstractions;

namespace SurveyBasket.Core.Errors;

public static class VoteErrors
{
    public static readonly Error InvalidQuestions =
        new("Vote.InvalidQuestions", "Invalid questions", StatusCodes.Status400BadRequest);

    public static readonly Error DuplicatedVote =
        new("Vote.DuplicatedVote", "This user already voted before for this poll", StatusCodes.Status409Conflict);
}
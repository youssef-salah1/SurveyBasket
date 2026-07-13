using SurveyBasket.Core.Contracts.Users;

namespace SurveyBasket.Api.Validation;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(r => r.UserName)
          .NotEmpty()
          .Length(3, 20)
          .Matches(RegexPatterns.UserName)
          .WithMessage("Username can only contain letters, digits, and these characters: - . _ @ +");

        RuleFor(x => x.Email)
       .Cascade(CascadeMode.Stop)
       .NotEmpty()
       .EmailAddress()
       .Must(e => e.Replace(" ", "") == e)
       .WithMessage("Invalid email address");

        RuleFor(r => r.FirstName)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(r => r.LastName)
            .NotEmpty()
            .Length(3, 100);

        RuleFor(r => r.Roles)
            .NotNull()
            .NotEmpty();

        RuleFor(r => r.Roles)
            .Must(x => x.Distinct().Count() == x.Count())
            .WithMessage("Roles must be distinct")
            .When(x => x.Roles is not null);
    }
}

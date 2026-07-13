using SurveyBasket.Core.Contracts.Users;

namespace SurveyBasket.Api.Validation;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
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


        RuleFor(r => r.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(RegexPatterns.Password)
            .WithMessage("Password must be at least 8 characters and include at least one lowercase letter, one uppercase letter, one digit, and one special character.");

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

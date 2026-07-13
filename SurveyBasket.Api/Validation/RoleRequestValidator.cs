using SurveyBasket.Core.Contracts.Roles;

namespace SurveyBasket.Api.Validation;

public class RoleRequestValidator : AbstractValidator<RoleRequest>
{
    public RoleRequestValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .Length(3, 200);

        RuleFor(r => r.permissions)
            .NotNull()
            .NotEmpty();

        RuleFor(r => r.permissions)
            .Must(x => x.Distinct().Count() == x.Count())
            .WithMessage("Permissions must be distinct")
            .When(x => x.permissions is not null);
    }
}
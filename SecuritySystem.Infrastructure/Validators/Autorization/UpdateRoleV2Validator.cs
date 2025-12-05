using FluentValidation;
using SecuritySystem.Core.QueryFilters.Autorization;

namespace SecuritySystem.Infrastructure.Validators.Autorization
{
    public class UpdateRoleV2Validator : AbstractValidator<RoleQueryFilter>
    {
        public UpdateRoleV2Validator()
        {
            RuleFor(role => role.Name)
                .NotEmpty()
                .WithMessage("The name is required.")
                .DependentRules(() =>
                {
                    RuleFor(role => role.Name)
                        .MaximumLength(50)
                        .WithMessage("The name must not exceed 50 characters.");
                });

            RuleFor(role => role.Description)
                .NotEmpty()
                .WithMessage("The description is required.")
                .DependentRules(() =>
                {
                    RuleFor(role => role.Description)
                        .MaximumLength(100)
                        .WithMessage("The description must not exceed 100 characters.");
                });
        }
    }
}

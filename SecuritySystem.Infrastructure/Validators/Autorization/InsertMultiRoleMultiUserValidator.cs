using FluentValidation;
using SecuritySystem.Core.QueryFilters.Autorization;

namespace SecuritySystem.Infrastructure.Validators.Autorization
{
    public class InsertMultiRoleMultiUserValidator : AbstractValidator<InsertUserRoleQueryFilter>
    {
        public InsertMultiRoleMultiUserValidator()
        {
            RuleFor(userRole => userRole.UserId)
                .NotEmpty()
                .WithMessage("The user id is required.");

            RuleFor(userRole => userRole.Roles)
                .NotEmpty()
                .WithMessage("Roles are required.")
                .DependentRules(() =>
                {
                    RuleForEach(x => x.Roles)
                        .SetValidator(new IdMultiUserRoleValidator());
                });
        }
    }
}

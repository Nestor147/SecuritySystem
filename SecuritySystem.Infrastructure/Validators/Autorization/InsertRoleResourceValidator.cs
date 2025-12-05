using FluentValidation;
using SecuritySystem.Core.QueryFilters.Autorization;

namespace SecuritySystem.Infrastructure.Validators.Autorization
{
    public class InsertRoleResourceValidator : AbstractValidator<InsertRoleResourceQueryFilter>
    {
        public InsertRoleResourceValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty()
                .WithMessage("The role id is required.");

            RuleFor(x => x.ResourceId)
                .NotEmpty()
                .WithMessage("The resource id is required.");
        }
    }
}

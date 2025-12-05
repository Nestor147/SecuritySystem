using FluentValidation;
using SecuritySystem.Core.QueryFilters.Autorization;

namespace SecuritySystem.Infrastructure.Validators.Autorization
{
    public class InsertRoleMenuV2Validator : AbstractValidator<InsertRoleResourceQueryFilter>
    {
        public InsertRoleMenuV2Validator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty()
                .WithMessage("The role id is required.");

            RuleFor(x => x.MenuId)
                .NotEmpty()
                .WithMessage("The menu id is required.");
        }
    }
}

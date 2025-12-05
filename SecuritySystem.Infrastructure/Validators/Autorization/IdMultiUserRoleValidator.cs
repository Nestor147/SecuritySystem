using FluentValidation;
using SecuritySystem.Core.QueryFilters.Autorization;

namespace SecuritySystem.Infrastructure.Validators.Autorization
{
    public class IdMultiUserRoleValidator : AbstractValidator<SelectedRole>
    {
        public IdMultiUserRoleValidator()
        {
            RuleFor(role => role.RoleId)
                .NotEmpty()
                .WithMessage("The role id is required.");

            RuleFor(role => role.IsSelected)
                .Must(x => x == true || x == false)
                .WithMessage("The selected state is required.");
        }
    }
}

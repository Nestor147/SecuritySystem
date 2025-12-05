using FluentValidation;
using SecuritySystem.Core.GetQueryFilter.Autorization;

namespace SecuritySystem.Infrastructure.Validators.Autorization
{
    public class GetUserRoleMenuV2Validator : AbstractValidator<GetMenuQueryFilter>
    {
        public GetUserRoleMenuV2Validator()
        {
            RuleFor(menu => menu.RoleId)
                .NotEmpty()
                .WithMessage("The role id is required.");

            RuleFor(menu => menu.UserId)
                .NotEmpty()
                .WithMessage("The user id is required.");
        }
    }
}

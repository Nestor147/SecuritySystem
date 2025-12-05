using FluentValidation;
using SecuritySystem.Core.GetQueryFilter.Autorization;

namespace SecuritySystem.Infrastructure.Validators.Autorization
{
    public class GetMenuByRoleV2Validator : AbstractValidator<GetMenuQueryFilter>
    {
        public GetMenuByRoleV2Validator()
        {
            RuleFor(menu => menu.RoleId)
                .NotEmpty()
                .WithMessage("The role id is required.");
        }
    }
}

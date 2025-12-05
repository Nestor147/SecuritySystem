using FluentValidation;
using SecuritySystem.Core.GetQueryFilter.Autorization;

namespace SecuritySystem.Infrastructure.Validators.Autorization
{
    public class GetUsersByRoleV2Validator : AbstractValidator<GetUsersByRoleQueryFilter>
    {
        public GetUsersByRoleV2Validator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty()
                .WithMessage("The role id is required.");
        }
    }
}

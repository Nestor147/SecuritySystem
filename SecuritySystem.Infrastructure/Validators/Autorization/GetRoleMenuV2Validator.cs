using FluentValidation;
using SecuritySystem.Core.GetQueryFilter.Autorization;

namespace SecuritySystem.Infrastructure.Validators.Autorization
{
    public class GetRoleMenuV2Validator : AbstractValidator<GetRoleMenuQueryFilter>
    {
        public GetRoleMenuV2Validator()
        {
            RuleFor(menu => menu.ApplicationId)
                .NotEmpty()
                .WithMessage("The application id is required.");
        }
    }
}

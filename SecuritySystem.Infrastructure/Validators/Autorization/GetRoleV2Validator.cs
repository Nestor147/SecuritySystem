using FluentValidation;
using SecuritySystem.Core.GetQueryFilter.Autorization;

namespace SecuritySystem.Infrastructure.Validators.Autorization
{
    public class GetRoleV2Validator : AbstractValidator<GetRoleQueryFilter>
    {
        public GetRoleV2Validator()
        {
            RuleFor(role => role.ApplicationId)
                .NotEmpty()
                .WithMessage("The application id is required.");
        }
    }
}

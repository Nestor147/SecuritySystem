using FluentValidation;
using SecuritySystem.Core.GetQueryFilter.Autorization;

namespace SecuritySystem.Infrastructure.Validators.Autorization
{
    public class GetRoleByUserV2Validator : AbstractValidator<GetUsersByRoleQueryFilter>
    {
        public GetRoleByUserV2Validator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("The user id is required.");
        }
    }
}

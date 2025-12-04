using FluentValidation;
using SecuritySystem.Core.GetQueryFilter.Autorization;

namespace SecuritySystem.Infrastructure.Validators.Autorization
{
    public class GetResourceV2Validator : AbstractValidator<GetResourcesQueryFilter>
    {
        public GetResourceV2Validator()
        {
            RuleFor(r => r.ApplicationId)
                .NotEmpty()
                .WithMessage("The application id is required.");
        }
    }
}

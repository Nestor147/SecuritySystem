using FluentValidation;
using SecuritySystem.Core.QueryFilters.Autorization;

namespace SecuritySystem.Infrastructure.Validators.Autorization
{
    public class UpdateApplicationV2Validator : AbstractValidator<ApplicationQueryFilter>
    {
        public UpdateApplicationV2Validator()
        {
            RuleFor(app => app.Id)
                .NotEmpty()
                .WithMessage("The id is required.");

            RuleFor(app => app.Code)
                .NotEmpty()
                .WithMessage("The code is required.")
                .DependentRules(() =>
                {
                    RuleFor(app => app.Code)
                        .MaximumLength(25)
                        .WithMessage("The code must not exceed 25 characters.");
                });

            RuleFor(app => app.Description)
                .NotEmpty()
                .WithMessage("The name is required.")
                .DependentRules(() =>
                {
                    RuleFor(app => app.Description)
                        .MaximumLength(250)
                        .WithMessage("The name must not exceed 250 characters.");
                });
        }
    }
}

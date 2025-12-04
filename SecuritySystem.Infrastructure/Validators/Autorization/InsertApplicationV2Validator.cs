using FluentValidation;
using SecuritySystem.Core.QueryFilters.Autorization;

namespace SecuritySystem.Infrastructure.Validators.Autorization
{
    public class InsertApplicationV2Validator : AbstractValidator<ApplicationQueryFilter>
    {
        public InsertApplicationV2Validator()
        {
            RuleFor(app => app.Code)
                .NotEmpty()
                .WithMessage("The code is required.")
                .DependentRules(() =>
                {
                    RuleFor(app => app.Code)
                        .MaximumLength(25)
                        .WithMessage("The code must not exceed 25 characters.");
                });

            RuleFor(app => app.Name)
                .NotEmpty()
                .WithMessage("The name is required.")
                .DependentRules(() =>
                {
                    RuleFor(app => app.Name)
                        .MaximumLength(250)
                        .WithMessage("The name must not exceed 250 characters.");
                });

            RuleFor(app => app.Url)
                .NotEmpty()
                .WithMessage("The URL is required.")
                .DependentRules(() =>
                {
                    RuleFor(app => app.Url)
                        .MaximumLength(250)
                        .WithMessage("The URL must not exceed 250 characters.");
                });

            RuleFor(app => app.Icon)
                .NotEmpty()
                .WithMessage("The icon value is required.")
                .DependentRules(() =>
                {
                    RuleFor(app => app.Icon)
                        .MaximumLength(50)
                        .WithMessage("The icon value must not exceed 50 characters.");
                });
        }
    }
}

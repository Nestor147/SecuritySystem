using FluentValidation;
using SecuritySystem.Core.QueryFilters.Autorization;

namespace SecuritySystem.Infrastructure.Validators.Autorization
{
    public class UpdatePageV2Validator : AbstractValidator<ResourceQueryFilter>
    {
        public UpdatePageV2Validator()
        {
            RuleFor(resource => resource.Id)
                .NotEmpty()
                .WithMessage("The resource id is required.");

            RuleFor(resource => resource.ApplicationId)
                .NotNull()
                .WithMessage("The application id is required.");

            RuleFor(resource => resource.Page)
                .NotEmpty()
                .WithMessage("The page name is required.")
                .DependentRules(() =>
                {
                    RuleFor(resource => resource.Page)
                        .MaximumLength(100)
                        .WithMessage("The page name must not exceed 100 characters.");
                });

            RuleFor(resource => resource.Description)
                .NotEmpty()
                .WithMessage("The description is required.")
                .DependentRules(() =>
                {
                    RuleFor(resource => resource.Description)
                        .MaximumLength(100)
                        .WithMessage("The description must not exceed 100 characters.");
                });

            RuleFor(resource => resource.Detail)
                .NotEmpty()
                .WithMessage("The detail is required.")
                .DependentRules(() =>
                {
                    RuleFor(resource => resource.Detail)
                        .MaximumLength(350)
                        .WithMessage("The detail must not exceed 350 characters.");
                });

            RuleFor(resource => resource.IconName)
                .NotEmpty()
                .WithMessage("The icon name is required.")
                .DependentRules(() =>
                {
                    RuleFor(resource => resource.IconName)
                        .MaximumLength(100)
                        .WithMessage("The icon name must not exceed 100 characters.");
                });

            RuleFor(resource => resource.IsNew)
                .NotNull()
                .WithMessage("The 'is new' value is required.");
        }
    }
}

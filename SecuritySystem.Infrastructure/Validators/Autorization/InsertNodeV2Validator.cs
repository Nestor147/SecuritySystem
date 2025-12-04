using FluentValidation;
using SecuritySystem.Core.QueryFilters.Autorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Infrastructure.Validators.Autorization
{
    public class InsertNodeV2Validator : AbstractValidator<ResourceQueryFilter>
    {
        public InsertNodeV2Validator()
        {
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

            RuleFor(resource => resource.ApplicationId)
                .NotNull()
                .WithMessage("The application id is required.");
        }
    }
}

using FluentValidation;
using SecuritySystem.Core.QueryFilters.Autorization;

namespace SecuritySystem.Infrastructure.Validators.Autorization
{
    public class InsertMenuV2NodeValidator : AbstractValidator<ContentMenuV2QueryFilter>
    {
        public InsertMenuV2NodeValidator()
        {
            RuleFor(menu => menu.ResourceId)
                .NotEmpty()
                .WithMessage("The resource id is required.");

            RuleFor(menu => menu.SubLinks)
                .NotNull()
                .WithMessage("The sublinks collection is required.")
                .DependentRules(() =>
                {
                    RuleForEach(menu => menu.SubLinks)
                        .SetValidator(this);
                });
        }
    }
}

using FluentValidation;
using SecuritySystem.Core.QueryFilters.Autorization;

namespace SecuritySystem.Infrastructure.Validators.Security
{
    public class InsertMenuV2NodeValidator : AbstractValidator<MenuV2ItemQueryFilter>
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

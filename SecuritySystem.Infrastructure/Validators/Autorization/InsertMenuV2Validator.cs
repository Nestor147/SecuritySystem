using FluentValidation;
using SecuritySystem.Core.QueryFilters.Autorization;

namespace SecuritySystem.Infrastructure.Validators.Autorization
{
    public class InsertMenuV2Validator : AbstractValidator<MenuV2QueryFilter>
    {
        public InsertMenuV2Validator()
        {
            RuleFor(menu => menu.Menu)
                .NotEmpty()
                .WithMessage("The menu is required.")
                .DependentRules(() =>
                {
                    RuleForEach(menu => menu.Menu)
                        .SetValidator(new InsertMenuV2NodeValidator());
                });

            RuleFor(menu => menu.RegisteredByUser)
                .NotEmpty()
                .WithMessage("The registered user is required.");

            RuleFor(menu => menu.ApplicationId)
                .NotEmpty()
                .WithMessage("The application id is required.");
        }
    }
}

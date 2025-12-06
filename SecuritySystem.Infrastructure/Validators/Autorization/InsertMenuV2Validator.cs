using FluentValidation;
using SecuritySystem.Core.QueryFilters.Autorization;

namespace SecuritySystem.Infrastructure.Validators.Security
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

            // Usa el nombre real de la propiedad de auditoría
            RuleFor(menu => menu.CreatedBy)
                .NotEmpty()
                .WithMessage("The registered user is required.");

            RuleFor(menu => menu.ApplicationId)
                .NotEmpty()
                .WithMessage("The application id is required.");
        }
    }

}

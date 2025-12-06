using FluentValidation;
using FluentValidation.Results;

namespace SecuritySystem.Infrastructure.Validators
{
    public class IdStringValidator : AbstractValidator<string>
    {
        public IdStringValidator()
        {
            RuleFor(data => data)
                .NotEmpty()
                .Must(data => data?.Trim().ToLower() != "undefined")
                .WithMessage("El id no debe ser nulo.");
        }

        protected override bool PreValidate(ValidationContext<string> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure("", "El id no debe ser nulo."));
                return false;
            }
            return base.PreValidate(context, result);
        }
    }
}

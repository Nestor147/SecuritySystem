using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi;
using SecuritySystem.Core.Interfaces.Validators.Helpers;
using SecuritySystem.Infrastructure.Validators.Core;

namespace SecuritySystem.Infrastructure.Validators.Helpers
{
    public class HelpersRepositoryVal : IHelperProcessVal
    {
        public ResponseModel ValidateStringId(string id)
        {
            try
            {
                IdStringValidator validator = new IdStringValidator();
                var validationResult = validator.Validate(id);
                return MainValidator.IterationValidationResult(validationResult);
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
    }
}

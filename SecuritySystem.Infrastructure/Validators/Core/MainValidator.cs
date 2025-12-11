using FluentValidation.Results;
using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi;
using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;
using SecuritySystem.Core.Enums;

namespace SecuritySystem.Infrastructure.Validators.Core
{
    public class MainValidator
    {
        public static ResponseModel IterationValidationResult(ValidationResult validationResult)
        {
            try
            {
                var responseModel = new ResponseModel();
                List<Message> ValidationMessages = new List<Message>();

                if (!validationResult.IsValid)
                {
                    responseModel.IsValid = false;
                    foreach (ValidationFailure failure in validationResult.Errors)
                    {
                        ValidationMessages.Add(new Message() { Type = TypeMessage.warning.ToString(), Description = failure.ErrorMessage });
                    }
                    responseModel.ValidationMessages = ValidationMessages;
                }

                return responseModel;
            }
            catch (Exception err)
            {
                throw new Exception(err.Message);
            }
        }
    }
}

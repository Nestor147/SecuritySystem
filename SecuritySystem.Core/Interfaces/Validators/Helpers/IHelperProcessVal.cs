using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi;

namespace SecuritySystem.Core.Interfaces.Validators.Helpers
{
    public interface IHelperProcessVal
    {
        ResponseModel ValidateStringId(string id);
    }
}

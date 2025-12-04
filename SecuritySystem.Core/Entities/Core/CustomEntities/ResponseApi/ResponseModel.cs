using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;

namespace SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi
{
    public class ResponseModel
    {
        public ResponseModel()
        {
            IsValid = true;
            ValidationMessages = new List<Message>();
        }
        public bool IsValid { get; set; }
        public List<Message> ValidationMessages { get; set; }
    }
}
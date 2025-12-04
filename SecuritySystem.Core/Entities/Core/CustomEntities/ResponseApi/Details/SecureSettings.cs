namespace SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details
{
    public class SecureSettings
    {
        //Verificar la conexion con sql server
        public string OracleDBConnection { get; set; }
        public string PasswordEncriptionValue { get; set; }
        public string GoogleAuthSettings { get; set; }
        public string UserGmail { get; set; }
        public string PasswordGmail { get; set; }
    }
}
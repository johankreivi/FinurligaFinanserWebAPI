namespace FinurligaFinanserWebAPI.DtoModels.UserAccountDTOs
{
    // Används när frontend skickar ett loginförsök.
    public class LoginUserDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

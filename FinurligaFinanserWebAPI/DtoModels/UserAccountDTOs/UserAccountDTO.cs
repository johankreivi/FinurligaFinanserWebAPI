namespace FinurligaFinanserWebAPI.DtoModels.UserAccountDTOs
{
    // Dessa data skickas från frontend till backend.
    public class UserAccountDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}

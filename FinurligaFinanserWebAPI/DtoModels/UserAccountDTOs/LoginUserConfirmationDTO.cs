namespace FinurligaFinanserWebAPI.DtoModels.UserAccountDTOs
{
    public class LoginUserConfirmationDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsAuthorized { get; set; } = false;
        public string Message { get; set; } = string.Empty;
    }
}

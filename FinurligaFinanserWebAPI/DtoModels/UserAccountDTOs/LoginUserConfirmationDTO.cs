namespace FinurligaFinanserWebAPI.DtoModels.UserAccountDTOs
{
    // Används när backend skickar en loginbekfräftelse till frontend.
    public class LoginUserConfirmationDTO
    {
        public string UserName { get; set; } = string.Empty;
        public bool IsAuthorized { get; set; } = false;
        public string Message { get; set; } = string.Empty;
    }
}

namespace FinurligaFinanserWebAPI.DtoModels.UserAccountDTOs
{
    // Dessa data skickas som en bekräftelse från backend till frontend när ett UserAccount skapats upp.
    public class UserAccountConfirmationDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}

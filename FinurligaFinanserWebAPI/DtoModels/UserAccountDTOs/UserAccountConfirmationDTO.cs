namespace FinurligaFinanserWebAPI.DtoModels.UserAccountDTOs
{
    public class UserAccountConfirmationDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}

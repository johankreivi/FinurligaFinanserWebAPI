namespace FinurligaFinanserWebAPI.DtoModels
{
    public class UserAccountDto
    {

    }
    public class CreateUserAccountDTO
    {
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
    }

    public class UserAccountConfirmationDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
    }
}

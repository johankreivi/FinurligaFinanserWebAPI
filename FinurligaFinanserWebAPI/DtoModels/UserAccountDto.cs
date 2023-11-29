using System.ComponentModel.DataAnnotations;

namespace FinurligaFinanserWebAPI.DtoModels
{
    // Dessa data skickas från frontend till backend.
    public class UserAccountDTO
    {        
        public string UserName { get; set; } = string.Empty;        
        public string FirstName { get; set; } = string.Empty;        
        public string LastName { get; set; } = string.Empty;        
        public string Password { get; set; } = string.Empty;
    }    
    
    // Dessa data skickas som en bekräftelse från backend till frontend när ett UserAccount skapats upp.
    public class UserAccountConfirmationDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    // Används när frontend skickar ett loginförsök.
    public class LoginUserDTO
    {        
        public string UserName { get; set; } = string.Empty;        
        public string Password { get; set; } = string.Empty;
    }

    // Används när backend skickar en loginbekfräftelse till frontend.
    public class LoginUserConfirmationDTO
    {
        public string UserName { get; set; } = string.Empty;
        public bool IsAuthorized { get; set; } = false;
        public string Message { get; set; } = string.Empty;
    }
}

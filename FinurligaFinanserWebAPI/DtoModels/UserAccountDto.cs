using System.ComponentModel.DataAnnotations;

namespace FinurligaFinanserWebAPI.DtoModels
{
    // Dessa data skickas från frontend till backend.
    public class UserAccountDto
    {
        [Required(ErrorMessage = "Användarnamn är obligatoriskt.")]
        [MinLength(6, ErrorMessage = "Användarnamnet måste vara minst 6 tecken långt.")]
        [StringLength(50, ErrorMessage = "Användarnamnet får inte vara längre än 50 tecken.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Förnamn är obligatoriskt.")]
        [StringLength(50, ErrorMessage = "Förnamnet får inte vara längre än 50 tecken.")]
        [MinLength(2, ErrorMessage = "Förnamnet måste vara minst 2 tecken långt.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Efternamn är obligatoriskt.")]
        [StringLength(50, ErrorMessage = "Efternamnet får inte vara längre än 50 tecken.")]
        [MinLength(2, ErrorMessage = "Efternamnet måste vara minst 2 tecken långt.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lösenord är obligatoriskt.")]
        [MinLength(8, ErrorMessage = "Lösenordet måste vara minst 8 tecken långt.")]
        [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
        ErrorMessage = "Lösenordet måste innehålla minst en liten bokstav, en stor bokstav, en siffra och ett specialtecken.")]
        public string Password { get; set; } = string.Empty;
    }    
    
    // Dessa data skickas som en bekräftelse från backend till frontend när ett UserAccount skapats upp.
    public class UserAccountConfirmationDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
    }
}

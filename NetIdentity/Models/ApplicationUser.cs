using Microsoft.AspNetCore.Identity;

namespace NetIdentity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime FechaNacimiento { get; set; }
        public string? NombreCompleto { get; set; }
        public string? genero { get; set; } //Masculino | Femenino | Otro | GeneroX | GeneroY ; M | F | O | X | Y
        //public bool isFemenino? (1) =0(masculino) | isMasculino 1|0 
    }

}

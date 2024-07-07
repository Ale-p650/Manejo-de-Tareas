using System.ComponentModel.DataAnnotations;

namespace Manejo_de_Tareas.Models
{
    public class Login_ViewModel
    {
        [Required(ErrorMessage = "El campo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo invalido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo es obligatorio")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name ="Recuerdame")]
        public bool Recuerdame { get; set; }
    }
}

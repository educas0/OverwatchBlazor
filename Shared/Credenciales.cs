using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace OverwatchBlazor.Shared
{
    public class Credenciales
    {
        [Required(ErrorMessage = "Email obligatorio")]
        [EmailAddress(ErrorMessage ="Formato Email inválido")]
        [MaxLength(100, ErrorMessage ="Email demasiado largo, no más de 100 carateres.")]
        public String Email { get; set; }

        [Required(ErrorMessage = "Password obligatoria")]
        [MinLength(length: 6, ErrorMessage = "password como MIN. 6 caracteres")]
        [MaxLength(length: 15, ErrorMessage = "password demasiado larga, como MAX.15 caracteres")]
        //[RegularExpression("^[a-zA-Z0-9!$#_]{6,15}$", ErrorMessage = "Debes meter una letra mins,mays,digito y caracter alfanumerico")]
        public String Password { get; set; }
        public String HashPassword { get; set; }

        public String Nickname { get; set; }
        public Boolean CuentaActivada { get; set; }
    }
}

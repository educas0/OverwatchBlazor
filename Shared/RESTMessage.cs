
using OverwatchBlazor.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverwatchBlazor.Shared
{
    public class RESTMessage
    {

        public string Mensage { get; set; }
        public List<String> Errores { get; set; }
        public Cliente DatosCliente { get; set; }
        public List<Persona> ListaPersonas { get; set; }
        public String Token { get; set; }
        public Object OtrosDatos { get; set; }
    }
}

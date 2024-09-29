using OverwatchBlazor.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OverwatchBlazor.Client.Services.interfaces
{
    interface IPersonaService
    {


        Task<List<Persona>> BuscarPerosnas();
        Task<RESTMessage> CrearPersona(Persona persona);
        Task<RESTMessage> EliminarPersona(Persona persona);
    
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using OverwatchBlazor.Shared;

namespace OverwatchBlazor.Client.Services.interfaces
{
    interface IAuthService
    {

        Task<RESTMessage> Registro(Cliente cliente);
        Task<RESTMessage> Login(Credenciales creds);
        Task<Cliente> GetClienteLogged();



    }
}

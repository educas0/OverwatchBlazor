using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OverwatchBlazor.Server.Modelos.Interfaces;
using OverwatchBlazor.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OverwatchBlazor.Server.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RESTPersonaController : ControllerBase
    {

        private IServiciosAccesoDBs _accesoDB;
        private IConfiguration _accesoappjson;

        public RESTPersonaController(IServiciosAccesoDBs servicioAccesoDbDI, IConfiguration accesoFichConfig)
        {
            this._accesoDB = servicioAccesoDbDI;
            this._accesoappjson = accesoFichConfig;
        }



        [HttpGet]
        public async Task<List<Persona>> BuscarPersonas()
        {

            //tenemos q acceder a la BD y insertar el objeto nuevocliente...
            List<Persona> _resultadoBusqueda = await this._accesoDB.BuscarPersonas();

     
            

            return _resultadoBusqueda;

        }







        [HttpPost]
        public async Task<RESTMessage> RegistroPersona([FromBody] Persona nuevapersona)
        {

            //tenemos q acceder a la BD y insertar el objeto nuevocliente...
            int _resultadoInsert = await this._accesoDB.CrearPersona(nuevapersona);

            RESTMessage respuertaAPersona = new RESTMessage();
            if (_resultadoInsert == 1)
            {
                respuertaAPersona.Mensage = "Cliente regitrado OK!!!";
                respuertaAPersona.Errores = null;
                respuertaAPersona.DatosCliente = null; //<---- tenemos q quitar el hash de la password....
                respuertaAPersona.Token = ""; //<----tenemos q generar token de sesion, jwt , en el login
            }
            else
            {
                respuertaAPersona.Mensage = "fallo al registrar cliente...";
                respuertaAPersona.Errores = new List<string> { "fallo al registrar cliente..." };
                respuertaAPersona.DatosCliente = null;
                respuertaAPersona.Token = null;

            }

            return respuertaAPersona;

        }



        [HttpPost]
        public async Task<RESTMessage> EliminarPersona([FromBody] Persona nuevapersona)
        {

            //tenemos q acceder a la BD y insertar el objeto nuevocliente...
            bool _resultadoInsert = await this._accesoDB.EliminarPersona(nuevapersona);

            RESTMessage respuertaAPersona = new RESTMessage();
            if (_resultadoInsert )
            {
                respuertaAPersona.Mensage = "Persona ELIMINADO OK!!!";
                respuertaAPersona.Errores = null;
                respuertaAPersona.DatosCliente = null; //<---- tenemos q quitar el hash de la password....
                respuertaAPersona.Token = ""; //<----tenemos q generar token de sesion, jwt , en el login
            }
            else
            {
                respuertaAPersona.Mensage = "fallo al ELIMINAR Persona...";
                respuertaAPersona.Errores = new List<string> { "fallo al ELIMINAR Persona..." };
                respuertaAPersona.DatosCliente = null;
                respuertaAPersona.Token = null;

            }

            return respuertaAPersona;

        }





    }





}



using Microsoft.JSInterop;
using Newtonsoft.Json;
using OverwatchBlazor.Client.Services.interfaces;
using OverwatchBlazor.Client.Services.interfaces;
using OverwatchBlazor.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;



namespace OverwatchBlazor.Client.Services
{
    public class PersonaService : IPersonaService
    {

        private HttpClient _http;
        private IJSRuntime _js;
        private IServiciosAccesoDBs _sqlSrv;
        List<Persona> _personas = new List<Persona>();

        public PersonaService(HttpClient servicioHttpDI, IJSRuntime servicioJsDI, IServicioSql)
        {
            this._http = servicioHttpDI;
            this._js = servicioJsDI;
        }


        public async Task<List<Persona>> BuscarPerosnas()
        {

            
            //this._personas = await 


            HttpResponseMessage _respServer = await this._http.GetAsync("/api/RESTCliente/ObtenerProvincias");
            RESTMessage _contenidoRespuesta = await _respServer.Content.ReadFromJsonAsync<RESTMessage>();

            if (_contenidoRespuesta.Errores == null)
            {
            //    //esto kaskaria...
            //    //return _contenidoRespuesta.OtrosDatos as List<Provincia>;

            //    //hay q des-serializar lo q hay en la propiedad OtrosDatos q se recibe como un
            //    //objeto: JsonElement
                await this._js.InvokeVoidAsync("console.log", _contenidoRespuesta.OtrosDatos.GetType().ToString());

                String _ListaPersonasSerializada = ((JsonElement)_contenidoRespuesta.OtrosDatos).GetRawText();
                return System.Text.Json.JsonSerializer.Deserialize<List<Persona>>(_ListaPersonasSerializada);
            }
            else
            {
                return null;
            }
        }   
        public async Task<RESTMessage> CrearPersona(Persona persona)
        {
            String _clienteSerialized = JsonConvert.SerializeObject(persona);
            StringContent _contenidoPetServer = new StringContent(_clienteSerialized, UnicodeEncoding.UTF8, "application/json");

            //invocar al servicio RESTFUL de asp.net core
            HttpResponseMessage respuestaServer = await this._http.PostAsync("api/RESTPersona/CrearPersona", _contenidoPetServer);
            RESTMessage _resultRegistro = await respuestaServer.Content.ReadFromJsonAsync<RESTMessage>();

            return _resultRegistro;
        }      
             
        public async Task<RESTMessage> EliminarPersona(Persona persona)
        {
            String _clienteSerialized = JsonConvert.SerializeObject(persona);
            StringContent _contenidoPetServer = new StringContent(_clienteSerialized, UnicodeEncoding.UTF8, "application/json");

            //invocar al servicio RESTFUL de asp.net core
            HttpResponseMessage respuestaServer = await this._http.PostAsync("api/RESTPersona/EliminarPersona", _contenidoPetServer);
            RESTMessage _resultRegistro = await respuestaServer.Content.ReadFromJsonAsync<RESTMessage>();

            return _resultRegistro;
        }
    }
}

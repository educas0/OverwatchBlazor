using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


using OverwatchBlazor.Shared;
using Microsoft.JSInterop;

using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using OverwatchBlazor.Client.Services.interfaces;

namespace OverwatchBlazor.Client.Servicios
{
    //servicio Blazor del lado del cliente para hacer login,registro....sobre el servicio restful del servidor
    public class AuthService: IAuthService
    {
        private HttpClient _http;
        private IJSRuntime _js;
        public AuthService(HttpClient servicioHttpDI, IJSRuntime servicioJsDI)
        {
            this._http = servicioHttpDI;
            this._js = servicioJsDI;
        }


        #region "........... metodos publicos de la clase del servicio ..............."

        #region "zona Cliente: Login y Registro..."

        public async Task<RESTMessage> Registro(Cliente cliente)
        {
            String _clienteSerialized = JsonConvert.SerializeObject(cliente);
            StringContent _contenidoPetServer = new StringContent(_clienteSerialized, UnicodeEncoding.UTF8, "application/json");

            //invocar al servicio RESTFUL de asp.net core
            HttpResponseMessage respuestaServer = await this._http.PostAsync("api/RESTCliente/Registro", _contenidoPetServer);
            RESTMessage _resultRegistro = await respuestaServer.Content.ReadFromJsonAsync<RESTMessage>();

            return _resultRegistro;

        }

        public async Task<RESTMessage> Login(Credenciales creds)
        {

            try
            {
                HttpResponseMessage respuestaServer = await this._http.PostAsync("api/RESTCliente/Login", JsonContent.Create(creds));
                RESTMessage _resultadoLogin = await respuestaServer.Content.ReadFromJsonAsync<RESTMessage>();

                //hay q guardar el token de sesion en el localstorage
                await _js.InvokeVoidAsync("manageLocalStorage.setLocalStorage", "token", _resultadoLogin.Token);
                await _js.InvokeVoidAsync("manageLocalStorage.setLocalStorage", "cliente", JsonConvert.SerializeObject(_resultadoLogin.DatosCliente));
                return _resultadoLogin;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw;
            }



            
        }

        public async Task<Cliente> GetClienteLogged()
        {
            try
            {
                String _jsoncliente = await _js.InvokeAsync<String>("manageLocalStorage.getLocalStorage", "cliente");
                Cliente _cliente = JsonConvert.DeserializeObject<Cliente>(_jsoncliente);

                await _js.InvokeVoidAsync("console.log", _cliente);

                return _cliente; //JsonConvert.DeserializeObject<Cliente>(_cliente);
            }
            catch (Exception ex)
            {
                await _js.InvokeVoidAsync("console.log", ex.Message);
                return null;
            }
        }

        #endregion


        #region "zona Cliente: PANEL=> mis direcciones -----------"
        //public async Task<List<Provincia>> DameProvincias() {
        //    HttpResponseMessage respserver=await this._http.GetAsync("api/RESTCliente/DameProvincias");
        //    RESTMessage _result = await respserver.Content.ReadFromJsonAsync<RESTMessage>();
        //    if (_result.Errores==null)
        //    {
        //        await _js.InvokeVoidAsync("console.log", _result.OtrosDatos.GetType().ToString() ); //<---System.Text.Json.JsonElement no puede parsearse directamente a un List<Provincia>, hay q obtener el String de este objeto y deserializrlo
        //        await _js.InvokeVoidAsync("console.log",  (((JsonElement)_result.OtrosDatos).GetRawText()) );

        //        List<Provincia> _listaprovs = JsonConvert.DeserializeObject<List<Provincia>>((((JsonElement)_result.OtrosDatos).GetRawText()));

        //        return _listaprovs;

        //    } else
        //    {
        //        return null;
        //    }
        
        //}

        //public async Task<List<Municipio>> DameMunicipios(int codprov)
        //{
        //    HttpResponseMessage respserver = await this._http.GetAsync("api/RESTCliente/DameMunicipios?codprov=" + codprov);
        //    RESTMessage _result = await respserver.Content.ReadFromJsonAsync<RESTMessage>();
        //    if (_result.Errores == null)
        //    {
        //        List<Municipio> _listamunis = JsonConvert.DeserializeObject<List<Municipio>>((((JsonElement)_result.OtrosDatos).GetRawText()));

        //        return _listamunis;
        //    }
        //    else {
        //        return null;
        //    }
        //}


        //public async Task<RESTMessage> OperarDireccion(String operacion, Direccion nuevadir) {
        //    #region "...solucion 1º: objetos dinamicos, como hacemos con angular..."
        //    /*
        //     el problema: https://stackoverflow.com/questions/63121877/dynamic-c-sharp-valuekind-object
        //     solucion: https://stackoverflow.com/questions/58271901/converting-newtonsoft-code-to-system-text-json-in-net-core-3-whats-equivalent/58273914#58273914

        //     complicado hacerlo con objetos dinamicos, pq en el lado del server el parametro se recogeria como Object <--el tipo es un System.Text.Json.JsonElement
        //     (y no Newtonsoft.Json pq es el conversor q ya utiliza por defecto asp.net core...si lo intentas poner por defecto en el startup.cs te kaska por todos los lados)

        //     y para convertirlo a un objeto con las props del objeto dinamico te tienes q crear un ViewModel aposta donde las propiedades de la clase de este ViewModel
        //     tienen q tener atributos: [JsonPropertyName]
        //        public class IncomingJsonDirec {
        //            [JsonPropertyName("Operacion")]
        //            public String Operacion {get; set;}

        //            [JsonPropertyName("Direccion")]
        //            public Direccion Direccion {get; set;}
        //        }

        //    y ya en el metodo del RESTClienteController el parametro lo defines como esa clase y se mapea bien: OperarDireccion([FromBody] IncomingJsonDirec body) ...
        //    (antes lo deberias declarar como Object (hay gente q lo declara como dynamic pero se desaconseja su uso)


        //    String _direcSerialized = JsonConvert.SerializeObject(new { Operacion = operacion, Direccion = nuevadir });
        //    StringContent _contenidoPetServer = new StringContent(_direcSerialized, UnicodeEncoding.UTF8, "application/json");
        //    */
        //    #endregion

        //    Dictionary<String, String> _contenidoPetServer = new Dictionary<string, string> {
        //        { "Operacion",operacion },
        //        { "Direccion", JsonConvert.SerializeObject(nuevadir) }
        //    };

            /* ---- ya no haria falta añadir las cabeceras a mano de autentificacion con el token pq hay definido un (DelegationHandler)"interceptor" ya en el program.cs para el httpclient----
            String _jwt = await this._js.InvokeAsync<String>("manageLocalStorage.getLocalStorage", "token");
            this._http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwt);
            */
            //HttpResponseMessage _respServer = await this._http.PostAsync("api/RESTCliente/OperarDireccion", JsonContent.Create<Dictionary<String,String>>(_contenidoPetServer));
            //RESTMessage _resultadoDirec=await _respServer.Content.ReadFromJsonAsync<RESTMessage>();

            //si no hay errores modificamos el cliente del storage, su lista de direcciones...
            // ---------- esto es cutre hacerlo aqui en el lado del cliente...
            //lo suyo es q ya el servidor te devolviera el cliente actualizado y solo tendrias q meterlo en el localstorage ----

        //    if (_resultadoDirec.Errores==null)
        //    {
        //        Cliente _cliente = await this.GetClienteLogged();

        //        switch (operacion)
        //        {
        //            case "crear":
        //                _cliente.Direcciones.Add(nuevadir);
        //                break;

        //            case "eliminar":
        //                //Boolean resp=_cliente.Direcciones.Remove(nuevadir);
        //                _cliente.Direcciones = _cliente.Direcciones.Where((Direccion direc) => direc.DireccionId != nuevadir.DireccionId).ToList<Direccion>();
        //                await this._js.InvokeVoidAsync("console.log", "cliente con direccion en teoria borrada: ", _cliente);

        //                break;

        //            case "modificar":
        //                _cliente.Direcciones = _cliente.Direcciones.Where((Direccion direc) => direc.DireccionId != nuevadir.DireccionId).ToList<Direccion>();
        //                _cliente.Direcciones.Add(nuevadir);
        //                break;
        //            default:
        //                break;
        //        }
        //        await this._js.InvokeVoidAsync("manageLocalStorage.setLocalStorage", "cliente", JsonConvert.SerializeObject(_cliente));
        //        await this._js.InvokeVoidAsync("console.log", "---cliente actualizado en storage ----", _cliente);
        //    }
        //    return _resultadoDirec;
        //}
        #endregion



        #endregion
    }
}

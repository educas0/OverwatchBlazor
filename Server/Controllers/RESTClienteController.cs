using OverwatchBlazor.Server.Modelos.Interfaces;
using OverwatchBlazor.Shared;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using System;
using System.Collections.Generic;

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace OverwatchBlazor.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RESTClienteController : ControllerBase
    {

        //inyecto el servicio de acceso a BD...
        private IServiciosAccesoDBs _accesoDB;
        private IConfiguration _accesoappjson;

        public RESTClienteController(IServiciosAccesoDBs servicioAccesoDbDI,IConfiguration accesoFichConfig)
        {
            this._accesoDB = servicioAccesoDbDI;
            this._accesoappjson = accesoFichConfig;
        }




        #region ".....metodos de clase de nuestro servicio restfull....."

        #region "metodos REST: login y registro..."
        [HttpPost]
        public async Task<RESTMessage> Registro([FromBody] Cliente nuevocliente)
        {

            //tenemos q acceder a la BD y insertar el objeto nuevocliente...
            int _resultadoInsert = await this._accesoDB.Registro(nuevocliente);

            RESTMessage respuestaAlCliente = new RESTMessage();
            if (_resultadoInsert == 1)
            {
                respuestaAlCliente.Mensage = "Cliente regitrado OK!!!";
                respuestaAlCliente.Errores = null;
                respuestaAlCliente.DatosCliente = nuevocliente; //<---- tenemos q quitar el hash de la password....
                respuestaAlCliente.Token = ""; //<----tenemos q generar token de sesion, jwt , en el login
            }
            else
            {
                respuestaAlCliente.Mensage = "fallo al registrar cliente...";
                respuestaAlCliente.Errores = new List<string> { "fallo al registrar cliente..." };
                respuestaAlCliente.DatosCliente = null;
                respuestaAlCliente.Token = null;

            }

            return respuestaAlCliente;

        }





        [HttpPost]
        public async Task<RESTMessage> Login([FromBody] Credenciales creds)
        {

            Cliente _cliente = await this._accesoDB.Login(creds);

            RESTMessage _respuestaLogin = new RESTMessage();

            if (_cliente != null)
            {
                _respuestaLogin.Mensage = "Login OKS!!!";
                _respuestaLogin.Errores = null;
                _respuestaLogin.DatosCliente = _cliente;
                _respuestaLogin.Token = this._crearJWT(_cliente);

            }
            else
            {
                _respuestaLogin.Mensage = "Login Fallido";
                _respuestaLogin.Errores = new List<string> { "Login Fallido", "Email o password incorrectos" };
                _respuestaLogin.DatosCliente = null;
                _respuestaLogin.Token = "";

            }
            return _respuestaLogin;
        }


        #endregion

        #region "metodos REST: PANEL -> misDirecciones"
        //[HttpGet]
        //public async Task<RESTMessage> DameProvincias()
        //{
        //    List<Provincia> _listaProvs = await this._accesoDB.DameProvincias();

        //    RESTMessage _respuestaCliente = new RESTMessage();
        //    if (_listaProvs != null)
        //    {
        //        _respuestaCliente.Mensaje = "lista provincias oks!!";
        //        _respuestaCliente.OtrosDatos = _listaProvs;
        //    }
        //    else
        //    {
        //        _respuestaCliente.Mensaje = "Fallo al devolver Provincias";
        //        _respuestaCliente.Errores = new List<String> { "Fallo al devolver Provincias", "Error de acceso al servidor de BD..." };
        //    }

        //    return _respuestaCliente;
        //}

        //[HttpGet]
        //public async Task<RESTMessage> DameMunicipios([FromQuery] int codprov)
        //{
        //    List<Municipio> _listaMunis = await this._accesoDB.DameMunicipios(codprov);

        //    RESTMessage _respuestaCliente = new RESTMessage();
        //    if (_listaMunis != null)
        //    {
        //        _respuestaCliente.Mensaje = $"lista municipios de la provincia {codprov} oks!!";
        //        _respuestaCliente.OtrosDatos = _listaMunis;
        //    }
        //    else
        //    {
        //        _respuestaCliente.Mensaje = "Fallo al devolver Municipios";
        //        _respuestaCliente.Errores = new List<String> { "Fallo al devolver Municipios", "Error de acceso al servidor de BD..." };
        //    }

        //    return _respuestaCliente;
        //}


        //[HttpPost]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //public async Task<RESTMessage> OperarDireccion([FromBody] Dictionary<String,String> body){
        //    //extraemos jwt de las cabeceras de la peticion
        //    String _jwtstring = this.Request.Headers["Authorization"][0].Split(" ")[1].ToString();

        //    JwtSecurityTokenHandler jwthandler = new JwtSecurityTokenHandler();
        //    JwtSecurityToken jwt=jwthandler.ReadJwtToken(_jwtstring);

        //    String _emailCliente=JsonConvert.DeserializeObject<MyClaim>(jwt.Claims.Where((Claim c) => c.Type == "Email").Single<Claim>().Value).Value;
        //    String _idCliente= JsonConvert.DeserializeObject<MyClaim>(jwt.Claims.Where((Claim c) => c.Type == "IdCliente").Single<Claim>().Value).Value;

        //    String _op = body["Operacion"];
        //    Direccion _direc = JsonConvert.DeserializeObject<Direccion>(body["Direccion"]);
        //    int _result = await this._accesoDB.OperarDireccion(_op, _direc , _idCliente);

        //    RESTMessage respuestaAlCliente = new RESTMessage();
        //    if (_result == 1)
        //    {
        //        respuestaAlCliente.Mensaje =$"Direccion {_op} OK!!!";
        //        respuestaAlCliente.Errores = null;
        //        respuestaAlCliente.OtrosDatos = _direc;
        //    }
        //    else
        //    {
        //        respuestaAlCliente.Mensaje = "fallo al crear/modificar/eliminar direccion...";
        //        respuestaAlCliente.Errores = new List<string> { 
        //                                                        "fallo al crear/modificar/eliminar direccion...", 
        //                                                        $"error: {_op} en {_direc.DireccionId}"
        //                                    };
        //    }

        //    return respuestaAlCliente;

        //}

        #endregion

        #endregion


        #region --------------- metodos privados de clase ------------------------

        private String _crearJWT(Cliente datoscliente) {

            Dictionary<String,Object> _claimsPayload = new Dictionary<String,object> {
                { "Email", new Claim(ClaimTypes.Email, datoscliente.CredencialesAcceso.Email) }
                //{ "Nickname", new Claim(ClaimTypes.NameIdentifier, datoscliente.Nickname) }
            };


            byte[] _clavefirma = System.Text.Encoding.UTF8.GetBytes(this._accesoappjson["JWT:clave"]);
            SecurityTokenDescriptor _jwt = new SecurityTokenDescriptor
            {
                Claims = _claimsPayload,
                Expires = DateTime.Now.AddHours(1),
                // Credenciales para generar el token usando nuestro secretykey y el algoritmo hash 256

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_clavefirma), SecurityAlgorithms.HmacSha256Signature)
            };


            SecurityToken JWT = new JwtSecurityTokenHandler().CreateToken(_jwt); 
            return (new JwtSecurityTokenHandler()).WriteToken(JWT);

        }
        //clase privada para deserializar los claim-values extraidos desde el jwt, q los saca en formato json y no hay ningun constructor de la clase claim q admita estos args
        public class MyClaim : Claim
        {
            public MyClaim(string type, string value, string valueType, string issuer, string originalIssuer) : base(type, value, valueType, issuer, originalIssuer)
            {

            }
        }
        #endregion
    }




}

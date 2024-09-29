using OverwatchBlazor.Server.Modelos.Interfaces;
using OverwatchBlazor.Shared;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;


namespace OverwatchBlazor.Server.Modelos
{
    public class ServicioSQLServerDB : IServiciosAccesoDBs
    {
        private SqlConnection _conexionBD;

        //public object BCrypt { get; private set; }

        public ServicioSQLServerDB(IConfiguration accesoFichConfDI)
        {
            this._conexionBD = new SqlConnection(accesoFichConfDI.GetConnectionString("SQLServerConnectionString"));
        }



        //        #region "...... metodos publicos de clase q expone el servicio cuando se inyecte......"

        //        #region "...del controlador CLIENTE....."
        public async Task<int> Registro(Cliente nuevocliente)
        {
            //instrucciones para hacer el iNSERT en sql server....
            try
            {
                this._conexionBD.Open();

                SqlCommand _insertCredenciales = new SqlCommand();
                _insertCredenciales.Connection = this._conexionBD;
                _insertCredenciales.CommandText = "INSERT INTO dbo.Credenciales VALUES(@Email, @Password, @Activa, @IdCliente)";
                _insertCredenciales.Parameters.AddWithValue("@Email", nuevocliente.CredencialesAcceso.Email);
                _insertCredenciales.Parameters.AddWithValue("@Password", BCrypt.Net.BCrypt.HashPassword(nuevocliente.CredencialesAcceso.Password));
                _insertCredenciales.Parameters.AddWithValue("@Activa", false);

                String _idCliente = Guid.NewGuid().ToString();
                _insertCredenciales.Parameters.AddWithValue("@idCliente", _idCliente);

                int _resultadoInsert = await _insertCredenciales.ExecuteNonQueryAsync();

                if (_resultadoInsert == 1)
                {
                    //hacemos insert en tabla clientes...
                    SqlCommand _insertCliente = new SqlCommand();
                    _insertCliente.Connection = this._conexionBD;
                    _insertCliente.CommandText = "INSERT INTO dbo.Clientes VALUES(@IdCliente,@Nombre,@Apellido)";
                    //_insertCliente.Parameters.AddWithValue("@IdCliente", _idCliente);
                    _insertCliente.Parameters.AddWithValue("@Nombre", nuevocliente.Nombre);
                    _insertCliente.Parameters.AddWithValue("@Apellido", nuevocliente.Apellido);

                    int _resultinsertCli = await _insertCliente.ExecuteNonQueryAsync();

                    if (_resultinsertCli == 1)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                this._conexionBD.Close();
            };

        }


        public async Task<Cliente> Login(Credenciales creds)
        {
            try
            {
                this._conexionBD.Open();
                SqlCommand _selectCreds = new SqlCommand("SELECT IdCliente,Password FROM dbo.Credenciales WHERE Email=@Email AND CuentaActivada='true'", this._conexionBD);
                _selectCreds.Parameters.AddWithValue("@Email", creds.Email);

                SqlDataReader _resultSelect = await _selectCreds.ExecuteReaderAsync();

                (String _hash, String _clienteId) valoresColumns = _resultSelect
                                                                        .Cast<IDataRecord>()
                                                                        .Select(fila => (fila["Password"].ToString(), fila["IdCliente"].ToString()))
                                                                        .Single<(String, String)>();

                _resultSelect.Close();

                if (BCrypt.Net.BCrypt.Verify(creds.Password, valoresColumns._hash) == false)
                {
                    //password incorrecta
                    return null;
                }
                else
                {
                    return await this.GetClienteLogged(valoresColumns._clienteId);
                }
            }
            catch (Exception ex)
            {

                return null;
            }
            finally
            {
                this._conexionBD.Close();
            }
        }

        public async Task<Cliente> GetClienteLogged(String idCliente)
        {
            try
            {
                if (this._conexionBD.State == ConnectionState.Closed) this._conexionBD.Open();
                SqlCommand _selectCliente = new SqlCommand("SELECT c.*, d.Email, d.CuentaActivada, d.Nickname from dbo.Clientes c INNER JOIN dbo.Credenciales d ON c.IdCliente=d.IdCliente WHERE c.IdCliente=@Id", this._conexionBD);
                _selectCliente.Parameters.AddWithValue("@Id", idCliente);

                SqlDataReader _resultSelectCli = await _selectCliente.ExecuteReaderAsync();

                if (_resultSelectCli.HasRows)
                {
                    Cliente _cliente = _resultSelectCli.Cast<IDataRecord>().Select(
                                                                                (IDataRecord fila) =>
                                                                                {
                                                                                    return new Cliente
                                                                                    {
                                                                                        IdCliente = Convert.ToInt32(fila["IdCliente"].ToString()),
                                                                                        Nombre = fila["Nombre"].ToString(),
                                                                                        Apellido = fila["Apellido"].ToString(),
                                                                                        CredencialesAcceso = new Credenciales
                                                                                        {
                                                                                            Email = fila["Email"].ToString(),
                                                                                            Password = "",
                                                                                            CuentaActivada = System.Convert.ToBoolean(fila["CuentaActivada"]),
                                                                                            Nickname = fila["Nickname"].ToString()
                                                                                        }
                                                                                    };
                                                                                }
                                                                            ).Single<Cliente>();
                    _resultSelectCli.Close();
                    //_cliente.Direcciones = await this.DireccionesCliente(_cliente.IdCliente);
                    return _cliente;

                    //recuperamos direcciones del cliente por si acaso accede al panel

                }
                else
                {
                    //el IdCliente no existe en tabla Clientes...esta rota relacion tabla Credenciales <---> tabla Cliente
                    return null;
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        //        public async Task<List<Provincia>> DameProvincias()
        //        {
        //            try
        //            {
        //                this._conexionBD.Open();
        //                SqlCommand _selectProvs = new SqlCommand("SELECT * FROM dbo.Provincias ORDER BY NombreProvincia", this._conexionBD);
        //                SqlDataReader _resultSelect = await _selectProvs.ExecuteReaderAsync();

        //                return _resultSelect
        //                        .Cast<IDataRecord>()
        //                        .Select(
        //                                (IDataRecord fila) => new Provincia
        //                                {
        //                                    CodPro = System.Convert.ToInt16(fila["CodPro"]),
        //                                    NombreProvincia = fila["NombreProvincia"].ToString()
        //                                }
        //                                )
        //                        .ToList<Provincia>();

        //            }
        //            catch (Exception ex)
        //            {

        //                return null;
        //            }
        //            finally
        //            {
        //                this._conexionBD.Close();
        //            }
        //        }

        //        public async Task<List<Municipio>> DameMunicipios(int codprov)
        //        {
        //            try
        //            {
        //                this._conexionBD.Open();
        //                SqlCommand _selectMunis = new SqlCommand("SELECT * FROM dbo.Municipios WHERE CodPro=@codprov ORDER BY NombreMunicipio", this._conexionBD);
        //                _selectMunis.Parameters.AddWithValue("@codprov", codprov);

        //                SqlDataReader _resultSelect = await _selectMunis.ExecuteReaderAsync();

        //                return _resultSelect
        //                           .Cast<IDataRecord>()
        //                           .Select(
        //                                    (IDataRecord fila) => new Municipio
        //                                    {
        //                                        CodPro = codprov,
        //                                        CodMun = System.Convert.ToInt16(fila["CodMun"]),
        //                                        NombreMunicipio = fila["NombreMunicipio"].ToString()
        //                                    }
        //                                   )
        //                           .ToList<Municipio>();
        //            }
        //            catch (Exception ex)
        //            {

        //                return null;
        //            }
        //            finally
        //            {
        //                this._conexionBD.Close();
        //            }
        //        }

        //        public async Task<int> OperarDireccion(string operacion, Direccion direc, String clientId)
        //        {
        //            try
        //            {
        //                this._conexionBD.Open();
        //                SqlCommand _commando = new SqlCommand();
        //                _commando.Connection = this._conexionBD;

        //                switch (operacion)
        //                {
        //                    case "crear":
        //                        _commando.CommandText = "INSERT INTO dbo.Direcciones VALUES(@DirId,@ClientId,@tipo,@nombre,@edif,@esc,@piso,@port,@cp,@codpro,@codmun,@alias,@ppal,@factu)";
        //                        _commando.Parameters.AddWithValue("@DirId", direc.DireccionId);
        //                        _commando.Parameters.AddWithValue("@ClientId", clientId);
        //                        _commando.Parameters.AddWithValue("@tipo", direc.TipoVia);
        //                        _commando.Parameters.AddWithValue("@nombre", direc.NombreVia);
        //                        _commando.Parameters.AddWithValue("@edif", direc.Edificio);
        //                        _commando.Parameters.AddWithValue("@esc", direc.Escalera);
        //                        _commando.Parameters.AddWithValue("@piso", direc.Piso);
        //                        _commando.Parameters.AddWithValue("@port", direc.Puerta);
        //                        _commando.Parameters.AddWithValue("@cp", direc.CP);
        //                        _commando.Parameters.AddWithValue("@codpro", direc.ProvinciaDir.CodPro);
        //                        _commando.Parameters.AddWithValue("@codmun", direc.MunicipioDir.CodMun);
        //                        _commando.Parameters.AddWithValue("@alias", direc.AliasDireccion);
        //                        _commando.Parameters.AddWithValue("@ppal", direc.EsPrincipal);
        //                        _commando.Parameters.AddWithValue("@factu", direc.EsFacturacion);

        //                        break;

        //                    case "eliminar":
        //                        _commando.CommandText = "DELETE FROM dbo.Direcciones WHERE DireccionId=@DirId";
        //                        _commando.Parameters.AddWithValue("@DirId", direc.DireccionId);

        //                        break;

        //                    case "modificar":
        //                        _commando.CommandText = "UPDATE  dbo.Direcciones SET TipoVia=@tipo,NombreVia=@nombre,Edificio=@edif,Escalera=@esc,Piso=@piso,Puerta=@port,CP=@cp,CodPro=@codpro,CodMun=@codmun,AliasDireccion=@alias,EsPrincipal=@ppal,EsFacturacion=@factu  WHERE DireccionId=@DirId";

        //                        _commando.Parameters.AddWithValue("@DirId", direc.DireccionId);
        //                        _commando.Parameters.AddWithValue("@tipo", direc.TipoVia);
        //                        _commando.Parameters.AddWithValue("@nombre", direc.NombreVia);
        //                        _commando.Parameters.AddWithValue("@edif", direc.Edificio);
        //                        _commando.Parameters.AddWithValue("@esc", direc.Escalera);
        //                        _commando.Parameters.AddWithValue("@piso", direc.Piso);
        //                        _commando.Parameters.AddWithValue("@port", direc.Puerta);
        //                        _commando.Parameters.AddWithValue("@cp", direc.CP);
        //                        _commando.Parameters.AddWithValue("@codpro", direc.ProvinciaDir.CodPro);
        //                        _commando.Parameters.AddWithValue("@codmun", direc.MunicipioDir.CodMun);
        //                        _commando.Parameters.AddWithValue("@alias", direc.AliasDireccion);
        //                        _commando.Parameters.AddWithValue("@ppal", direc.EsPrincipal);
        //                        _commando.Parameters.AddWithValue("@factu", direc.EsFacturacion);

        //                        break;
        //                    default:
        //                        break;
        //                }

        //                return await _commando.ExecuteNonQueryAsync();
        //            }
        //            catch (Exception ex)
        //            {

        //                return -1;
        //            }
        //            finally
        //            {
        //                this._conexionBD.Close();
        //            }
        //        }
        //        #endregion


        //        #region "....del controlador TIENDA...."

        //        public async Task<List<Categoria>> DameCategorias(Categoria? categoria)
        //        {
        //            try
        //            {
        //                this._conexionBD.Open();

        //                SqlCommand _selCat = new SqlCommand("SELECT * FROM dbo.Categorias", this._conexionBD);

        //                String _patron = categoria == null ? @"^[0-9]{1,}$" : @"^" + categoria.PathCategoria + "->[0-9]{1,}$";
        //                Func<IDataRecord, Boolean> filtro = (IDataRecord fila) => System.Text.RegularExpressions.Regex.IsMatch(fila["PathCategoria"].ToString(), _patron);

        //                return (await _selCat.ExecuteReaderAsync())
        //                        .Cast<IDataRecord>()
        //                        .Where(filtro)
        //                        .Select(
        //                            (IDataRecord fila) => new Categoria
        //                            {
        //                                NombreCategoria = fila["NombreCategoria"].ToString(),
        //                                PathCategoria = fila["PathCategoria"].ToString(),
        //                                IdCategoria = System.Convert.ToInt16(fila["IdCategoria"])
        //                            }
        //                        ).ToList<Categoria>();

        //            }
        //            catch (Exception ex)
        //            {
        //                return null;
        //            }
        //            finally
        //            {
        //                this._conexionBD.Close();
        //            }
        //        }


        //        public async Task<List<Producto>> DameProductos(int idcategoria)
        //        {
        //            try
        //            {
        //                this._conexionBD.Open();

        //                SqlCommand _selProd = new SqlCommand("SELECT * FROM dbo.Productos WHERE IdCategoria=@id", this._conexionBD);
        //                _selProd.Parameters.AddWithValue("@id", idcategoria);


        //                return (await _selProd.ExecuteReaderAsync())
        //                        .Cast<IDataRecord>()
        //                        .Select(
        //                            (IDataRecord fila) => new Producto
        //                            {
        //                                NombreProducto = fila["NombreProducto"].ToString(),
        //                                PrecioProducto = fila["PrecioProducto"].ToString(),
        //                                InformacionGeneral = fila["InformacionGeneral"].ToString(),
        //                                Ingredientes = fila["Ingredientes"].ToString(),
        //                                Nutrientes = fila["Nutrientes"].ToString(),
        //                                Conservacion = fila["Conservacion"].ToString(),
        //                                Ean = fila["Ean"].ToString(),
        //                                IdCategoria = idcategoria
        //                            }
        //                        ).ToList<Producto>();

        //            }
        //            catch (Exception ex)
        //            {
        //                return null;
        //            }
        //            finally
        //            {
        //                this._conexionBD.Close();
        //            }
        //        }

        public async Task<int> CrearPedido(Pedido pedido)
        {
            //instrucciones para hacer el iNSERT en sql server....
            try
            {
                this._conexionBD.Open();

                //se genera id del pedido
                pedido.IdPedido = Guid.NewGuid().ToString();

                //hacemos insert en tabla pedido...
                SqlCommand _insertPedido = new SqlCommand();
                _insertPedido.Connection = this._conexionBD;

                string sql = "INSERT INTO dbo.Pedidos ";

                sql += " ( ";
                sql += " IdPedido, ";
                sql += " IdHeroe, ";
                sql += " FechaPedido, ";
                sql += " NicknameCliente, ";
                sql += " IdHeroe, ";
                sql += " Cantidades, ";
                sql += " SubTotalPedido, ";
                sql += " TotalPedido ";
                sql += " ) ";

                sql += " VALUES ";

                sql += " ( ";
                sql += " @IdPedido, ";
                sql += " @IdHeroe, ";
                sql += " @FechaPedido, ";
                sql += " @NicknameCliente, ";
                sql += " @IdHeroe, ";
                sql += " @Cantidades, ";
                sql += " @TotalPedido ";
                sql += " ) ";

                _insertPedido.CommandText = sql;

                _insertPedido.Parameters.AddWithValue("@IdPedido", pedido.IdPedido);
                _insertPedido.Parameters.AddWithValue("@FechaPedido", pedido.FechaPedido);
                _insertPedido.Parameters.AddWithValue("@IdHeroe", pedido.IdHeroe);
                _insertPedido.Parameters.AddWithValue("@Cantidades", pedido.Cantidades);
                _insertPedido.Parameters.AddWithValue("@TotalPedido", pedido.TotalPedido);

                int _resultinsertPed = await _insertPedido.ExecuteNonQueryAsync();

                if (_resultinsertPed == 1)
                {
                    //si se inserto el pedido se insertan los articulos del mismo

                    //foreach (ItemPedido _IT in pedido.ElementosPedido)
                    //{
                    //    SqlCommand _insertItem = new SqlCommand();
                    //    _insertItem.Connection = this._conexionBD;

                    //    sql = "INSERT INTO dbo.ItemsPedidos ";
                    //    sql += " ( ";
                    //    sql += " IdPedido, ";
                    //    sql += " Ean, ";
                    //    sql += " cantidad ";
                    //    sql += " ) ";

                    //    sql += " VALUES ";

                    //    sql += " ( ";
                    //    sql += " @IdPedido, ";
                    //    sql += " @Ean, ";
                    //    sql += " @cantidad ";
                    //    sql += " ) ";

                    //    _insertItem.CommandText = sql;

                    //    _insertItem.Parameters.AddWithValue("@IdPedido", pedido.IdPedido);
                    //    _insertItem.Parameters.AddWithValue("@Ean", _IT.producto.Ean);
                    //    _insertItem.Parameters.AddWithValue("@cantidad", _IT.cantidad);

                    //    await _insertItem.ExecuteNonQueryAsync();
                    //}

                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                this._conexionBD.Close();
            };

        }

        public async Task<List<Pedido>> DamePedidos(string idCliente)
        {
            try
            {
                this._conexionBD.Open();
                SqlCommand _selProd = new SqlCommand("SELECT * FROM dbo.Pedidos WHERE IdCliente=@id ORDER BY FechaPedido DESC", this._conexionBD);
                _selProd.Parameters.AddWithValue("@id", idCliente);


                return (await _selProd.ExecuteReaderAsync())
                        .Cast<IDataRecord>()
                        .Select(
                            (IDataRecord fila) => new Pedido
                            {


                                //IdPedido = fila["IdPedido"].ToString(),
                                //IdCliente = idCliente,
                                //FechaPedido = Convert.ToDateTime(fila["FechaPedido"]),
                                //EstadoPedido = fila["EstadoPedido"].ToString(),
                                //SubTotalPedido = Convert.ToDecimal(fila["SubTotalPedido"]),
                                //GastosDeEnvio = Convert.ToDecimal(fila["GastosEnvio"]),
                                //TotalPedido = Convert.ToDecimal(fila["TotalPedido"])
                            }
                        ).ToList<Pedido>();

            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this._conexionBD.Close();
            }
        }

        public async Task<int> CrearPersona(Persona persona)
        {
            //instrucciones para hacer el iNSERT en sql server....
            try
            {
                this._conexionBD.Open();


                //hacemos insert en tabla pedido...
                SqlCommand _insertPersona = new SqlCommand();
                _insertPersona.Connection = this._conexionBD;

                string sql = "INSERT INTO dbo.Persona ";

                sql += " ( ";
                sql += " Id, ";
                sql += " Nombre, ";
                sql += " DNI, ";
                sql += " Edad ";
                sql += " ) ";

                sql += " VALUES ";

                sql += " ( ";
                sql += " @Id, ";
                sql += " @Nonbre, ";
                sql += " @DNI, ";
                sql += " @Edad ";
                sql += " ) ";

                _insertPersona.CommandText = sql;

                _insertPersona.Parameters.AddWithValue("@Id", persona.Id);
                _insertPersona.Parameters.AddWithValue("@Nonbre", persona.Nombre);
                _insertPersona.Parameters.AddWithValue("@DNI", persona.DNI);
                _insertPersona.Parameters.AddWithValue("@Edad", persona.Edad);
 
                int _resultinsertPer = await _insertPersona.ExecuteNonQueryAsync();


                return _resultinsertPer;
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                this._conexionBD.Close();
            };
        }

        public async Task<bool> EliminarPersona(Persona persona)
        {
            try
            {
                this._conexionBD.Open();

                //hacemos insert en tabla pedido...
                SqlCommand _eliminarPersona = new SqlCommand();
                _eliminarPersona.Connection = this._conexionBD;

                string sql = string.Empty;


                sql = "DELETE FROM Persona WHERE Id = @Id";

                _eliminarPersona.CommandText = sql;

                _eliminarPersona.Parameters.AddWithValue("@Id", persona.Id);


                int _resulElim = await _eliminarPersona.ExecuteNonQueryAsync();

                if (_resulElim > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return false;
                throw;

            }
            finally
            {
                this._conexionBD.Close();
            }
        }

        public Task<int> EditarPersona(Persona persona)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Persona>> BuscarPersonas()
        {
            try
            {
                this._conexionBD.Open();


                string query = "SELECT Id, Nombre, DNI, Edad " +
                                "FROM dbo.Persona  " ;


                SqlCommand _selPer = new SqlCommand(query, this._conexionBD);


                return (await _selPer.ExecuteReaderAsync())
                        .Cast<IDataRecord>()
                        .Select(
                            (IDataRecord fila) => new Persona
                            {


                                Id = Convert.ToInt32(fila["Id"]),
                                Nombre = fila["Nombre"].ToString(),
                                DNI = fila["DNI"].ToString(),
                                Edad = Convert.ToInt32(fila["Edad"]),


                            }
                        ).ToList<Persona>();

            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                this._conexionBD.Close();
            }

        }

        
    }
}

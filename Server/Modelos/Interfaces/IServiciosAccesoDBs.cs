using OverwatchBlazor.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OverwatchBlazor.Server.Modelos.Interfaces
{
    public interface IServiciosAccesoDBs
    {
        Task<int> Registro(Cliente nuevocliente);
        Task<Cliente> Login(Credenciales creds);
        Task<Cliente> GetClienteLogged(String idCliente);
        //Task<List<Provincia>> DameProvincias();
        //Task<List<Municipio>> DameMunicipios(int codprov);
        //Task<int> OperarDireccion(String operacion, Direccion direc, String clientid);
        //Task<List<Categoria>> DameCategorias(Categoria? categoria);
        //Task<List<Producto>> DameProductos(int idcategoria);
        Task<int> CrearPedido(Pedido pedido);

        Task<List<Pedido>> DamePedidos(string idCliente);

        //Task<List<DetalleProducto>> DameProductos(string idPedido);

        //Task<bool> GuardarItemPedido(DetalleProducto detalleProducto, bool nuevo);
        //Task<bool> EliminarItemPedido(DetalleProducto detalleProducto);



        Task<int> CrearPersona(Persona persona);
        Task<bool> EliminarPersona(Persona persona);
        Task<int> EditarPersona(Persona persona);
        Task<List<Persona>> BuscarPersonas();

    }
}

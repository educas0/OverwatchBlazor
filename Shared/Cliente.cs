using HipercorBLAZOR.Shared;
using OverwatchBlazor.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OverwatchBlazor.Shared
{
    public class Cliente
    {
        public int IdCliente { get; set; }

        public String Nickname { get; set; }
        public String Password { get; set; }

        public String Nombre { get; set; }
        public String Apellido { get; set; }
        public Pedido PedidoActual { get; set; }

        List<Producto> ListaProductos { get; set; }

        public List<Pedido> HistoricoPedidos { get; set; }

        public Credenciales CredencialesAcceso { get; set; }

    }
}

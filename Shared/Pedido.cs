using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverwatchBlazor.Shared
{
    public class Pedido
    {    
        #region ....propiedades clase modelo Pedido....
    public String IdPedido { get; set; }
    public String NickClientePedido { get; set; }
    public DateTime FechaPedido { get; set; }
        public int IdHeroe { get; set; }
        public Decimal TotalPedido { get; set; }
        public int Cantidades { get; set; }
        //public List<ItemPedido> ElementosPedido { get; set; }

        #endregion


        #region ...Métodos de Clase...


        public Pedido()
        {
            IdPedido = System.Guid.NewGuid().ToString();
            //ElementosPedido = new List<ItemPedido>();
        }

 

        #endregion




    }



}

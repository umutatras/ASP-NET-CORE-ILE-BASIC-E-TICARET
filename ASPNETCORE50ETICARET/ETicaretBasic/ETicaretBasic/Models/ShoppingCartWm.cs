using System.Collections;
using System.Collections.Generic;

namespace ETicaretBasic.Models
{
    public class ShoppingCartWm
    {
        public IEnumerable<ShoppingCart> ListCart { get; set; }
        public OrderHeader OrderHeader { get; set; }    


    }
}

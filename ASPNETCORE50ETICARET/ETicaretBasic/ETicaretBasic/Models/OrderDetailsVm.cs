using System.Collections.Generic;

namespace ETicaretBasic.Models
{
    public class OrderDetailsVm
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
    }
}

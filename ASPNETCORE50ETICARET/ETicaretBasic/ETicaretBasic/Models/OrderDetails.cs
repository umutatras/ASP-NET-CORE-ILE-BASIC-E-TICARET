using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETicaretBasic.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailsID { get; set; }
        [Required]
        public int OrderID { get; set; }
        [ForeignKey("OrderID")]
        public OrderHeader OrderHeader { get; set; }
        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public Product Product { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }

    }
}

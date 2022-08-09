using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETicaretBasic.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
        }
        [Key]
        public int ShoppingCartID { get; set; }
        public string  ApplicationUserID  { get; set; }
        [ForeignKey("ApplicationUserID")]
        public ApplicationUser  ApplicationUser  { get; set; }
        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public Product Product { get; set; }
        public int Count { get; set; }
        [NotMapped]
        public double Price { get; set; }
    }
}

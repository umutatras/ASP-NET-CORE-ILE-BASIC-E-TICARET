using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETicaretBasic.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public string Image { get; set; }
        [Required]
        public double Price { get; set; }
        public bool HomeStatus { get; set; }
        public bool StockStatus { get; set; }
        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        public Category Category { get; set; }
    }
}

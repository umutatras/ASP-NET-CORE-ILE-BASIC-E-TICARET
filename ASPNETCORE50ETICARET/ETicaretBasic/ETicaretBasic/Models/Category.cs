using System.ComponentModel.DataAnnotations;

namespace ETicaretBasic.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }
        [Required]
        public string CategoryName { get; set; }
    }
}

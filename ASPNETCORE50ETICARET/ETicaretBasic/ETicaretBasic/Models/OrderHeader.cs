using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ETicaretBasic.Models
{
    public class OrderHeader
    {
        [Key]
        public int ShoppingCartID { get; set; }
        public string ApplicationUserID { get; set; }
        [ForeignKey("ApplicationUserID")]
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime  OrderDate { get; set; }
        public double  OrderTotal { get; set; }
        public string  OrderStatus { get; set; }
        [Required]
        public string  Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string İl { get; set; }
        [Required]
        public string İlçe { get; set; }
        [Required]
        public string PostaKodu { get; set; }
        [Required]
        public string CartName { get; set; }
        [Required]
        public string CartNumber { get; set; }
        [Required]
        public string ExpMonth { get; set; }
        [Required]
        public string ExpYear { get; set; }
        [Required]
        public string Cvc { get; set; }
    }
}

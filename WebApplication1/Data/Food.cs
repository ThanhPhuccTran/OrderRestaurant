using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrderRestaurant.Data
{
    [Table("Food")]
    public partial class Food
    {
        [Key]
        public int FoodId { get; set; }
        [Required]
        [MaxLength(100)]
        public string NameFood { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
        [Required]
        public string UrlImage { get; set; }

         public int? CategoryId { get; set; }
         
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
        public List<OrderDetails> OrderDetails { get; set; }
    }
}

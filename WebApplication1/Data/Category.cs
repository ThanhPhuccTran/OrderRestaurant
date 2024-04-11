using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderRestaurant.Data
{
    [Table("Category")]
    public partial class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        [MaxLength(100)]
        public string TenLoai { get; set; } = string.Empty;

        public List<Food> Foods { get; set; } = new List<Food>();
    }
}

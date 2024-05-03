using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderRestaurant.Data
{
    [Table("RefreshToken")]
    public partial class RefreshToken
    {
        [Key]
        public Guid RefreshTokeId { get; set; }
        public int EmployeeId { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public Employee Employees { get; set; }
        public string Token { get; set; }
        public string JwtId { get; set; }
        public bool IsUsed { get; set; } // đã được sử dụng
        public bool IsRevoked { get; set; } // được thu hồi hay chưa
        public DateTime IssueAt { get; set; } // time tạo
        public DateTime ExpiredAt { get; set; } // hết hạn vào lúc nào


    }
}

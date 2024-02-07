using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Domain.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreateAt { get; set;}
        public DateTime? UpdateAt { get; set;}
    }
}
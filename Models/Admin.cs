using System.ComponentModel.DataAnnotations;

namespace deneme.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }

        [Required]
        [MaxLength(100)]
        public string AdminKullaniciAdi { get; set; } = "";

        [Required]
        [MaxLength(255)]
        public string AdminSifre { get; set; } = "";
    }
}


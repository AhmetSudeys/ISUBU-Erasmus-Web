using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace deneme.Models
{
    public class Yorum
    {
        [Key]
        public int YorumId { get; set; }

        //madde 13 server side validation

        // Kullanıcı adı (string olarak saklanır)
        [Required]
        [MaxLength(200)]
        public string KullaniciAdi { get; set; } = "";

        // E-posta adresi
        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public string EmailAdresi { get; set; } = "";

        // Foreign Key - ErasmusProgrami
        [Required]
        public int ErasmusId { get; set; }

        [Required]
        [MinLength(20, ErrorMessage = "Yorum en az 20 karakter olmalıdır.")]
        [MaxLength(200, ErrorMessage = "Yorum en fazla 200 karakter olabilir.")]
        public string YorumYazisi { get; set; } = "";

        [MaxLength(500)]
        public string? Fotograf { get; set; }

        [ForeignKey("ErasmusId")]
        public virtual ErasmusProgrami ErasmusProgrami { get; set; } = null!;
    }
}


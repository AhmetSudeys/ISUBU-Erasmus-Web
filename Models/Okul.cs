using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace deneme.Models
{
    public class Okul
    {
        [Key]
        public int OkulId { get; set; }

        [Required]
        [MaxLength(200)]
        public string OkulAd { get; set; } = "";

        [Required(ErrorMessage = "İnternet sitesi zorunludur.")]
        [MaxLength(500)]
        [Url(ErrorMessage = "Geçerli bir URL giriniz (örn: http://www.example.com veya https://www.example.com)")]
        public string InternetSitesi { get; set; } = "";

        // Foreign Key - Ulke
        [Required]
        public int UlkeId { get; set; }

        // Koordinat bilgileri - Zorunlu (nullable ama controller'da kontrol edilecek)
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Navigation Properties
        [ForeignKey("UlkeId")]
        public virtual Ulke Ulke { get; set; } = null!;

        public virtual ICollection<ErasmusProgrami> ErasmusProgramlari { get; set; } = new List<ErasmusProgrami>();
    }
}


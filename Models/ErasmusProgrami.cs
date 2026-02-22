using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace deneme.Models
{
    public class ErasmusProgrami
    {
        [Key]
        public int ErasmusId { get; set; }

        // Foreign Key - Okul
        [Required]
        public int OkulId { get; set; }

        [MaxLength(200)]
        public string? BolumAdi { get; set; }

        [Required]
        [MaxLength(50)]
        public string ErasmusKodu { get; set; } = "";

        [MaxLength(100)]
        public string? GecerlilikTarihi { get; set; }

        [MaxLength(100)]
        public string? GuzNomisyon { get; set; }

        [MaxLength(100)]
        public string? BaharNomisyon { get; set; }

        [MaxLength(200)]
        public string? AnlasmayaAraciOlan { get; set; }

        [MaxLength(50)]
        public string? TopOgrKont { get; set; }

        [MaxLength(50)]
        public string? TopStajKont { get; set; }

        [MaxLength(50)]
        public string? PersonelDersVerme { get; set; }

        [MaxLength(50)]
        public string? PersonelEgitimAlma { get; set; }

        // Foreign Key - Dil
        [Required]
        public int DilId { get; set; }

        [MaxLength(200)]
        public string? AnlasmaDurumu { get; set; }

        [MaxLength(1000)]
        public string? KontenjanDetayi { get; set; }

        // Navigation Properties
        [ForeignKey("OkulId")]
        public virtual Okul Okul { get; set; } = null!;

        [ForeignKey("DilId")]
        public virtual Dil Dil { get; set; } = null!;

        // Many-to-Many: Bir Erasmus programının birden fazla eğitim seviyesi olabilir
        public virtual ICollection<EgitimSeviyesi> EgitimSeviyeleri { get; set; } = new List<EgitimSeviyesi>();

        // Bir Erasmus programının birden fazla yorumu olabilir
        public virtual ICollection<Yorum> Yorumlar { get; set; } = new List<Yorum>();
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace deneme.Models
{
    public class Dil
    {
        [Key]
        public int DilId { get; set; }

        [Required]
        [MaxLength(100)]
        public string DilAdi { get; set; } = "";

        // Foreign Key - EgitimSeviyesi
        [Required]
        public int EgitimSeviyesiId { get; set; }

        // Navigation
        [ForeignKey("EgitimSeviyesiId")]
        public virtual EgitimSeviyesi EgitimSeviyesi { get; set; } = null!;

        public virtual ICollection<ErasmusProgrami> ErasmusProgramlari { get; set; } = new List<ErasmusProgrami>();
    }
}


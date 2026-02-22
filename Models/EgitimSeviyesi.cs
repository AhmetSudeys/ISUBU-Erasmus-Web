using System.ComponentModel.DataAnnotations;

namespace deneme.Models
{
    public class EgitimSeviyesi
    {
        [Key]
        public int EgitimSeviyesiId { get; set; }

        [Required]
        [MaxLength(100)]
        public string EgitimSeviyesiAdi { get; set; } = "";

        public virtual ICollection<ErasmusProgrami> ErasmusProgramlari { get; set; } = new List<ErasmusProgrami>();
        public virtual ICollection<Dil> Diller { get; set; } = new List<Dil>();
    }
}


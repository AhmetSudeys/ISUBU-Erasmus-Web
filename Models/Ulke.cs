using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace deneme.Models
{
    public class Ulke
    {
        [Key]
        public int UlkeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string UlkeIsim { get; set; } = "";

        // Navigation Property - Bir ülkenin birden fazla okulu olabilir
        public virtual ICollection<Okul> Okullar { get; set; } = new List<Okul>();
    }
}


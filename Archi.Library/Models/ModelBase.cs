using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Archi.Library.Models
{
    public abstract class ModelBase
    {
        public int ID { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Name { get; set; }

        [RegularExpression(@"^[a-zA-Z\s]*$")]
        [Required]
        [StringLength(30)]
        public string Type { get; set; }

        [Range(1, 100)]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        private int _rating;  // Backing store
        public int Rating
        {
            get => _rating;
            set
            {
                if ((value > 0) && (value < 5))
                {
                    _rating = value;
                }
            }
        }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public bool Active { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archi.Library.Models;


namespace Archi.API.Model
{
    public class Product : ModelBase
    {
        private int _rating;  // Backing store

        //public int ID { get; set; }
        //public string Name { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public int Rating
        {
            get => _rating;
            set
            {
                if ((value > 0) && (value < 10))
                {
                    _rating = value;
                }
            }
        }
        public DateTime Date { get; set; }

    }
}
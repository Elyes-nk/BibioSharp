using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Archi.Library.Models;


namespace Archi.API.Model
{
    public class Pizza : ModelBase
    {
        //public int ID { get; set; }
        [Required]
        //public string Name { get; set; }
        public decimal Price { get; set; }
        public string Topping { get; set; }
    }
}
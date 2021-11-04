using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Archi.Library.Models;


namespace Archi.API.Model
{
    public class Customer : ModelBase
    {
        //public int Id { get; set; }
        public String Email { get; set; }
        public String Phone { get; set; }
        [Required]
        public String Lastname { get; set; }
        public String Firstname { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Formservicecrud.Models
{
    public class Dropdown
    {

        public int id { get; set; }

        [Required]
        public string dropdown { get; set; }

        [Required(ErrorMessage = "Select Gender")]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

       
        [Display(Name = "Date")]
        public string Date { get; set; }

        [Display(Name = "Image")]
        public string Image { get; set; }



    }
}
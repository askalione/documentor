using Documentor.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Documentor.Models
{
    public class PageModifyCommand
    {
        [Required]
        [Display(Name = "Current virtual path")]
        public string VirtualPath { get; set; }
        [Required]
        [Display(Name = "Virtual name")]
        [RegularExpression("[0-9a-zA-Z-]+", ErrorMessage = "Invalid characters")]
        public string VirtualName { get; set; }
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}

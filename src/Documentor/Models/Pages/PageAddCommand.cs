using System.ComponentModel.DataAnnotations;

namespace Documentor.Models.Pages
{
    public class PageAddCommand
    {
        [Display(Name = "Parent virtual path")]
        public string ParentVirtualPath { get; set; } = default!;

        [Required]
        [Display(Name = "Virtual name")]
        [RegularExpression("[0-9a-zA-Z-]+", ErrorMessage = "Invalid characters")]
        public string VirtualName { get; set; } = default!;

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; } = default!;

        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
}

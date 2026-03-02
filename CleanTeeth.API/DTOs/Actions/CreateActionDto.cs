using System.ComponentModel.DataAnnotations;

namespace CleanTeeth.API.DTOs.Actions
{
    public class CreateActionDto
    {
        [Required]
        [StringLength(256)]
        public required string Name { get; set; }

        [Required]
        [StringLength(256)]
        public required string Title { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace CleanTeeth.API.DTOs.Roles
{
    public class CreateRoleDto
    {
        [Required]
        [StringLength(256)]
        public required string Title { get; set; }
    }
}

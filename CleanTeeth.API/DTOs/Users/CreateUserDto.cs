using System.ComponentModel.DataAnnotations;

namespace CleanTeeth.API.DTOs.Users
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(256)]
        public required string Login { get; set; }
    }
}

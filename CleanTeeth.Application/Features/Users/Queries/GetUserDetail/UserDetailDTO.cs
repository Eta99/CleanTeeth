namespace CleanTeeth.Application.Features.Users.Queries.GetUserDetail
{
    public class UserDetailDTO
    {
        public long Id { get; set; }
        public required string Login { get; set; }
        public List<RoleDTO> Roles { get; set; } = new();
        public List<ActionDTO> Actions { get; set; } = new();
    }
}

namespace CleanTeeth.Application.Features.Roles.Queries.GetRoleDetail
{
    public class RoleDetailDTO
    {
        public long Id { get; set; }
        public required string Title { get; set; }
        public List<ActionDTO> Actions { get; set; } = new();
    }
}

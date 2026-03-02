namespace CleanTeeth.Application.Features.Users.Queries.GetUsersList
{
    public class UserListDTO
    {
        public long Id { get; set; }
        public required string Login { get; set; }
    }
}

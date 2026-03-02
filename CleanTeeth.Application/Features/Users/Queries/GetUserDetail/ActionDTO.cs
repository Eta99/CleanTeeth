namespace CleanTeeth.Application.Features.Users.Queries.GetUserDetail
{
    public class ActionDTO
    {
        public long Id { get; set; }
        public long TypeId { get; set; }
        public required string Name { get; set; }
        public required string Title { get; set; }
    }
}

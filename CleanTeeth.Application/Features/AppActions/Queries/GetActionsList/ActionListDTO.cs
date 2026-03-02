namespace CleanTeeth.Application.Features.AppActions.Queries.GetActionsList
{
    public class ActionListDTO
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public required string Title { get; set; }
    }
}

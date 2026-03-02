using CleanTeeth.Domain.Entities;

namespace CleanTeeth.Application.Features.AppActions.Queries.GetActionDetail
{
    internal static class MapperExtensions
    {
        public static ActionDetailDTO ToDTO(this AppAction action)
        {
            return new ActionDetailDTO { Id = action.Id, Name = action.Name, Title = action.Title };
        }
    }
}

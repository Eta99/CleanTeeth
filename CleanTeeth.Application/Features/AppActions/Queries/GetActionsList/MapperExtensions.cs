using CleanTeeth.Domain.Entities;

namespace CleanTeeth.Application.Features.AppActions.Queries.GetActionsList
{
    internal static class MapperExtensions
    {
        public static ActionListDTO ToDTO(this AppAction action)
        {
            return new ActionListDTO { Id = action.Id, Name = action.Name, Title = action.Title };
        }
    }
}

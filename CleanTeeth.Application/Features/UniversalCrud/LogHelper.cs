namespace CleanTeeth.Application.Features.UniversalCrud
{
    internal static class LogHelper
    {
        /// <summary>Получить Id сущности в виде строки (Guid или long).</summary>
        public static string GetEntityId(object entity)
        {
            var idProp = entity.GetType().GetProperty("Id");
            var id = idProp?.GetValue(entity);
            return id?.ToString() ?? string.Empty;
        }
    }
}

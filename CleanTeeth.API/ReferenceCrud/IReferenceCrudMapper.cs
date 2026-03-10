namespace CleanTeeth.API.ReferenceCrud
{
    /// <summary>
    /// Маппер для CRUD справочника: сущность ↔ DTO.
    /// </summary>
    public interface IReferenceCrudMapper<TEntity, TListDto, TDetailDto, TCreateDto, TUpdateDto>
    {
        TListDto ToListDto(TEntity entity);
        TDetailDto ToDetailDto(TEntity entity);
        TEntity ToEntity(TCreateDto dto);
        void ApplyUpdate(TEntity entity, TUpdateDto dto);
        /// <summary>
        /// Идентификатор сущности (для Location в 201 Created).
        /// </summary>
        object GetId(TEntity entity);
    }
}

namespace CleanTeeth.Application.Contracts.Repositories
{
    public interface IRepositoryLongKey<T> where T : class
    {
        Task<T?> GetById(long id);
        Task<IEnumerable<T>> GetAll();
        Task<T> Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        Task<int> GetTotalAmountOfRecords();
    }
}

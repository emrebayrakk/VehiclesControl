namespace VehiclesControl.Domain.Interfaces.Dapper
{
    public interface IGenericRepository<T> where T : class, new()
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<bool> InsertAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(long id);
        Task<T> GetByIdAsync(long id);
    }
}

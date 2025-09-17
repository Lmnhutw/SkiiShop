namespace Core.Abstractions
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T?> GetByIdAsync(int id);

        void Add(T entity);

        void Update(T entity);

        void Delete(T entity);

        Task<bool> ItemExists(int id);

        Task<bool> SaveChanges();
    }
}
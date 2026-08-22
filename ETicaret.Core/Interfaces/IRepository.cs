using System.Linq.Expressions;

namespace ETicaret.Core.Interfaces
{
    public interface IRepository<T> where T : class // T herhangi bir sınıf yerine geçer. Product, Category, Order gibi. 
    {
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);

        Task<T?> GetByIdAsync(int id);

        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes); // Arama filtreleme.

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);

        Task SaveChangesAsync();
    }
}
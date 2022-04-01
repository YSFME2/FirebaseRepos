using System.Collections.Generic;
using System.Threading.Tasks;

namespace FirebaseRepos.Base
{
    public interface IRepository<T> where T : IFireBaseClass
    {
        Task<T> GetAsync(string id);
        T Get(string id);
        Task<List<T>> GetAllAsync();
        List<T> GetAll();
        Task<string> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity, string id);
        Task<bool> DeleteAsync(string id);
        void RemoveListener();
        void RemoveListener<B>();
    }
}

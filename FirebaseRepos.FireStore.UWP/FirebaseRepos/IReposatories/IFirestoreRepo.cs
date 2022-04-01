using FirebaseRepos.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FirebaseRepos.IReposatories
{
    public interface IFirestoreRepo<T> : IRepository<T> where T : IFireBaseClass
    {
        Task<List<T>> GetAsync(List<(string fieldName, object value)> filterFields);

        void SetListener(Action<T> action);
        void SetListener<B>(Action<B> action, string path) where B : IFireBaseClass;
    }
}

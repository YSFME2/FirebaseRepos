using Firebase.Database;
using Firebase.Database.Streaming;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FirebaseRepos.Reposatories
{
    public interface IFirestoreRepo<T> : IRepository<T>
    {
        Task<List<T>> GetAsync(List<(string fieldName, object value)> filterFields);

        void SetListener(Action<T> action);
        void SetListener<B>(Action<B> action, string path) where B : new();
    }
}

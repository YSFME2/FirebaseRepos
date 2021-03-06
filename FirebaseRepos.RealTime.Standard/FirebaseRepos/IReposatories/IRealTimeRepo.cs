using Firebase.Database;
using Firebase.Database.Streaming;
using FirebaseRepos.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FirebaseRepos.IReposatories
{
    public interface IRealTimeRepo<T> : IRepository<T> where T : IFireBaseClass
    {
        Task<List<T>> GetAsync(Func<FirebaseObject<T>, bool> predec);
        void SetListener(Action<FirebaseEvent<T>> action);
        void SetListener<B>(Action<FirebaseEvent<B>> action, string path);
    }
}
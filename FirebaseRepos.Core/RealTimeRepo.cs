using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Database.Streaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FirebaseRepos.Core.Reposatories
{
    public class RealTimeRepo<T> : IRealTimeRepo<T>, IDisposable where T : class
    {
        private IDisposable _mainListener;
        private IDisposable _subListener;
        public readonly ChildQuery Child;
        public RealTimeRepo(ChildQuery child)
        {
            Child = child;
        }
        public async Task<string> AddAsync(T entity)
        {
            try
            {
                string id = (await Child.PostAsync<T>(entity)).Key;
                if (entity.GetType().GetProperties().Any(x => x.Name.ToUpper() == "ID"))
                {
                    await Child.Child(id).Child("ID").PutAsync<string>(id);
                }
                return id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                await Child.Child(id).DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<List<T>> GetAllAsync()
        {
            try
            {
                return (await Child.OnceAsync<T>()).Select(x => x.Object).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<T> GetAsync(string id)
        {
            try
            {
                return await Child.Child(id).OnceSingleAsync<T>();
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public T Get(string id)
        {
            try
            {
                return Child.Child(id).OnceSingleAsync<T>().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }
        public List<T> GetAll()
        {
            try
            {
                return Child.OnceAsync<T>().GetAwaiter().GetResult().Select(x => x.Object).ToList();
            }
            catch (Exception ex)
            {
                return new List<T>();
            }
        }

        public async Task<List<T>> GetAsync(Func<FirebaseObject<T>, bool> predec)
        {
            try
            {
                return (await Child.OnceAsync<T>()).Where(predec).Select(x => x.Object).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<List<T>> GetAsync(string propertyName, object value)
        {
            if (typeof(T).GetProperties().Any(p => p.Name == propertyName))
                return (await Child.OrderBy(propertyName).EqualTo(value + "").OnceAsync<T>()).Select(x => x.Object).ToList();
            return new List<T>();
        }

        public void RemoveListener()
        {
            if (_mainListener != null)
                _mainListener.Dispose();
        }

        public void RemoveListener<B>()
        {
            if (_subListener != null)
                _subListener.Dispose();
        }

        public void SetListener<B>(Action<FirebaseEvent<B>> action, string path)
        {
            _subListener = Child.Child(path).AsObservable<B>().Subscribe(action);
        }

        public void SetListener(Action<FirebaseEvent<T>> action)
        {
            _mainListener = Child.AsObservable<T>().Subscribe(action);
        }

        public async Task<bool> UpdateAsync(T entity, string id)
        {
            try
            {
                await Child.Child(id).PutAsync<T>(entity);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
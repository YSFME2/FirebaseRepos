using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Database.Streaming;
using FirebaseRepos.Base;
using FirebaseRepos.IReposatories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FirebaseRepos.RealTime
{
    public class RealTimeRepo<T> : IRealTimeRepo<T>, IDisposable where T : IFireBaseClass
    {
        private IDisposable _mainListener;
        private IDisposable _LocalListener;
        private IDisposable _subListener;
        public readonly ChildQuery Child;
        public bool UseLocalLestiner { get; set; }
        private ObservableCollection<T> local;

        public ObservableCollection<T> Local
        {
            get
            {
                if (local == null)
                {
                    local = new ObservableCollection<T>();
                    if (UseLocalLestiner)
                        SetLocalLestiner();
                }
                return local;
            }
        }

        private void SetLocalLestiner()
        {
            _LocalListener = Child.AsObservable<T>().Subscribe<FirebaseEvent<T>>(x =>
            {
                if (x.EventType == FirebaseEventType.Delete)
                {
                    local.Remove(local.FirstOrDefault(y => y.ID == x.Object.ID));
                }
                else
                {
                    var obj = local.FirstOrDefault(y => y.ID == x.Object.ID);
                    if (obj == null)
                        local.Add(x.Object);
                    else
                    {
                        var index = local.IndexOf(obj);
                        local.RemoveAt(index);
                        local.Insert(index, x.Object);
                    }
                }
                GC.Collect();

            });
        }

        public RealTimeRepo(ChildQuery child, bool useLocalListener = false)
        {
            Child = child;
            UseLocalLestiner = useLocalListener;
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
            if (_LocalListener != null)
                _LocalListener.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<List<T>> GetAllAsync()
        {
            try
            {
                var list = (await Child.OnceAsync<T>());
                foreach (var item in list)
                {
                    item.Object.ID = item.Key;
                }
                return list.Select(x => x.Object).ToList();
            }
            catch (Exception ex)
            {
                return new List<T>();
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
                var x = Child.Child(id).OnceSingleAsync<T>().GetAwaiter().GetResult();
                x.ID = id;
                return x;
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
                var list = Child.OnceAsync<T>().GetAwaiter().GetResult();
                foreach (var item in list)
                {
                    item.Object.ID = item.Key;
                }
                return list.Select(x => x.Object).ToList();
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
                return new List<T>();
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
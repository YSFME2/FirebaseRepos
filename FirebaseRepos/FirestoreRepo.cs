using Firebase.Database;
using Firebase.Database.Streaming;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static FirebaseRepos.SnapshotSerialization;

namespace FirebaseRepos.Reposatories
{
    public class FirestoreRepo<T> : IFirestoreRepo<T>, IDisposable where T : new()
    {
        private readonly CollectionReference _collection;
        private FirestoreChangeListener main;
        private FirestoreChangeListener sub;
        public FirestoreRepo(CollectionReference collection)
        {
            _collection = collection;
        }
        public async Task<string> AddAsync(T entity)
        {
            Dictionary<string, object> entityDic = new Dictionary<string, object>();
            foreach (var property in entity.GetType().GetProperties())
            {
                entityDic.Add(property.Name, property.GetValue(entity));
            }
            var refrance = await _collection.AddAsync(entityDic);
            entityDic.Clear();
            entityDic.Add("Id", refrance.Id);
            await refrance.UpdateAsync(entityDic);
            return refrance.Id;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            await _collection.Document(id).DeleteAsync();
            return true;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(_collection);
            GC.SuppressFinalize(this);
        }

        public async Task<List<T>> GetAllAsync()
        {
            var snapshots = await _collection.GetSnapshotAsync();
            return DeserializeSnapshots<T>(snapshots);
        }


        public async Task<List<T>> GetAsync(List<(string fieldName, object value)> filterFields)
        {
            if (filterFields.Count > 0)
            {
                var query = _collection.WhereArrayContains(filterFields[0].fieldName, filterFields[0].value);
                for (int i = 1; i < filterFields.Count; i++)
                {
                    query = query.WhereArrayContains(filterFields[i].fieldName, filterFields[i].value);
                }
                var snapshots = await query.GetSnapshotAsync();
                return DeserializeSnapshots<T>(snapshots);
            }
            else
                return await GetAllAsync();
        }

        public async Task<T> GetAsync(string id)
        {
            var snapshot = await _collection.Document(id).GetSnapshotAsync();
            if (snapshot.Exists)
            {
                return DeserializeSnapshot<T>(snapshot);
            }
            return default(T);
        }
        public T Get(string id)
        {
            var snapshot = _collection.Document(id).GetSnapshotAsync().GetAwaiter().GetResult();
            if (snapshot.Exists)
            {
                return DeserializeSnapshot<T>(snapshot);
            }
            return default(T);
        }


        public void RemoveListener()
        {
            if (main != null)
                main.StopAsync();
        }

        public void RemoveListener<B>()
        {
            if (sub != null)
                sub.StopAsync();
        }

        public void SetListener(Action<T> action)
        {
            main = _collection.Document().Listen((DocumentSnapshot) => { action(DeserializeSnapshot<T>(DocumentSnapshot)); });
        }

        public void SetListener<B>(Action<B> action, string path) where B : new()
        {
           sub = _collection.Document(path).Listen((DocumentSnapshot) => { action(DeserializeSnapshot<B>(DocumentSnapshot)); });
        }

        public async Task<bool> UpdateAsync(T entity, string id)
        {
            Dictionary<string, object> entityDic = new Dictionary<string, object>();
            foreach (var property in entity.GetType().GetProperties())
            {
                entityDic.Add(property.Name, property.GetValue(entity));
            }
            await _collection.Document(id).UpdateAsync(entityDic);
            return true;
        }

        public List<T> GetAll()
        {
            var snapshots = _collection.GetSnapshotAsync().GetAwaiter().GetResult();
            return DeserializeSnapshots<T>(snapshots);
        }
    }
}
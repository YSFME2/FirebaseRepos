using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FirebaseRepos.Base.Extensions
{
    public class SnapshotSerialization
    {
        public static List<T> DeserializeSnapshots<T>(QuerySnapshot snapshots) where T : IFireBaseClass
        {
            List<T> result = new List<T>();
            foreach (var snapshot in snapshots.Documents)
            {
                result.Add(DeserializeSnapshot<T>(snapshot));
            }
            return result;
        }

        public static T DeserializeSnapshot<T>(DocumentSnapshot snapshot) where T : IFireBaseClass
        {
            dynamic value = snapshot.ConvertTo<dynamic>();
            var result = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(value));
            var p = typeof(T).GetProperty("ID");
            if (p != null && string.IsNullOrEmpty(p.GetValue(result) + ""))
            {
                p.SetValue(result, snapshot.Id);
            }
            return result;
        }
    }
}

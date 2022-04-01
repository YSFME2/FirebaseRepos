using System;
using System.Collections.Generic;
using System.Linq;

namespace FirebaseRepos.Base.Extensions
{
    public static class Extensions
    {
        public static Dictionary<string, object> GetDec(this IFireBaseClass entity, params string[] exceptionsFields)
        {
            Dictionary<string, object> entityDic = new Dictionary<string, object>();
            foreach (var property in entity.GetType().GetProperties())
            {
                if (exceptionsFields.Contains(property.Name))
                    continue;
                if (property.Name == nameof(entity.ID))
                    entityDic.Add("id", property.GetValue(entity));
                else
                {
                    var x = property.Name.Length > 0 ? (property.Name.Substring(0, 1).ToLower() + (property.Name.Length > 1 ? property.Name.Substring(1) : "")) : "";
                    if (property.PropertyType.IsEnum)
                    {
                        var uu = Enum.Parse(property.PropertyType, property.GetValue(entity) + "").ToString();
                        entityDic.Add(x, uu);
                    }
                    else
                        entityDic.Add(x, property.GetValue(entity));
                }
            }
            return entityDic;
        }
        public static Dictionary<string, object>[] GetDecs<T>(this List<T> entities, params string[] exceptionsFields) where T : IFireBaseClass
        {
            Dictionary<string, object>[] list = new Dictionary<string, object>[entities.Count];
            for (int i = 0; i < entities.Count; i++)
            {
                list[i] = GetDec(entities[i], exceptionsFields);
            }
            return list;
        }
    }
}

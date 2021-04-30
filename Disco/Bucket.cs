using System;
using System.Collections.Generic;

namespace Disco
{
    public class Bucket<T> : Singleton<Bucket<T>>
    {
        internal Dictionary<string, T> bucketContents = new Dictionary<string, T>();

        public static T GetItem(string id)
        {
            if (Instance.bucketContents.TryGetValue(id, out T value))
                throw new Exception($"Bucket item of type {typeof(T).Name} has no value with id {id}");
            return value;
        }
    }
}

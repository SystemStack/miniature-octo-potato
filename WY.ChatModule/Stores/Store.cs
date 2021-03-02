using ChatModule.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ChatModule.Stores
{
    public sealed class Store<T> : IStore<T>
        where T : IModel
    {
        //userKey : internalKey
        private readonly ConcurrentDictionary<string, string> Keys;
        //internalKey: data model
        private readonly ConcurrentDictionary<string, T> Data;

        public class KeyComparer : IEqualityComparer<string>
        {
            bool IEqualityComparer<string>.Equals(string x, string y)
                => x.Trim().ToLowerInvariant() == y.Trim().ToLowerInvariant();
            int IEqualityComparer<string>.GetHashCode(string obj)
                => obj.GetHashCode();
        }
        public Store()
        {
            Keys = new ConcurrentDictionary<string, string>(Environment.ProcessorCount * 2, 100, new KeyComparer());
            Data = new ConcurrentDictionary<string, T>(Environment.ProcessorCount * 2, 100, new KeyComparer());
        }

        public bool Exists(string key)
        {
            Utils.IsNotNull(key, nameof(key));
            return Data.ContainsKey(key) || Keys.ContainsKey(key);
        }

        public T Get(string key)
        {
            Utils.IsNotNull(key, nameof(key));
            if (Data.TryGetValue(key, out var Result))
            {
                return Result;
            }
            else if (Keys.TryGetValue(key, out var internalKey))
            {
                return Get(internalKey);
            }
            throw new KeyNotFoundException(string.Format("No {0} with key: {1} found", typeof(T), key));
        }

        public bool Add(T element)
        {
            Utils.IsNotNullModel(element, nameof(element));
            if (Data.TryAdd(element.InternalKey, element))
            {
                return Keys.TryAdd(element.UserKey, element.InternalKey);
            }
            return false;
        }
        public bool Update(string key, T newValue, T oldValue = default)
        {
            Utils.IsNotNull(key, nameof(key));
            Utils.IsNotNullModel(newValue, nameof(newValue));
            if (oldValue.Equals(default(T)))
            {
                Data.TryGetValue(key, out oldValue);
            }
            return Data.TryUpdate(key, newValue, oldValue);
        }

        public bool Update(string key, T newValue)
        {
            Utils.IsNotNull(key, nameof(key));
            Utils.IsNotNullModel(newValue, nameof(newValue));
            Data.TryRemove(key, out _);
            return Data.TryAdd(key, newValue);
        }

        public bool Remove(string key, out T RemovedElement)
        {
            Utils.IsNotNull(key, nameof(key));
            if (Data.ContainsKey(key))
            {
                Data.TryRemove(key, out RemovedElement);
                return Keys.TryRemove(RemovedElement.UserKey, out _);
            }
            else if (Keys.ContainsKey(key))
            {
                Keys.TryRemove(key, out var RemovedInternalKey);
                return Data.TryRemove(RemovedInternalKey, out RemovedElement);
            }
            throw new Exception("Failed to remove element");
        }

        public int Populate(IEnumerable<T> elements)
        {
            var count = 0;
            foreach (var e in elements)
            {
                if (Add(e))
                {
                    count++;
                }
            }
            return count;
        }
    }
}

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

        public Store()
        {
            Keys = new ConcurrentDictionary<string, string>(Environment.ProcessorCount * 2, 100);
            Data = new ConcurrentDictionary<string, T>(Environment.ProcessorCount * 2, 100);
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
            } else if (Keys.TryGetValue(key, out var internalKey)) 
            {
                return Get(internalKey);
            }
            throw new KeyNotFoundException(string.Format("No {0} with key: {1} found", typeof(T), key));
        }

        public bool GetByUserKey(string userKey, out string internalKey)
        {
            Utils.IsNotNull(userKey, nameof(userKey));
            return Keys.TryGetValue(userKey, out internalKey);
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
        public bool Update(string key, T newValue, T oldValue)
        {
            Utils.IsNotNull(key, nameof(key));
            Utils.IsNotNullModel(newValue, nameof(newValue));
            Utils.IsNotNullModel(oldValue, nameof(oldValue));
            return Data.TryUpdate(key, newValue, oldValue);
        }
        
        public bool Remove(string key, out T RemovedElement)
        {
            Utils.IsNotNull(key, nameof(key));
            if (Data.ContainsKey(key))
            {
                Data.TryRemove(key, out RemovedElement);
                return Keys.TryRemove(RemovedElement.UserKey, out _);
            } else if (Keys.ContainsKey(key))
            {
                Keys.TryRemove(key, out var RemovedInternalKey);
                return Data.TryRemove(RemovedInternalKey, out RemovedElement);
            }
            throw new Exception("Failed to remove element");
        }

        public int Populate(IEnumerable<T> elements)
        {
            var count = 0;
            foreach(var e in elements)
            {
                if(Add(e))
                {
                    count++;
                }
            }
            return count;
        }
    }
}

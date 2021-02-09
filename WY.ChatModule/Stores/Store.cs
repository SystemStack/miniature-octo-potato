using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ChatModule.Models;

namespace ChatModule.Stores
{
    public sealed class Store<T> : IStore<T>
        where T : IModel
    {
        //userKey : internalKey
        private readonly ConcurrentDictionary<string, string> Keys;
        //internalKey: data model
        private readonly ConcurrentDictionary<string, T> Data;

        public Store() {
            Keys = new ConcurrentDictionary<string, string>(Environment.ProcessorCount * 2, 100);
            Data = new ConcurrentDictionary<string, T>(Environment.ProcessorCount * 2, 100);
        }
        public bool Exists(string key)
        {
            Utils.IsNotNull(key, nameof(key));
            return Data.ContainsKey(key);
        }
        public T Get(string key)
        {
            Utils.IsNotNull(key, nameof(key));
            if(!Data.TryGetValue(key, out T Result))
            {
                throw new KeyNotFoundException(string.Format("No {0} with key: {1} found", typeof(T), key));
            }
            return Result;
        }
        public bool GetByUserKey(string userKey, out string internalKey)
        {
            Utils.IsNotNull(userKey, nameof(userKey));
            return Keys.TryGetValue(userKey, out internalKey);
        }
        public bool Add(string key, T element)
        {
            Utils.IsNotNull(key, nameof(key));
            Utils.IsNotNullT(element, nameof(element));
            if(Data.TryAdd(key, element))
            {
                return Keys.TryAdd(element.UserKey, element.InternalKey);
            }
            return false;
        }
        public bool Update(string key, T newValue, T oldValue)
        {
            Utils.IsNotNull(key, nameof(key));
            Utils.IsNotNullT(newValue, nameof(newValue));
            Utils.IsNotNullT(oldValue, nameof(oldValue));
            return Data.TryUpdate(key, newValue, oldValue);
        }
        public bool Remove(string key, out T RemovedElement)
        {
            Utils.IsNotNull(key, nameof(key));
            return Data.TryRemove(key, out RemovedElement);
        }

#pragma warning disable IDE0051 // Remove unused private members
        private T Mapper(string key)
        {
#pragma warning restore IDE0051 // Remove unused private members
            throw new NotImplementedException();
        }

#pragma warning disable IDE0051 // Remove unused private members
        private T NearestNeighbor(string key)
#pragma warning restore IDE0051 // Remove unused private members
        {
            throw new NotImplementedException();
        }
    }
}

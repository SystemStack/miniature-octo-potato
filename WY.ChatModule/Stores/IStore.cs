using ChatModule.Models;

namespace ChatModule.Stores
{
    public interface IStore<T>
        where T : IModel
    {
        public bool Add(T Element);
        public bool Exists(string key);
        public T Get(string key);
        public bool Update(string key, T newValue, T oldValue);
        public bool Remove(string key, out T RemovedElement);
    }
}

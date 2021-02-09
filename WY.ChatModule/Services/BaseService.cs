using System;
using ChatModule.Models;
using ChatModule.Stores;

namespace ChatModule.Services
{
    public interface IService<T1, T2>
        where T1 : IModel
    {
        public Store<T1> Store { get; set; }
        public T2 Client { get; set; }
    }

    public class BaseService<T1, T2> : IService<T1, T2>
        where T1 : IModel
    {
        public Store<T1> Store { get; set; }
        public T2 Client { get; set; }
        public BaseService(IServiceProvider serviceProvider)
        {
            Store = (Store<T1>)serviceProvider.GetService(typeof(Store<T1>));
        }
    }
}

using ChatModule.Models;
using ChatModule.Stores;
using System;

namespace ChatModule.Services
{
    public interface IService<T1, T2>
        where T1 : IModel
    {
        public IServiceProvider ServiceProvider { get; }
        public Store<T1> Store { get; set; }
        public T2 Client { get; set; }
    }


    public class BaseService<T1, T2> : IService<T1, T2>
        where T1 : IModel
    {
        public IServiceProvider ServiceProvider { get; }
        public Store<T1> Store { get; set; }
        public T2 Client { get; set; }
        public BaseService(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Store = (Store<T1>)serviceProvider.GetService(typeof(Store<T1>));
        }
    }
}

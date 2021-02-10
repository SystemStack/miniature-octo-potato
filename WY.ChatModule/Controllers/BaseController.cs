using System;

namespace ChatModule.Controllers
{
    public class BaseController<T> : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        public T Service { get; }
        public BaseController(IServiceProvider serviceProvider) : base()
        {
            Service = (T)serviceProvider.GetService(typeof(T));
        }
    }
}

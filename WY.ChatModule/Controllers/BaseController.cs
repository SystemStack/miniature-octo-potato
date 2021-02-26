using ChatModule.Services;
using System;

namespace ChatModule.Controllers
{
    public class BaseController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        public UserService Service { get; }
        public BaseController(IServiceProvider serviceProvider) : base()
        {
            Service = (UserService)serviceProvider.GetService(typeof(UserService));
        }
    }
    public class BaseController<T> : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        public UserService UserService { get; }
        public T Service { get; }
        public BaseController(IServiceProvider serviceProvider) : base()
        {
            UserService = (UserService)serviceProvider.GetService(typeof(UserService));
            Service = (T)serviceProvider.GetService(typeof(T));
        }
    }
}

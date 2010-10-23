namespace Mvc3Ninject.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Ninject;

    [System.Diagnostics.DebuggerStepThrough]
    public class NinjectResolver : IDependencyResolver
    {
        private static IKernel kernel;

        public NinjectResolver()
        {
            kernel = new StandardKernel();
            RegisterServices(kernel);
        }

        public NinjectResolver(IKernel myKernel)
        {
            kernel = myKernel;
            RegisterServices(kernel);
        }

        public static void RegisterServices(IKernel kernel)
        {
            //kernel.Bind<IThingRepository>().To<SqlThingRepository>();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }
    }

}
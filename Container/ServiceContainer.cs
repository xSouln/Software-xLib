using System;
using System.Collections.Generic;

namespace xLibV100
{
    public static class Services
    {
        private static readonly Dictionary<string, RegisteredService> services = new Dictionary<string, RegisteredService>();

        public static int RegisterBySignature(string signature, object service)
        {
            if (signature == null || signature.Length == 0 || service == null)
            {
                throw new Exception("xLibV100.Container.Services: RegisterBySignature");
            }

            if (services.ContainsKey(signature))
            {
                throw new Exception("xLibV100.Container.Services: RegisterBySignature");
            }

            services[signature] = new RegisteredService(service);

            return 0;
        }

        public static TService GetBySignature<TService>(string signature) where TService : class
        {
            if (!services.ContainsKey(signature))
            {
                throw new Exception("xLibV100.Container.Services: GetBySignature");
            }

            return services[signature].Service as TService;
        }

        public static int RegisterByType(object service)
        {
            if (service == null)
            {
                throw new Exception("xLibV100.Container.Services: RegisterByType");
            }

            var type = service.GetType();

            if (services.ContainsKey(type.FullName))
            {
                throw new Exception("xLibV100.Container.Services: RegisterByType");
            }

            services[type.FullName] = new RegisteredService(service);

            return 0;
        }

        public static TService GetByType<TService>() where TService : class
        {
            var type = typeof(TService);

            if (!services.ContainsKey(type.FullName))
            {
                throw new Exception("xLibV100.Container.Services: GetByType");
            }

            return services[type.FullName].Service as TService;
        }
    }
}

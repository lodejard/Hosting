using System;
using Microsoft.AspNet.Http.Features.Internal;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.AspNet.Hosting.Internal
{
    public class RequestServicesContainerFeature : IServiceProvidersFeature, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        IServiceProvider _requestServices;
        private bool _requestServicesAssigned;
        private IServiceScope _serviceScope;

        public RequestServicesContainerFeature(IServiceProvider applicationServices)
        {
            ApplicationServices = applicationServices;
        }

        public RequestServicesContainerFeature(IServiceProvider applicationServices, IServiceScopeFactory serviceScopeFactory) 
        {
            ApplicationServices = applicationServices;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public IServiceProvider ApplicationServices { get; set; }

        public IServiceProvider RequestServices
        {
            get
            {
                if (!_requestServicesAssigned)
                {
                    _serviceScope = _serviceScopeFactory.CreateScope();
                    _requestServices = _serviceScope.ServiceProvider;
                    _requestServicesAssigned = true;
                }
                return _requestServices;
            }

            set
            {
                _requestServicesAssigned = true;
                _requestServices = value;
            }
        }

        public void Dispose()
        {
            if (_serviceScope != null)
            {
                _serviceScope.Dispose();
                _serviceScope = null;
            }
            _requestServices = null;
        }
    }
}

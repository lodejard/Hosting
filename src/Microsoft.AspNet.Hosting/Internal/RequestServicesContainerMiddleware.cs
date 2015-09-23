// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;
using Microsoft.AspNet.Http.Features.Internal;
using Microsoft.AspNet.Http.Features;

namespace Microsoft.AspNet.Hosting.Internal
{
    public class RequestServicesContainerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _applicationServices;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RequestServicesContainerMiddleware(
            RequestDelegate next,
            IServiceProvider applicationServices,
            IServiceScopeFactory serviceScopeFactory)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (applicationServices == null)
            {
                throw new ArgumentNullException(nameof(applicationServices));
            }

            if (serviceScopeFactory == null)
            {
                throw new ArgumentNullException(nameof(serviceScopeFactory));
            }

            _next = next;
            _applicationServices = applicationServices;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var existingFeature = httpContext.Features.Get<IServiceProvidersFeature>();

            // All done if there request services is set
            if (existingFeature != null && existingFeature.RequestServices != null)
            {
                await _next.Invoke(httpContext);
                return;
            }

            using (var servicesFeature = new RequestServicesContainerFeature(_applicationServices, _serviceScopeFactory))
            {
                try
                {
                    httpContext.Features.Set<IServiceProvidersFeature>(servicesFeature);
                    await _next.Invoke(httpContext);
                }
                finally
                {
                    httpContext.Features.Set<IServiceProvidersFeature>(existingFeature);
                }
            }
        }
    }
}

﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNet.Http.Features;

namespace Microsoft.AspNet.Hosting.Internal
{
    public interface IApplication : IDisposable
    {
        IFeatureCollection ServerFeatures { get; }

        IServiceProvider Services { get; }
    }
}

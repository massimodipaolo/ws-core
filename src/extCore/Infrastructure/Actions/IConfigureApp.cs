// Copyright © 2017 Dmitry Sikorsky. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Builder;

namespace ExtCore.Infrastructure.Actions
{
    /// <summary>
    /// Describes an action that must be executed inside the Configure method of a web application's Startup class
    /// and might be used by the extensions to configure a web application's request pipeline.
    /// </summary>
    public interface IConfigureApp : IConfigureAction
    {
        /// <summary>
        /// Contains any code/middleware/pipe that must be executed inside the Program class file.
        /// </summary>
        /// <param name="WebApplication">
        /// Will be provided by the ExtCore and might be used to configure a web application's request pipeline.
        /// </param>
        void Execute(WebApplication app);
    }
}
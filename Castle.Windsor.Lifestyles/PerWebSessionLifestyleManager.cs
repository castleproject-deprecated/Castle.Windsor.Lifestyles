#region license
// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Web;
using Castle.MicroKernel.Context;

namespace Castle.MicroKernel.Lifestyle {
    /// <summary>
    /// Implements a Lifestyle manager for web apps that creates at most one object per http session.
    /// </summary>
    /// <remarks>
    /// Since the http session end event is not really reliable (it only fires with an InProc session provider) there is no way to properly release any components with this lifestyle
    /// </remarks>
    public class PerWebSessionLifestyleManager : AbstractLifestyleManager {
        private readonly string objectID = "PerWebSessionLifestyleManager_" + Guid.NewGuid();

        internal Func<HttpContextBase> ContextProvider { get; set; }

        public PerWebSessionLifestyleManager() {
            ContextProvider = () => new HttpContextWrapper(HttpContext.Current);
        }

        public override object Resolve(CreationContext context) {
            var httpContext = ContextProvider();
            if (httpContext == null)
                throw new InvalidOperationException("HttpContext.Current is null. PerWebSessionLifestyle can only be used in ASP.Net");
            var session = httpContext.Session;
            if (session == null)
                throw new InvalidOperationException("ASP.NET session not found");
            if (session[objectID] == null) {
                var instance = base.Resolve(context);
                session[objectID] = instance;
                return instance;
            }
            return session[objectID];
        }

        public override void Dispose() {
            var current = ContextProvider();
            if (current == null) {
                return;
            }

            if (current.Session == null)
                throw new InvalidOperationException("ASP.NET session not found");

            var instance = current.Session[objectID];
            if (instance == null) {
                return;
            }

            Kernel.ReleaseComponent(instance);
        }
    }
}
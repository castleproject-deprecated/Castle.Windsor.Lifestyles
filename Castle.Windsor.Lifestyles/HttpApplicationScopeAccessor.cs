using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Castle.MicroKernel.Lifestyle.Scoped;

namespace Castle.MicroKernel.Lifestyle {
    public class HttpApplicationScopeAccessor: IScopeAccessor {
        private const string lifetimeScopeKey = "HttpApplicationScopeAccessor_LifetimeScope";

        private readonly Func<HttpContextBase> ContextProvider = () => new HttpContextWrapper(HttpContext.Current);

        public HttpApplicationScopeAccessor(Func<HttpContextBase> contextProvider) {
            ContextProvider = contextProvider;
        }

        public HttpApplicationScopeAccessor() {}

        private PerHttpApplicationLifestyleModule GetHttpModule() {
            var context = ContextProvider();
            if (context == null)
                throw new InvalidOperationException("HttpContext.Current is null. PerHttpApplicationLifestyle can only be used in ASP.NET");

            var app = context.ApplicationInstance;
            var lifestyleModule = app.Modules
                .Cast<string>()
                .Select(k => app.Modules[k])
                .OfType<PerHttpApplicationLifestyleModule>()
                .FirstOrDefault();

            if (lifestyleModule == null) {
                var message = string.Format("Looks like you forgot to register the http module {0}" +
                                               "\r\nAdd '<add name=\"PerHttpApplicationLifestyle\" type=\"{1}\" />' " +
                                               "to the <httpModules> section on your web.config",
                                               typeof(PerWebRequestLifestyleModule).FullName,
                                               typeof(PerWebRequestLifestyleModule).AssemblyQualifiedName);
                throw new Exception(message);
            }
            return lifestyleModule;
        }

        public ILifetimeScope GetScope(Context.CreationContext context) {
            return GetHttpModule().GetScope();
        }

        public void Dispose() {
            GetHttpModule().ClearScope();
        }
    }
}

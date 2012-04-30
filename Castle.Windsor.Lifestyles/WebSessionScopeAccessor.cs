using System;
using Castle.MicroKernel.Lifestyle.Scoped;
using System.Web;

namespace Castle.MicroKernel.Lifestyle {
    public class WebSessionScopeAccessor: IScopeAccessor {
        private const string lifetimeScopeKey = "WebSessionScopeAccessor_LifetimeScope";
        private readonly Func<HttpContextBase> ContextProvider = () => new HttpContextWrapper(HttpContext.Current);

        public WebSessionScopeAccessor(Func<HttpContextBase> contextProvider) {
            ContextProvider = contextProvider;
        }

        public WebSessionScopeAccessor() {}

        private static ILifetimeScope GetOrCreateSessionScope(HttpSessionStateBase session) {
            var scope = (ILifetimeScope) session[lifetimeScopeKey];
            if (scope != null)
                return scope;
            scope = new DefaultLifetimeScope();
            session[lifetimeScopeKey] = scope;
            return scope;
        }

        private HttpSessionStateBase GetSession() {
            var httpContext = ContextProvider();
            if (httpContext == null)
                throw new InvalidOperationException("HttpContext.Current is null. PerWebSessionLifestyle can only be used in ASP.Net");
            var session = httpContext.Session;
            if (session == null)
                throw new InvalidOperationException("ASP.NET session not found");
            return session;
        }

        public ILifetimeScope GetScope(Context.CreationContext context) {
            var session = GetSession();
            return GetOrCreateSessionScope(session);
        }

        public void Dispose() {
            var session = GetSession();
            var scope = GetOrCreateSessionScope(session);
            session.Remove(lifetimeScopeKey);
            scope.Dispose();
        }
    }
}

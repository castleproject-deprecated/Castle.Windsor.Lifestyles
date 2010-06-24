using System;
using System.Web;

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
            if (ContextProvider() == null)
                throw new InvalidOperationException("HttpContext.Current is null. PerWebSessionLifestyle can only be used in ASP.Net");
            var session = ContextProvider().Session;
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

            var instance = current.Session[objectID];
            if (instance == null) {
                return;
            }

            Kernel.ReleaseComponent(instance);
        }
    }
}
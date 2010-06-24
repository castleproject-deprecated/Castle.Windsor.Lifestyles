using System;
using System.Web;
using Castle.MicroKernel;
using Castle.MicroKernel.Lifestyle;

namespace ExtraWindsorLifestyles {
    public class PerWebSessionLifestyleManager : AbstractLifestyleManager {
        private readonly string objectID = "PerWebSessionLifestyleManager_" + Guid.NewGuid();

        public override object Resolve(CreationContext context) {
            if (HttpContext.Current == null)
                throw new InvalidOperationException("HttpContext.Current is null. PerWebSessionLifestyle can only be used in ASP.Net");
            var session = HttpContext.Current.Session;
            if (session[objectID] == null) {
                var instance = base.Resolve(context);
                session[objectID] = instance;
            }
            return session[objectID];
        }

        public override void Dispose() {
            var current = HttpContext.Current;
            if (current == null) {
                return;
            }

            var instance = current.Items[objectID];
            if (instance == null) {
                return;
            }

            Kernel.ReleaseComponent(instance);
        }
    }
}
using System;
using Castle.MicroKernel.Lifestyle.Scoped;

namespace Castle.MicroKernel.Lifestyle {
    public class TransientScopeAccessor: IScopeAccessor {
        public ILifetimeScope GetScope(Context.CreationContext context) {
            return new DefaultLifetimeScope();
        }

        public void Dispose() {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Lifestyle.Scoped;

namespace Castle.MicroKernel.Lifestyle {
    public class HybridScopeAccessor : IScopeAccessor {
        private readonly IEnumerable<IScopeAccessor> accessors;

        public HybridScopeAccessor(IEnumerable<IScopeAccessor> accessors) {
            this.accessors = accessors;
        }

        public ILifetimeScope GetScope(Context.CreationContext context) {
            return accessors.Select(a => a.GetScope(context)).FirstOrDefault(s => s != null);
        }

        public void Dispose() {
            foreach (var a in accessors)
                a.Dispose();
        }
    }
}

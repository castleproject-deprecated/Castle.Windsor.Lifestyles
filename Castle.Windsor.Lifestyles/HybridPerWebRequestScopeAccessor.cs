using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Lifestyle.Scoped;
using System.Web;

namespace Castle.MicroKernel.Lifestyle {
    public class HybridPerWebRequestScopeAccessor: IScopeAccessor {
        private readonly IScopeAccessor webRequestScopeAccessor = new WebRequestScopeAccessor();
        private readonly IScopeAccessor secondaryScopeAccessor;

        public HybridPerWebRequestScopeAccessor(IScopeAccessor secondaryScopeAccessor) {
            this.secondaryScopeAccessor = secondaryScopeAccessor;
        }

        public ILifetimeScope GetScope(Context.CreationContext context) {
            if (HttpContext.Current != null && PerWebRequestLifestyleModuleUtils.IsInitialized)
                return webRequestScopeAccessor.GetScope(context);
            return secondaryScopeAccessor.GetScope(context);
        }

        public void Dispose() {
            webRequestScopeAccessor.Dispose();
            secondaryScopeAccessor.Dispose();
        }
    }
}

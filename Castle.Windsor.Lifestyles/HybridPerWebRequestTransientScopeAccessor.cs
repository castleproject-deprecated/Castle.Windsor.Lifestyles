using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Lifestyle.Scoped;

namespace Castle.MicroKernel.Lifestyle {
    public class HybridPerWebRequestTransientScopeAccessor: HybridScopeAccessor {
        public HybridPerWebRequestTransientScopeAccessor() : 
            base(new IScopeAccessor[] {
                new WebRequestScopeAccessor(), 
                new TransientScopeAccessor(),
            }) {}
    }
}

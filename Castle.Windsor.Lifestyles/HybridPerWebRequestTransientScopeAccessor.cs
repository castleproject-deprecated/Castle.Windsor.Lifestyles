using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Lifestyle.Scoped;

namespace Castle.MicroKernel.Lifestyle {
    public class HybridPerWebRequestTransientScopeAccessor : HybridPerWebRequestScopeAccessor {
        public HybridPerWebRequestTransientScopeAccessor() : 
            base(new TransientScopeAccessor()) {}
    }
}

using System;
using System.Collections.Generic;
using Castle.MicroKernel.Lifestyle.Scoped;

namespace Castle.MicroKernel.Lifestyle {
    public class HybridPerWebRequestPerThreadScopeAccessor: HybridPerWebRequestScopeAccessor {
        public HybridPerWebRequestPerThreadScopeAccessor() :
            base(new ThreadScopeAccessor()) { }
    }
}

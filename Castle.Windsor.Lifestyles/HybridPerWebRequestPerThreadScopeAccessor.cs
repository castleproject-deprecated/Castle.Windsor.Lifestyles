using System;
using Castle.MicroKernel.Lifestyle.Scoped;

namespace Castle.MicroKernel.Lifestyle {
    public class HybridPerWebRequestPerThreadScopeAccessor: HybridPerWebRequestScopeAccessor {
        public HybridPerWebRequestPerThreadScopeAccessor() :
            base(new ThreadScopeAccessor()) { }
    }
}

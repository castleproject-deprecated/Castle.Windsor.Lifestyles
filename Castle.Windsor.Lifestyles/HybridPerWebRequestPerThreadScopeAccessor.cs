using System;
using System.Collections.Generic;
using Castle.MicroKernel.Lifestyle.Scoped;

namespace Castle.MicroKernel.Lifestyle {
    public class HybridPerWebRequestPerThreadScopeAccessor: HybridScopeAccessor {
        public HybridPerWebRequestPerThreadScopeAccessor() :
            base(new IScopeAccessor[] {
                new WebRequestScopeAccessor(),
                new ThreadScopeAccessor(),
            }) { }
    }
}

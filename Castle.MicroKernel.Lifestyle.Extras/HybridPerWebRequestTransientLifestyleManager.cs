using System.Web;

namespace Castle.MicroKernel.Lifestyle {
    public class HybridPerWebRequestTransientLifestyleManager : HybridLifestyleManager<PerWebRequestLifestyleManager, TransientLifestyleManager> {
        public override object Resolve(CreationContext context) {
            if (HttpContext.Current != null)
                return lifestyle1.Resolve(context);
            return lifestyle2.Resolve(context);
        }
    }
}
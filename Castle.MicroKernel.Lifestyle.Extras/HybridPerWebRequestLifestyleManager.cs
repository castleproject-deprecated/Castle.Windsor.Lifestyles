using System.Web;

namespace Castle.MicroKernel.Lifestyle {
    /// <summary>
    /// Hybrid lifestyle manager where the main lifestyle is <see cref = "PerWebRequestLifestyleManager" />
    /// </summary>
    /// <typeparam name = "T">Secondary lifestyle</typeparam>
    public class HybridPerWebRequestLifestyleManager<T> : HybridLifestyleManager<PerWebRequestLifestyleManager, T>
        where T : ILifestyleManager, new() {
        public override object Resolve(CreationContext context) {
            if (HttpContext.Current != null)
                return lifestyle1.Resolve(context);
            return lifestyle2.Resolve(context);
        }
        }
}
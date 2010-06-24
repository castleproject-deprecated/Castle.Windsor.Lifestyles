using System.Web;

namespace Castle.MicroKernel.Lifestyle {
    /// <summary>
    /// Hybrid lifestyle manager, 
    /// the main lifestyle is <see cref="PerWebRequestLifestyleManager"/>,
    /// the secondary lifestyle is <see cref="TransientLifestyleManager"/>
    /// </summary>
    public class HybridPerWebRequestTransientLifestyleManager : HybridPerWebRequestLifestyleManager<TransientLifestyleManager> {}
}
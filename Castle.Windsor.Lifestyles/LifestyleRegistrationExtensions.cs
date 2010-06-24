using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration.Lifestyle;

namespace Castle.MicroKernel.Registration {
    public static class LifestyleRegistrationExtensions {
        public static ComponentRegistration<S> PerWebSession<S>(this LifestyleGroup<S> @group) {
            return @group.Custom<PerWebSessionLifestyleManager>();
        }

        public static ComponentRegistration<S> PerHttpApplication<S>(this LifestyleGroup<S> @group) {
            return @group.Custom<PerHttpApplicationLifestyleManager>();
        }

        public static ComponentRegistration<S> HybridPerWebRequestTransient<S>(this LifestyleGroup<S> @group) {
            return @group.Custom<HybridPerWebRequestTransientLifestyleManager>();
        }
    }
}
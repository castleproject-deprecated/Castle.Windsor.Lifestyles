using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Registration.Lifestyle;
using Castle.Windsor;

namespace Castle.MicroKernel.Lifestyle.Contextual
{
    public static class ContextualExtensions
    {
        public static ContainerContext CreateContext(this IWindsorContainer container)
        {
            return new ContainerContext(container);
        }

        public static ContainerContext CreateContext(this IKernel kernel)
        {
            return new ContainerContext(kernel);
        }
        
        public static ComponentRegistration<T> Contextual<T>(this LifestyleGroup<T> group)
        {
            return group.Custom(typeof(ContextualLifestyle));
        }
    }
}
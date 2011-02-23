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
    }
}
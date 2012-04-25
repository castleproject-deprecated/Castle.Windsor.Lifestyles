using Castle.MicroKernel.Context;

namespace Castle.MicroKernel.Lifestyle.Contextual
{
    public class ContextualLifestyle : AbstractLifestyleManager
    {
        public override object Resolve(CreationContext context, IReleasePolicy releasePolicy)
        {
            EnsureContainerContextStoreRegistered();

            var containerContextStore = Kernel.Resolve<IContainerContextStore>();
            var currentContext = containerContextStore.GetCurrent();
            if (currentContext == null)
            {
                using (new ContainerContext(Kernel))
                {
                    return base.Resolve(context, releasePolicy);
                }
            }
            
            var instance = currentContext.GetInstance(Model.Name, Model.Implementation);
            if (instance == null)
            {
                instance = base.Resolve(context, releasePolicy);
                currentContext.Register(Model.Name, Model.Implementation, instance);  //Model.Service,
            }
            return instance;
        }

        private void EnsureContainerContextStoreRegistered()
        {
            if (Kernel.HasComponent(typeof(IContainerContextStore)) == false)
            {
                Kernel.AddComponent<ContainerContextStore>(typeof(IContainerContextStore));
            }
        }

        public override void Dispose()
        {
        }
    }
}
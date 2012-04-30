using Castle.MicroKernel.Context;
using System;
using System.Linq;
using Castle.MicroKernel.Registration;

namespace Castle.MicroKernel.Lifestyle.Contextual
{
    [Obsolete("Use ScopedLifestyleManager instead")]
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
            
            var instance = currentContext.GetInstance(Model.Name, Model.Services.First());
            if (instance == null)
            {
                instance = base.Resolve(context, releasePolicy);
                currentContext.Register(Model.Name, Model.Services.First(), instance);
            }
            return instance;
        }

        private void EnsureContainerContextStoreRegistered()
        {
            if (!Kernel.HasComponent(typeof(IContainerContextStore))) {
                Kernel.Register(Component.For<IContainerContextStore>().ImplementedBy<ContainerContextStore>());
            }
        }

        public override void Dispose()
        {
        }
    }
}
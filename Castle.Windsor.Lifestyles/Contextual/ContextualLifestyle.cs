using Castle.MicroKernel.Context;

namespace Castle.MicroKernel.Lifestyle.Contextual
{
	public class ContextualLifestyle : AbstractLifestyleManager
	{
		public override object Resolve(CreationContext context)
		{
			EnsureContainerContextStoreRegistered();

			var containerContextStore = Kernel.Resolve<IContainerContextStore>();
			var currentContext = containerContextStore.GetCurrent();
			if (currentContext == null)
			{
				using (new ContainerContext(Kernel))
				{
					return base.Resolve(context);
				}
			}
			
			var instance = currentContext.GetInstance(Model.Name, Model.Service);
			if (instance == null)
			{
				instance = base.Resolve(context);
				currentContext.Register(Model.Name, Model.Service, instance);
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
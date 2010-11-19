using System;
using System.Collections.Generic;
using Castle.Core;
using Castle.Windsor;

namespace Castle.MicroKernel.Lifestyle.Contextual
{
	public class ContainerContext : IDisposable
	{
		private readonly IKernel kernel;
		private readonly Dictionary<Pair<string, Type>, object> contextualComponents = new Dictionary<Pair<string, Type>, object>();
		private readonly IContainerContextStore contextStore;

		public ContainerContext(IWindsorContainer container) : this(container.Kernel)
		{
		}

		public ContainerContext(IKernel kernel)
		{
			this.kernel = kernel;
			
			EnsureContainerContextStoreRegistered();

			contextStore = kernel.Resolve<IContainerContextStore>();
			contextStore.RegisterCurrent(this);
		}

		public void Dispose()
		{
			contextStore.UnregisterCurrent(this);
            foreach (var c in contextualComponents)
                kernel.ReleaseComponent(c.Value);
		}

		public object GetInstance(string name, Type type)
		{
			object instance;
			contextualComponents.TryGetValue(new Pair<string, Type>(name, type), out instance);
			return instance;
		}

		public void Register(string name, Type type, object instance)
		{
			contextualComponents.Add(new Pair<string, Type>(name, type), instance);
		}

		private void EnsureContainerContextStoreRegistered()
		{
			if (kernel.HasComponent(typeof(IContainerContextStore)) == false)
			{
				kernel.AddComponent<ContainerContextStore>(typeof(IContainerContextStore));
			}
		}
	}
}
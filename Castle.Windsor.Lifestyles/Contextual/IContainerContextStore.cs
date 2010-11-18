namespace Castle.MicroKernel.Lifestyle.Contextual
{
	public interface IContainerContextStore
	{
		void RegisterCurrent(ContainerContext context);
		void UnregisterCurrent(ContainerContext context);
		ContainerContext GetCurrent();
	}
}
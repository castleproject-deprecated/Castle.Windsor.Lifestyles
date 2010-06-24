using Castle.Core;

namespace Castle.MicroKernel.Lifestyle {
    public abstract class HybridLifestyleManager<M1, M2> : AbstractLifestyleManager
        where M1 : ILifestyleManager, new()
        where M2 : ILifestyleManager, new() {
        protected readonly M1 lifestyle1 = new M1();
        protected readonly M2 lifestyle2 = new M2();

        public override void Dispose() {
            lifestyle1.Dispose();
            lifestyle2.Dispose();
        }

        public override void Init(IComponentActivator componentActivator, IKernel kernel, ComponentModel model) {
            lifestyle1.Init(componentActivator, kernel, model);
            lifestyle2.Init(componentActivator, kernel, model);
            base.Init(componentActivator, kernel, model);
        }

        public override bool Release(object instance) {
            lifestyle1.Release(instance);
            lifestyle2.Release(instance);
            return base.Release(instance);
        }

        public abstract object Resolve(CreationContext context);
        }
}
using Castle.Core;
using Castle.MicroKernel.Handlers;
using NUnit.Framework;

namespace Castle.MicroKernel.Lifestyle.Tests {
    [TestFixture]
    public class HybridPerWebRequestTransientLifestyleManagerTests {
        [Test]
        public void No_context_uses_transient() {
            var m = new HybridPerWebRequestTransientLifestyleManager();
            var kernel = new DefaultKernel();
            var model = new ComponentModel("bla", typeof(object), typeof(object));
            var activator = kernel.CreateComponentActivator(model);
            m.Init(activator, kernel, model);
            var creationContext = new CreationContext(new DefaultHandler(model), kernel.ReleasePolicy, typeof(object), null, null);
            var instance1 = m.Resolve(creationContext);
            Assert.IsNotNull(instance1);
            var instance2 = m.Resolve(creationContext);
            Assert.IsNotNull(instance2);
            Assert.AreNotSame(instance1, instance2);

        }
    }
}
using System;
using Castle.Core;
using Castle.MicroKernel.Handlers;
using NUnit.Framework;

namespace Castle.MicroKernel.Lifestyle.Tests {
    [TestFixture]
    public class HybridLifestyleManagerTests {
        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "HttpContext.Current is null. PerWebRequestLifestyle can only be used in ASP.Net")]
        public void PerWebRequestTransient() {
            var m = new HybridLifestyleManager<PerWebRequestLifestyleManager, TransientLifestyleManager>();
            var kernel = new DefaultKernel();
            var model = new ComponentModel("bla", typeof(object), typeof(object));
            var activator = kernel.CreateComponentActivator(model);
            m.Init(activator, kernel, model);
            var creationContext = new CreationContext(new DefaultHandler(model), kernel.ReleasePolicy, typeof (object), null, null);
            m.Resolve(creationContext);
        }
    }
}
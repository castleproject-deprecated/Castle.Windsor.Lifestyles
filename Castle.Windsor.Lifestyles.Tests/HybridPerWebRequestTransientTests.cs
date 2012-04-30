using System;
using NUnit.Framework;
using Castle.MicroKernel.Registration;
using System.IO;
using System.Web.Hosting;
using System.Web;

namespace Castle.MicroKernel.Lifestyle.Tests {
    [TestFixture]
    public class HybridPerWebRequestTransientTests {
        [Test]
        public void No_context_uses_transient() {
            using (var k = new DefaultKernel()) {
                k.Register(Component.For<Dummy>().LifeStyle.HybridPerWebRequestTransient());
                var d1 = k.Resolve<Dummy>();
                Assert.IsNotNull(d1);
                var d2 = k.Resolve<Dummy>();
                Assert.IsNotNull(d2);
                Assert.AreNotSame(d1, d2);
            }
        }

        [Test]
        public void With_context_uses_context() {
            var tw = new StringWriter();
            var wr = new SimpleWorkerRequest("/", Directory.GetCurrentDirectory(), "default.aspx", null, tw);
            var module = new PerWebRequestLifestyleModule();

            var ctx = HttpModuleRunner.GetContext(wr, new[] { module });
            HttpContext.Current = ctx.Key;

            using (var k = new DefaultKernel()) {
                k.Register(Component.For<Dummy>().LifeStyle.HybridPerWebRequestTransient());
                var d1 = k.Resolve<Dummy>();
                Assert.IsNotNull(d1);
                var d2 = k.Resolve<Dummy>();
                Assert.IsNotNull(d2);
                Assert.AreSame(d1, d2);
                ctx.Value.FireEndRequest();
                ctx.Key.Items["castle.per-web-request-lifestyle-cache"] = null;
                var d3 = k.Resolve<Dummy>();
                Assert.AreNotSame(d1, d3);
            }
        }
    }

    public class Dummy {}
}

using System;
using Castle.Core;
using Castle.MicroKernel.Handlers;
using Castle.MicroKernel.Releasers;
using NUnit.Framework;

namespace Castle.MicroKernel.Lifestyle.Tests {
    [TestFixture]
    public class PerWebSessionLifestyleManagerTests {
        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "HttpContext.Current is null. PerWebSessionLifestyle can only be used in ASP.Net")]
        public void NoContextThrows() {
            var m = new PerWebSessionLifestyleManager {ContextProvider = () => null};
            m.Resolve(new CreationContext(new DefaultHandler(new ComponentModel("", typeof (object), typeof (object))), new NoTrackingReleasePolicy(), typeof(object), null, null));

        }
    }
}
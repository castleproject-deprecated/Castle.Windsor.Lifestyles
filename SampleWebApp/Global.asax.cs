using System;
using System.Web;
using Castle.Facilities.FactorySupport;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;

namespace SampleWebApp {
    public class Global : HttpApplication {
        protected void Application_Start(object sender, EventArgs e) {
            var kernel = new DefaultKernel();
            kernel.AddFacility<FactorySupportFacility>();
            kernel.Register(Component.For<HttpContextBase>()
                                .LifeStyle.Transient
                                .UsingFactoryMethod(() => new HttpContextWrapper(HttpContext.Current)));
            kernel.Register(Component.For<SomeService>()
                .LifeStyle.HybridPerWebRequestTransient());
            kernel.Resolve<SomeService>();
        }

        public class SomeService {
            public SomeService(HttpContextBase req) {}
        }

        protected void Session_Start(object sender, EventArgs e) {}

        protected void Application_BeginRequest(object sender, EventArgs e) {}

        protected void Application_AuthenticateRequest(object sender, EventArgs e) {}

        protected void Application_Error(object sender, EventArgs e) {}

        protected void Session_End(object sender, EventArgs e) {}

        protected void Application_End(object sender, EventArgs e) {}
    }
}
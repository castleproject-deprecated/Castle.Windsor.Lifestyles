#region license
// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using Castle.Core;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Handlers;
using Castle.MicroKernel.Registration;
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
            var creationContext = new Func<CreationContext>(() => new CreationContext(new DefaultHandler(model), kernel.ReleasePolicy, typeof(object), null, null, null));
            var instance1 = m.Resolve(creationContext());
            Assert.IsNotNull(instance1);
            var instance2 = m.Resolve(creationContext());
            Assert.IsNotNull(instance2);
            Assert.AreNotSame(instance1, instance2);

        }

        [Test]
        public void PerWebRequestLifestyleManagerTest() {
            var tw = new StringWriter();
            var wr = new SimpleWorkerRequest("/", Directory.GetCurrentDirectory(), "default.aspx", null, tw);
            var module = new PerWebRequestLifestyleModule();

            var ctx = HttpModuleRunner.GetContext(wr, new[] { module });
            HttpContext.Current = ctx.Key;

            using (var kernel = new DefaultKernel()) {
                kernel.Register(Component.For<object>().LifeStyle.PerWebRequest);
                var instance1 = kernel.Resolve<object>();
                Assert.IsNotNull(instance1);
                var instance2 = kernel.Resolve<object>();
                Assert.IsNotNull(instance2);
                Assert.AreSame(instance1, instance2);
            }
        }

        [Test]
        public void With_context_uses_context() {
            var tw = new StringWriter();
            var wr = new SimpleWorkerRequest("/", Directory.GetCurrentDirectory(), "default.aspx", null, tw);
            var module = new PerWebRequestLifestyleModule();

            var ctx = HttpModuleRunner.GetContext(wr, new[] { module });
            HttpContext.Current = ctx.Key;

            using (var kernel = new DefaultKernel()) {
                kernel.Register(Component.For<object>().LifeStyle.HybridPerWebRequestTransient());
                var instance1 = kernel.Resolve<object>();
                Assert.IsNotNull(instance1);
                var instance2 = kernel.Resolve<object>();
                Assert.IsNotNull(instance2);
                Assert.AreSame(instance1, instance2);

                HttpContext.Current = HttpModuleRunner.GetContext(wr, new[] { new PerWebRequestLifestyleModule() }).Key;
                var instance3 = kernel.Resolve<object>();
                Assert.AreNotSame(instance1, instance3);
            }

        }
    }
}
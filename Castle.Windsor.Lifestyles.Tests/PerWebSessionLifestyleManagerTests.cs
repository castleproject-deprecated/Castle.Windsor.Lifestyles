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
using System.Web;
using Castle.Core;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Handlers;
using Castle.MicroKernel.Releasers;
using NUnit.Framework;
using Rhino.Mocks;

namespace Castle.MicroKernel.Lifestyle.Tests {
    [TestFixture]
    public class PerWebSessionLifestyleManagerTests {
        [Test]
        [ExpectedException(typeof (InvalidOperationException), ExpectedMessage = "HttpContext.Current is null. PerWebSessionLifestyle can only be used in ASP.Net")]
        public void NoContextThrows() {
            var m = new PerWebSessionLifestyleManager {ContextProvider = () => null};
            var componentModel = new ComponentModel("", typeof(object), typeof(object));
            var handler = new DefaultHandler(componentModel);
            m.Resolve(new CreationContext(handler, new NoTrackingReleasePolicy(), typeof (object), null, null, null));
        }

        [Test]
        public void ResolveInSameSession() {
            var context = GetMockContext();
            var m = new PerWebSessionLifestyleManager {ContextProvider = () => context};
            var kernel = new DefaultKernel();
            var model = new ComponentModel("bla", typeof (object), typeof (object));
            var activator = kernel.CreateComponentActivator(model);
            m.Init(activator, kernel, model);
            var handler = new DefaultHandler(model);
            var creationContext = new CreationContext(handler, kernel.ReleasePolicy, typeof (object), null, null, null);
            var instance = m.Resolve(creationContext);
            Assert.IsNotNull(instance);
            var instance2 = m.Resolve(creationContext);
            Assert.AreSame(instance, instance2);
        }

        [Test]
        public void ResolveInDifferentSessions() {
            var context = GetMockContext();
            var m = new PerWebSessionLifestyleManager {ContextProvider = () => context};
            var kernel = new DefaultKernel();
            var model = new ComponentModel("bla", typeof (object), typeof (object));
            var activator = kernel.CreateComponentActivator(model);
            m.Init(activator, kernel, model);
            var creationContext = new Func<CreationContext>(() => new CreationContext(new DefaultHandler(model), kernel.ReleasePolicy, typeof (object), null, null, null));
            var instance = m.Resolve(creationContext());
            Assert.IsNotNull(instance);
            context.Session.Abandon();
            var instance2 = m.Resolve(creationContext());
            Assert.AreNotSame(instance, instance2);
        }

        public HttpContextBase GetMockContext() {
            var session = new HashtableSessionState();
            var context = MockRepository.GenerateMock<HttpContextBase>();
            context.Expect(x => x.Session).Return(session);
            return context;
        }
    }
}
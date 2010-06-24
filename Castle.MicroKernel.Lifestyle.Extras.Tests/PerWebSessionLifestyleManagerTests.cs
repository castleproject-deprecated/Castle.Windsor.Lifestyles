﻿using System;
using System.Web;
using Castle.Core;
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
            m.Resolve(new CreationContext(new DefaultHandler(new ComponentModel("", typeof (object), typeof (object))), new NoTrackingReleasePolicy(), typeof (object), null, null));
        }

        [Test]
        public void ResolveInSameSession() {
            var context = GetMockContext();
            var m = new PerWebSessionLifestyleManager {ContextProvider = () => context};
            var kernel = new DefaultKernel();
            var model = new ComponentModel("bla", typeof (object), typeof (object));
            var activator = kernel.CreateComponentActivator(model);
            m.Init(activator, kernel, model);
            var creationContext = new CreationContext(new DefaultHandler(model), kernel.ReleasePolicy, typeof (object), null, null);
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
            var creationContext = new CreationContext(new DefaultHandler(model), kernel.ReleasePolicy, typeof (object), null, null);
            var instance = m.Resolve(creationContext);
            Assert.IsNotNull(instance);
            context.Session.Abandon();
            var instance2 = m.Resolve(creationContext);
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
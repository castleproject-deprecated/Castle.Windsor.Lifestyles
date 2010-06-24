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
using System.Collections.Generic;
using System.Reflection;
using System.Web;

namespace Castle.MicroKernel.Lifestyle.Tests {
    public class HttpModuleRunner {
        public static KeyValuePair<HttpContext, HttpApplication> GetContext(HttpWorkerRequest wr, params IHttpModule[] modules) {
            var ctx = new HttpContext(wr);
            var app = new MyApp(modules);
            SetHttpApplicationFactoryCustomApplication(app);
            InitInternal(app, ctx);
            AssignContext(app, ctx);
            return new KeyValuePair<HttpContext, HttpApplication>(ctx, app);
        }

        public static HttpContext Run(HttpWorkerRequest wr, params IHttpModule[] modules) {
            var kv = GetContext(wr, modules);
            ProcessRequest(kv.Value, kv.Key);
            return kv.Key;
        }

        public static void ProcessRequest(HttpApplication app, HttpContext ctx) {
            var r = beginProcessRequestMethod.Invoke(app, new object[] { ctx, null, null });
            endProcessRequestMethod.Invoke(app, new[] { r });
        }

        private static void AssignContext(HttpApplication app, HttpContext ctx) {
            assignContextMethod.Invoke(app, new[] { ctx });
        }

        private static void SetHttpApplicationFactoryCustomApplication(HttpApplication app) {
            customAppProperty.SetValue(null, app);
        }

        private static void InitInternal(HttpApplication app, HttpContext ctx) {
            initInternalMethod.Invoke(app, new object[] { ctx, null, null });
        }

        private static readonly MethodInfo beginProcessRequestMethod = typeof(HttpApplication).GetMethod("System.Web.IHttpAsyncHandler.BeginProcessRequest", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo endProcessRequestMethod = typeof(HttpApplication).GetMethod("System.Web.IHttpAsyncHandler.EndProcessRequest", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo assignContextMethod = typeof (HttpApplication).GetMethod("AssignContext", BindingFlags.NonPublic | BindingFlags.Instance); 
        private static readonly MethodInfo initInternalMethod = typeof (HttpApplication).GetMethod("InitInternal", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly Type httpApplicationFactory = Type.GetType("System.Web.HttpApplicationFactory, System.Web, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
        private static readonly FieldInfo customAppProperty = httpApplicationFactory.GetField("_customApplication", BindingFlags.NonPublic | BindingFlags.Static);

        public class MyApp : HttpApplication {
            private readonly ICollection<IHttpModule> modules;

            public MyApp(ICollection<IHttpModule> modules) {
                this.modules = modules;
            }

            public override void Init() {
                Console.WriteLine("app init");
                base.Init();
                foreach (var m in modules) {
                    Console.WriteLine("Initializing module {0}", m);
                    m.Init(this);
                }
            }
        }
    }
}
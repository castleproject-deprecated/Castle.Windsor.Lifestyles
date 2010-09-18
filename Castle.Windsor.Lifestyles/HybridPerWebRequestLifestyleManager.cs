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

using System.Reflection;
using System.Web;
using Castle.MicroKernel.Context;

namespace Castle.MicroKernel.Lifestyle {
    /// <summary>
    /// Hybrid lifestyle manager where the main lifestyle is <see cref = "PerWebRequestLifestyleManager" />
    /// </summary>
    /// <typeparam name = "T">Secondary lifestyle</typeparam>
    public class HybridPerWebRequestLifestyleManager<T> : HybridLifestyleManager<PerWebRequestLifestyleManager, T>
        where T : ILifestyleManager, new() {

        // TODO make this public in Windsor
        private static readonly PropertyInfo PerWebRequestLifestyleModuleInitialized = typeof (PerWebRequestLifestyleModule).GetProperty("Initialized", BindingFlags.Static | BindingFlags.NonPublic);

        private static bool IsPerWebRequestLifestyleModuleInitialized {
            get {
                return (bool) PerWebRequestLifestyleModuleInitialized.GetValue(null, null);
            }
        }

        public override object Resolve(CreationContext context) {
            if (HttpContext.Current != null && IsPerWebRequestLifestyleModuleInitialized)
                return lifestyle1.Resolve(context);
            return lifestyle2.Resolve(context);
        }
        }
}
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

using System.Collections;
using System.Web;

namespace Castle.MicroKernel.Lifestyle.Tests {
    public class HashtableSessionState : HttpSessionStateBase {
        private readonly Hashtable dict = new Hashtable();

        public override object this[string name] {
            get { return dict[name]; }
            set { dict[name] = value; }
        }

        public override void Clear() {
            dict.Clear();
        }

        public override void Abandon() {
            Clear();
        }
    }
}
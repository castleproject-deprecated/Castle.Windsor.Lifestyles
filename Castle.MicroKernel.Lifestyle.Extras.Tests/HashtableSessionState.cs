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
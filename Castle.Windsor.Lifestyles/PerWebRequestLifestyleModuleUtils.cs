using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Castle.MicroKernel.Lifestyle {
    public class PerWebRequestLifestyleModuleUtils {
        // TODO make this public in Windsor
        private static readonly FieldInfo InitializedFieldInfo = typeof(PerWebRequestLifestyleModule).GetField("initialized", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetField);

        public static bool IsInitialized {
            get {
                return (bool)InitializedFieldInfo.GetValue(null);
            }
        }
    }
}

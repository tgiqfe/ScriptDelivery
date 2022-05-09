using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptDelivery.Requires.Matcher
{
    internal class MatcherParameterAttribute : Attribute
    {
        public bool Mandatory { get; set; }

        public int MandatoryAny { get; set; }

        public bool Expand { get; set; }

        public bool Unsigned { get; set; }

        public char Delimiter { get; set; }

        public char EqualSign { get; set; }
    }
}

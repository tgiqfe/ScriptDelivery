using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptDelivery.Map.Requires.Matcher
{
    internal class EnvMatcher : MatcherBase
    {
        [MatcherParameter, Keys("Name", "Key")]
        public string Name { get; set; }

        
    }
}

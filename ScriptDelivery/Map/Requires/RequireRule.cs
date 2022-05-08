using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet;
using YamlDotNet.Serialization;

namespace ScriptDelivery.Map.Requires
{
    internal class RequireRule
    {
        [YamlMember(Alias = "target")]
        public RuleTarget? RuleTarget { get; set; }

        [YamlMember(Alias = "match")]
        public MatchType? MatchType { get; set; }

        [YamlMember(Alias = "invert")]
        public bool? Invert { get; set; }

        [YamlMember(Alias = "param")]
        public Dictionary<string, string> Param { get; set; }

        public bool Match()
        {


            return false;
        }
    }
}

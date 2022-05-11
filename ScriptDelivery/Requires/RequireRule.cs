using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet;
using YamlDotNet.Serialization;
using ScriptDelivery.Lib;

namespace ScriptDelivery.Requires
{
    internal class RequireRule
    {
        [YamlMember(Alias = "target"), Values("Target")]
        public string RuleTarget { get; set; }

        [YamlMember(Alias = "match"), Values("Match")]
        public string MatchType { get; set; }

        [YamlMember(Alias = "invert")]
        public string Invert { get; set; }

        [YamlMember(Alias = "param")]
        public Dictionary<string, string> Param { get; set; }

        
        public RuleTarget GetRuleTarget()
        {
            return ValuesAttribute.GetEnumValue<RuleTarget>(
                this.GetType().GetProperty("RuleTarget"), this.RuleTarget);
        }

        public MatchType GetMatchType()
        {
            return ValuesAttribute.GetEnumValue<MatchType>(
                this.GetType().GetProperty("MatchType"), this.MatchType);
        }

        public bool GetInvert()
        {
            return !(BooleanCandidate.IsNullableFalse(this.Invert) ?? true);
        }
    }
}

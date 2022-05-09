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

        [YamlIgnore]
        public RuleTarget enum_RuleTarget
        {
            get
            {
                _enum_RuleTarget ??= ValuesAttribute.GetEnumValue<RuleTarget>(
                    this.GetType().GetProperty("RuleTarget"), this.RuleTarget);
                return (RuleTarget)_enum_RuleTarget;
            }
        }
        private RuleTarget? _enum_RuleTarget = null;

        [YamlIgnore]
        public MatchType enum_MatchType
        {
            get
            {
                _enum_MatchType ??= ValuesAttribute.GetEnumValue<MatchType>(
                    this.GetType().GetProperty("MatchType"), this.MatchType);
                return (MatchType)_enum_MatchType;
            }
        }
        private MatchType? _enum_MatchType = null;

        [YamlIgnore]
        public bool bool_Invert
        {
            get
            {
                _bool_Invert ??= !(BooleanCandidate.IsNullableFalse(this.Invert) ?? false);
                return (bool)_bool_Invert;
            }
        }
        private bool? _bool_Invert = null;

    }
}

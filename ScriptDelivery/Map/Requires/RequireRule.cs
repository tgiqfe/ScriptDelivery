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
        public string RuleTarget { get; set; }

        [YamlMember(Alias = "match")]
        public string MatchType { get; set; }

        [YamlMember(Alias = "invert")]
        public bool? Invert { get; set; }

        [YamlMember(Alias = "param")]
        public Dictionary<string, string> Param { get; set; }

        //  RuleTargetの値を候補から取り出す処理をここに
        //  MatchTypeの値を候補から取り出す処理をここに

        public bool Match()
        {


            return false;
        }
    }
}

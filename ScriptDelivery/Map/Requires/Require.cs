using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace ScriptDelivery.Map.Requires
{
    internal class Require
    {
        [YamlMember(Alias = "mode")]
        public RequireMode? RequireMode { get; set; }

        [YamlMember(Alias = "rule")]
        public List<RequireRule> RequireRule { get; set; }
    }
}

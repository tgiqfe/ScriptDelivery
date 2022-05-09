using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using System.Reflection;

namespace ScriptDelivery.Requires
{
    internal class Require
    {
        [YamlMember(Alias = "mode"), Values("Mode")]
        public string RequireMode { get; set; }

        [YamlMember(Alias = "rule")]
        public RequireRule[] RequireRules { get; set; }

        public RequireMode GetRequireMode()
        {
            return ValuesAttribute.GetEnumValue<RequireMode>(
                this.GetType().GetProperty("RequireMode"), this.RequireMode);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using System.Reflection;

namespace ScriptDelivery.Map.Requires
{
    internal class Require
    {
        [YamlMember(Alias = "mode"), Values("Mode")]
        public string RequireMode { get; set; }

        [YamlMember(Alias = "rule")]
        public RequireRule[] RequireRule { get; set; }

        [YamlIgnore]
        public RequireMode enum_RequireMode
        {
            get
            {
                _enum_RequireMode ??= ValuesAttribute.GetEnumValue<RequireMode>(
                    this.GetType().GetProperty("RequireMode"), this.RequireMode);
                return (RequireMode)_enum_RequireMode;
            }
        }
        private RequireMode? _enum_RequireMode;





    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using System.Diagnostics;

namespace ScriptDelivery.Map.Works
{
    internal class Download
    {
        [YamlMember(Alias = "path")]
        public string Path { get; set; }

        [YamlMember(Alias = "overwrite"), Values("Overwrite")]
        public string Overwrite { get; set; }

        [YamlMember(Alias = "user")]
        public string UserName { get; set; }

        [YamlMember(Alias = "password")]
        public string Password { get; set; }

        [YamlIgnore]
        public Overwrite enum_Overwrite
        {
            get
            {
                _enum_Overwrite ??= ValuesAttribute.GetEnumValue<Overwrite>(
                    this.GetType().GetProperty("Overwrite"), this.Overwrite);
                return (Overwrite)_enum_Overwrite;
            }
        }
        private Overwrite? _enum_Overwrite = null;
    }
}

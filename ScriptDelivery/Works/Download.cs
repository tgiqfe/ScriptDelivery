using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using System.Diagnostics;

namespace ScriptDelivery.Works
{
    internal class Download
    {
        [YamlMember(Alias = "path")]
        public string Path { get; set; }

        [YamlMember(Alias = "protocol"), Values("Protocol")]
        public string Protocol { get; set; }

        [YamlMember(Alias = "overwrite"), Values("Overwrite")]
        public string Overwrite { get; set; }

        [YamlMember(Alias = "user")]
        public string UserName { get; set; }

        [YamlMember(Alias = "password")]
        public string Password { get; set; }

        public Overwrite GetOverwrite()
        {
            return ValuesAttribute.GetEnumValue<Overwrite>(
                this.GetType().GetProperty("Overwrite"), this.Overwrite);
        }

        public Protocol GetProtocol()
        {
            return ValuesAttribute.GetEnumValue<Protocol>(
                this.GetType().GetProperty("Protocol"), this.Protocol);
        }
    }
}

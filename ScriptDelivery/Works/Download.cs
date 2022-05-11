using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using System.Diagnostics;
using ScriptDelivery.Lib;

namespace ScriptDelivery.Works
{
    internal class Download
    {
        [YamlMember(Alias = "source")]
        public string SourcePath { get; set; }

        [YamlMember(Alias = "destination")]
        public string DestinationPath { get; set; }

        [YamlMember(Alias = "protocol"), Values("Protocol")]
        public string Protocol { get; set; }

        [YamlMember(Alias = "force")]
        public string Force { get; set; }

        [YamlMember(Alias = "user")]
        public string UserName { get; set; }

        [YamlMember(Alias = "password")]
        public string Password { get; set; }

        public Protocol GetProtocol()
        {
            return ValuesAttribute.GetEnumValue<Protocol>(
                this.GetType().GetProperty("Protocol"), this.Protocol);
        }

        public bool GetForce()
        {
            return !(BooleanCandidate.IsNullableFalse(this.Force) ?? true);
        }
    }
}

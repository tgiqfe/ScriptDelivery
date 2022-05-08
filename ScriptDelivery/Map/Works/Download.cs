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

        [YamlMember(Alias = "overwrite")]
        public Overwrite? Overwrite { get; set; }

        [YamlMember(Alias = "user")]
        public string UserName { get; set; }

        [YamlMember(Alias = "password")]
        public string Password { get; set; }

        public void GetFile()
        {

        }
    }
}

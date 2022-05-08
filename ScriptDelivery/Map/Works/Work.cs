using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace ScriptDelivery.Map.Works
{
    internal class Work
    {
        [YamlMember(Alias = "download")]
        public Download[] Download { get; set; }
    }
}

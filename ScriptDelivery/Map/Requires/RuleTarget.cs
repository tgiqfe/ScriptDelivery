using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptDelivery.Map.Requires
{
    internal enum RuleTarget
    {
        None,
        HostName,
        IPAddress,
        Env,
        File,
        Directory,
        Registry
    }
}

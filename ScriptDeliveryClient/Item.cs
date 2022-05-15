using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace ScriptDeliveryClient
{
    internal class Item
    {
        public static string ProcessName = "ScriptDeliveryClient";

        public static string WorkDirectoryPath = Path.Combine(Path.GetTempPath(), "EnumRun");

        public static string ExecDirectoryPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
    }
}

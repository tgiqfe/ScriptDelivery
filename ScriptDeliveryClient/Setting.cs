using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptDeliveryClient
{
    internal class Setting
    {
        /// <summary>
        /// ScriptDelivery設定情報
        /// </summary>
        public ParamScriptDelivery ScriptDelivery { get; set; }

        public class ParamScriptDelivery
        {
            public string[] Server { get; set; }
            public string Process { get; set; }
            public string TrashPath { get; set; }
            public bool? LogTransport { get; set; }

            public override string ToString()
            {
                return string.Format("[ Server={0} Process={1} TrashPath={2}, LogTransport={3} ]",
                    string.Join(", ", this.Server),
                    this.Process,
                    this.TrashPath,
                    this.LogTransport);
            }
        }
    }
}

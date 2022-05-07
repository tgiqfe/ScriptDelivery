using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptDelivery.Map;
using ScriptDelivery.Lib;

namespace ScriptDelivery.Rule
{
    internal class HostNameRule
    {


        public bool Invert { get; set; }


        private string _source { get; set; }

        public bool CheckMatch(Dictionary<string, string> param)
        {
            this._source = Environment.MachineName;

            string name = param["name"];

            if (name.Contains("*"))
            {
                return _source.IsLike(name);
            }

            return _source.Equals(name, StringComparison.OrdinalIgnoreCase);
        }

        public bool CheckRange(Dictionary<string, string> param)
        {
            this._source = Environment.MachineName;

            int startNum = int.TryParse(param["start"], out int tempStart) ? tempStart : 0;
            int endNum = int.TryParse(param["end"], out int tempEnd) ? tempEnd : 0;
            var info = new HostNameInfo(_source);

            return info.Number >= startNum && info.Number <= endNum;
        }
    }
}

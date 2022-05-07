using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptDelivery.Map
{
    internal class KeysAttribute : Attribute
    {
        private string _keyword { get; set; }

        public KeysAttribute(string keyword)
        {
            this._keyword = keyword;
        }

        public string[] GetCandidate()
        {
            _map ??= LoadMap();
            return _map[_keyword];     //  例外対策は無し。もし例外が発生した場合はコード見直し
        }

        #region Key candidate map

        private static Dictionary<string, string[]> _map = null;

        private Dictionary<string, string[]> LoadMap()
        {
            return new Dictionary<string, string[]>()
            {
                { "Name", new string[]{ "name" } },
                { "Start", new string[] { "start", "start", "begin", "startname" } },
                { "End", new string[] { "end", "finish", "fin", "endname" } },
                { "IPAddress", new string[] { "ipaddress", "address", "ipaddr", "addr", "ipaddresses" } },
                { "NetworkAddress", new string[] { "networkaddress", "networkaaddr", "nwaddress", "nwaddr", "network", "nw" } },
                { "Interface", new string[]{ "interface", "if"} },
            };
        }

        #endregion
    }
}

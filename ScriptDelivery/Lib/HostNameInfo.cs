﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ScriptDelivery.Lib
{
    internal class HostNameInfo
    {
        public string PreName { get; set; }
        public int Number { get; set; }
        public string SufName { get; set; }

        private static Regex _regPre = new Regex(@"^.*[^\d](?=\d+[^\d]*$)|^[^\d]+");
        private static Regex _regNum = new Regex(@"\d+(?=[^\d]*$)");
        private static Regex _regSuf = new Regex(@"(?<=\d)[^\d]+$");

        public HostNameInfo() { }
        public HostNameInfo(string name)
        {
            Match match;
            if ((match = _regPre.Match(name)).Success)
            {
                this.PreName = match.Value;
            }
            if ((match = _regNum.Match(name)).Success)
            {
                this.Number = int.Parse(match.Value);
            }
            if ((match = _regSuf.Match(name)).Success)
            {
                this.SufName = match.Value;
            }
        }

    }
}

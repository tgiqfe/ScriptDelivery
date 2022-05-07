using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptDelivery.Map
{
    internal class SubCandidate
    {
        public string Name { get; set; }
        public string[] Candidate { get; set; }

        public SubCandidate(string name, params string[] candidate)
        {
            this.Name = name;
            this.Candidate = candidate;
        }

        public string Check(string text)
        {
            return this.Candidate.Any(x => x.Equals(x, StringComparison.OrdinalIgnoreCase)) ?
                Name : null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptDelivery.Requires;

namespace ScriptDelivery
{
    internal class Rancher
    {
        private List<Mapping> _mappingList = null;

        public Rancher() { }

        public Rancher(string filePath)
        {
            _mappingList = Mapping.Deserialize(filePath);
        }

        public void RequestProcess()
        {
            foreach (var mapping in _mappingList)
            {
                RequireMode mode = mapping.Require.GetRequireMode();

                foreach (var rule in mapping.Require.RequireRule)
                {

                }
            }
        }
    }
}

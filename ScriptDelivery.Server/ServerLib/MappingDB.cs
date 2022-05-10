using System.IO;
using ScriptDelivery;

namespace ScriptDelivery.Server.ServerLib
{
    internal class MappingDB
    {
        private static List<Mapping> _mappingList = null;

        public List<Mapping> GetMappingDB()
        {
            _mappingList ??= Mapping.Deserialize("aaa");




            return null;
        }
    }
}

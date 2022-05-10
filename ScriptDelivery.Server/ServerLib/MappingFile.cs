using System;
using ScriptDelivery;
using System.Security.Cryptography;
using System.IO;

namespace ScriptDelivery.Server.ServerLib
{
    internal class MappingFile : FileBase
    {
        public List<Mapping> MappingList { get; set; }

        public MappingFile() { }
        public MappingFile(string filePath) : base(filePath)
        {
            this.MappingList = Mapping.Deserialize(filePath);
        }
    }
}

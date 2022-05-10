using System;
using System.IO;
using System.Security.Cryptography;

namespace ScriptDelivery.Server.ServerLib
{
    public class DownloadFile : FileBase
    {
        public DownloadFile() { }
        public DownloadFile(string filePath) : base(filePath) { }
    }
}

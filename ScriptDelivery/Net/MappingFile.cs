using System;
using ScriptDelivery;
using System.Security.Cryptography;
using System.IO;

namespace ScriptDelivery.Net
{
    /// <summary>
    /// サーバ側でのみ使用。Mappingファイルの情報を格納
    /// </summary>
    internal class MappingFile
    {
        public string Path { get; set; }
        public DateTime LastWriteTime { get; set; }
        public string Hash { get; set; }
        public List<Mapping> MappingList { get; set; }

        public MappingFile() { }
        public MappingFile(string filePath)
        {
            this.Path = filePath;
            this.LastWriteTime = File.GetLastWriteTime(filePath);
            this.Hash = GetHash(filePath);
            this.MappingList = Mapping.Deserialize(filePath);
        }

        /// <summary>
        /// ファイルのハッシュ化
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private string GetHash(string filePath)
        {
            string ret = null;
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var md5 = MD5.Create();
                byte[] bytes = md5.ComputeHash(fs);
                ret = BitConverter.ToString(bytes);
                md5.Clear();
            }
            return ret;
        }

        /// <summary>
        /// Mappingの元データの変更チェック
        /// </summary>
        /// <returns>null⇒削除済み/true⇒変更有/false⇒変更無し</returns>
        public bool? CheckSource()
        {
            if (!File.Exists(this.Path))
            {
                return null;
            }
            return (File.GetLastWriteTime(this.Path) != this.LastWriteTime) || (GetHash(this.Path) != this.Hash);
        }
    }
}

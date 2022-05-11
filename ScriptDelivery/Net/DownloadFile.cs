using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace ScriptDelivery.Net
{
    public class DownloadFile
    {
        [JsonIgnore]
        public string Path { get; set; }
        public string Name { get; set; }
        public DateTime LastWriteTime { get; set; }
        public string Hash { get; set; }

        public string DestinationPath { get; set; }

        public DownloadFile() { }
        public DownloadFile(string basePath, string filePath)
        {
            this.Path = filePath;
            this.Name = System.IO.Path.GetRelativePath(basePath, filePath);
            this.LastWriteTime = File.GetLastWriteTime(filePath);
            this.Hash = GetHash(filePath);
        }

        /// <summary>
        /// ファイルのハッシュ化
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        protected string GetHash(string filePath)
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
        /// 対象ファイルと比較
        /// </summary>
        /// <param name="baseDir">true⇒一致。false⇒不一致</param>
        /// <returns></returns>
        public bool CompareFile(string path)
        {
            return File.Exists(path) && 
                (File.GetLastWriteTime(path) == this.LastWriteTime) &&
                (GetHash(path) == this.Hash);
        }
    }
}

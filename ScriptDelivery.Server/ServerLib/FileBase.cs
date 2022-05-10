using System;
using System.IO;
using System.Security.Cryptography;

namespace ScriptDelivery.Server.ServerLib
{
    public class FileBase
    {
        public enum ExistsResult
        {
            NotChange,
            Changed,
            Deleted,
        }

        public virtual string Path { get; set; }
        public virtual DateTime LastWriteTime { get; set; }
        public virtual string Hash { get; set; }

        public FileBase() { }
        public FileBase(string filePath)
        {
            this.Path = filePath;
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
        /// Mappingの元データの変更チェック
        /// </summary>
        /// <returns></returns>
        public ExistsResult CheckSource()
        {
            if (!File.Exists(this.Path))
            {
                return ExistsResult.Deleted;
            }
            if ((File.GetLastWriteTime(this.Path) != this.LastWriteTime) ||
                (GetHash(this.Path) != this.Hash))
            {
                return ExistsResult.Changed;
            }

            return ExistsResult.NotChange;
        }
    }
}

﻿using System.IO;

namespace ScriptDelivery.Net
{
    public class DownloadFileCollection
    {
        private List<DownloadFile> _list = null;

        private string _filesPath = null;

        public DownloadFileCollection() { }

        public DownloadFileCollection(string filesPath)
        {
            //string basePath = filesPath.Trim('/').Trim('\\');

            _list = new List<DownloadFile>();
            _filesPath = filesPath;
            foreach (string file in Directory.GetFiles(filesPath))
            {
                _list.Add(new DownloadFile(filesPath, file));
            }
        }

        /// <summary>
        /// 受け取ったDownloadFileリストから、ダウンロード可否を確認
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public void GetResponse(List<DownloadFile> list)
        {
            list.ForEach(x =>
            {
                var dlFile = _list.FirstOrDefault(y => y.Name == x.Name);
                if(dlFile != null)
                {
                    x.Downloadable = true;
                    x.LastWriteTime = dlFile.LastWriteTime;
                    x.Hash = dlFile.Hash;
                }
            });
        }

        /// <summary>
        /// (サーバ側処理)
        /// DownloadFileの変更/追加/削除チェック
        /// </summary>
        public void RecheckSource()
        {
            _list.Clear();
            foreach (string file in Directory.GetFiles(_filesPath))
            {
                _list.Add(new DownloadFile(_filesPath, file));
            }
        }

    }
}


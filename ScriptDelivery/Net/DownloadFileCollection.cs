using System.IO;

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

        public IEnumerable<DownloadFile> GetResponse(List<string> reqList)
        {
            return _list.Where(x => reqList.Any(y => x.Name == y));
        }

        /// <summary>
        /// (クライアント側処理)
        /// DownloadFileの有無チェック
        /// </summary>
        /// <returns></returns>
        public List<string> CheckLocalFile()
        {
            foreach (var file in _list)
            {
                string path = Path.GetRelativePath(_filesPath, file.Name);
                bool isMatch = file.CompareFile(path);

            }
            return null;
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


namespace ScriptDelivery.Net
{
    public class DownloadFileCollection
    {
        private List<DownloadFile> _list = null;

        public DownloadFileCollection() { }

        public DownloadFileCollection(string filesPath)
        {
            _list = new List<DownloadFile>();
            foreach (string file in Directory.GetFiles(filesPath))
            {
                _list.Add(new DownloadFile(file));
            }
        }

        public DownloadFileResponse GetResponse(DownloadFileRequest request)
        {
            return new DownloadFileResponse()
            {
                Files = _list.Where(x => request.Files.Any(y => x.Path == y))
            };
        }
    }

}


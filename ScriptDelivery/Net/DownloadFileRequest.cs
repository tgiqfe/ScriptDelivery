/// <summary>
/// サーバ側にダウンロード要求するリスト
/// </summary>
namespace ScriptDelivery.Net
{
    public class DownloadFileRequest
    {
        public List<string> Files { get; set; }

        public DownloadFileRequest() { }
        public DownloadFileRequest(bool init)
        {
            this.Files = new List<string>();
        }
    }
}


//  このファイル必要?
//  単純にList<string>で対応できそうな気がする。

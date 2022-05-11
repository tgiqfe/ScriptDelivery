/// <summary>
/// ダウンロード要求に対する、HTTPサーバ上でマッチしたFilesのリスト
/// </summary>
namespace ScriptDelivery.Net
{
    public class DownloadFileResponse
    {
        public IEnumerable<DownloadFile> Files { get; set; }
    }
}

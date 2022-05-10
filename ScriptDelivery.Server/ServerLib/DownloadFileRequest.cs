/// <summary>
/// サーバ側にダウンロード要求するリスト
/// </summary>
namespace ScriptDelivery.Server.ServerLib
{
    public class DownloadFileRequest
    {
        public List<string> Files { get; set; }
    }
}

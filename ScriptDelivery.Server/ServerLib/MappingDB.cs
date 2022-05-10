using System.IO;
using ScriptDelivery;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScriptDelivery.Server.ServerLib
{
    internal class MappingDB
    {
        public string Content { get; set; }

        public MappingDB() { }

        public MappingDB(string mapsPath)
        {
            var mapList = Mapping.Deserialize(mapsPath);
            this.Content = JsonSerializer.Serialize(mapList);
        }

        //  ファイル変更確認の為に、更新時間の情報もしくはハッシュ値を格納するクラスを作る
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using ScriptDelivery.Requires;
using ScriptDelivery.Works;
using System.Text.Json;

namespace ScriptDelivery.Net
{
    internal class ScriptDeliveryClient
    {
        public async Task MappingRequest(string server)
        {
            List<Mapping> mappingList = null;

            using (var client = new HttpClient())
            {
                var content = new StringContent("");
                var response = await client.PostAsync(server + "/map", content);
                string json = await response.Content.ReadAsStringAsync();

                mappingList = JsonSerializer.Deserialize<List<Mapping>>(json);
            }

            Rancher rancher = new Rancher() { MappingList = mappingList };
            rancher.MapMathcingCheck();
            if (rancher.SmbDownloadList?.Count > 0)
            {
                //  ファイルサーバ(Smb)からダウンロード
            }
            if (rancher.DownloadFileRequest.Files?.Count > 0)
            {
                //  ScriptDeliveryサーバからダウンロード
                using (var client = new HttpClient())
                {
                    var content = new StringContent(
                        JsonSerializer.Serialize(rancher.DownloadFileRequest),
                        Encoding.UTF8,
                        "application/json");
                    var response = await client.PostAsync(server + "/download/list", content);
                    string json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(json);
                    

                }




            }







        }
    }
}

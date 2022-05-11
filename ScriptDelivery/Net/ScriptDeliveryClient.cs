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
        public async Task MappingRequest(string url)
        {
            var content = new StringContent("");
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(url, content);
                string json = await response.Content.ReadAsStringAsync();

                var mappingList = JsonSerializer.Deserialize<List<Mapping>>(json);

                foreach (Mapping mapping in mappingList)
                {
                    Console.WriteLine(mapping.Require.RequireMode);
                }
            }
        }

        public async Task DownloadListRequest(string url)
        {
            new StringContent("", Encoding.UTF8, "application/json");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptDelivery.Requires;
using ScriptDelivery.Requires.Matcher;
using ScriptDelivery.Works;
using ScriptDelivery.Net;
using System.Text.Json;
using System.Net;

namespace ScriptDelivery.Net
{
    internal class ClientSession
    {
        public List<Mapping> MappingList { get; set; }
        public List<string> SmbDownloadList { get; set; }
        public List<DownloadFile> HttpDownloadList { get; set; }

        public async void DownloadMappingFile(string server)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(server + "/map", new StringContent(""));
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    this.MappingList = JsonSerializer.Deserialize<List<Mapping>>(json);
                }
            }
        }

        public void MapMathcingCheck()
        {
            MappingList = MappingList.Where(x =>
            {
                return CheckRequire(x.Require.RequireRules, x.Require.GetRequireMode());
            }).ToList();

            this.SmbDownloadList = new List<string>();
            this.HttpDownloadList = new List<DownloadFile>();

            foreach (var mapping in MappingList)
            {
                mapping.Work.Downloads.ToList().ForEach(x =>
                {
                    if (x.SourcePath.StartsWith("\\\\"))
                    {
                        //  Smbダウンロード用ファイル
                        SmbDownloadList.Add(x.SourcePath);
                    }
                    else
                    {
                        //  Htttpダウンロード用ファイル
                        HttpDownloadList.Add(new DownloadFile()
                        {
                            Name = x.SourcePath,
                            DestinationPath = x.DestinationPath,
                        });
                    }
                });
            }

        }

        public void DownloadSmbFile()
        {

        }

        public async Task DownloadHttpFile(string server)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(HttpDownloadList),
                    Encoding.UTF8,
                    "application/json");
                var response = await client.PostAsync(server + "/download/list", content);
                string json = await response.Content.ReadAsStringAsync();

                List<DownloadFile> tempList = JsonSerializer.Deserialize<List<DownloadFile>>(json);


            }
        }




        private bool CheckRequire(RequireRule[] rules, RequireMode mode)
        {
            if (mode == RequireMode.None)
            {
                //  ReuqieModeがNoneの場合は、チェック無しにtrue
                return true;
            }
            var results = rules.ToList().Select(x =>
            {
                MatcherBase matcher = MatcherBase.Get(x.GetRuleTarget());
                matcher.SetParam(x.Param);
                return matcher.CheckParam() && matcher.IsMatch(x.GetMatchType());
            });

            return mode switch
            {
                RequireMode.And => results.All(x => x),
                RequireMode.Or => results.Any(x => x),
                _ => false,
            };
        }

    }
}

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
        public List<string> HttpDownloadList { get; set; }

        public async void DownloadMappingFile(string server)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(server + "/map", new StringContent(""));
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    this.MappingList = JsonSerializer.Deserialize<List<Mapping>>(json);
                }
            }
        }

        public void MapMathcingCheck()
        {
            foreach (var mapping in MappingList)
            {
                RequireMode mode = mapping.Require.GetRequireMode();

                bool ret = CheckRequire(mapping.Require.RequireRules, mode);
                if (ret)
                {
                    foreach (var download in mapping.Work.Downloads)
                    {
                        if (download.SourcePath.StartsWith("\\\\"))
                        {
                            //  ファイルサーバ(Smb)からダウンロード
                        }
                        else
                        {
                            //  ScriptDeliveryサーバからHTTPでダウンロード
                            HttpDownloadList ??= new List<string>();
                            HttpDownloadList.Add(download.SourcePath);



                            DownloadFile dlFile = new DownloadFile()
                            {
                                Name = download.SourcePath,
                                DestinationPath = download.DestinationPath,
                            };
                            
                            //  ⇒DownloadFileを送信/受信する感じで

                        }
                    }
                }
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

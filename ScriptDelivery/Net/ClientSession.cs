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


        private JsonSerializerOptions options = new System.Text.Json.JsonSerializerOptions()
        {
            IgnoreReadOnlyProperties = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        };



        public async void DownloadMappingFile(string server)
        {
            using (var client = new HttpClient())
            using (var content = new StringContent(""))
            using (var response = await client.PostAsync(server + "/map", content))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    this.MappingList = JsonSerializer.Deserialize<List<Mapping>>(json);
                }
            }
        }

        public void MapMathcingCheck()
        {
            Func<RequireRule[], RequireMode, bool> checkRequire = (rules, mode) =>
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
            };

            MappingList = MappingList.Where(x =>
            {
                return checkRequire(x.Require.RequireRules, x.Require.GetRequireMode());
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
                            Overwrite = x.GetForce(),
                        });
                    }
                });
            }
        }

        public void DownloadSmbFile()
        {

        }

        public async Task DownloadHttpSearch(string server)
        {
            using (var client = new HttpClient())
            using (var content = new StringContent(
                 JsonSerializer.Serialize(HttpDownloadList, options), Encoding.UTF8, "application/json"))
            using (var response = await client.PostAsync(server + "/download/list", content))
            {
                string json = await response.Content.ReadAsStringAsync();
                HttpDownloadList = JsonSerializer.Deserialize<List<DownloadFile>>(json);
            }
        }

        public async Task DownloadHttpStart(string server)
        {
            using (var client = new HttpClient())
            {
                foreach (var dlFile in HttpDownloadList)
                {
                    //  ローカル側のファイルとの一致チェック
                    if (!(dlFile.Downloadable ?? false)) { continue; }
                    if (dlFile.CompareFile(dlFile.DestinationPath) && !(dlFile.Overwrite ?? false))
                    {
                        continue;
                    }

                    string fileName = dlFile.Name.Replace("\\", "/");
                    using (var response = await client.GetAsync(server + "/download/files/" + dlFile.Name))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            using (var stream = await response.Content.ReadAsStreamAsync())
                            using (var fs = new FileStream(dlFile.DestinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                stream.CopyTo(fs);
                            }
                        }
                    }
                }
            }
        }
    }
}

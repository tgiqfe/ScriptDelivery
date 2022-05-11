using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptDelivery.Requires;
using ScriptDelivery.Requires.Matcher;
using ScriptDelivery.Works;
using ScriptDelivery.Net;

namespace ScriptDelivery
{
    internal class Rancher
    {
        public List<string> SmbDownloadList { get; set; }
        public DownloadFileRequest DownloadFileRequest { get; set; }

        public List<Mapping> MappingList = null;

        public Rancher() { }

        public Rancher(string filePath)
        {
            MappingList = Mapping.Deserialize(filePath);
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
                            DownloadFileRequest ??= new DownloadFileRequest(init: true);
                            DownloadFileRequest.Files.Add(download.SourcePath);
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

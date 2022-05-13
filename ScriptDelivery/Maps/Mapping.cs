using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptDelivery.Maps.Works;
using ScriptDelivery.Maps.Requires;

namespace ScriptDelivery.Maps
{
    internal class Mapping
    {
        public Require Require { get; set; }

        public Work Work { get; set; }

        /// <summary>
        /// CSV出力用。各lineを作成
        /// RequireのRule、WorkのDownloadは、それぞれ最初の1つのみを使用。(Csvでは、2つ以上のRuleやDownloadに対応しない想定)
        /// </summary>
        /// <returns></returns>
        public string[] ToParamArray()
        {
            return new string[]
            {
                Require.GetRequireMode().ToString(),
                Require.RequireRules[0].GetRuleTarget().ToString(),
                Require.RequireRules[0].GetRuleMatch().ToString(),
                Require.RequireRules[0].GetInvert().ToString(),
                Require.RequireRules?.Length > 0 ?
                    string.Join(" ", Require.RequireRules[0].Param.Select(x => $"{x.Key}={x.Value}")) : "",
                Work.Downloads[0].SourcePath ?? "",
                Work.Downloads[0].DestinationPath ?? "",
                Work.Downloads[0].GetForce().ToString(),
                Work.Downloads[0].UserName ?? "",
                Work.Downloads[0].Password ?? "",
            };
        }


    }
}

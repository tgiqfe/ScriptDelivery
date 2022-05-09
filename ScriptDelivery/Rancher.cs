using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptDelivery.Requires;
using ScriptDelivery.Requires.Matcher;
using ScriptDelivery.Works;

namespace ScriptDelivery
{
    internal class Rancher
    {
        private List<Mapping> _mappingList = null;

        public Rancher() { }

        public Rancher(string filePath)
        {
            _mappingList = Mapping.Deserialize(filePath);
        }

        public void RequestProcess()
        {
            foreach (var mapping in _mappingList)
            {
                RequireMode mode = mapping.Require.GetRequireMode();

                bool ret = TestRequire(mapping.Require.RequireRules, mode);
                if (ret)
                {
                    ProcessWork(mapping.Work.Downloads);
                }
            }
        }

        private bool TestRequire(RequireRule[] rules, RequireMode mode)
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

        private void ProcessWork(Download[] downloads)
        {


            //  ここにダウンロード処理を開始させていく記述を


        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptDelivery.Lib;

namespace ScriptDelivery.Map.Requires.Matcher
{
    /// <summary>
    /// IPアドレスのマッチ確認
    /// - Equal     : 完全一致確認。ワイルドカードで部分確認にも対応
    /// - Range     : 第四オクテットのみ範囲確認
    /// - InNetwork : 指定のネットワークアドレスに所属しているかどうかの確認
    /// </summary>
    internal class IPAddressMatcher : MatcherBase
    {
        [MatcherParameter, Keys("IPAddress")]
        public string IPAddress { get; set; }

        [MatcherParameter, Keys("NetworkAddress")]
        public string NetworkAddress { get; set; }

        [MatcherParameter, Keys("Start")]
        public string StartAddress { get; set; }

        [MatcherParameter, Keys("End")]
        public string EndAddress { get; set; }

        [MatcherParameter, Keys("Interface")]
        public string Interface { get; set; }

        private static NetworkInfo _info = null;

        public bool IsMatch(MatchType matchType, string text)
        {
            _info ??= new NetworkInfo();

            return matchType switch
            {
                MatchType.Equal => EqualMatch(),
                MatchType.Range => RangeMatch(),
                MatchType.InNetwork => InNetworkMatch(),
                _ => false,
            };
        }

        #region Match methods

        /// <summary>
        /// Equal確認
        /// </summary>
        /// <returns></returns>
        private bool EqualMatch()
        {
            var nics = GetNICsFromInterface();

            if (IPAddress.Contains("*"))
            {
                var pattern = this.IPAddress.GetWildcardPattern();
                return nics.Any(x => x.GetIPAddresses().Any(y => pattern.IsMatch(y)));
            }
            else
            {
                return nics.Any(x => x.GetIPAddresses().Any(y => y == IPAddress));
            }
        }

        /// <summary>
        /// 第四オクテットの範囲確認
        /// </summary>
        /// <returns></returns>
        private bool RangeMatch()
        {


            return false;
        }

        /// <summary>
        /// ネットワーク所属確認
        /// </summary>
        /// <returns></returns>
        private bool InNetworkMatch()
        {
            return false;
        }

        #endregion

        private IEnumerable<NetworkInfo.NIC> GetNICsFromInterface()
        {
            if (!string.IsNullOrEmpty(this.Interface))
            {
                if (Interface.Contains("*"))
                {
                    var pattern = Interface.GetWildcardPattern();
                    return _info.NICs.Where(x => pattern.IsMatch(x.Name));
                }
                else
                {
                    return _info.NICs.Where(x => x.Name.Equals(Interface, StringComparison.OrdinalIgnoreCase));
                }
            }
            return _info.NICs;
        }
    }
}

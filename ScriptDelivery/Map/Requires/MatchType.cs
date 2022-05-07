using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptDelivery.Map.Requires
{
    internal enum MatchType
    {
        None,           //  マッチ条件無し。
        Equal,          //  完全一致判定、ワイルドカードを使用してContains,StartsWith,EndsWith
        Range,          //  末尾数字部分のみの範囲判定。IPアドレスの場合は第四オクテットのみ
        NameRange,      //  ホスト名の前半部分と数字の範囲で判定
        InNetwork,      //  IPアドレスがネットワークアドレスの範囲かどうか
        File,           //  ファイルに関する判定
        Directory,      //  フォルダーに関する判定
        Registry        //  レジストリキー、レジストリ値に関する判定
    }
}

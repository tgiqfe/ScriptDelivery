using System;
using System.Text;
using ScriptDelivery;
using System.Runtime.InteropServices;
using System.Diagnostics;

/// <summary>
/// 静的パラメータを格納
/// </summary>
namespace ScriptDelivery.Server.ServerLib
{
    internal class Item
    {
        /// <summary>
        /// 実行中のOSの判定
        /// </summary>
        public static Platform Platform { get; set; }

        /// <summary>
        /// アプリケーション全体の制御情報
        /// </summary>
        public static Setting Setting { get; set; }

        /// <summary>
        /// Mappingのリストとそれらの操作
        /// </summary>
        public static MappingFileCollection MappingFileCollection { get; set; }

        /// <summary>
        /// ダウンロード対象ファイルのリスト
        /// </summary>
        public static DownloadFileCollection DownloadFileCollection { get; set; }
    }
}

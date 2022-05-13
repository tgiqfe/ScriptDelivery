using System;
using System.Text;
using ScriptDelivery;
using System.Runtime.InteropServices;
using System.Diagnostics;
using ScriptDelivery.Files;

/// <summary>
/// 静的パラメータを格納
/// </summary>
namespace ScriptDelivery
{
    internal class Item
    {
        #region Path

        /// <summary>
        /// 実行ファイルへのパス
        /// 単一実行ファイルにした場合、Assembly.Locationが使用できないので、Processからファイルパスを取得
        /// </summary>
        //public static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;
        public static readonly string ExecFilePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

        /// <summary>
        /// 実行ファイルの名前(プロセス名)
        /// </summary>
        //public static readonly string ProcessName = Path.GetFileNameWithoutExtension(ExecFilePath);
        public static readonly string ProcessName = "ScriptDelivery";

        /// <summary>
        /// 実行ファイルの場所
        /// </summary>
        public static readonly string ExecDirectoryPath = Path.GetDirectoryName(ExecFilePath);

        /// <summary>
        /// ワークフォルダー
        /// システムアカウントの場合は実行ファイルの場所。それ以外はTempフォルダー配下
        /// </summary>
        //  無しで良いかもしれない
        /*
        public static readonly string WorkDirectoryPath =
            EnumRun.Lib.UserInfo.IsSystemAccount ?
                ExecDirectoryPath :
                Path.Combine(Path.GetTempPath(), "EnumRun");
        */

        #endregion
        #region Serial
        private static string _serial = null;

        public static string Serial
        {
            get
            {
                if (_serial == null)
                {
                    var md5 = System.Security.Cryptography.MD5.Create();
                    var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(
                        DateTime.Now.ToString() + Environment.MachineName + Process.GetCurrentProcess().Id.ToString()));
                    _serial = BitConverter.ToString(bytes).Replace("-", "");
                    md5.Clear();
                }
                return _serial;
            }
        }
        #endregion

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

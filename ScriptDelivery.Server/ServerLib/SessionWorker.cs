using System;
using System.Text;
using ScriptDelivery;
using System.Runtime.InteropServices;
using System.Diagnostics;
using ScriptDelivery.Net;

namespace ScriptDelivery.Server.ServerLib
{
    public class SessionWorker
    {
        public SessionWorker() { }

        /// <summary>
        /// アプリケーション実行開始時の動作
        /// </summary>
        public void OnStart()
        {
            //  OSチェック
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Item.Platform = Platform.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Item.Platform = Platform.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Item.Platform = Platform.MacOSX;
            }

            //  設定ファイル読み込み
            Item.Setting = Setting.Deserialize("setting.json");
            Item.Setting.ChangePath();

            //  Mappingリストを取得
            Item.MappingFileCollection = new MappingFileCollection(Item.Setting.MapsPath);

            //  ダウンロードリストを取得
            Item.DownloadFileCollection = new DownloadFileCollection(Item.Setting.FilesPath);
        }

        /// <summary>
        /// アプリケーション終了時の動作
        /// </summary>
        public void OnStop()
        {
            
        }
    }
}

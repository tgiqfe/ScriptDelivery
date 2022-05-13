using System;
using System.Text;
using ScriptDelivery;
using System.Runtime.InteropServices;
using System.Diagnostics;
using ScriptDelivery.Files;
using ScriptDelivery.Logs;

namespace ScriptDelivery
{
    public class SessionWorker : IDisposable
    {
        public SessionWorker()
        {
            OnStart();
        }

        /// <summary>
        /// アプリケーション実行開始時の動作
        /// </summary>
        private void OnStart()
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

            //  ログ出力開始
            Item.Logger = new Logs.ServerLogger(Item.Setting);

            //  アプリケーションバージョン
            Item.CurrentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// アプリケーション終了時の動作
        /// </summary>
        private void OnStop()
        {
            //  ログ出力終了
            if (Item.Logger != null)
            {
                Item.Logger.Dispose();
                Item.Logger = null;
            }
        }

        #region Dispose

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OnStop();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

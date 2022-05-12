
using System.Text;
using System.Diagnostics;

namespace ScriptDelivery
{
    internal class ProcessLogger
    {
        private string _logPath;
        protected StreamWriter _writer = null;
        protected ReaderWriterLock _rwLock = null;
        private LogLevel _minLogLevel = LogLevel.Info;

        public ProcessLogger() { }

        public ProcessLogger(string logPath)
        {
            _logPath = logPath;
            _writer = new StreamWriter(logPath, true, Encoding.UTF8);
            _rwLock = new ReaderWriterLock();

            Write("開始");
        }

        #region Log output

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="level"></param>
        /// <param name="scriptFile"></param>
        /// <param name="message"></param>
        public void Write(LogLevel level, string scriptFile, string message)
        {
            if (level >= _minLogLevel)
            {
                try
                {
                    _rwLock.AcquireWriterLock(1000);
                    _writer.WriteLine(string.Join("[{0}]<{1}>{2} {3}",
                        DateTime.Now.ToString("yyyy/MM/dd HH::mm:ss"),
                        level,
                        scriptFile,
                        message));
                }
                catch { }
                finally
                {
                    _rwLock.ReleaseWriterLock();
                }
            }
        }

        /// <summary>
        /// ログ出力 (strign.Format対応)
        /// </summary>
        /// <param name="level"></param>
        /// <param name="scriptFile"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void Write(LogLevel level, string scriptFile, string format, params object[] args)
        {
            Write(level, scriptFile, string.Format(format, args));
        }

        /// <summary>
        /// ログ出力 (スクリプトファイル:無し)
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void Write(LogLevel level, string message)
        {
            Write(level, null, message);
        }

        /// <summary>
        /// ログ出力 (レベル:Info, スクリプトファイル:無し)
        /// </summary>
        /// <param name="message"></param>
        public void Write(string message)
        {
            Write(LogLevel.Info, null, message);
        }

        #endregion

        public void Close()
        {
            Write("終了");

            try
            {
                _rwLock.AcquireWriterLock(10000);
                _rwLock.ReleaseWriterLock();
            }
            catch { }

            if (_writer != null) { _writer.Dispose(); }
        }

        #region Dispose

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Close();
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

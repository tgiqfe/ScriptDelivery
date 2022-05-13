using ScriptDelivery.Lib;
using System.Text;
using LiteDB;
using ScriptDelivery.Logs;
using ScriptDelivery.Lib.Syslog;
using System.Diagnostics;

namespace ScriptDelivery.Logs
{
    internal class ServerLogger : LoggerBase
    {
        protected override bool _logAppend { get { return true; } }

        private LogLevel _minLogLevel = LogLevel.Info;
        //private ILiteCollection<ProcessLogBody> _logstashCollection = null;
        private ILiteCollection<ServerLogBody> _syslogCollection = null;

        public ServerLogger(Setting setting)
        {
            string logFileName =
                $"ScriptDelivery_{DateTime.Now.ToString("yyyyMMdd")}.log";
            string logPath = Path.Combine(setting.GetLogsPath(), logFileName);
            TargetDirectory.CreateParent(logPath);

            _logDir = setting.GetLogsPath();
            _writer = new StreamWriter(logPath, _logAppend, Encoding.UTF8);
            _rwLock = new ReaderWriterLock();
            _minLogLevel = LogLevelMapper.ToLogLevel(setting.MinLogLevel);

            if (!string.IsNullOrEmpty(setting.Syslog?.Server))
            {
                _syslog = new SyslogTransport(setting);
                _syslog.Facility = FacilityMapper.ToFacility(setting.Syslog.Facility);
                _syslog.AppName = Item.ProcessName;
                _syslog.ProcId = ServerLogBody.TAG;
            }

            Write("開始");
        }

        #region Log output

        public void Write(LogLevel level, string clientIP, int clientPort, string title, string message)
        {
            if (level >= _minLogLevel)
            {
                SendAsync(new ServerLogBody(init: true)
                {
                    Date = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                    Level = level,
                    ClientIP = clientIP,
                    ClientPort = clientPort,
                    Title = title,
                    Message = message,
                }).ConfigureAwait(false);
            }
        }

        public void Write(string message)
        {
            Write(LogLevel.Info, clientIP: null, clientPort: 0, title: "", message);
        }

        #endregion

        private async Task SendAsync(ServerLogBody body)
        {
            try
            {
                _rwLock.AcquireWriterLock(10000);

                string json = body.GetJson();

                //  ファイル書き込み
                await _writer.WriteLineAsync(json);

                //  Syslog転送
                if (_syslog?.Enabled ?? false)
                {
                    await _syslog.SendAsync(body.Level, body.Title, body.Message);
                }
                else
                {
                    _liteDB ??= GetLiteDB();
                    _syslogCollection ??= GetCollection<ServerLogBody>(ServerLogBody.TAG + "_syslog");
                    _syslogCollection.Upsert(body);
                }
            }
            catch { }
            finally
            {
                _rwLock.ReleaseWriterLock();
            }
        }

        public override void Close()
        {
            Write("終了");

            try
            {
                _rwLock.AcquireWriterLock(10000);
                _rwLock.ReleaseWriterLock();
            }
            catch { }

            if (_writer != null) { _writer.Dispose(); }
            if (_liteDB != null) { _liteDB.Dispose(); }
            if (_syslog != null) { _syslog.Dispose(); }
        }
    }
}

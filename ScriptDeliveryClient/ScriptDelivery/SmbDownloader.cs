﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using ScriptDeliveryClient.Logs;
using ScriptDeliveryClient.Logs.ProcessLog;

namespace ScriptDeliveryClient.ScriptDelivery
{
    internal class SmbDownloader
    {
        const int _timeout = 3000;

        private Dictionary<string, SmbSession> _sessions = null;
        private List<DownloadSmb> _list = null;
        private ProcessLogger _logger = null;

        public SmbDownloader(ProcessLogger logger)
        {
            _logger = logger;
            this._sessions = new Dictionary<string, SmbSession>(StringComparer.OrdinalIgnoreCase);
            this._list = new List<DownloadSmb>();
        }

        public void Add(string targetPath, string destination, string userName, string password, bool overwrite)
        {
            if (CheckParam(targetPath, destination, userName, password))
            {
                _list.Add(new DownloadSmb()
                {
                    TargetPath = targetPath,
                    ShareName = SmbSession.GetShareName(targetPath),
                    Destination = destination,
                    UserName = userName,
                    Password = password,
                    Overwrite = overwrite,
                });
            }
        }

        private bool CheckParam(string targetPath, string destination, string userName, string password)
        {
            string logTitle = "CheckParam";

            if (string.IsNullOrEmpty(targetPath) ||
                !(string.IsNullOrEmpty(userName) ^ string.IsNullOrEmpty(password)))
            {
                _logger.Write(LogLevel.Attention, logTitle, "Smb download parameter is not enough.");
                return false;
            }
            if (targetPath.Contains("*") || (destination?.Contains("*") ?? false))
            {
                _logger.Write(LogLevel.Attention, logTitle, "Smb download parameter is incorrect, * is included.");
                return false;
            }

            return true;
        }

        public void DownloadProcess()
        {
            string logTitle = "DownloadProcess";

            foreach (var smb in _list)
            {
                string targetPath = smb.TargetPath;
                string destination = smb.Destination;
                bool overwrite = smb.Overwrite;

                if (FileExists(targetPath))
                {
                    DownloadFile(targetPath, destination, overwrite);
                    return;
                }
                if (DirectoryExists(targetPath))
                {
                    DownloadDirectory(targetPath, destination, overwrite);
                    return;
                }

                bool ret = ConnectServer(smb);
                if (ret)
                {
                    _logger.Write(LogLevel.Info, logTitle, "Connect success.");
                    if (FileExists(targetPath))
                    {
                        DownloadFile(targetPath, destination, overwrite);
                        return;
                    }
                    if (DirectoryExists(targetPath))
                    {
                        DownloadDirectory(targetPath, destination, overwrite);
                        return;
                    }
                }
                else
                {
                    _logger.Write(LogLevel.Warn, logTitle, "Connect failed.");
                }
            }
        }

        private bool FileExists(string path)
        {
            var task = Task.Factory.StartNew(() => File.Exists(path));
            return task.Wait(_timeout) && task.Result;
        }

        private bool DirectoryExists(string path)
        {
            var task = Task.Factory.StartNew(() => Directory.Exists(path));
            return task.Wait(_timeout) && task.Result;
        }

        public bool ConnectServer(DownloadSmb smb)
        {
            string logTitle = "ConnectServer";

            string shareName = smb.ShareName;
            _logger.Write(LogLevel.Debug, logTitle, "Connect server => {0}", shareName);

            if (_sessions.ContainsKey(shareName))
            {
                _sessions[shareName].Disconnect();
            }
            _sessions[shareName] = new SmbSession(shareName, smb.UserName, smb.Password);
            _sessions[shareName].Connect();

            return _sessions[shareName].Connected;
        }

        private void DownloadFile(string targetPath, string destination, bool overwrite)
        {
            string logTitle = "DownloadFile";

            _logger.Write(LogLevel.Info, logTitle, "File download. => {0}", targetPath);

            //  destinationパスの最後が「\」の場合はフォルダーとして扱い、その配下にダウンロード。
            if (destination.EndsWith("\\"))
            {
                _logger.Write(LogLevel.Debug, logTitle, "Destination path as directory, to => {0}", destination);

                string destinationFilePath = Path.Combine(destination, Path.GetFileName(targetPath));
                if (File.Exists(destinationFilePath) && !overwrite)
                {
                    //  上書き禁止 終了
                    _logger.Write(LogLevel.Info, logTitle, "Skip Smb download, already exist. => {0}", destinationFilePath);
                    return;
                }
                if (!Directory.Exists(destination))
                {
                    Directory.CreateDirectory(destination);
                }
                File.Copy(targetPath, destinationFilePath, overwrite: true);

                return;
            }

            //  destinationのパスのフォルダーが存在する場合、その配下にダウンロード。
            if (Directory.Exists(destination))
            {
                _logger.Write(LogLevel.Debug, logTitle, "Destination is in directory, to => {0}", destination);

                string destinationFilePath = Path.Combine(destination, Path.GetFileName(targetPath));
                if (File.Exists(destinationFilePath) && !overwrite)
                {
                    //  上書き禁止 終了
                    _logger.Write(LogLevel.Info, logTitle, "Skip Smb download, already exist. => {0}", destinationFilePath);
                    return;
                }
                File.Copy(targetPath, destinationFilePath, overwrite: true);

                return;
            }

            //  ファイルをダウンロード。
            if (File.Exists(destination) && !overwrite)
            {
                //  上書き禁止 終了
                _logger.Write(LogLevel.Info, logTitle, "Skip Smb download, already exist. => {0}", destination);
                return;
            }
            string parent = Path.GetDirectoryName(destination);
            if (!Directory.Exists(parent))
            {
                Directory.CreateDirectory(parent);
            }
            _logger.Write(LogLevel.Debug, logTitle, "File copy, to => {0}", destination);
            File.Copy(targetPath, destination, overwrite: true);
        }

        private void DownloadDirectory(string targetPath, string destination, bool overwrite)
        {
            string logTitle = "DownloadDirectory";

            _logger.Write(LogLevel.Info, logTitle, "Directory download => {0}", targetPath);

            Action<string, string> robocopy = (src, dst) =>
            {
                using (var proc = new Process())
                {
                    proc.StartInfo.FileName = "robocopy.exe";
                    proc.StartInfo.Arguments = $"\"{src}\" \"{dst}\" /COPY:DAT /MIR /NP";
                    proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.Start();
                    proc.WaitForExit();
                }
            };

            destination = ExpandEnvironment(destination);

            //  destinationパスの最後が「\」の場合はフォルダーとして扱い、その配下にダウンロード。
            if (destination.EndsWith("\\"))
            {
                _logger.Write(LogLevel.Debug, logTitle, "Destination path as directory, to => {0}", destination);

                string destinationChild = Path.Combine(destination, Path.GetFileName(targetPath));
                if (Directory.Exists(destinationChild) && !overwrite)
                {
                    //  上書き禁止 終了
                    _logger.Write(LogLevel.Info, logTitle, "Skip Smb download, already exist. => {0}", destinationChild);
                    return;
                }
                robocopy(targetPath, destinationChild);

                return;
            }

            //  フォルダーをダウンロード
            if (Directory.Exists(destination) && !overwrite)
            {
                //  上書き禁止 終了
                _logger.Write(LogLevel.Info, logTitle, "Skip Smb download, already exist. => {0}", destination);
                return;
            }
            _logger.Write(LogLevel.Debug, logTitle, "Directory copy, to => {0}", destination);
            robocopy(targetPath, destination);
        }

        private string ExpandEnvironment(string text)
        {
            for (int i = 0; i < 5 && text.Contains("%"); i++)
            {

                text = Environment.ExpandEnvironmentVariables(text);
            }
            return text;
        }

        //  [案]後で確認。Smbで接続した後の切断処理が実装されているかどうか

        public void Close()
        {
            foreach (var pair in _sessions)
            {
                pair.Value.Disconnect();
            }
        }
    }
}

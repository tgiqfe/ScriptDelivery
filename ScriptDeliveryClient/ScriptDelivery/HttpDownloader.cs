﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptDelivery.Files;
using ScriptDeliveryClient.Logs;
using ScriptDeliveryClient.Logs.ProcessLog;
using System.Text.Json;
using System.Net;
using ScriptDeliveryClient.Lib;

namespace ScriptDeliveryClient.ScriptDelivery
{
    internal class HttpDownloader
    {
        private string _filesPath = null;
        private ProcessLogger _logger = null;
        private JsonSerializerOptions _options = null;
        private List<DownloadHttp> _list = null;

        public HttpDownloader(string filesPath, ProcessLogger logger)
        {
            //this._uri = uri;
            this._filesPath = filesPath;
            this._logger = logger;
            this._options = new System.Text.Json.JsonSerializerOptions()
            {
                //Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                IgnoreReadOnlyProperties = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                //WriteIndented = true,
                //Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };
            this._list = new List<DownloadHttp>();
        }

        public void Add(string path, string destination, bool overwrite)
        {
            if (CheckParam(path))
            {
                _list.Add(new DownloadHttp()
                {
                    Path = path,
                    DestinationPath = destination,
                    Overwrite = overwrite,
                });
            }
        }

        private bool CheckParam(string path)
        {
            string logTitle = "CheckParam";

            if (string.IsNullOrEmpty(path))
            {
                _logger.Write(LogLevel.Attention, logTitle, "Http download parameter is not enough.");
                return false;
            }
            if (Path.IsPathRooted(path))
            {
                _logger.Write(LogLevel.Attention, logTitle, "Http download parameter is incorrect, path is absolute path.");
                return false;
            }

            return true;
        }

        public void DownloadProcess(HttpClient client, string uri)
        {
            if (this._list.Count > 0)
            {
                DownloadHttpSearch(client, uri).Wait();
                DownloadHttpStart(client, uri).Wait();
            }
        }

        /// <summary>
        /// Httpダウンロードする場合に、ScriptDeliveryサーバにダウンロード可能ファイルを問い合わせ
        /// </summary>
        /// <returns></returns>
        private async Task DownloadHttpSearch(HttpClient client, string _uri)
        {
            string logTitle = "DownloadHttpSearch";
            _logger.Write(LogLevel.Debug, logTitle, "Search, download file from ScriptDelivery server.");

            string reqJson = JsonSerializer.Serialize(_list, _options);

            //  デバッグ用
            //Console.WriteLine(reqJson);

            using (var content = new StringContent(reqJson, Encoding.UTF8, "application/json"))
            using (var response = await client.PostAsync(_uri + "/download/list", content))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string resJson = await response.Content.ReadAsStringAsync();
                    _list = JsonSerializer.Deserialize<List<DownloadHttp>>(resJson);

                    _logger.Write(LogLevel.Info, logTitle, "Success, download DownloadFile list object");
                    foreach (var downloadHttp in _list)
                    {
                        _logger.Write(downloadHttp.ToLog());
                    }
                }
                else
                {
                    _logger.Write(LogLevel.Error, logTitle, "Failed, download DownloadFile list object");
                }
            }
        }

        /// <summary>
        /// ScriptDeliveryサーバからファイルダウンロード
        /// </summary>
        /// <returns></returns>
        private async Task DownloadHttpStart(HttpClient client, string _uri)
        {
            string logTitle = "DownloadHttpStart";
            _logger.Write(LogLevel.Debug, logTitle, "Start, Http download.");

            foreach (var dlFile in _list)
            {
                string dstPath = string.IsNullOrEmpty(dlFile.DestinationPath) ?
                    Path.Combine(_filesPath, Path.GetFileName(dlFile.Path)) :
                    Path.Combine(dlFile.DestinationPath, Path.GetFileName(dlFile.Path));

                //  ローカル側のファイルとの一致チェック
                if (!(dlFile.Downloadable ?? false)) { continue; }
                if (File.Exists(dstPath) &&
                    (dlFile.CompareFile(dstPath) || !(dlFile.Overwrite ?? false)))
                {
                    _logger.Write(LogLevel.Info, logTitle, "Skip Http download, already exist. => [{0}]", dstPath);
                    continue;
                }
                TargetDirectory.CreateParent(dstPath);

                //  ダウンロード要求を送信し、ダウンロード開始
                var query = new Dictionary<string, string>()
                {
                    { "fileName", dlFile.Path }
                };
                using (var response = await client.GetAsync(_uri + $"/download/files?{await new FormUrlEncodedContent(query).ReadAsStringAsync()}"))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        using (var fs = new FileStream(dstPath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            stream.CopyTo(fs);
                        }
                        if (dlFile.LastWriteTime != null)
                        {
                            File.SetLastWriteTime(dstPath, (DateTime)dlFile.LastWriteTime);
                        }
                        _logger.Write(LogLevel.Info, logTitle, "Success, file download. [{0}]", dstPath);
                    }
                    else
                    {
                        _logger.Write(LogLevel.Info, logTitle, "Failed, file download. [{0}]", dstPath);
                    }
                }
            }
        }
    }
}

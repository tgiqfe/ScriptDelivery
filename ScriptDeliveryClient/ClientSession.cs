﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptDelivery.Maps.Requires;
using ScriptDeliveryClient.Maps.Matcher;
using ScriptDelivery.Maps;
using System.Text.Json;
using System.Net;
using ScriptDelivery.Files;

namespace ScriptDelivery
{
    internal class ClientSession : IDisposable
    {
        public List<Mapping> MappingList { get; set; }
        public List<string> SmbDownloadList { get; set; }
        public List<DownloadFile> HttpDownloadList { get; set; }

        private HttpClient _client = null;

        private string _server = null;
        private ProcessLogger _logger = null;

        private JsonSerializerOptions _options = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ClientSession(string server, ProcessLogger logger)
        {
            this._client = new HttpClient();
            this._server = server;
            this._logger = logger;
            this._options = new System.Text.Json.JsonSerializerOptions()
            {
                //Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                IgnoreReadOnlyProperties = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                //WriteIndented = true,
                //Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };
            _logger.Write(LogLevel.Info, "Connect server => {0}", _server);
        }

        /// <summary>
        /// ScriptDeliveryサーバからMappingファイルをダウンロード
        /// </summary>
        /// <returns></returns>
        public async Task DownloadMappingFile()
        {
            _logger.Write(LogLevel.Debug, "ScriptDelivery init.");
            using (var content = new StringContent(""))
            using (var response = await _client.PostAsync(_server + "/map", content))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    this.MappingList = JsonSerializer.Deserialize<List<Mapping>>(json);
                    _logger.Write(LogLevel.Info, "Success, download mapping object.");
                }
                else
                {
                    _logger.Write(LogLevel.Error, "Failed, download mapping object.");
                }
            }
        }

        /// <summary>
        /// MappingデータとローカルPC情報を確認し、ダウンロード対象のファイルを取得
        /// </summary>
        public void MapMathcingCheck()
        {
            _logger.Write(LogLevel.Debug, "Check, mapping object.");

            MappingList = MappingList.Where(x =>
            {
                RequireMode mode = x.Require.GetRequireMode();
                if (mode == RequireMode.None)
                {
                    return true;
                }
                IEnumerable<bool> results = x.Require.Rules.Select(x =>
                {
                    MatcherBase matcher = MatcherBase.Activate(x.GetRuleTarget());
                    matcher.SetLogger(_logger);
                    matcher.SetParam(x.Param);
                    return (matcher.CheckParam() ^ x.GetInvert()) && matcher.IsMatch(x.GetRuleMatch());
                });
                return mode switch
                {
                    RequireMode.And => results.All(x => x),
                    RequireMode.Or => results.Any(x => x),
                    _ => false,
                };
            }).ToList();

            _logger.Write(LogLevel.Debug, null, "Finish, require check [Match => {0} count]", MappingList.Count);
            this.SmbDownloadList = new List<string>();
            this.HttpDownloadList = new List<DownloadFile>();

            foreach (var mapping in MappingList)
            {
                foreach (var download in mapping.Work.Downloads)
                {
                    if (string.IsNullOrEmpty(download.Source) || string.IsNullOrEmpty(download.Destination))
                    {
                        _logger.Write(LogLevel.Attention, null, "Parameter mission, Source or Destination or both.");
                    }
                    else if (download.Source.StartsWith("\\\\"))
                    {
                        //  Smbダウンロード用ファイル
                        SmbDownloadList.Add(download.Source);
                    }
                    else
                    {
                        //  Htttpダウンロード用ファイル
                        HttpDownloadList.Add(new DownloadFile()
                        {
                            Name = download.Source,
                            DestinationPath = download.Destination,
                            Overwrite = !download.GetKeep(),
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Smbダウンロード
        /// </summary>
        public void DownloadSmbFile()
        {
            _logger.Write(LogLevel.Debug, "Search, download file from SMB server.");

            if (SmbDownloadList?.Count > 0) { }
        }

        /// <summary>
        /// Httpダウンロードする場合に、ScriptDeliveryサーバにダウンロード可能ファイルを問い合わせ
        /// </summary>
        /// <returns></returns>
        public async Task DownloadHttpSearch()
        {
            _logger.Write(LogLevel.Debug, "Search, download file from ScriptDelivery server.");

            if (HttpDownloadList?.Count > 0)
            {
                using (var content = new StringContent(
                     JsonSerializer.Serialize(HttpDownloadList, _options), Encoding.UTF8, "application/json"))
                using (var response = await _client.PostAsync(_server + "/download/list", content))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        HttpDownloadList = JsonSerializer.Deserialize<List<DownloadFile>>(json);

                        _logger.Write(LogLevel.Info, "Success, download DownloadFile list object");
                    }
                    else
                    {
                        _logger.Write(LogLevel.Error, "Failed, download DownloadFile list object");
                    }
                }
            }
        }

        /// <summary>
        /// ScriptDeliveryサーバからファイルダウンロード
        /// </summary>
        /// <returns></returns>
        public async Task DownloadHttpStart()
        {
            _logger.Write(LogLevel.Debug, "Start, Http download.");

            if (HttpDownloadList?.Count > 0)
            {
                foreach (var dlFile in HttpDownloadList)
                {
                    string dstPath = ExpandEnvironment(dlFile.DestinationPath);

                    //  ローカル側のファイルとの一致チェック
                    if (!(dlFile.Downloadable ?? false)) { continue; }
                    if (dlFile.CompareFile(dstPath) && !(dlFile.Overwrite ?? false))
                    {
                        continue;
                    }

                    //  ダウンロード要求を送信し、ダウンロード開始
                    var query = new Dictionary<string, string>()
                    {
                        { "fileName", dlFile.Name }
                    };
                    using (var response = await _client.GetAsync(_server + $"/download/files?{await new FormUrlEncodedContent(query).ReadAsStringAsync()}"))
                    {
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            using (var stream = await response.Content.ReadAsStreamAsync())
                            using (var fs = new FileStream(dstPath, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                stream.CopyTo(fs);
                            }

                            _logger.Write(LogLevel.Info, "Success, file download. [{0}]", dstPath);
                        }
                        else
                        {
                            _logger.Write(LogLevel.Info, "Failed, file download. [{0}]", dstPath);
                        }
                    }
                }
            }
        }

        private string ExpandEnvironment(string text)
        {
            for (int i = 0; i < 5 && text.Contains("%"); i++)
            {
                text = Environment.ExpandEnvironmentVariables(text);
            }
            return text;
        }

        /// <summary>
        /// 終了
        /// </summary>
        public void Close()
        {
            if (_client != null)
            {
                _client.Dispose();
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

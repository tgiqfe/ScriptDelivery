using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptDelivery.Requires;
using ScriptDelivery.Requires.Matcher;
using ScriptDelivery.Works;
using ScriptDelivery.Net;
using System.Text.Json;
using System.Net;

namespace ScriptDelivery
{
    internal class ClientSession : IDisposable
    {
        public List<Mapping> MappingList { get; set; }
        public List<string> SmbDownloadList { get; set; }
        public List<DownloadFile> HttpDownloadList { get; set; }

        private HttpClient _client = null;

        private string _server = null;

        private JsonSerializerOptions _options = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ClientSession(string server)
        {
            this._client = new HttpClient();
            this._server = server;
            this._options = new System.Text.Json.JsonSerializerOptions()
            {
                //Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                IgnoreReadOnlyProperties = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                //WriteIndented = true,
                //Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };
            //  ●[log]接続先サーバの情報
        }

        /// <summary>
        /// ScriptDeliveryサーバからMappingファイルをダウンロード
        /// </summary>
        /// <returns></returns>
        public async Task DownloadMappingFile()
        {
            //  ●[log]Init接続。Mappingファイルの要求
            using (var content = new StringContent(""))
            using (var response = await _client.PostAsync(_server + "/map", content))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    this.MappingList = JsonSerializer.Deserialize<List<Mapping>>(json);
                    //  ●[log]接続成功。Mappingファイル取得
                }
                else
                {
                    //  ●[log]Mappingファイル取得失敗
                }
            }
        }

        /// <summary>
        /// MappingデータとローカルPC情報を確認し、ダウンロード対象のファイルを取得
        /// </summary>
        public void MapMathcingCheck()
        {
            //  ●[log]Mappingファイルの中のRequireチェック

            MappingList = MappingList.Where(x =>
            {
                RequireMode mode = x.Require.GetRequireMode();
                if (mode == RequireMode.None)
                {
                    return true;
                }
                IEnumerable<bool> results = x.Require.RequireRules.Select(x =>
                {
                    MatcherBase matcher = MatcherBase.Activate(x.GetRuleTarget());
                    matcher.SetParam(x.Param);
                    return matcher.CheckParam() && matcher.IsMatch(x.GetMatchType());
                });
                return mode switch
                {
                    RequireMode.And => results.All(x => x),
                    RequireMode.Or => results.Any(x => x),
                    _ => false,
                };
            }).ToList();

            //  ●[log]Requireチェック完了。Match: {MappingList.Length}件

            this.SmbDownloadList = new List<string>();
            this.HttpDownloadList = new List<DownloadFile>();

            foreach (var mapping in MappingList)
            {
                mapping.Work.Downloads.ToList().ForEach(x =>
                {
                    if (x.SourcePath.StartsWith("\\\\"))
                    {
                        //  Smbダウンロード用ファイル
                        SmbDownloadList.Add(x.SourcePath);
                    }
                    else
                    {
                        //  Htttpダウンロード用ファイル
                        HttpDownloadList.Add(new DownloadFile()
                        {
                            Name = x.SourcePath,
                            DestinationPath = x.DestinationPath,
                            Overwrite = x.GetForce(),
                        });
                    }
                });
            }
        }

        /// <summary>
        /// Smbダウンロード
        /// </summary>
        public void DownloadSmbFile()
        {
            if (SmbDownloadList?.Count > 0) { }
        }

        /// <summary>
        /// Httpダウンロードする場合に、ScriptDeliveryサーバにダウンロード可能ファイルを問い合わせ
        /// </summary>
        /// <returns></returns>
        public async Task DownloadHttpSearch()
        {
            //  ●[log]Http Downloadファイル確認をサーバに送信

            if (HttpDownloadList?.Count > 0)
            {
                using (var content = new StringContent(
                     JsonSerializer.Serialize(HttpDownloadList, _options), Encoding.UTF8, "application/json"))
                using (var response = await _client.PostAsync(_server + "/download/list", content))
                {
                    if(response.StatusCode == HttpStatusCode.OK)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        HttpDownloadList = JsonSerializer.Deserialize<List<DownloadFile>>(json);
                        //  ●[log]接続成功。Downloadファイルリスト取得
                    }
                    else
                    {
                        //  ●[log]Downloadファイルリスト取得に失敗
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
            //  ●[log]Http Downloadファイルを要求

            if (HttpDownloadList?.Count > 0)
            {
                foreach (var dlFile in HttpDownloadList)
                {
                    //  ローカル側のファイルとの一致チェック
                    if (!(dlFile.Downloadable ?? false)) { continue; }
                    if (dlFile.CompareFile(dlFile.DestinationPath) && !(dlFile.Overwrite ?? false))
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
                            using (var fs = new FileStream(dlFile.DestinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                stream.CopyTo(fs);
                            }
                            //  ●[log]Http Download完了。Path: {dlFile.DestinationPath}
                        }
                        else
                        {
                            //  ●[log]ファイルのダウンロードに失敗。Path: {dlFile.Destination}
                        }
                    }
                }
            }
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

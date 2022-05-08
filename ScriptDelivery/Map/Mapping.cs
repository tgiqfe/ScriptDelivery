using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptDelivery.Map.Works;
using ScriptDelivery.Map.Requires;
using YamlDotNet;
using YamlDotNet.Serialization;
using YamlDotNet.RepresentationModel;
using ScriptDelivery.Lib;
using Csv;

namespace ScriptDelivery.Map
{
    internal class Mapping
    {
        public Require Require { get; set; }

        public Work Work { get; set; }

        #region Deserialize/Serialize

        /// <summary>
        /// 指定したファイルから読み込んでデシリアライズ
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<Mapping> Deserialize(string filePath)
        {
            List<Mapping> list = null;

            if (File.Exists(filePath))
            {
                using (var sr = new StreamReader(filePath, Encoding.UTF8))
                {
                    string extention = Path.GetExtension(filePath);
                    list = extention switch
                    {
                        ".yml" => DeserializeYaml(sr),
                        ".yaml" => DeserializeYaml(sr),
                        ".csv" => DeserializeCsv(sr),
                        ".txt" => null,
                        _ => null,
                    };
                }
            }
            else if (Directory.Exists(filePath))
            {
                foreach (string childPath in Directory.GetFiles(filePath))
                {
                    list.AddRange(Deserialize(childPath));
                }
            }

            return list;
        }

        /// <summary>
        /// シリアライズして指定したファイルへ書き込み
        /// </summary>
        /// <param name="list"></param>
        /// <param name="filePath"></param>
        public static void Serialize(List<Mapping> list, string filePath)
        {
            using (var sw = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                string extention = Path.GetExtension(filePath);
                switch (extention)
                {
                    case ".yml":
                    case ".yaml":
                        SerializeYml(list, sw);
                        break;
                    case ".csv":
                        SerializeCsv(list, sw);
                        break;
                    case ".txt":
                        break;
                }
            }
        }

        #endregion
        #region Yml

        private static ISerializer _serializer = null;

        public static List<Mapping> DeserializeYaml(TextReader tr)
        {
            //  「---」区切りのyamlデータ読み込み
            var list = new List<Mapping>();
            var yaml = new YamlStream();
            yaml.Load(tr);
            foreach (YamlDocument document in yaml.Documents)
            {
                YamlNode node = document.RootNode;
                using (var stream = new MemoryStream())
                using (var writer = new StreamWriter(stream))
                using (var reader = new StreamReader(stream))
                {
                    new YamlStream(new YamlDocument[] { new YamlDocument(node) }).Save(writer);
                    writer.Flush();
                    stream.Position = 0;
                    list.Add(new Deserializer().Deserialize<Mapping>(reader));
                }
            }
            return list;
        }

        private static void SerializeYml(List<Mapping> list, TextWriter tw)
        {
            _serializer ??= new SerializerBuilder().
                WithEmissionPhaseObjectGraphVisitor(x =>
                    new YamlIEnumerableSkipEmptyObjectGraphVisitor(x.InnerVisitor)).
                    Build();
            list.ForEach(x =>
            {
                tw.WriteLine("---");
                tw.WriteLine(_serializer.Serialize(x));
            });
        }

        #endregion
        #region Csv

        private static string[] _csvHeader = null;

        public static List<Mapping> DeserializeCsv(TextReader tr)
        {
            var separator = new System.Text.RegularExpressions.Regex(@"\s(?=[^\s]+=)");

            var list = new List<Mapping>();
            foreach (var line in CsvReader.Read(tr))
            {
                var mapping = new Mapping();
                mapping.Require = new Require();
                mapping.Require.RequireMode = line["Mode"];

                mapping.Require.RequireRule = new RequireRule[1];
                mapping.Require.RequireRule[0].RuleTarget = line["Target"];
                mapping.Require.RequireRule[0].MatchType = line["Match"];
                mapping.Require.RequireRule[0].Invert = line["Invert"];

                var dictionary = new Dictionary<string, string>();
                separator.Split(line["Param"]).
                    ToList().
                    ForEach(x =>
                    {
                        string[] fields = x.Split('=');
                        dictionary[fields[0]] = fields[1];
                    });
                mapping.Require.RequireRule[0].Param = dictionary;

                mapping.Work = new Work();
                mapping.Work.Download = new Download[1];
                mapping.Work.Download[0].Path = line["Path"];
                mapping.Work.Download[0].Overwrite = line["Overwrite"];
                mapping.Work.Download[0].UserName = line["UserName"];
                mapping.Work.Download[0].Password = line["Password"];

                list.Add(mapping);
            }
            return list;
        }

        private static void SerializeCsv(List<Mapping> list, TextWriter tw)
        {
            _csvHeader ??= new string[]
            {
                "Mode",
                "Target",
                "Match",
                "Invert",
                "Param",
                "Path",
                "Overwrite",
                "UserName",
                "Password",
            };
            CsvWriter.Write(tw, _csvHeader, list.Select(x => x.ToParamArray()));
        }

        /// <summary>
        /// CSV出力用。各lineを作成
        /// RequireのRule、WorkのDownloadは、それぞれ最初の1つのみを使用。(Csvでは、2つ以上のRuleやDownloadに対応しない想定)
        /// </summary>
        /// <returns></returns>
        private string[] ToParamArray()
        {
            //  ヘッダーの数を直接指定・・・
            string[] array = new string[9];

            array[0] = this.Require.enum_RequireMode.ToString();
            if (this.Require.RequireRule?.Length > 0)
            {
                array[1] = Require.RequireRule[0].enum_RuleTarget.ToString();
                array[2] = Require.RequireRule[0].enum_MatchType.ToString();
                array[3] = Require.RequireRule[0].bool_Invert.ToString();
                var tempList = new List<string>();
                foreach (var pair in Require.RequireRule[0].Param)
                {
                    tempList.Add($"{pair.Key}={pair.Value}");
                }
                array[4] = string.Join(" ", tempList);
            }
            if (this.Work.Download?.Length > 0)
            {
                array[5] = Work.Download[0].Path ?? "";
                array[6] = Work.Download[0].enum_Overwrite.ToString();
                array[7] = Work.Download[0].UserName ?? "";
                array[8] = Work.Download[0].Password ?? "";
            }

            return array;
        }

        #endregion
    }
}

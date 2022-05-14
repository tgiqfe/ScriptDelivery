using YamlDotNet.Serialization;
using YamlDotNet.RepresentationModel;
using ScriptDelivery.Lib;
using Csv;
using System.IO;
using System.Text;
using ScriptDelivery.Maps.Works;
using ScriptDelivery.Maps.Requires;

namespace ScriptDelivery.Maps
{
    internal class MappingGenerator
    {
        #region Deserialize/Serialize

        /// <summary>
        /// 指定したファイルから読み込んでデシリアライズ
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<Mapping> Deserialize(string filePath)
        {
            List<Mapping> list = null;
            string fileName = Path.GetFileName(filePath);

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
                    //list.ForEach(x => x.FileName = fileName);
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
                mapping.Require.Mode = line["Mode"];

                mapping.Require.Rules = new RequireRule[1] { new RequireRule() };
                mapping.Require.Rules[0].Target = line["Target"];
                mapping.Require.Rules[0].Match = line["Match"];
                mapping.Require.Rules[0].Invert = line["Invert"];

                var dictionary = new Dictionary<string, string>();
                separator.Split(line["Param"]).
                    ToList().
                    ForEach(x =>
                    {
                        if (x.Contains("="))
                        {
                            string[] fields = x.Split('=');
                            dictionary[fields[0]] = fields[1];
                        }
                    });
                mapping.Require.Rules[0].Param = dictionary;

                mapping.Work = new Work();
                mapping.Work.Downloads = new Download[1] { new Download() };
                mapping.Work.Downloads[0].Source = line["Source"];
                mapping.Work.Downloads[0].Source = line["Destination"];
                mapping.Work.Downloads[0].Force = line["Force"];
                mapping.Work.Downloads[0].UserName = line["UserName"];
                mapping.Work.Downloads[0].Password = line["Password"];

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
                "Source",
                "Destination",
                "Force",
                "UserName",
                "Password",
            };

            Func<Mapping, string[]> toParamArray = (mapping) =>
            {
                return new string[]
                {
                    mapping.Require.GetRequireMode().ToString(),
                    mapping.Require.Rules[0].GetRuleTarget().ToString(),
                    mapping.Require.Rules[0].GetRuleMatch().ToString(),
                    mapping.Require.Rules[0].GetInvert().ToString(),
                    mapping.Require.Rules?.Length > 0 ?
                        string.Join(" ", mapping.Require.Rules[0].Param.Select(x => $"{x.Key}={x.Value}")) : "",
                    mapping.Work.Downloads[0].Source ?? "",
                    mapping.Work.Downloads[0].Destination ?? "",
                    mapping.Work.Downloads[0].GetForce().ToString(),
                    mapping.Work.Downloads[0].UserName ?? "",
                    mapping.Work.Downloads[0].Password ?? "",
                };
            };

            //CsvWriter.Write(tw, _csvHeader, list.Select(x => x.ToParamArray()));
            CsvWriter.Write(tw, _csvHeader, list.Select(x => toParamArray(x)));
        }

        #endregion
    }
}

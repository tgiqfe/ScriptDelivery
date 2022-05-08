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




        #region Deserialize

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
                        ".csv" => null,
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

        #endregion
        #region Serialize

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
                "User",
                "Password",
            };
            CsvWriter.Write(tw, _csvHeader, list.Select(x => x.GetParamArray()));
        }





        public void Serialize(string filePath, bool append = true)
        {
            using (var sw = new StreamWriter(filePath, append, Encoding.UTF8))
            {
                string extention = Path.GetExtension(filePath);
                switch (extention)
                {
                    case ".yml":
                    case ".yaml":
                        SerializeYml(sw);
                        break;
                    case ".csv":
                        break;
                    case ".txt":
                        break;
                }
            }
        }

        public void SerializeYml(TextWriter tw)
        {
            var serializer = new SerializerBuilder().
                  WithEmissionPhaseObjectGraphVisitor(x =>
                      new YamlIEnumerableSkipEmptyObjectGraphVisitor(x.InnerVisitor)).
                      Build();
            tw.WriteLine("---");
            tw.WriteLine(serializer.Serialize(this));
        }

        public void SerializeCsv(TextWriter tw)
        {
            var lines = new string[][]
            {
                this.GetParamArray(),
            };
            CsvWriter.Write(tw, Mapping._csvHeader, lines, ',', true);
        }

        #endregion

        private static ISerializer _serializer = null;

        private static string[] _csvHeader = null;

        private string[] GetParamArray()
        {
            //  ヘッダーの数を直接指定・・・
            string[] array = new string[8];

            array[0] = this.Require.enum_RequireMode.ToString();
            if (this.Require.RequireRule?.Length > 0)
            {
                array[1] = Require.RequireRule[0].enum_RuleTarget.ToString();
                array[2] = Require.RequireRule[0].enum_MatchType.ToString();
                array[3] = Require.RequireRule[0].Invert?.ToString();
                var sb = new StringBuilder();
                foreach (var pair in Require.RequireRule[0].Param)
                {
                    sb.Append($"{pair.Key}={pair.Value}");
                }
                array[4] = sb.ToString();
            }
            if (this.Work.Download?.Length > 0)
            {
                array[5] = Work.Download[0].Path;
                array[6] = Work.Download[0].enum_Overwrite.ToString();
                array[7] = Work.Download[0].UserName;
                array[8] = Work.Download[0].Password;
            }

            return array;
        }




    }
}

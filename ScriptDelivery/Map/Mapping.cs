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
using System.Text.Json;
using System.Text.Json.Serialization;
using ScriptDelivery.Lib;

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
                        ".json" => DeserializeJson(sr),
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

        public static List<Mapping> DeserializeJson(TextReader tr)
        {
            return JsonSerializer.Deserialize<List<Mapping>>(tr.ReadToEnd());
        }

        #endregion
        #region Serialize

        public void Serialize(string filePath, bool append = false)
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
                    case ".json":
                        SerializeJson(sw);
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

        public void SerializeJson(TextWriter tw)
        {
            var options = new JsonSerializerOptions()
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                IgnoreReadOnlyProperties = true,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
            };
            JsonSerializer.Serialize(tw, options);
        }

        #endregion
    }
}

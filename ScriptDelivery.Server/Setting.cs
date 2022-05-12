using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Runtime.InteropServices;

namespace ScriptDelivery.Server
{
    public class Setting
    {
        public string MapsPath { get; set; }
        public string FilesPath { get; set; }



        public void Init()
        {
            this.MapsPath = "store/maps";
            this.FilesPath = "store/files";
        }

        public void ChangePath()
        {
            switch (Item.Platform)
            {
                case Platform.Windows:
                    this.MapsPath = MapsPath.Replace("/", "\\");
                    this.FilesPath = FilesPath.Replace("/", "\\");
                    break;
                case Platform.Linux:
                case Platform.MacOSX:
                    this.MapsPath = MapsPath.Replace("\\", "/");
                    this.FilesPath = FilesPath.Replace("\\", "/");
                    break;
            }
        }


        #region Serialize/Deserialize

        public static Setting Deserialize(string filePath)
        {
            Setting setting = null;
            try
            {
                using (var sr = new StreamReader(filePath, Encoding.UTF8))
                {
                    setting = JsonSerializer.Deserialize<Setting>(sr.ReadToEnd(),
                        new JsonSerializerOptions()
                        {
                            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                            //IgnoreReadOnlyProperties = true,
                            //DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                            //WriteIndented = true,
                            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                        });
                }
            }
            catch { }
            if (setting == null)
            {
                setting = new Setting();
                setting.Init();
            }

            return setting;
        }

        public void Serialize(string filePath)
        {
            if (filePath.Contains(Path.DirectorySeparatorChar))
            {
                string parent = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(parent))
                {
                    Directory.CreateDirectory(parent);
                }
            }
            try
            {
                using (var sw = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    string json = JsonSerializer.Serialize(this,
                         new JsonSerializerOptions()
                         {
                             Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                             //IgnoreReadOnlyProperties = true,
                             //DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                             WriteIndented = true,
                             Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                         });
                    sw.WriteLine(json);
                }
            }
            catch { }
        }

        #endregion
    }
}

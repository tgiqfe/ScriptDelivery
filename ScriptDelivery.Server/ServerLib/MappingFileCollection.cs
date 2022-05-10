using System.IO;
using ScriptDelivery;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScriptDelivery.Server.ServerLib
{
    internal class MappingFileCollection
    {
        private List<MappingFile> _list = null;

        public string Content { get; set; }

        public MappingFileCollection() { }

        public MappingFileCollection(string mapsPath)
        {
            _list = new List<MappingFile>();
            foreach (string file in Directory.GetFiles(mapsPath))
            {
                _list.Add(new MappingFile(file));
            }

            var list = _list.SelectMany(x => x.MappingList.Select(y => y)).ToList();
            this.Content = JsonSerializer.Serialize(list);
        }

        public void ContentToList(string content)
        {
            _list = JsonSerializer.Deserialize<List<MappingFile>>(content);
        }
    }
}

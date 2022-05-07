using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptDelivery.Map
{
    internal class ValuesAttribute : Attribute
    {
        private string _keyword { get; set; }

        public ValuesAttribute(string keyword)
        {
            this._keyword = keyword;
        }

        public SubCandidate[] GetCandidate()
        {
            _map ??= LoadMap();
            return _map[_keyword];     //  例外対策は無し。もし例外が発生した場合はコード見直し
        }

        #region Value candidate map

        private static Dictionary<string, SubCandidate[]> _map = null;

        private Dictionary<string, SubCandidate[]> LoadMap()
        {
            return new Dictionary<string, SubCandidate[]>()
            {
                {
                    "Mode", new SubCandidate[]{
                        new SubCandidate("none", "none", "no"),
                        new SubCandidate("and", "and", "all") ,
                        new SubCandidate("or", "or", "any") }
                },
                {
                    "Match", new SubCandidate[]{
                        new SubCandidate("none", "none", "no"),
                        new SubCandidate("equal", "equal", "equals"),
                        new SubCandidate("range", "range", "within", "numberrange"),
                        new SubCandidate("namerange", "namerange"),
                        new SubCandidate("innetwork", "innetwork", "insame"),
                        new SubCandidate("file", "file"),
                        new SubCandidate("directory", "directory", "folder"),
                        new SubCandidate("registry", "registry", "reg") }
                },
            };
        }

        #endregion
    }
}

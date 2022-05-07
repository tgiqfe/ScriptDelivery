using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptDelivery.Lib;
using System.IO;

namespace ScriptDelivery.Map.Requires.Matcher
{
    internal class ExistsMatcher : MatcherBase
    {
        [MatcherParameter, Keys("Path", "RegistryKey")]
        public string Path { get; set; }

        [MatcherParameter, Keys("Name", "RegistryName")]
        public string Name { get; set; }

        public bool IsMatch(MatchType matchType)
        {
            return matchType switch
            {
                MatchType.File => FileMatch(),
                MatchType.Directory => DirectoryMatch(),
                MatchType.Registry => RegistryMatch(),
                _ => false,
            };
        }

        #region Match methods

        /// <summary>
        /// ファイルの有無チェック
        /// </summary>
        /// <returns></returns>
        private bool FileMatch()
        {
            if (this.Path.Contains("*"))
            {
                return SearchWildFile(this.Path, false);
            }
            return File.Exists(this.Path);
        }

        /// <summary>
        /// ディレクトリの有無チェック
        /// </summary>
        /// <returns></returns>
        private bool DirectoryMatch()
        {
            if (this.Path.Contains("*"))
            {
                return SearchWildFile(this.Path, isDirectory: true);
            }
            return Directory.Exists(this.Path);
        }

        /// <summary>
        /// レジストリキー or レジストリ値の有無チェック
        /// </summary>
        /// <returns></returns>
        private bool RegistryMatch()
        {


            return false;
        }

        #endregion

        /// <summary>
        /// ワイルドカードを含むパスからファイル/ディレクトリの有無チェック
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isDirectory"></param>
        /// <returns></returns>
        public bool SearchWildFile(string path, bool isDirectory)
        {
            string parentPath = System.IO.Path.GetDirectoryName(path.TrimEnd('\\'));
            var pattern = path.TrimEnd('\\').GetWildcardPattern();
            var list = new List<string>();

            Action<string> recurse = null;
            recurse = (dirPath) =>
            {
                if (!isDirectory)
                {
                    foreach (string subFilePath in Directory.GetFiles(dirPath))
                    {
                        if (pattern.IsMatch(subFilePath))
                        {
                            //  デバッグ用
                            //Console.WriteLine("[file]" + subFilePath);
                            list.Add(subFilePath);
                        }
                    }
                }
                foreach (string subDirPath in Directory.GetDirectories(dirPath))
                {
                    if (isDirectory)
                    {
                        if (pattern.IsMatch(subDirPath))
                        {
                            //  デバッグ用
                            //Console.WriteLine("[directory]" + subDirPath);
                            list.Add(subDirPath);
                        }
                    }
                    if (dirPath != parentPath)
                    {
                        recurse(subDirPath);
                    }
                }
            };

            string wildParent = System.IO.Path.GetDirectoryName(path.Substring(0, path.IndexOf("*")));
            recurse(wildParent);

            return list.Count > 0;
        }
    }
}

using ScriptDelivery.Lib;
using System.IO;
using System.Text.RegularExpressions;


//  C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe
string wildPath1 = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe";
string wildPath2 = @"C:\Windows\Microsoft.NET\Framework64\v4*\InstallUtil.exe";
string wildPath3 = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\install*.exe";
string wildPath4 = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\*";
string wildPath5 = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\*.exe";
string wildPath6 = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.*";
string wildPath7 = @"C:\Windows\Microsoft.NET\Framework*\v4*\InstallUtil.exe";
string wildPath8 = @"C:\Windows\Microsoft.NET\*\v4.0.30319\InstallUtil.exe";
string wildPath9 = @"C:\Windows\Microsoft.NET\*\v4.0.30319";
string wildPath10 = @"C:\Windows\Microsoft.NET\*\v4*";
string wildPath11 = @"C:\Windows\Microsoft.NET\*\";
string wildPath12 = @"C:\Windows\Microsoft.NET\Framework*";


bool ret = localCheck(wildPath11, true);

Console.WriteLine(ret);



bool localCheck(string path, bool isDirectory)
{
    if (path.Contains("*"))
    {
        string parentPath = Path.GetDirectoryName(path.TrimEnd('\\'));
        Regex pattern = path.TrimEnd('\\').GetWildcardPattern();

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
                        Console.WriteLine("[file]" + subFilePath);
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
                        Console.WriteLine("[directory]" + subDirPath);
                        list.Add(subDirPath);
                    }
                }
                if(dirPath != parentPath)
                {
                    recurse(subDirPath);
                }
            }
        };

        string wildParent = Path.GetDirectoryName(path.Substring(0, path.IndexOf("*")));
        recurse(wildParent);

        return list.Count > 0;
    }
    return File.Exists(path);
}



Console.ReadLine();


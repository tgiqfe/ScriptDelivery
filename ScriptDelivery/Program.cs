using ScriptDelivery.Lib;
using System.IO;
using System.Text.RegularExpressions;
using ScriptDelivery.Requires;
using ScriptDelivery.Works;
using ScriptDelivery;
using ScriptDelivery.Net;

bool debug = false;
if (debug)
{
    ScriptDelivery.Test.SettingFile.Create01();

    Console.ReadLine();
    Environment.Exit(0);
}


using (var session = new ClientSession("http://localhost:5000"))
{
    session.DownloadMappingFile().Wait();
    session.MapMathcingCheck();
    session.DownloadSmbFile();
    session.DownloadHttpSearch().Wait();
    session.DownloadHttpStart().Wait();
}




Console.ReadLine();


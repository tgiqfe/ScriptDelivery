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


var session = new ClientSession();
session.DownloadMappingFile("http://localhost:5000").Wait();
session.MapMathcingCheck();
session.DownloadSmbFile();
session.DownloadHttpSearch("http://localhost:5000").Wait();
session.DownloadHttpStart("http://locahost:5000").Wait();





Console.ReadLine();


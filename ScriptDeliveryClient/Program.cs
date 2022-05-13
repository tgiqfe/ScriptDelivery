using ScriptDeliveryClient.Lib;
using System.IO;
using System.Text.RegularExpressions;
using ScriptDelivery.Maps.Requires;
using ScriptDelivery.Maps.Works;
using ScriptDelivery;

bool debug = false;
if (debug)
{
    //ScriptDelivery.Test.SettingFile.Create01();

    Console.ReadLine();
    Environment.Exit(0);
}


ProcessLogger logger = new ProcessLogger("sd.log");

using (var session = new ClientSession("http://localhost:5160", logger))
{
    session.DownloadMappingFile().Wait();
    session.MapMathcingCheck();
    session.DownloadSmbFile();
    session.DownloadHttpSearch().Wait();
    session.DownloadHttpStart().Wait();
}


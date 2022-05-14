using ScriptDeliveryClient.Lib;
using System.IO;
using System.Text.RegularExpressions;
using ScriptDelivery.Maps.Requires;
using ScriptDelivery.Maps.Works;
using ScriptDelivery;

bool debug = false;
if (debug)
{
    Console.ReadLine();
    Environment.Exit(0);
}


ProcessLogger logger = new ProcessLogger("sd.log");

using (var session = new ClientSession("http://localhost:5000", logger))
{
    session.DownloadMappingFile().Wait();
    session.MapMathcingCheck();
    session.DownloadSmbFile();
    session.DownloadHttpSearch().Wait();
    session.DownloadHttpStart().Wait();
}


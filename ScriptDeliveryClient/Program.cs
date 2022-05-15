using ScriptDeliveryClient.Lib;
using System.IO;
using System.Text.RegularExpressions;
using ScriptDelivery.Maps.Requires;
using ScriptDelivery.Maps.Works;
using ScriptDeliveryClient;
using ScriptDeliveryClient.Logs.ProcessLog;

bool debug = false;
if (debug)
{
    Console.ReadLine();
    Environment.Exit(0);
}


ProcessLogger logger = new ProcessLogger("sd.log");
var setting = new Setting();

//  ScriptDeliveryサーバからスクリプトをダウンロード
var sdc = new ClientSession(setting, logger);
sdc.StartDownload();

Console.ReadLine();

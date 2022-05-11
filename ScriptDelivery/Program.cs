using ScriptDelivery.Lib;
using System.IO;
using System.Text.RegularExpressions;
using ScriptDelivery.Requires;
using ScriptDelivery.Works;
using ScriptDelivery;
using ScriptDelivery.Net;

bool debug = true;
if (debug)
{
    ScriptDelivery.Test.SettingFile.Create01();

    Console.ReadLine();
    Environment.Exit(0);
}


//var client = new ScriptDeliveryClient();
//client.MappingRequest("http://localhost:5000").Wait();




Console.ReadLine();


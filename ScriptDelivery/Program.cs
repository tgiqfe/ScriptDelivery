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
    Console.ReadLine();
    Environment.Exit(0);
}


var client = new ScriptDeliveryClient();
client.MappingRequest("http://localhost:5000/map").Wait();




Console.ReadLine();


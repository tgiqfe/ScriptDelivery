using ScriptDelivery.Lib;
using System.IO;
using System.Text.RegularExpressions;
using ScriptDelivery.Map;
using ScriptDelivery.Map.Requires;
using ScriptDelivery.Map.Works;

bool init = true;
if (init)
{
    Mapping mapping = new Mapping()
    {
        Require = new Require()
        {
            RequireMode = "all",
            RequireRule = new RequireRule[]
            {
                new RequireRule()
                {
                    RuleTarget = "HostName",
                    MatchType = "Equal",
                    Param = new Dictionary<string, string>()
                    {
                        { "Name", "AAAA01" },
                    },
                },
                new RequireRule()
                {
                    RuleTarget = "IPAddress",
                    MatchType = "Equal",
                    Param = new Dictionary<string, string>()
                    {
                        {"IPAddress", "192.168.10.100" },
                    },
                },
            }
        },
        Work = new Work()
        {
            Download = new Download[]
            {
                new Download()
                {
                    Path = @"\\192.168.20.101\share1\Sample001.txt",
                    Overwrite = "force",
                },
            },
        }
    };

    List<Mapping> list = new List<Mapping>()
    {
        mapping
    };

    Mapping.Serialize(list, "sample.csv");




    Console.ReadLine();
    Environment.Exit(0);
}







Console.ReadLine();


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptDelivery.Lib;
using ScriptDelivery.Requires;
using ScriptDelivery.Works;

namespace ScriptDelivery.Test
{
    internal class SettingFile
    {
        public static void Create01()
        {
            Mapping mapping = new Mapping()
            {
                Require = new Require()
                {
                    RequireMode = "all",
                    RequireRules = new RequireRule[]
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
                    Downloads = new Download[]
                    {
                        new Download()
                        {
                            SourcePath = @"Sample001.txt",
                            DestinationPath = @"C:\App\Sample\Sample001.txt",
                            Force = "true",
                        },
                        new Download()
                        {
                            SourcePath = @"Sample002.txt",
                            DestinationPath = @"C:\App\Sample\Sample002.txt",
                            Force = "true",
                        },
                    },
                }
            };

            List<Mapping> list = new List<Mapping>()
            {
                mapping
            };

            Mapping.Serialize(list, "sample03.yml");
            Mapping.Serialize(list, "sample03.csv");

        }
    }
}

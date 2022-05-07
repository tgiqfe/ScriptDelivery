using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Management;
using System.Net;

namespace ScriptDelivery.Lib
{
    internal class NetworkInfo
    {
        #region Private class

        public class NIC
        {
            public string Name { get; set; }
            public string GUID { get; set; }
            public IEnumerable<AddressSet> AddressSets { get; set; }

            private string[] _ipAddresses = null;
            public string[] GetIPAddresses()
            {
                _ipAddresses ??= this.AddressSets.Select(x => x.IPAddress.ToString()).ToArray();
                return _ipAddresses;
            }
        }

        public class AddressSet
        {
            public IPAddress IPAddress { get; set; }
            public IPAddress SunbnetMask { get; set; }
            public bool IPv4 { get; set; }
            public int PrefixLength { get; set; }
        }

        #endregion

        public List<NIC> NICs { get; set; }

        public NetworkInfo()
        {
            this.NICs = new List<NIC>();

            var adapters = new ManagementClass("Win32_NetworkAdapter").
                GetInstances().
                OfType<ManagementObject>().
                Where(x => x["NetConnectionID"] != null);
            
            var nis = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var adapter in adapters)
            {
                string guid = adapter["GUID"] as string;
                var ni = nis.FirstOrDefault(x => x.Id == guid);

                var addressSets = ni == null ? null :
                    ni.GetIPProperties().UnicastAddresses.
                        Select(x => new AddressSet()
                        {
                            IPAddress = x.Address,
                            SunbnetMask = x.IPv4Mask,
                            IPv4 = x.Address.AddressFamily == AddressFamily.InterNetwork,
                            PrefixLength = x.PrefixLength,
                        });
                NICs.Add(new NIC()
                {
                    Name = adapter["NetConnectionID"] as string,
                    GUID = guid,
                    AddressSets = addressSets
                }) ;
            }
        }
    }
}

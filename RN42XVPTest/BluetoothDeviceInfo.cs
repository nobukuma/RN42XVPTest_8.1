using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrawhatNet.Study.RN42XVPTest
{
    public class BluetoothDeviceInfo
    {
        public bool IsConnected { get; set; }
        public string DisplayName { get; set; }
        public string ServiceName { get; set; }
        public Windows.Networking.HostName HostName { get; set; }

        public string HostNameString
        {
            get
            {
                return (this.HostName != null) ? this.HostName.CanonicalName : String.Empty;
            }
        }
    }
}

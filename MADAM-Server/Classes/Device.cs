using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MADAM_Server.Classes
{   [Serializable, XmlRoot("Device")]
    public class Device
    {
        public string name { get; set; }
        public string ipAddr { get; set; }
        public string macAddr { get; set; }
        public string hostName { get; set; }
        public string osVersion { get; set; }
        public string Manufacturer { get; set; }
        public bool isAd { get; set; }
        public List<Users> UserList { get; set; }
    }
}

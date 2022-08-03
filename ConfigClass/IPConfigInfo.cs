using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GradeMonitorApplication.ConfigClass
{
    public class IPConfigInfo
    {
        private string cameraMZ;
        string cameraPZ;
        string scanerHX;
        string scanerZX;
        string rFIDReader;

        Dictionary<string, string> otherValues;
        public IPConfigInfo()
        {
            OtherValues = new Dictionary<string, string>();
            StreamReader reader = new StreamReader("../../ConfigClass/IPSetting.txt");
            CameraMZ = reader.ReadLine().Split(':')[1];
            CameraPZ = reader.ReadLine().Split(':')[1];
            ScanerHX = reader.ReadLine().Split(':')[1];
            ScanerZX = reader.ReadLine().Split(':')[1];
            RFIDReader = reader.ReadLine().Split(':')[1];
            while(!reader.EndOfStream)
            {
                string PR = reader.ReadLine();
                OtherValues.Add(PR.Split(':')[0], PR.Split(':')[1]);
            }

            reader.Close();
        }
        public bool UpdateIP(string mz,string pz,string hx,string zx,string reader)
        {
            StreamWriter writer = new StreamWriter("../../ConfigClass/IPSetting.txt");
            writer.WriteLine("CameraMZ:" + mz);
            writer.WriteLine("CameraPZ:" + pz);
            writer.WriteLine("ScanerHX:" + hx);
            writer.WriteLine("ScanerZX:" + zx);
            writer.WriteLine("RFIDReader:" + reader);
            
            writer.Flush();
            writer.Close();
            return (true);
        }
        public IPAddress ToIPAddress(string ip)
        {
            List<byte> b = new List<byte>();
            foreach(string item in ip.Split('.'))
            {
                int num = Convert.ToInt32(item);
                b.Add(Convert.ToByte(num));
            }
            return (new IPAddress(b.ToArray()));
        }
        public string CameraMZ { get => cameraMZ; set => cameraMZ = value; }
        public string CameraPZ { get => cameraPZ; set => cameraPZ = value; }
        public string ScanerHX { get => scanerHX; set => scanerHX = value; }
        public string ScanerZX { get => scanerZX; set => scanerZX = value; }
        public string RFIDReader { get => rFIDReader; set => rFIDReader = value; }
        public Dictionary<string, string> OtherValues { get => otherValues; set => otherValues = value; }

    }
}

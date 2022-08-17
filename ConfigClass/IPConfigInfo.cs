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
        public string cameraMZ;
        public string cameraPZ;
        public string scanerHX;
        public string scanerZX;
        public string rFIDReader;
        public Dictionary<string, string> otherValues;
        public IPConfigInfo()
        {
            otherValues = new Dictionary<string, string>();
            StreamReader reader = new StreamReader("IPSetting.txt");
            cameraMZ = reader.ReadLine().Split(':')[1];
            cameraPZ = reader.ReadLine().Split(':')[1];
            scanerHX = reader.ReadLine().Split(':')[1];
            scanerZX = reader.ReadLine().Split(':')[1];
            rFIDReader = reader.ReadLine().Split(':')[1];
            while(!reader.EndOfStream)
            {
                string PR = reader.ReadLine();
                otherValues.Add(PR.Split(':')[0], PR.Split(':')[1]);
            }

            reader.Close();
        }
        public bool UpdateIP(string mz,string pz,string hx,string zx,string reader)
        {
            StreamWriter writer = new StreamWriter("IPSetting.txt");
            writer.WriteLine("CameraMZ:" + mz);
            writer.WriteLine("CameraPZ:" + pz);
            writer.WriteLine("ScanerHX:" + hx);
            writer.WriteLine("ScanerZX:" + zx);
            writer.WriteLine("RFIDReader:" + reader);
            foreach (KeyValuePair<string, string> item in otherValues)
                writer.WriteLine(item.Key + ":" + item.Value);
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
      // public string CameraMZ { get => cameraMZ; set => cameraMZ = value; }
      // public string CameraPZ { get => cameraPZ; set => cameraPZ = value; }
      // public string ScanerHX { get => scanerHX; set => scanerHX = value; }
      // public string ScanerZX { get => scanerZX; set => scanerZX = value; }
      // public string RFIDReader { get => rFIDReader; set => rFIDReader = value; }
      // public Dictionary<string, string> OtherValues { get => otherValues; set => otherValues = value; }

    }
}

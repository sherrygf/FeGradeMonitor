using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace GradeMonitorApplication.FunctionClass
{
    class SoftReg
    {
        //主要思路：获取机器的CPU序列号，得到机器码。通过对机器码进行MD5运算之后得到注册码。将注册码写进注册表或系统目录。
        //1.在程序的入口处（winform）检测注册文件是否存在，如果不存在，则提示用户注册文件损坏或者用户未注册，
        //如果注册文件存在，则读取文件内容，判断注册码是否正确。在程序的入口函数所在的文件要引用 using System.IO;来进行文件操作。
        public int[] intCode = new int[127]; //存储密钥
        public char[] charCode = new char[25]; //存储ASCII码
        public int[] intNumber = new int[25]; //存储ASCII码值

        ///<summary>
        /// 获取硬盘卷标号
        ///</summary>
        ///<returns></returns>
        public string GetDiskVolumeSerialNumber()
        {
            ManagementClass mc = new ManagementClass("win32_NetworkAdapterConfiguration");//ManagementClass类和获取硬件的信息
            ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"c:\"");//
            disk.Get();
            return disk.GetPropertyValue("VolumeSerialNumber").ToString();
        }

        ///<summary>
        /// 获取CPU序列号
        ///</summary>
        ///<returns></returns>
        public string GetCpu()
        {
            string strCpu = null;
            ManagementClass myCpu = new ManagementClass("win32_Processor");
            ManagementObjectCollection myCpuCollection = myCpu.GetInstances();
            foreach (ManagementObject myObject in myCpuCollection)
            {
                strCpu = myObject.Properties["Processorid"].Value.ToString();
            }

            return strCpu;
        }

        ///<summary>
        /// 生成机器码
        ///</summary>
        ///<returns></returns>
        public string GetMNum()
        {
            string strNum = GetCpu() + GetDiskVolumeSerialNumber();
            string strMNum = strNum.Substring(0, 24); //截取前24位作为机器码
            return strMNum;
            //string strMNum = "";
            //string[] strid = new string[24];//
            //for (int i = 0; i < 24; i++)//把字符赋给数组
            //{
            //    strid[i] = strNum.Substring(i, 1);
            //}

            //Random rdid = new Random();
            //for (int i = 0; i < 24; i++)//从数组随机抽取24个字符组成新的字符生成机器码
            //{
            //    strMNum += strid[rdid.Next(0, 24)];
            //}

            //return strMNum;
        }

        ///<summary>
        /// 生成注册码
        ///</summary>
        ///<returns></returns>
        public string GetRNum()
        {
            SetIntCode();
            string strMNum = GetMNum();
            for (int i = 1; i < charCode.Length; i++) //存储机器码
            {
                charCode[i] = Convert.ToChar(strMNum.Substring(i - 1, 1));
            }
            for (int j = 1; j < intNumber.Length; j++) //改变ASCII码值
            {
                intNumber[j] = Convert.ToInt32(charCode[j]) + intCode[Convert.ToInt32(charCode[j])];
            }
            string strAsciiName = ""; //注册码
            for (int k = 1; k < intNumber.Length; k++) //生成注册码
            {

                if ((intNumber[k] >= 48 && intNumber[k] <= 57) || (intNumber[k] >= 65 && intNumber[k]
                <= 90) || (intNumber[k] >= 97 && intNumber[k] <= 122)) //判断如果在0-9、A-Z、a-z之间
                {
                    strAsciiName += Convert.ToChar(intNumber[k]).ToString();
                }
                else if (intNumber[k] > 122) //判断如果大于z
                {
                    strAsciiName += Convert.ToChar(intNumber[k] - 10).ToString();
                }
                else
                {
                    strAsciiName += Convert.ToChar(intNumber[k] - 9).ToString();
                }
            }
            return strAsciiName;
        }

        /// <summary>
        /// 初始化密钥
        /// </summary>
        public void SetIntCode()
        {
            for (int i = 1; i < intCode.Length; i++)
            {
                intCode[i] = i % 9;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeMonitorApplication.ConfigClass
{
    public class DataBase_ConfigInfo
    {
        //数据库配置
        public static string DB_DataSource = "localhost";
        public static string DB_InitialCatalog = "DB_MengKuGradeMonitor";
        public static string DB_UserID = "sa";//本地数据库使用windows验证，是不需要账号和秘密。
        public static string DB_Password = "123456";//远程连接的话，安装数据库时选择sql sever验证，设置账号和秘密。

        //摄像头配置
        public static string CAM_IP_S = "10.1.8.252";//摄像头IP
        public static string CAM_PORT_S = "8091";
        public static string GDH_COM_S = "COM4";
        public static string CAM_IP_N = "10.1.8.251";//摄像头IP
        public static string CAM_PORT_N = "8000";
        public static string GDH_COM_N = "COM5";

        /// <summary>
        /// 扫描仪配置
        /// </summary>
        public class ConfigInfo_Auto
        {
            public static int LMS_Frequency = 25;  //频率
            public static double LMS_AngleResolution = 0.25f; //角度分辨率
            public static int LMS_H_StartAngle = 65;
            public static int LMS_H_StopAngle = 110;
            public static int LMS_Z_StartAngle = 30;
            public static int LMS_Z_StopAngle = 150;
            public static string LMS_H_IP = "10.1.8.88";//横
            public static string LMS_Z_IP = "10.1.8.111";//纵
            public static string JDQ_IP = "10.1.8.110";
            public static string LMS_H_PORT = "2111";
            public static string LMS_Z_PORT = "2111";
            public static string JDQ_PORT = "6000";
            public static string GDH_COM = "COM4";
            public static string CLCGQ_COM = "COM7";

            //数据库地址
            public static string DB_DataSource = "WIN7-20140902QY\\SQLEXPRESS";// WIN7-20140902QY\\SQLEXPRESS  01VFD2LQAYPVHSD
            public static string DB_InitialCatalog = "DYTKDBNEW";
            public static string DB_UserID = "sa";
            public static string DB_Password = "123456";
        }

    }
}

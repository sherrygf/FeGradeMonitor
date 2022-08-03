using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using ReaderB;

namespace GradeMonitorApplication.FunctionClass
{
    /// <summary>
    /// 控制并处理RFID的数据和监视进程(2022/8/1修改 基于RFID信号的测量控制方案)
    /// </summary>
    public class RFIDModule
    {
        #region 变量
        /// <summary>
        /// 监测到车头RIFD进入读卡范围
        /// </summary>
        public bool TrainIsCome;
        /// <summary>
        /// 外部控制Monitor监视器
        /// </summary>
        //public ManualResetEvent WatchingTrain;
        /// <summary>
        /// Monitor监视器线程
        /// </summary>
        public static Thread monitor;
        /// <summary>
        /// 列车名称
        /// </summary>
        public string TrainName;
        /// <summary>
        /// 读写器地址
        /// </summary>
        byte fComAdr;
        /// <summary>
        /// 读取器连接句柄
        /// </summary>
        int FrmHandle;
        /// <summary>
        /// 重试计数
        /// </summary>
        int RetryCount;
        /// <summary>
        /// 计时器
        /// </summary>
        System.Timers.Timer tm;

        bool monitorAbort = false;
        #endregion

        #region 构造函数
        public RFIDModule()
        {
            TrainIsCome = false;
            //WatchingTrain.Set();
            fComAdr = Convert.ToByte("FF", 16);
            FrmHandle = 0;
            TrainName = "H000";
            RetryCount = 0;
            monitor = new Thread(Connect);
            monitor.IsBackground = true;
            monitorAbort = false;
        }
        #endregion

        #region 连接设备
        void Connect()
        {
            int openResult = 0;
            openResult = StaticClassReaderB.OpenNetPort(6000, "192.168.2.83", ref fComAdr, ref FrmHandle);
            if (openResult == 0)
            {
                RetryCount = 0;
                try
                {
                    byte[] Parameter = new byte[6];
                    Parameter[0] = Convert.ToByte(1);
                    Parameter[1] = Convert.ToByte(0 * 1 + 1 * 2 + 1 * 4 + 0 * 8 + 0 * 16);
                    Parameter[2] = Convert.ToByte(1);
                    Parameter[3] = Convert.ToByte("02", 16);
                    Parameter[4] = Convert.ToByte(6);
                    Parameter[5] = Convert.ToByte(0);
                    int fCmdRet = StaticClassReaderB.SetWorkMode(ref fComAdr, Parameter, FrmHandle);
                    if (fCmdRet == 0)
                    {
                        //Console.WriteLine("connected:" + DateTime.Now.ToString());
                        while (!TrainIsCome)
                            if (monitorAbort)
                                return;
                            else
                                Reader();
                    }
                    else
                        Connect();
                }
                catch (Exception ex)
                {

                }
                //return (true);

            }

            else
            {
                if (RetryCount == 15)
                {
                    Function.WriteErrorLog("RFID 连接错误,Error code:" + openResult);
                    //MessageBox.Show(DateTime.Now.ToString()+ "\n30次连接超时,请检查网络环境,或转为手动测量\nError code:" + openResult);

                }
                if (openResult == 53)//0x35 comportopened
                    StaticClassReaderB.CloseNetPort(FrmHandle);
                Thread.Sleep(100);
                RetryCount++;
                //Thread.Sleep(100);
                Connect();

            }

        }
        #endregion

        #region 判断触发
        /// <summary>
        /// 启动监视进程,实际执行入口
        /// </summary>
        public void StartMonitor()
        {
            try
            {
                //复位
                TrainIsCome = false;
                TrainName = "H000";
                RetryCount = 0;
                //WatchingTrain.Set();
                //按固定时间间隔重启网络端口连接
                Monitor(new object(), new EventArgs());
                tm = new System.Timers.Timer();
                tm.Interval = 120000;
                tm.Elapsed += Monitor;
                tm.Start();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "!!!");
                throw (new Exception("RFID连接中断"));
            }
        }

        /// <summary>
        /// 结束监视进程
        /// </summary>
        public void StopMonitor()
        {
            if (monitor.IsAlive)
            {
                //WatchingTrain.Reset();
                monitor.Abort();
                monitorAbort = true;
            }
            //复位
            tm.Stop();
            Thread.Sleep(100);
            monitorAbort = false;
            RetryCount = 0;
            TrainIsCome = false;
            //monitor = null;
        }

        void Monitor(object sender, EventArgs e)
        {

            {
                if (monitor.IsAlive)
                {
                    monitor.Abort();
                    monitorAbort = true;
                }
                //monitor = null;
                TrainName = "H000";
                Thread.Sleep(100);
                RetryCount = 0;
                monitorAbort = false;
                monitor = new Thread(Connect);
                monitor.Name = "RFID监视线程" + DateTime.Now.ToString();
                monitor.IsBackground = true;
                monitor.Start();
            }

        }
        /// <summary>
        /// 读取数据
        /// </summary>
        void Reader()
        {
            int fCmdRet = 0;//返回标志
            byte[] ScanModeData = new byte[40960];
            int ValidDatalength = 0, i;
            string temp, temps;
            fCmdRet = StaticClassReaderB.ReadActiveModeData(ScanModeData, ref ValidDatalength, FrmHandle);
            if (fCmdRet == 0)
            {
                temp = "";
                temps = ByteArrayToHexString(ScanModeData);
                for (i = 0; i < ValidDatalength; i++)
                {
                    temp = temp + temps.Substring(i * 2, 2) + " ";
                }
                // Console.WriteLine(temp);

                if (temp.Substring(0, 12) == "11 00 EE 00 ")
                {
                    TrainIsCome = true;
                    if (temp.Length >= 54)
                    //temp = temp.Substring(0, 54);
                    {
                        temp = temp.Substring(12, 36);

                    }
                    MainForm.database.GetTrainNameFromRFID(temp, ref TrainName);
                    // if (temp == "11 00 EE 00 E2 80 68 94 00 00 50 1D 5D 21 61 12 0A EC " || temp == "11 00 EE 00 E2 80 68 94 00 00 40 1D 5D 21 61 10 A8 8D ")
                    //     TrainName = "H002";
                    // else if (temp == "11 00 EE 00 E2 80 68 94 00 00 50 1D 5D 21 61 0F 6E 27 " || temp == "11 00 EE 00 E2 80 68 94 00 00 40 1D 5D 21 5D D6 90 31 ")
                    //     TrainName = "H003";
                    // else if (temp == "11 00 EE 00 E2 80 68 94 00 00 50 1D 5D 21 61 0C F5 15 " || temp == "11 00 EE 00 E2 80 68 94 00 00 40 1D 5D 21 5D D9 67 C9 ")
                    //     TrainName = "H005";
                }
                else
                    Function.WriteErrorLog("RFID卡号错误:" + temp);

            }

        }

        private string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            return sb.ToString().ToUpper();

        }
        #endregion
    }

    /// <summary>
    /// 处理车轮传感器的输入(2022/1/3修改 作为实际测量触发机制)
    /// </summary>
    public class CLCGQModule
    {
        /// <summary>
        /// 车轮传感器串口对象
        /// </summary>
        SerialPort Clcgq;

        /// <summary>
        /// 串口接收缓存区
        /// </summary>
        List<byte> ReaderBuffer;

        /// <summary>
        /// 列车分节时间,可用于读取过轮时间与数量
        /// </summary>
        public List<DateTime> times;

        /// <summary>
        /// 上一时刻状态
        /// </summary>
        string last="";

        /// <summary>
        /// 计时器
        /// </summary>
        DateTime timer;

        /// <summary>
        /// 车来
        /// </summary>
        public bool TrainIsCome;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CLCGQModule()
        {
            Clcgq = new SerialPort();
            ReaderBuffer = new List<byte>();
            times = new List<DateTime>();
            last = "00";
            //注册数据接收委托
            Clcgq.DataReceived += new SerialDataReceivedEventHandler(Datareceived);
            timer = new DateTime(1970, 1, 1, 0, 0, 0);//初始化为1970年1月1日0时整
            TrainIsCome = false;
        }

        /// <summary>
        /// 连接设备
        /// </summary>
        /// <returns>连接是否成功</returns>
        public bool Connect()
        {
            Clcgq.PortName = ConfigInfo_Auto.CLCGQ_COM;
            Clcgq.BaudRate = int.Parse("9600");
            Clcgq.NewLine = "\r\n";
            try
            {
                Clcgq.Open();
                if (Clcgq.IsOpen)
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("车轮传感器连接失败！");
                    return false;
                }

            }
            catch (Exception ex)
            {
                Clcgq = new SerialPort();
                MessageBox.Show("车轮传感器连接失败！");
                Function.WriteErrorLog("车轮传感器连接失败：" + ex.Message);
                return false;
            }
        }

        void Datareceived(object sender,SerialDataReceivedEventArgs e)
        {
            try
            {
                int length = Clcgq.BytesToRead;
                byte[] buffer = new byte[length];
                Clcgq.Read(buffer, 0, length);
                ReaderBuffer.AddRange(buffer);
                TimeSpan TS;

                //数据解析部分,需要具体设备具体实现
                while(ReaderBuffer.Count>=8)
                {
                    if (ReaderBuffer[0] == 0x40 && ReaderBuffer[1] == 0x01)
                    {
                        if (ReaderBuffer.Count >= 8)
                        {
                            byte[] binary_data_1 = ReaderBuffer.GetRange(2, 1).ToArray();
                            string next = binary_data_1[0].ToString("X2");

                            if (last == "00" && next == "01")//00到01表示被触发
                            {
                                TS = DateTime.Now - timer;
                                //若距离上次触发超过五分钟，则触发开始扫描，并计数，更新计时器
                                if(TS.TotalSeconds>300)
                                {
                                    timer = DateTime.Now;
                                    TrainIsCome = true;

                                }
                                //若在五分钟内连续触发，则增加车轮计数


                                times.Add(DateTime.Now);
                            }
                            last = next;
                            ReaderBuffer.RemoveRange(0, 8);
                        }
                        else
                            break;
                    }
                    else
                        ReaderBuffer.RemoveAt(0);
                }

            }
            catch(Exception ex)
            {
                Function.WriteErrorLog("车轮传感器数据接收出错:" + ex.Message);

            }
        }
         
        
    }

    /// <summary>
    /// 读取地磅测量结果(2022/2/9 注意:此类为基于局域网内TCP协议直接传输的实现,若后续确定改用数据库读写获取的方式应及时重写此类 )
    /// </summary>
    public class GDHModule
    {
        /// <summary>
        /// 地磅数据接收程序，端口号14266
        /// </summary>
        EndPoint local = new IPEndPoint(IPAddress.Any, 14266);
        /// <summary>
        /// 地磅数据接收程序，服务器端套接字
        /// </summary>
        Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        /// <summary>
        /// 车厢皮重数据缓存链表
        /// </summary>
        public List<List<double>> CarWeights;
        /// <summary>
        /// 整车皮重缓存链表
        /// </summary>
        public List<double> TrainWeight;

        Thread Listener; 

        /// <summary>
        /// 构造函数：初始化与开始监听回传地磅数据
        /// </summary>
        public GDHModule()
        {
            CarWeights = new List<List<double>>();
            server.Bind(local);
            server.Listen(100);
            Listener = new Thread(RecordsReceiver);
            Listener.IsBackground = true;
            Listener.Start();
        }

        /// <summary>
        /// 地磅机回传数据接收线程
        /// </summary>
        void RecordsReceiver()
        {
            byte[] bytbuffer = new byte[2048];
            List<byte> listbuffer = new List<byte>();
            while(true)
            {
                //等待回传
                Socket accept = server.Accept();
                accept.Receive(bytbuffer);
                listbuffer.AddRange(bytbuffer);
                if(listbuffer[0]==0x02)
                {
                    if (listbuffer.Exists(param => param.Equals(0x03)))
                    {
                        int tail = listbuffer.IndexOf(0x03);
                        byte[] data = listbuffer.GetRange(1, tail - 1).ToArray();
                        //本次数据解析，并写入一个double的链表
                        List<double> weights = new List<double>();
                        string strdata = Encoding.ASCII.GetString(data);
                        string[] strweights = strdata.Split(' ');
                        for(int i=0;i<strweights.Length;i++)
                        {
                            double weight = Convert.ToDouble(strweights[i]);
                            weights.Add(weight);
                        }
                        //将本次产生的double链表写入缓存
                        CarWeights.Add(weights);
                        listbuffer.Clear();
                    }
                }
            }    
        }

        /// <summary>
        /// 调取地磅数据
        /// </summary>
        /// <returns>读取是否成功</returns>
        public bool ReadRecords(ref List<double> WeightList )
        {
            try
            {
                //读入车厢重量
                WeightList.AddRange(CarWeights[0]);
                CarWeights.RemoveAt(0);
                return (true);

            }
            catch(Exception e)
            {
                Function.WriteErrorLog("地磅数据读取出错：" + e.Message);
                return (false);
            }
        }
    }

    /// <summary>
    /// 网络摄像机显示控制(2022/1/3修改 摄像头不接入本系统，直接接入调度室的监控系统)
    /// 注意:建议提前将摄像头的后台推流编码修改为非h.265的其他格式
    /// </summary>
    public class WebCamModule
    {
        /// <summary>
        /// 播放窗口控件
        /// </summary>
        PictureBox Picbox;
        /// <summary>
        /// 重车摄像头登录id
        /// 由摄像头返回,登陆失败返回为-1
        /// </summary>
        private Int32 m_lUserIDMZ = -1;

        /// <summary>
        /// 重车播放推流状态码
        /// -1为初始值.表示未设置推流方式并播放
        /// </summary>
        private Int32 m_lRealHandleMZ = -1;

        /// <summary>
        /// 重车视频流通道号
        /// </summary>
        private byte m_DChanMZ;
        public WebCamModule(PictureBox picbox)
        {
            Picbox = picbox;
            ConfigClass.CHCNetSDK.NET_DVR_Init();
            ConnectCAM();
        }
        /// <summary>
        /// 链接相机
        /// </summary>
        /// <returns></returns>
        private bool ConnectCAM()
        {

            try
            {
                if (CameraLoginMZ())
                {
                    PlayCameraMZ();
                }
                return true;
            }
            catch (SocketException socketEx)
            {
                MessageBox.Show("摄像头连接失败！");
                return false;
            }
        }
        /// <summary>
        /// 摄像头登录
        /// </summary>
        private bool CameraLoginMZ()
        {

            if (m_lUserIDMZ < 0)
            {
                //登录信息需要在设备安装时现场设置
                string DVRIPAddress = "192.168.104.33"; //设备IP地址或者域名
                int DVRPortNumber = 8000;//设备服务端口号
                string DVRUserName = "admin";//设备登录用户名
                string DVRPassword = "qazw1234";//设备登录密码

                ConfigClass.CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new ConfigClass.CHCNetSDK.NET_DVR_DEVICEINFO_V30();

                //登录设备 Login the device
                m_lUserIDMZ = ConfigClass.CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref DeviceInfo);
                if (m_lUserIDMZ < 0)
                {

                    return false;
                }
                else
                {
                    m_DChanMZ = DeviceInfo.byStartChan;//模拟通道起始号
                    //MessageBox.Show("m_lUserIDMZ:"+m_lUserIDMZ.ToString()) ;
                    return true;
                }
            }
            else
                return false;

        }

        /// <summary>
        /// 视频播放
        /// </summary>
        private void PlayCameraMZ()
        {

            if (m_lRealHandleMZ < 0)
            {
                //manResetEvent.WaitOne();
                ConfigClass.CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new ConfigClass.CHCNetSDK.NET_DVR_PREVIEWINFO();
                lpPreviewInfo.hPlayWnd = Picbox.Handle;//预览窗口
                lpPreviewInfo.lChannel = m_DChanMZ;//预te览的设备通道
                lpPreviewInfo.dwStreamType = 0;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                lpPreviewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流

                //CHCNetSDK.REALDATACALLBACK RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数
                IntPtr pUser = new IntPtr();//用户数据

                //打开预览 Start live view 
                m_lRealHandleMZ = ConfigClass.CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserIDMZ, ref lpPreviewInfo, null/*RealData*/, pUser);
                //MessageBox.Show("m_lRealHandleMZ:"+m_lRealHandleMZ.ToString()+ "\nm_DChanMZ:" + m_DChanMZ.ToString());
                //MessageBox.Show("lastError:" + CHCNetSDK.NET_DVR_GetLastError().ToString());
            }

        }
    }
}

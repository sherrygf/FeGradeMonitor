using Kitware.VTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GradeMonitorApplication.FunctionClass
{
    class MeasureModule
    {

        #region 变量
        #region 扫描仪
        #region 毛重扫描仪共用变量
        /// <summary>
        /// 链表最大容量
        /// </summary>
        private static int nMaxSize = 4096;
        /// <summary>
        /// 扫描开始事件
        /// </summary>
        private ManualResetEvent scanStartEvent_MZ;
        /// <summary>
        /// 是否输出扫描数据
        /// </summary>
        bool bIsOutPutScanData_MZ;
        /// <summary>
        /// 主动控制体积计算线程的开关
        /// </summary>
        private bool bIsCalVolumFinish_MZ = false;
        /// <summary>
        /// 被动控制体积计算线程的关闭，在MeasLineToMeasCar中判断，在ReleaseAll中复位
        /// </summary>
        private bool bIsMeasureFinish_MZ = false; //扫描正常完成
        /// <summary>
        /// 扫描线处理、归类（归类到对应的车厢）线程
        /// </summary>
        private Thread threadProcessMeasData_MZ;
        /// <summary>
        /// 主动控制扫描线处理、归类（归类到对应的车厢）线程的开关
        /// </summary>
        private bool bIsSaveDataFinish_MZ = false;
        /// <summary>
        /// 体积计算线程
        /// </summary>
        private Thread threadCalculateVolume_MZ;

        /// <summary>
        /// 毛重扫描仪来车方向对象
        /// </summary>
        TRAINDIRECTION trainDirect_MZ = TRAINDIRECTION.UNARRIVE;
        /// <summary>
        /// 毛重扫描仪来车方向的状态
        /// </summary>
        string strDirectState_MZ = "未到达";
        /// <summary>
        /// 毛重扫描仪判断是否是地面
        /// </summary>
        int bCurIsGround_MZ = -1;
        /// <summary>
        /// 毛重扫描仪最近一次判断是否是地面
        /// </summary>
        int bLastIsGround_MZ = -1;
        /// <summary>
        /// 毛重扫描仪数据存储文件夹路径
        /// </summary>
        string strFolderPath_MZ;
        /// <summary>
        /// 毛重扫描仪临时存储一节车厢的体积的相关数据
        /// </summary>
        private MeasCar_Vol measCarVol_MZ;
        /// <summary>
        /// 毛重扫描仪临时存储多节车厢的体积的相关数据链表
        /// </summary>
        private List<MeasCar_Vol> measCarVolList_MZ;
        /// <summary>
        /// 毛重扫描仪最终存储一列车的所有相关数据
        /// </summary>
        private MeasTrain measTrain_MZ;
        /// <summary>
        /// 毛重扫描仪计算体积时标记的车厢号
        /// </summary>
        int CarID_MZ = 0;
        #endregion


        #region 毛重横向扫描仪
        /// <summary>
        /// 毛重横向扫描仪收发包操作对象
        /// </summary>
        public SocketClient socket_LMS_MZHX;
        /// <summary>
        /// 毛重横向扫描仪接收的字节数据链表
        /// </summary>
        private List<byte> byteLst_MZHX;
        /// <summary>
        /// 毛重横向扫描仪扫描断面id
        /// </summary>
        public int scanLineID_MZHX = 0;
        /// <summary>
        /// 毛重横向扫描仪矿车扫描线数量
        /// </summary>
        int CarScanLineCount_MZHX = 0;
        /// <summary>
        /// 毛重横向扫描仪断面数据链表
        /// </summary>
        private List<MeasLine> measLineList_MZHX;
        /// <summary>
        /// 毛重横向扫描仪地面扫描线数量
        /// </summary>
        int GroundScanLineCount_MZHX = 0;
        /// <summary>
        /// 毛重横向扫描仪最后的扫描线ID
        /// </summary>
        int FinishLineID_MZHX = 0;
        /// <summary>
        /// 毛重横向扫描仪断面点云数据存储路径
        /// (20210825)尚未初始化值
        /// </summary>
        string strScanLineSavePath_MZHX;
        #endregion

        #region 毛重纵向扫描仪
        /// <summary>
        /// 毛重纵向扫描仪收发包操作对象
        /// </summary>
        public SocketClient socket_LMS_MZZX;
        /// <summary>
        /// 毛重纵向扫描仪接收的字节数据链表
        /// </summary>
        private List<byte> byteLst_MZZX;
        /// <summary>
        /// 毛重纵向扫描仪扫描断面id
        /// </summary>
        public int scanLineID_MZZX = 0;
        /// <summary>
        /// 毛重纵向扫描仪断面数据链表
        /// </summary>
        private List<MeasLine> measLineList_MZZX;
        /// <summary>
        /// 毛重纵向扫描仪断面点云数据存储路径
        /// (20210825)尚未初始化值
        /// </summary>
        string strScanLineSavePath_MZXX;
        #endregion
        #endregion
        #region (20210825)全局计算辅助变量

        /// <summary>
        /// 车厢扫描线计数变量
        /// </summary>
        int line_count = 0;
        /// <summary>
        /// 累计出现无法判断矿车来向的纵扫描线数
        /// </summary>
        int Unarrival_count;
        /// <summary>
        /// 计算体积时标记的车厢号
        /// </summary>
        int CarID;
        #endregion

        #region 辅助计算定值
        /// <summary>
        /// 基准面高度,以及车厢的平均高度阈值
        /// </summary>
        const double BaseHeight = 3.5;
        /// <summary>
        /// x轴的筛选下限值
        /// </summary>
        const double X_LeftLimit = -1.0;
        /// <summary>
        /// x轴的筛选上限值
        /// </summary>
        const double X_RightLimt = 2.0;
        #endregion
          static FileStream fs ;
          static FileStream ft ;
          string fsn = "E://测试数据//saomiaoxian_H-"+DateTime.Now.ToString("yyyyMMddHHmmss")+".txt";
          string ftn = "E://测试数据//saomiaoxian_Z-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
        Button Flag;
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public MeasureModule(Button flag)
        {
            #region 扫描仪连接前配置
            scanStartEvent_MZ = new ManualResetEvent(false);
            //毛重扫描仪
            socket_LMS_MZHX = new SocketClient();
            socket_LMS_MZZX = new SocketClient();
            byteLst_MZHX = new List<byte>(nMaxSize);
            byteLst_MZZX = new List<byte>(nMaxSize);
            //+:为委托链。表示订阅者订阅了事件，并设置响应函数。当发布者触发事件时DataReceivedEvent?.Invoke(this, EventArgs.Empty)，订阅者执行响应函数ReceivedEventResponse。
            socket_LMS_MZHX.DataReceivedEvent += new EventHandler(SocketLMSMZHXReceivedEventResponse);
            socket_LMS_MZZX.DataReceivedEvent += new EventHandler(SocketLMSMZZXReceivedEventResponse);
            #endregion
            fs = new FileStream(fsn, FileMode.OpenOrCreate);
            ft = new FileStream(ftn, FileMode.OpenOrCreate);
            Flag = flag;
        }

        #region 设备连接前配置：定义DataRecieveEvent事件触发时MeasureModule执行的响应函数
        #region 毛重横向扫描仪连接前配置：委托绑定的函数，事件响应函数
        /// <summary>
        /// 毛重横向扫描仪Socket数据接收Receive事件的响应：一条扫描线数据一条扫描线数据接收（此时还未裁剪矿车两边的点）。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SocketLMSMZHXReceivedEventResponse(object sender, EventArgs e)
        {
            try
            {
                string str, header, mine;
                string[] strtemp;
                byte[] bs = this.socket_LMS_MZHX.ReceivedBytes;  //定义byte数组存放从客户端接收过来的数据
                byteLst_MZHX.AddRange(bs);
                while (byteLst_MZHX.Count >= 5 && !bIsMeasureFinish_MZ)//（此处需要调试：数据链表的个数）
                {
                    //1、找到数据开始标志和结束标志，提取有用数据
                    if (byteLst_MZHX[0] == 0x02)  //数据以0x02开头（ASCII字符表中02表示正文开始）（此处需要调试：数据开始标志）
                    {
                        byte byt = 0x03;  //数据以0x03结束（ASCII字符表中03表示正文结束）（此处需要调试：数据结束标志）
                        if (byteLst_MZHX.Exists(param => param.Equals(byt)))  //判断是否包含元素byt
                        {
                            int indexETX = byteLst_MZHX.IndexOf(byt);  //结束标志的索引
                            byte[] data = byteLst_MZHX.GetRange(1, indexETX - 1).ToArray(); //有用数据为开始标志和结束标志中间的内容
                            //2、处理有用数据
                            #region 处理数据
                            str = Encoding.ASCII.GetString(data);  //ASCII转String
                            strtemp = str.Split(' ');  //以‘’标志切分数据
                            header = strtemp[0] + " " + strtemp[1];  //头两个数据组合
                            if (header == "sSN LMDscandata")  //依据扫描仪开发说明文档（此处需要调试：有用数据的标志，依据扫描仪开发文档）
                            {
                                scanLineID_MZHX++;  //断面计数，从1开始  
                                mine = scanLineID_MZHX.ToString() + "\t";
                                //车子来了才解析：trainDirect_MZ的值在SocketLMSMZZXReceivedEvent纵向扫描仪接收事件中进行判断。
                                //即两个扫描仪同时接收数据，等由纵向扫描仪判断出车子来了之后赋值trainDirect_MZ，使其不等于TRAINDIRECTION.UNARRIVE
                                //if (trainDirect_MZ != TRAINDIRECTION.UNARRIVE)//由于测试数据不完全故去掉此条件
                                {
                                    int dataLength = Convert.ToInt32(strtemp[25], 16);  //第26个是长度，指点的个数。16为进制（此处需要调试：表示点的个数的表示，依据扫描仪开发文档）
                                    int startAngular = Convert.ToInt32(ConfigInfo_Auto.LMS_MZHX_StartAngle);  //毛重横向扫描仪起始角度
                                    double angularResolution = Convert.ToDouble(ConfigInfo_Auto.LMS_AngleResolution);  //扫描仪角度分辨率

                                    //①扫描线：赋值编号
                                    MeasLine line = new MeasLine();
                                    line.SectionID = scanLineID_MZHX;  //从1开始
                                    //②扫描线：赋值时间戳
                                    //int year = Convert.ToInt32(strtemp[strtemp.GetLength(0) - 8], 16);
                                    //int month = Convert.ToInt32(strtemp[strtemp.GetLength(0) - 7], 16);
                                    //int day = Convert.ToInt32(strtemp[strtemp.GetLength(0) - 6], 16);
                                    int hour = Convert.ToInt32(strtemp[strtemp.GetLength(0) - 5], 16);
                                    int minute = Convert.ToInt32(strtemp[strtemp.GetLength(0) - 4], 16);
                                    int second = Convert.ToInt32(strtemp[strtemp.GetLength(0) - 3], 16);
                                    int microsecond = Convert.ToInt32(strtemp[strtemp.GetLength(0) - 2], 16);
                                    double sec = second + (double)microsecond / 1000000;
                                    string strTimStamp = string.Format("{0:00}:{1:00}:{2:00.000}", hour, minute, sec);
                                    DateTime timeStamp = Convert.ToDateTime(strTimStamp);
                                    line.timeStamp = timeStamp;

                                    //③扫描线：赋值点集
                                    double averageY = 0; //Y平均值，用于判断是否为地面扫描线
                                    int nCount = 0;  //计算Y平均值的点的个数
                                    Win32API.COPYDATASTRUCT ldata = new Win32API.COPYDATASTRUCT();  //存储点坐标值，用于输出txt文件保存点数据
                                    for (int i = 0; i < dataLength; i++)  //依次处理扫描线上的点
                                    {
                                        //计算点的X、Y值：极坐标转平面坐标
                                        int dist_ = Convert.ToInt32(strtemp[i + 26], 16);  //极坐标距离。点数据从第27个开始。
                                        double dist = 2.0 * dist_ / 1000.0;
                                        if (dist <= 0.01 || dist >= 20) //踢出噪点（此处需要调试：踢出噪点的距离范围）
                                        {
                                            continue;
                                        }
                                        double x = dist * Math.Cos((startAngular + angularResolution * i) * Math.PI / 180.0);  //计算点的X值：极坐标转平面坐标
                                        double y = dist * Math.Sin((startAngular + angularResolution * i) * Math.PI / 180.0);  //计算点的Y值：极坐标转平面坐标
                                        mine += dist.ToString() + "," + (startAngular + angularResolution * i) + "," + x.ToString() + "," + y.ToString() + ";";
                                        if (y < 1.35 || y > 5.5) //踢出噪点。<1.35是电线，>5.5是低于地面的错误点（此处需要调试：踢出噪点的Y值范围）
                                        {
                                            continue;
                                        }
                                        line.points.Add(new MeasPoint(x, y));
                                        ldata.lpData += x.ToString("#0.000") + "," + y.ToString("#0.000") + ";";

                                        //统计X值在-0.5~+0.5m内的平均y
                                        if (x < 0.5 && x > -0.5)//（此处需要调试：X值的统计范围，及有用值的有用范围）
                                        {
                                            averageY += y;
                                            nCount++;
                                        }
                                    }
                                    mine += "\n";
                                     fs.Write(Encoding.UTF8.GetBytes(mine), 0, Encoding.UTF8.GetBytes(mine).Length);
                                     fs.Flush();
                                    averageY /= nCount;  //计算Y平均值
                                    //统计判断是否是地面。规则：-0.5~+0.5m内，平均y>6.5m
                                    if (averageY > 4.5)  //坐标横轴在扫描仪水平位置，向下为Y轴正方向。（此处需要调试：多少条数据后开始判断车子是否已经经过测量范围）
                                        bCurIsGround_MZ = 1;
                                    else
                                    {
                                        CarScanLineCount_MZHX++;
                                        bCurIsGround_MZ = 0;
                                    }

                                    //④扫描线：存储该扫描线
                                    Monitor.Enter(measLineList_MZHX);
                                    measLineList_MZHX.Add(line);
                                    Monitor.Exit(measLineList_MZHX);


                                    //在已经扫描100条数据开始，判断车子是否已经经过测量范围（此处需要调试：多少条数据后开始判断车子是否已经经过测量范围）
                                    if (CarScanLineCount_MZHX > 100)
                                    {
                                        if (IsScanGround(line))//判断是否为地面
                                        {
                                            GroundScanLineCount_MZHX++;
                                        }
                                        if (GroundScanLineCount_MZHX > 50 && FinishLineID_MZHX == 0)//车子经过测量范围后扫描地面超过50条数据(此处需要调试：地面扫描线的数量)
                                        {
                                            //FinishMeasure();
                                            bIsMeasureFinish_MZ = true;

                                            //记下此刻的id
                                            FinishLineID_MZHX = line.SectionID;
                                            break;
                                        }
                                    }

                                    bLastIsGround_MZ = bCurIsGround_MZ;

                                    if (bIsOutPutScanData_MZ)//输出数据
                                    {
                                        string strSaveToTxt = ldata.lpData;
                                        ///Function.SaveToTxt(scanLineID_MZHX.ToString() + "\t" + strTimStamp + "\t" + strSaveToTxt.Remove(ldata.lpData.Length - 1), strScanLineSavePath_MZHX);
                                    }

                                    scanStartEvent_MZ.Set(); //数据开始接收事件（技术问题：此事件的作用）
                                }
                            }
                            #endregion

                            byteLst_MZHX.RemoveRange(0, indexETX + 1);//移除依据处理过的数据
                        }
                        else
                        {
                            break;
                        }

                    }
                    else//如果开头字节不为0x02 则剔除
                    {
                        byteLst_MZHX.RemoveAt(0);
                    }

                }

            }
            catch (Exception ex)
            {
                ///Function.WriteErrorLog("横向扫描数据接收和处理函数失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 判断是否扫描的是地面：判断依据轨道衡中心线附近的点的平均Y值是否满足阈值
        /// </summary>
        /// <param name="line">扫描线</param>
        /// <returns></returns>
        public static bool IsScanGround(MeasLine line)
        {
            bool result;  //判断结果

            //轨道中心线所在位置的X值
            double minRange = 0;  //一侧边界X值（此处需要调试：架设好硬件后进行测量。车厢边界还是轨道边界？）
            double maxRange = 1.35;  //另一侧边界X值 
            double CenterX = (minRange + maxRange) / 2;  //轨道中心线所在位置X值
            try
            {
                //数据粗差剔除（此处需要调试）
                ExcludeErrorData(ref line);

                double sum = 0;
                int count = 0;
                for (int i = 0; i < line.points.Count; i++)
                {
                    double x = line.points[i].X;
                    double y = line.points[i].Y;
                    if (x > CenterX - 0.2 && x < CenterX + 0.2)
                    {
                        sum += y;
                        count++;
                    }
                }
                double average = sum / count;

                if (average > 4.5)//扫描地面
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                ///Function.WriteErrorLog("IsScanGround失败：" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 点云去躁：阶跃点求解边缘出车厢的边缘作为基准面
        /// (为何采用这种思路取参考面和去噪暂不理解）
        /// </summary>
        static void ExcludeErrorData(ref MeasLine line)
        {
            List<MeasPoint> Points = line.points;

            try
            {

                double planeY = 0;
                int numy = 0;

                for (int i = 0; i < Points.Count; i++) //计算周边正常点高度
                {
                    if (Points[i].X > 0 && Points[i].X < 0.4)//（此处需要调试：0.4的由来）
                    {
                        planeY += Points[i].Y;
                        numy++;
                    }

                }
                planeY /= numy;

                for (int i = 0; i < Points.Count - 2; i++)
                {
                    if (Points[i].X > -0.3 && Points[i].X < 0)//控制搜索范围在车厢两边缘处
                    {
                        MeasPoint p1 = Points[i];
                        MeasPoint p2 = Points[i + 1];
                        MeasPoint p3 = Points[i + 2];

                        double dist1 = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
                        double dist2 = Math.Sqrt(Math.Pow(p2.X - p3.X, 2) + Math.Pow(p2.Y - p3.Y, 2));
                        double delt_dist = dist1 - dist2;

                        double dbcDPP = Math.Abs(delt_dist);

                        double dist3 = Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
                        double dist4 = Math.Sqrt(Math.Pow(p2.X - p3.X, 2) + Math.Pow(p2.Y - p3.Y, 2));
                        double ration_dist = dist3 / dist4;
                        if (ration_dist <= 1)
                        {
                            ration_dist = 1 / ration_dist;
                        }

                        double dbcRPP = ration_dist;

                        if (dbcDPP > 0.2 && dbcRPP > 3 && Points[i].Y > planeY + 0.15)//判断边缘点（此处需要调试）
                        {
                            line.points.RemoveAt(i);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ///Function.WriteErrorLog(" ExcludeErrorData函数失败：" + ex.Message);
            }

        }
        #endregion

        #region 毛重纵向扫描仪连接前配置：委托绑定的函数，事件响应函数
        /// <summary>
        /// 毛重纵向扫描仪Socket数据接收Receive事件的响应：一条扫描线数据一条扫描线数据接收。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SocketLMSMZZXReceivedEventResponse(object sender, EventArgs e)
        {
            string error = "";
            try
            {
                string str, header, mine;
                string[] strtemp;
                byte[] bs = this.socket_LMS_MZZX.ReceivedBytes;  //定义byte数组存放从客户端接收过来的数据
                byteLst_MZZX.AddRange(bs);
                while (byteLst_MZZX.Count >= 20 && !bIsMeasureFinish_MZ)//（此处需要调试：数据链表的个数）
                {
                    //1、找到数据开始标志和结束标志，提取有用数据
                    if (byteLst_MZZX[0] == 0x02)  //数据以0x02开头（ASCII字符表中02表示正文开始）（此处需要调试：数据开始标志）
                    {
                        byte byt = 0x03;  //数据以0x03结束（ASCII字符表中03表示正文结束）（此处需要调试：数据结束标志）
                        if (byteLst_MZZX.Exists(param => param.Equals(byt))) //判断是否包含元素byt
                        {
                            int indexETX = byteLst_MZZX.IndexOf(byt); //结束标志的索引
                            byte[] data = byteLst_MZZX.GetRange(1, indexETX - 1).ToArray(); //有用数据为开始标志和结束标志中间的内容
                            //2、处理有用数据
                            #region  处理数据
                            str = Encoding.ASCII.GetString(data); //ASCII转String
                            strtemp = str.Split(' ');  //以‘’标志切分数据
                            header = strtemp[0] + " " + strtemp[1];  //头两个数据组合

                            if (header == "sSN LMDscandata") //依据扫描仪开发说明文档（此处需要调试：有用数据的标志，依据扫描仪开发文档）
                            {

                                scanLineID_MZZX++;  //断面计数，从1开始
                                mine = scanLineID_MZZX.ToString() + "\t";
                                int dataLength = Convert.ToInt32(strtemp[25], 16);  //第26个是长度，指点的个数。16为进制（此处需要调试：表示点的个数的表示，依据扫描仪开发文档）
                                int startAngular = Convert.ToInt32(ConfigInfo_Auto.LMS_MZZX_StartAngle);  //毛重纵向扫描仪起始角度
                                double angularResolution = Convert.ToDouble(ConfigInfo_Auto.LMS_AngleResolution);  //扫描仪角度分辨率
                                //①扫描线：赋值编号
                                MeasLine line = new MeasLine();
                                line.SectionID = scanLineID_MZZX;

                                //②扫描线：赋值时间戳
                                int hour = Convert.ToInt32(strtemp[strtemp.GetLength(0) - 5], 16);
                                int minute = Convert.ToInt32(strtemp[strtemp.GetLength(0) - 4], 16);
                                int second = Convert.ToInt32(strtemp[strtemp.GetLength(0) - 3], 16);
                                int microsecond = Convert.ToInt32(strtemp[strtemp.GetLength(0) - 2], 16);
                                double sec = second + (double)microsecond / 1000000;
                                string strtimstamp = string.Format("{0:00}:{1:00}:{2:00.000}", hour, minute, sec);
                                DateTime timeStamp = Convert.ToDateTime(strtimstamp);
                                line.timeStamp = timeStamp;

                                Win32API.COPYDATASTRUCT ldata = new Win32API.COPYDATASTRUCT();
                                double dAverageY_XLeft = 0.0;
                                double dAverageY_XRight = 0.0;
                                int nCount_XLeft = 0;
                                int nCount_XRight = 0;


                                //③扫描线：赋值点集
                                for (int i = 0; i < dataLength; i++)
                                {
                                    error = strtemp[i + 26];
                                    //计算点的X、Y值：极坐标转平面坐标
                                    int dist_ = Convert.ToInt32(strtemp[i + 26], 16);   //距离。点数据从第27个开始。
                                    double dist = 2.0 * dist_ / 1000.0;
                                    if (dist <= 0.01 || dist >= 20) //踢出噪点？（此处需要调试：踢出噪点的距离范围）
                                    {
                                        continue;
                                    }
                                    double x = dist * Math.Cos((startAngular + angularResolution * i) * Math.PI / 180.0);  //计算点的X值：极坐标转平面坐标
                                    double y = dist * Math.Sin((startAngular + angularResolution * i) * Math.PI / 180.0);  //计算点的Y值：极坐标转平面坐标
                                    mine += dist.ToString() + "," + (startAngular + angularResolution * i) + "," + x.ToString() + "," + y.ToString() + ";";
                                    if (y < 1.35 || y > 5.5) //踢出噪点。<1.35是电线，>5.5是低于地面的错误点（此处需要调试：踢出噪点的Y值范围）
                                    {
                                        continue;
                                    }

                                    line.points.Add(new MeasPoint(x, y));
                                    ldata.lpData += x.ToString("#0.000") + "," + y.ToString("#0.000") + ";";

                                    if (trainDirect_MZ == TRAINDIRECTION.UNARRIVE)//车子方向状态还是没来时执行车的方向判断：依据不同方向驶来时左右点云的平均Y值的不同。
                                    {
                                        if (x > 0)
                                        {
                                            dAverageY_XLeft += y;
                                            nCount_XLeft++;
                                        }
                                        else
                                        {
                                            dAverageY_XRight += y;
                                            nCount_XRight++;
                                        }
                                    }
                                }
                                mine += "\n";
                                ft.Write(Encoding.UTF8.GetBytes(mine), 0, Encoding.UTF8.GetBytes(mine).Length);
                                ft.Flush();

                                //判断来车方向，车子来了才进行纵向数据的采集。规则：分别统计x>0的点y的平均值和x<0的点y的平均值。
                                if (trainDirect_MZ == TRAINDIRECTION.UNARRIVE) //车子没来或者刚来
                                {

                                    dAverageY_XLeft /= nCount_XLeft;
                                    dAverageY_XRight /= nCount_XRight;
                                    if (dAverageY_XLeft < 4) //左边来车（此处需要调试：判断车来的平均Y值）
                                    {
                                        trainDirect_MZ = TRAINDIRECTION.WEST;//正向
                                        strDirectState_MZ = "车头进";
                                        measTrain_MZ.bMeasDirection = true;
                                    }
                                    //删除车尾进的情况，直接作为未到达，未到达时间积累到一定量之后认为出现异常，开始通过测量中断的流程以结束测量
                                    else
                                    {
                                        strDirectState_MZ = "未到达";
                                        Unarrival_count++;
                                        if (Unarrival_count > 250)//累计250次扫描(约10s后)仍无法确定来向即判定为误报，执行测量中断
                                            StopMeasure();
                                    }

                                }
                                else  //车子已来
                                {
                                    Monitor.Enter(measLineList_MZZX);
                                    measLineList_MZZX.Add(line);
                                    Monitor.Exit(measLineList_MZZX);

                                    if (bIsOutPutScanData_MZ)//保存点云数据
                                    {
                                        string strSaveToTxt = ldata.lpData;
                                        ///Function.SaveToTxt(scanLineID_MZZX.ToString() + "\t" + strtimstamp + "\t" + strSaveToTxt.Remove(ldata.lpData.Length - 1), strScanLineSavePath_MZXX);
                                    }
                                }
                            }
                            #endregion
                            byteLst_MZZX.RemoveRange(0, indexETX + 1);
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        byteLst_MZZX.RemoveAt(0);
                    }
                }
            }
            catch (Exception ex)
            {
                

            }
        }
        #endregion
        #endregion

        #region 连接设备：启动数据接收，当有数据传入时，触发DataRecieveEvent事件，并执行响应函数EventResponse。
        /// <summary>
        /// 连接所有设备
        /// </summary>
        /// <returns></returns>
        public bool ConnectAllDevice()
        {
            if (ConnectLMS_MZHX() && ConnectLMS_MZZX())
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        #region 毛重扫描仪连接设备

        /// <summary>
        /// 连接毛重横向扫描仪
        /// </summary>
        /// <returns></returns>
        private bool ConnectLMS_MZHX()
        {
            try
            {

                //socket连接
                this.socket_LMS_MZHX.Connect(MainForm.ipinfo.ToIPAddress(MainForm.ipinfo.scanerHX),2112);
                //this.socket_LMS_MZHX.Connect(new IPAddress(new byte[] { 192, 168, 110, 154 }), 2112);
                //socket接收
                this.socket_LMS_MZHX.BeginReceive();//开始接收数据（异步接收），接收到数据时，触发事件DataReceivedEvent?.Invoke(this, EventArgs.Empty);，然后MeasureModule执行事件响应函数ReceivedEventResponse

                //设置输出内容格式
                this.Socket_Send(this.socket_LMS_MZHX, "sMN SetAccessMode 03 F4724744"); //"sMN SetAccessMode 03 F4724744"：请求登录报文

                //校准扫描仪时间
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month;
                int day = DateTime.Now.Day;
                int hour = DateTime.Now.Hour;
                int minute = DateTime.Now.Minute;
                int second = DateTime.Now.Second;
                int mircosecond = DateTime.Now.Millisecond * 1000;
                string strSend = String.Format("sMN LSPsetdatetime {0} {1} {2} {3} {4} {5} {6}", year.ToString("X"), month.ToString("X"), day.ToString("X"), hour.ToString("X"), minute.ToString("X"), second.ToString("X"), mircosecond.ToString("X"));
                //this.Socket_Send(this.socket_LMS_MZHX, strSend);

                //配置数据内容
                //this.Socket_Send(this.socket_LMS_MZHX, "sWN LMDscandatacfg 01 00 1 1 0 00 00 0 0 0 1 +1");

                //设置分辨率和频率
                //this.Socket_Send(this.socket_LMS_MZHX, "sMN mLMPsetscancfg +2500 +1 +2500 -450000 +2250000");

                //设置输出范围
                //this.Socket_Send(this.socket_LMS_MZHX, "sWN LMPoutputRange 1 9C4 +650000 +1100000");//65-110

                //参数固化
                this.Socket_Send(this.socket_LMS_MZHX, "sMN mEEwriteall");// "sMN mEEwriteall"：设置参数

                //启动设备
                //this.Socket_Send(this.socket_LMS_MZHX, "sMN Run");//"sMN Run"：注销并启动设备

                return true;
            }
            catch (Exception socketEx)
            {
                ///Function.WriteErrorLog("ConnectLMS_MZHX函数：横向扫描仪连接失败。" + socketEx.Message);
                return false;
            }
        }

        /// <summary>
        /// 连接毛重纵向扫描仪
        /// </summary>
        /// <returns></returns>
        private bool ConnectLMS_MZZX()
        {
            try
            {
                this.socket_LMS_MZZX.Connect(MainForm.ipinfo.ToIPAddress(MainForm.ipinfo.scanerZX), 2112);
                //this.socket_LMS_MZZX.Connect(new IPAddress(new byte[] { 192, 168, 110, 155 }), 2112);
                this.socket_LMS_MZZX.BeginReceive();

                //设置输出内容格式
                //login
                this.Socket_Send(this.socket_LMS_MZZX, "sMN SetAccessMode 03 F4724744");
                //设置时间
                int year = DateTime.Now.Year;
                int month = DateTime.Now.Month;
                int day = DateTime.Now.Day;
                int hour = DateTime.Now.Hour;
                int minute = DateTime.Now.Minute; ;
                int second = DateTime.Now.Second;
                int mircosecond = DateTime.Now.Millisecond * 1000;
                string strSend = String.Format("sMN LSPsetdatetime {0} {1} {2} {3} {4} {5} {6}", year.ToString("X"), month.ToString("X"), day.ToString("X"), hour.ToString("X"), minute.ToString("X"), second.ToString("X"), mircosecond.ToString("X"));
                //this.Socket_Send(this.socket_LMS_MZZX, strSend);
                //configure data content
                //this.Socket_Send(this.socket_LMS_MZZX, "sWN LMDscandatacfg 01 00 1 1 0 00 00 0 0 0 1 +1");

                ////设置分辨率和频率
                //this.Socket_Send(this.socket_LMS_MZZX, "sMN mLMPsetscancfg +2500 +1 +2500 -450000 +2250000");

                ////设置输出范围
                //this.Socket_Send(this.socket_LMS_MZZX, "sWN LMPoutputRange 1 9C4 +300000 +1500000");//30-150

                //参数固化
                this.Socket_Send(this.socket_LMS_MZZX, "sMN mEEwriteall");

                //this.Socket_Send(this.socket_LMS_MZZX, "sMN Run");

                return true;
            }
            catch (SocketException socketEx)
            {
                //MessageBox.Show("纵向扫描仪连接失败！");
                ///Function.WriteErrorLog("横向扫描仪连接失败：" + socketEx.Message);
                return false;
            }

        }

        #endregion

        #endregion

        #region 运行设备：向扫描仪发送扫描指令，扫描仪返回数据，接到数据后事件DataRecieveEvent将被触发。
        /// <summary>
        /// 运行所有设备
        /// </summary>
        public void StartMeasure()
        {
            //此处应先初始化所有变量
            InitAllVariable();
            //  fs = new FileStream("E://saomiaoxian_H.txt", FileMode.OpenOrCreate);
            //  ft = new FileStream("E://saomiaoxian_Z.txt", FileMode.OpenOrCreate);
            //校准扫描仪时间，防止延迟
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            int hour = DateTime.Now.Hour;
            int minute = DateTime.Now.Minute;
            int second = DateTime.Now.Second;
            int mircosecond = DateTime.Now.Millisecond * 1000;
            string strSend = String.Format("sMN LSPsetdatetime {0} {1} {2} {3} {4} {5} {6}", year.ToString("X"), month.ToString("X"), day.ToString("X"), hour.ToString("X"), minute.ToString("X"), second.ToString("X"), mircosecond.ToString("X"));

            //向扫描仪发送指令
            //this.Socket_Send(this.socket_LMS_MZHX, "sMN mEEwriteall");
            //this.Socket_Send(this.socket_LMS_MZZX, "sMN mEEwriteall");
            this.Socket_Send(this.socket_LMS_MZHX, "sMN LMCstartmeas");
            this.Socket_Send(this.socket_LMS_MZZX, "sMN LMCstartmeas");
            this.Socket_Send(this.socket_LMS_MZHX, "sMN Run");//扫描仪运行命令
            this.Socket_Send(this.socket_LMS_MZZX, "sMN Run");
            this.Socket_Send(this.socket_LMS_MZHX, "sEN LMDscandata 1");//sEN LMDscandata：连续输出测量值，直到使用sEN LMDscandata再次停止测量值输出为止
            this.Socket_Send(this.socket_LMS_MZZX, "sEN LMDscandata 1");//sEN LMDscandata：连续输出测量值，直到使用sEN LMDscandata再次停止测量值输出为止

            //扫描线处理、归类（归类到对应的车厢）线程：存储扫描线到对应车厢，接收到一条处理一条
            threadProcessMeasData_MZ = new Thread(ThreadProcessMeasData);
            threadProcessMeasData_MZ.IsBackground = true;
            threadProcessMeasData_MZ.Start();

            //体积计算线程：计算车厢最终体积，接收到一个完整车厢扫描线处理一个
            bIsSaveDataFinish_MZ = false;
            bIsCalVolumFinish_MZ = false;
            threadCalculateVolume_MZ = new Thread(ThreadCalculateVolome);
            threadCalculateVolume_MZ.IsBackground = true;
            threadCalculateVolume_MZ.Start();


            //if (bIsFull)
            //{
            //    strFolderPath = "ProjectData\\" + FormAutoMeasureNew.frmMeasure.texbox_ID.Text + "\\重车";
            //}
            //else
            //{
            //    strFolderPath = "ProjectData\\" + FormAutoMeasureNew.frmMeasure.texbox_ID.Text + "\\空车";
            //}
            //strScanLineH = strFolderPath + "\\横断面数据.txt";

            //strScanLineZ = strFolderPath + "\\纵断面数据.txt";

        }

        /// <summary>
        /// 初始化变量
        /// </summary>
        public void InitAllVariable()
        {
            #region 毛重扫描仪
            bIsMeasureFinish_MZ = false;
            CarScanLineCount_MZHX = 0;
            GroundScanLineCount_MZHX = 0;
            if (byteLst_MZHX != null)
                byteLst_MZHX.Clear();
            if (byteLst_MZZX != null)
                byteLst_MZZX.Clear();
            measLineList_MZHX = new List<MeasLine>();
            measLineList_MZZX = new List<MeasLine>();
            measCarVolList_MZ = new List<MeasCar_Vol>();
            measTrain_MZ = new MeasTrain();
            scanLineID_MZHX = 0;
            scanLineID_MZZX = 0;
            CarID_MZ = 0;
            bCurIsGround_MZ = -1;
            bLastIsGround_MZ = -1;
            trainDirect_MZ = TRAINDIRECTION.UNARRIVE;
            strDirectState_MZ = "未到达";
            FinishLineID_MZHX = 0;
            #endregion
            #region (20210825)计算辅助变量

            measCarVol_MZ = new MeasCar_Vol();
            line_count = 0;
            CarID = 0;
            Unarrival_count = 0;
            #endregion
        }
        #endregion

        #region (20210825新增)中断测量
        /// <summary>
        /// 当确认到由于重车轨道车辆逆行、或空车轨道过车误触发RFID导致进入测量流程后,执行此函数令扫描仪停止扫描并不保存测量结果
        /// </summary>
        public void StopMeasure()
        {
            Function.WriteMissingLog();
            //中断体积数据整理与计算线程
            threadCalculateVolume_MZ.Abort();
            threadProcessMeasData_MZ.Abort();
            bIsMeasureFinish_MZ = true;
            bIsSaveDataFinish_MZ = true;
            bIsCalVolumFinish_MZ = true;
            Flag.BackgroundImage = Image.FromFile(".\\UI\\启动128.png");
            //然后通知扫描仪停止运行
            if (this.socket_LMS_MZHX.IsConnected && this.socket_LMS_MZZX.IsConnected)
            {
                this.Socket_Send(this.socket_LMS_MZHX, "sMN SetAccessMode 03 F4724744");
                this.Socket_Send(this.socket_LMS_MZZX, "sMN SetAccessMode 03 F4724744");
                this.Socket_Send(this.socket_LMS_MZHX, "sMN LMCstandby");
                this.Socket_Send(this.socket_LMS_MZZX, "sMN LMCstandby");
                this.Socket_Send(this.socket_LMS_MZHX, "sMN Run");
                this.Socket_Send(this.socket_LMS_MZZX, "sMN Run");
                this.Socket_Send(this.socket_LMS_MZHX, "sEN LMDscandata 0");
                this.Socket_Send(this.socket_LMS_MZZX, "sEN LMDscandata 0");

                Thread.Sleep(50);
                socket_LMS_MZHX._socket.Close();
                socket_LMS_MZZX._socket.Close();
            }
            fs.Close();
            ft.Close();
            InitAllVariable();
            //如果已经有数据输出到日志则删除
            if (bIsOutPutScanData_MZ)
            {
                if (File.Exists(strScanLineSavePath_MZHX))
                    File.Delete(strScanLineSavePath_MZHX);
                if (File.Exists(strScanLineSavePath_MZXX))
                    File.Delete(strScanLineSavePath_MZXX);
            }
        }
        #endregion

        #region (20210825新增)完成测量
        /// <summary>
        /// 在完成整列车的体积测量后将数据上传入库保存,并通知扫描仪停止返回数据
        /// </summary>
        public void FinishMeasure()
        {
            //首先停止接收扫描仪数据，终止所有数据处理线程
            if (this.socket_LMS_MZHX.IsConnected && this.socket_LMS_MZZX.IsConnected)
            {
                this.Socket_Send(this.socket_LMS_MZHX, "sMN SetAccessMode 03 F4724744");
                this.Socket_Send(this.socket_LMS_MZZX, "sMN SetAccessMode 03 F4724744");
                this.Socket_Send(this.socket_LMS_MZHX, "sMN LMCstandby");
                this.Socket_Send(this.socket_LMS_MZZX, "sMN LMCstandby");
                this.Socket_Send(this.socket_LMS_MZHX, "sMN Run");
                this.Socket_Send(this.socket_LMS_MZZX, "sMN Run");
                this.Socket_Send(this.socket_LMS_MZHX, "sEN LMDscandata 0");
                this.Socket_Send(this.socket_LMS_MZZX, "sEN LMDscandata 0");

                Thread.Sleep(50);
                socket_LMS_MZHX._socket.Close();
                socket_LMS_MZZX._socket.Close();

            }
            fs.Close();
            ft.Close();
            Flag.BackgroundImage = Image.FromFile(".\\UI\\启动128.png");
            //输出体积数据(保存为txt体积日志)
            DateTime now = DateTime.Now;
            string VolumeLogsSavePath = "./ProjectData/VolumeLogs/" + now.Year.ToString() + now.Month.ToString() + now.Day.ToString() + "_" + now.Hour.ToString() + now.Minute.ToString() + ".txt";
            string OutputBuffer = "车厢号\t体积\t长度";
            for (int i = 0; i < measTrain_MZ.meascarvollst.Count; i++)
            {
                OutputBuffer += i.ToString() + "\t";
                OutputBuffer += measTrain_MZ.meascarvollst[i].VolumeFrombase.ToString("0.00") + "\t";
                OutputBuffer += measTrain_MZ.meascarvollst[i].carlength.ToString("0.00") + "\t\n";
            }
            OutputBuffer += "==================================================";
            ///Function.SaveToTxt(OutputBuffer, VolumeLogsSavePath);

            //读取重量数量
            string filename = "";
            if (!SearchXMLZFile(ref filename))
            {
                Thread.Sleep(30000);
                if (!SearchXMLZFile(ref filename))
                {
                    Thread.Sleep(180000);
                    SearchXMLZFile(ref filename);
                }
            }
            List<double> Weights = new List<double>();
            double Volumn = 0, weight = 0,sp=0;
            List<double> speed = new List<double>();
            try
            {
                if (filename != "")//如果成功找到对应的重量文件
                {
                    GetWeights(filename, ref Weights, ref speed);
                    int fin = 2;//2、完备
                    int i = 0, limit = Weights.Count;
                    if (measTrain_MZ.meascarvollst.Count < Weights.Count)
                    {
                        fin = 5;//5、体积不足的情况
                        limit = measTrain_MZ.meascarvollst.Count;
                    }
                    for (; i < limit; i++)
                    {
                        Volumn += measTrain_MZ.meascarvollst[i].VolumeFrombase;
                        weight += Weights[i];
                        sp += speed[i];
                        if (Weights[i] < 15.0)
                            fin = 3;//3、取到重量数据，有“n”车厢重量数据有误
                        MainForm.database.AddCarResult(now, measTrain_MZ.meascarvollst[i].VolumeFrombase, Weights[i], speed[i]);
                    }
                    if (measTrain_MZ.meascarvollst.Count < Weights.Count)
                    {
                        for (; i < Weights.Count; i++)
                        {
                            weight += Weights[i];
                            sp += speed[i];
                            MainForm.database.AddCarResult(now, 0, Weights[i], speed[i]);
                        }
                        MainForm.database.AddTrainResult(now, Weights.Count, Volumn, weight, 0, speed.Average(), fin);
                    }
                    else if (measTrain_MZ.meascarvollst.Count > Weights.Count)
                    {
                        fin = 4;//4、取到重量数据，重量数据个数不够
                        for (; i < measTrain_MZ.meascarvollst.Count; i++)
                        {
                            Volumn += measTrain_MZ.meascarvollst[i].VolumeFrombase;
                            MainForm.database.AddCarResult(now, measTrain_MZ.meascarvollst[i].VolumeFrombase, 0, speed[i]);
                        }
                        if (speed.Count != 0)
                            MainForm.database.AddTrainResult(now, measTrain_MZ.meascarvollst.Count, Volumn, 0, 0, speed.Average(), fin);
                        else
                            MainForm.database.AddTrainResult(now, measTrain_MZ.meascarvollst.Count, Volumn, 0, 0, 0, fin);
                    }
                    else if (fin == 3)
                        MainForm.database.AddTrainResult(now, measTrain_MZ.meascarvollst.Count, Volumn, 0, 0, speed.Average(), fin);
                    else
                    {
                        MainForm.database.AddTrainResult(now, measTrain_MZ.meascarvollst.Count, Volumn, weight, 0, speed.Average(), fin);
                    }
                }
                else//如果没找到则只存体积
                {
                    int fin = 1;//1、未取到重量数据
                    int i = 0;
                    for (; i < measTrain_MZ.meascarvollst.Count; i++)
                    {
                        Volumn += measTrain_MZ.meascarvollst[i].VolumeFrombase;
                        MainForm.database.AddCarResult(now, measTrain_MZ.meascarvollst[i].VolumeFrombase, 0, 0);
                    }
                    MainForm.database.AddTrainResult(now, measTrain_MZ.meascarvollst.Count, Volumn, weight, 0, 0, fin);
                }
                //清空变量并初始化
                InitAllVariable();
            }
            catch(Exception e)
            {
                Function.WriteErrorLog("数据存储出错：" + e.Message);
                InitAllVariable();
            }

            // //车走完后休息五分钟再重新启动测量
            // Thread.Sleep(10000);
            // //2022.3.25
            // ConnectAllDevice();
            // StartMeasure();
        }
        #endregion

        #region 相关计算线程函数
        #region 扫描线处理、归类（归类到对应的车厢）线程：存储扫描线到对应车厢
        /// <summary>
        /// 扫描线处理、归类（归类到对应的车厢）线程函数
        /// </summary>
        /// <param name="obj"></param>
        private void ThreadProcessMeasData(object obj)
        {
            try
            {
                while (!bIsSaveDataFinish_MZ)
                {
                    while (measLineList_MZHX.Count > 0) //接收到一条处理一条
                    {
                        scanStartEvent_MZ.WaitOne();//如果processData没有调用set,则线程在此处阻塞（技术问题）

                        //1、临时存储接收到的扫描线
                        List<MeasLine> tmeaslinelst_cal = new List<MeasLine>();
                        Monitor.Enter(measLineList_MZHX);
                        tmeaslinelst_cal.AddRange(measLineList_MZHX);
                        measLineList_MZHX.Clear();  //处理一条清空一条
                        Monitor.Exit(measLineList_MZHX);

                        //2、扫描线归类到对应的车厢
                        while (tmeaslinelst_cal.Count > 0)//判断
                        {
                            if (tmeaslinelst_cal[0].SectionID == FinishLineID_MZHX)//最后一条，则停止扫描线处理、归类（归类到对应的车厢）线程进程
                            {
                                bIsSaveDataFinish_MZ = true;
                            }

                            //扫描线匹配到对应的车厢，并存储到对应的车厢数据里面
                            MeasLineToMeasCar(tmeaslinelst_cal[0]);
                            tmeaslinelst_cal.RemoveAt(0); //处理一个移除一个

                        }
                        Thread.Sleep(50);
                    }
                }
            }
            catch (Exception ex)
            {
                ///Function.WriteErrorLog(" ThreadProcessMeasData函数失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 扫描线匹配到对应的车厢：来一条匹配一条，并存储到对应的车厢数据里面
        /// </summary>
        /// <param name="line"></param>
        private void MeasLineToMeasCar(MeasLine line)
        {
            try
            {
                if (IsPackingCase(ref line) && line_count == 0)//车厢刚开始进入，保存属于车厢的第一条断面线
                {
                    measCarVol_MZ = new MeasCar_Vol();
                    measCarVol_MZ.startTime = line.timeStamp;
                    measCarVol_MZ.startScanMatchIndex = line.SectionID;//记录过程开始的断面号
                    measCarVol_MZ.t_measlinelst.Add(line);
                    line_count++;

                }
                else if (IsPackingCase(ref line) && line_count != 0)//扫描车厢进行中，保存后面属于车厢的断面线
                {
                    measCarVol_MZ.t_measlinelst.Add(line);
                    line_count++;
                }
                else if (!IsPackingCase(ref line) && line_count != 0)//车厢刚离开，结束一节车厢的断面线存储，下次保存到下一节车厢里面
                {
                    if (measCarVol_MZ.t_measlinelst.Count < 20)
                    {
                        line_count = 0;
                        measCarVol_MZ = new MeasCar_Vol();
                        return;
                    }
                    line_count = 0;//一节车厢测量结束
                    measCarVol_MZ.EndTime = line.timeStamp;
                    measCarVol_MZ.endScanMatchIndex = line.SectionID - 1;//上一断面为最后一个断面

                    measCarVolList_MZ.Add(measCarVol_MZ);

                }
                //else if (meastrain.meascarvollst.Count > 0 && IsScanGround(line))//扫描地面，测量过程结束，停止测量 IsScanGround判断方法不严谨，可能出现误判
                //else if (meastrain.meascarvollst.Count == 6 && IsScanGround(line)) //只有在测了6个车厢时才结束 
                //else if (meastrain.meascarvollst.Count > 0 && IsScanGround(line)) //有时候只有两节车厢 又要换回来20150731
                //{
                //    bIsMeasureFinish = true;
                //}
            }
            catch (Exception ex)
            {
                ///Function.WriteErrorLog(" MeasLineToMeasCar函数失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 判断扫描线是否属于车厢
        /// </summary>
        static public bool IsPackingCase(ref MeasLine line)
        {
            try
            {
                //数据粗差剔除
                ExcludeErrorData(ref line);

                double sum = 0;
                for (int i = 0; i < line.points.Count; i++)
                {
                    if (line.points[i] != null)
                        sum += line.points[i].Y;
                }
                double average = sum / line.points.Count;

                if (average < 3.5)//通过平均高度判断
                {
                    return true; // 非地面
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                ///Function.WriteErrorLog(" IsPackingCase函数失败：" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 通过固定范围求出车厢的边缘作为基准面
        /// </summary>
        static public double GetDatumPlane(List<MeasPoint> Points)
        {
            double minRange = -0.85;  //（此处需要调试:）
            double maxRange = 1.45;

            List<double> Listmins = new List<double>();
            for (int i = 0; i < Points.Count; i++)
            {
                if ((Points[i].X > minRange - 0.05 && Points[i].X < minRange + 0.05)//搜索范围（此处需要调试）
                    || (Points[i].X > maxRange - 0.05 && Points[i].X < maxRange + 0.05))
                {
                    if (Points[i].Y < 2.5)//达到边缘的范围才有效（此处需要调试）
                    {
                        Listmins.Add(Points[i].Y);
                    }
                }
            }
            if (Listmins.Count < 1)
            {
                return 2.5;
            }
            else
            {
                return Listmins.Min();//最小值为边缘基准
            }


        }
        #endregion

        #region 体积计算线程：计算车厢最终体积
        /// <summary>
        /// 体积计算线程函数
        /// </summary>
        /// <param name="obj"></param>
        private void ThreadCalculateVolome(object obj)
        {

            while (!bIsCalVolumFinish_MZ)
            {
                while (measCarVolList_MZ.Count > 0)  //接收到一个车厢的完整扫描线
                {
                    if (measLineList_MZZX.Count > 0 &&
                        measLineList_MZZX[measLineList_MZZX.Count - 1].SectionID > measCarVolList_MZ[0].endScanMatchIndex + 20)//纵断面链表中最后一天扫描线的id要大于第一个体积中横断面的id20个
                    {
                        try
                        {
                            MeasCar_Vol meascarvol; //临时存储变量
                            Monitor.Enter(measCarVolList_MZ);//Monitor.Enter(object)方法是获取锁，Monitor.Exit(object)方法是释放锁
                            meascarvol = measCarVolList_MZ[0];
                            measCarVolList_MZ.RemoveAt(0);//执行完一个车厢移除一个车厢
                            Monitor.Exit(measCarVolList_MZ);

                            //在list_ScanMatch找到第一个断面的位置
                            Monitor.Enter(measLineList_MZZX);
                            List<MeasLine> listemp = measLineList_MZZX.Where(s => s.SectionID == meascarvol.startScanMatchIndex).ToList();
                            MeasLine item = new MeasLine();
                            if (listemp.Count > 0) //
                            {
                                item = listemp[0];
                            }
                            //如果没有找到序号直接对应的就扩大搜索范围
                            else
                            {
                                for (int i = 0; i < 9; i++)
                                {
                                    List<MeasLine> listemp1 = measLineList_MZZX.Where(s => s.SectionID == meascarvol.startScanMatchIndex - 5 + i).ToList();
                                    if (listemp1.Count > 0)
                                    {
                                        item = listemp1[0];
                                        break;
                                    }
                                }
                            }
                            int n = measLineList_MZZX.IndexOf(item);
                            int lengthCount = 0;
                            if (meascarvol.t_measlinelst.Count + 12 > measLineList_MZZX.Count - n)
                            {
                                lengthCount = measLineList_MZZX.Count - n;
                            }
                            List<MeasLine> templines = measLineList_MZZX.GetRange(n, meascarvol.t_measlinelst.Count + 12);
                            meascarvol.v_measlinelst.AddRange(templines);  //根据横断面id取对应的纵断面，两头分别延伸
                            measLineList_MZZX.RemoveRange(0, n + meascarvol.t_measlinelst.Count);
                            Monitor.Exit(measLineList_MZZX);

                            //计算两个横断面间距：即用于体积计算的梯形计算公式中的高
                            ComputeSectionDist(ref meascarvol);//此处不利用vtk，自己编写代码

                            double Carslength = 0;
                            //if (IsTrainCarsMeasurement(ref meascarvol, ref Carslength)) //判断为车头还是车厢,车厢就不算体积了
                            {
                                CarID++;
                                //体积最终存储位置
                                meascarvol.GetVolome();//计算体积
                                meascarvol.carID = CarID; //表明是第几节车厢
                                meascarvol.carlength = Carslength;
                                measTrain_MZ.meascarvollst.Add(meascarvol); //体积最终存储

                                //更新界面
                                //if (bIsFull)//当前测量的是重车
                                //{
                                //    FormAutoMeasureNew.frmMeasure.BeginInvoke(FormAutoMeasureNew.outputfullMethod, meascarvol);
                                //}
                                //else //当前测量的是空车
                                //{
                                //    FormAutoMeasureNew.frmMeasure.BeginInvoke(FormAutoMeasureNew.outputemptMethod, meascarvol);
                                //}

                            }
                        }
                        // catch (Exception ex)
                        // {
                        //     ///Function.WriteErrorLog(" ThreadCalculateVolome函数失败：" + ex.Message);
                        //     richTextBox1.AppendText(ex.Message);
                        // }
                        finally
                        {
                            //为了避免获取锁之后因为异常，致锁无法释放，所以需要在try{} catch(){}之后的finally{}结构体中释放锁(Monitor.Exit())。
                            if (Monitor.IsEntered(measCarVolList_MZ))
                                Monitor.Exit(measCarVolList_MZ);
                            if (Monitor.IsEntered(measLineList_MZZX))
                                Monitor.Exit(measLineList_MZZX);
                        }

                    }
                    else
                    {

                        break;
                    }
                }

                Thread.Sleep(1000); //此处延迟1s是为了等待体积存储线程

                //以下未测试 2015.3.17 22:10
                if ((bIsSaveDataFinish_MZ && measCarVolList_MZ.Count == 0))//判断扫描结束bIsMeasureFinish（通常先输出）并且measCarVolList_MZ全部处理完后结束测量过程
                {
                    bIsCalVolumFinish_MZ = true;
                   // string vfilename = "E://volumns.txt";
                   // FileStream fv = new FileStream(vfilename, FileMode.OpenOrCreate);
                   // for (int i = 0; i < measTrain_MZ.meascarvollst.Count; i++)
                   // {
                   //     string vl = measTrain_MZ.meascarvollst[i].VolumeFrombase.ToString() + "\n";
                   //     fv.Write(Encoding.UTF8.GetBytes(vl), 0, Encoding.UTF8.GetBytes(vl).Length);
                   //     fv.Flush();
                   //
                   //     string filename = "E://Car" + i + "_pts.txt";
                   //     FileStream fs = new FileStream(filename, FileMode.OpenOrCreate);
                   //
                   //     for (int pr = 0; pr < measTrain_MZ.meascarvollst[i].t_measlinelst.Count; pr++)
                   //     {
                   //         string line = "";
                   //         line += pr.ToString() + "\t";
                   //         for (int pt = 0; pt < measTrain_MZ.meascarvollst[i].t_measlinelst[pr].points.Count; pt++)
                   //         {
                   //             line += measTrain_MZ.meascarvollst[i].t_measlinelst[pr].points[pt].X + "," + measTrain_MZ.meascarvollst[i].t_measlinelst[pr].points[pt].Y + ";";
                   //         }
                   //         line += "\n";
                   //         fs.Write(Encoding.UTF8.GetBytes(line), 0, Encoding.UTF8.GetBytes(line).Length);
                   //     }
                   //     fs.Flush();
                   //     fs.Close();
                   //
                   // }
                   // fv.Close();
                    FinishMeasure();
                }

            }

        }

        /// <summary>
        /// 计算两个横断面间距：即用于体积计算的梯形计算公式中的高
        /// </summary>
        private void ComputeSectionDist(ref MeasCar_Vol measureMent)
        {

            List<MeasLine> listLine2d = new List<MeasLine>();
            listLine2d.AddRange(measureMent.v_measlinelst);
            int startID = listLine2d[0].SectionID;
            int enddID = listLine2d[listLine2d.Count - 1].SectionID;

            List<ScanMatchDist> listDist = new List<ScanMatchDist>();
            #region 断面匹配
            while (listLine2d.Count >= 11)
            {
                double dist = 0;
                string sectionID = "";
                ScanMatchings(listLine2d[0], listLine2d[1], ref dist);
                #region < 0.02
                if (Math.Abs(dist) < 0.02)
                {
                    ScanMatchings(listLine2d[0], listLine2d[10], ref dist);
                    if (Math.Abs(dist) < 0.02)//火车处于静止状态(时速小于0.36km/h)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            ScanMatchDist scanMatchDist = new ScanMatchDist();
                            scanMatchDist.dist = 0;
                            scanMatchDist.sectionID = listLine2d[j].SectionID;
                            listDist.Add(scanMatchDist);

                        }
                        sectionID = listLine2d[0].SectionID + "_" + listLine2d[10].SectionID;
                    }
                    else //火车处于运动缓慢运动中，求取平均速度(取一段估计位移在0.2m左右的，进行平均)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            ScanMatchDist scanMatchDist = new ScanMatchDist();
                            scanMatchDist.dist = dist / 10;
                            scanMatchDist.sectionID = listLine2d[j].SectionID;
                            listDist.Add(scanMatchDist);

                        }
                        sectionID = listLine2d[0].SectionID + "_" + listLine2d[10].SectionID;
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        listLine2d.RemoveAt(0);//删除点集，防止内存占用过大
                    }
                }
                #endregion
                //下面均为根据首段距离取一段估计位移在0.2m左右的,然后取平均距离
                #region < 0.03
                else if (Math.Abs(dist) < 0.03)
                {
                    ScanMatchings(listLine2d[0], listLine2d[6], ref dist);
                    for (int j = 0; j < 6; j++)
                    {
                        ScanMatchDist scanMatchDist = new ScanMatchDist();
                        scanMatchDist.dist = dist / 6;
                        scanMatchDist.sectionID = listLine2d[j].SectionID;
                        listDist.Add(scanMatchDist);
                    }
                    sectionID = listLine2d[0].SectionID + "_" + listLine2d[6].SectionID;
                    for (int i = 0; i < 6; i++)
                    {
                        listLine2d.RemoveAt(0);//删除点集，防止内存占用过大
                    }

                }
                #endregion
                #region  < 0.04
                else if (Math.Abs(dist) < 0.04)
                {
                    ScanMatchings(listLine2d[0], listLine2d[5], ref dist);
                    for (int j = 0; j < 5; j++)
                    {
                        ScanMatchDist scanMatchDist = new ScanMatchDist();
                        scanMatchDist.dist = dist / 5;
                        scanMatchDist.sectionID = listLine2d[j].SectionID;
                        listDist.Add(scanMatchDist);
                        sectionID = listLine2d[0].SectionID + "_" + listLine2d[5].SectionID;
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        listLine2d.RemoveAt(0);//删除点集，防止内存占用过大
                    }
                }
                #endregion
                #region < 0.05
                else if (Math.Abs(dist) < 0.05)
                {
                    ScanMatchings(listLine2d[0], listLine2d[4], ref dist);
                    for (int j = 0; j < 4; j++)
                    {
                        ScanMatchDist scanMatchDist = new ScanMatchDist();
                        scanMatchDist.dist = dist / 4;
                        scanMatchDist.sectionID = listLine2d[j].SectionID;
                        listDist.Add(scanMatchDist);
                        sectionID = listLine2d[0].SectionID + "_" + listLine2d[4].SectionID;
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        listLine2d.RemoveAt(0);//删除点集，防止内存占用过大
                    }
                }
                #endregion
                #region < 0.06
                else if (Math.Abs(dist) < 0.06)
                {
                    ScanMatchings(listLine2d[0], listLine2d[3], ref dist);
                    for (int j = 0; j < 3; j++)
                    {
                        ScanMatchDist scanMatchDist = new ScanMatchDist();
                        scanMatchDist.dist = dist / 3;
                        scanMatchDist.sectionID = listLine2d[j].SectionID;
                        listDist.Add(scanMatchDist);
                        sectionID = listLine2d[0].SectionID + "_" + listLine2d[3].SectionID;
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        listLine2d.RemoveAt(0);//删除点集，防止内存占用过大
                    }
                }
                #endregion
                #region < 0.07
                else if (Math.Abs(dist) < 0.07)
                {
                    ScanMatchings(listLine2d[0], listLine2d[2], ref dist);
                    for (int j = 0; j < 2; j++)
                    {
                        ScanMatchDist scanMatchDist = new ScanMatchDist();
                        scanMatchDist.dist = dist / 2;
                        scanMatchDist.sectionID = listLine2d[j].SectionID;
                        listDist.Add(scanMatchDist);

                        sectionID = listLine2d[0].SectionID + "_" + listLine2d[2].SectionID;
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        listLine2d.RemoveAt(0);//删除点集，防止内存占用过大
                    }
                }
                #endregion
                #region 其他
                else
                {
                    ScanMatchDist scanMatchDist = new ScanMatchDist();
                    scanMatchDist.dist = dist;
                    scanMatchDist.sectionID = listLine2d[0].SectionID;
                    listDist.Add(scanMatchDist);
                    listLine2d.RemoveAt(0);
                    sectionID = listLine2d[0].SectionID + "_" + listLine2d[1].SectionID;
                }
                #endregion

            }
            #endregion
            #region 赋值给对应横断面
            if (measureMent.startScanMatchIndex >= listDist[0].sectionID)//
            {
                List<ScanMatchDist> listtmep = listDist.Where(s => s.sectionID == startID).ToList();
                int dex = listDist.IndexOf(listtmep[0]);
                measureMent.dist_Section.AddRange(listDist.GetRange(dex, measureMent.t_measlinelst.Count - 1));
            }
            else
            {
                for (int i = 0; i < measureMent.t_measlinelst.Count - 1; i++)
                {
                    int minus = listDist[0].sectionID - startID;
                    if (i < minus)
                    {
                        measureMent.dist_Section.Add(listDist[0]);
                    }
                    else
                    {
                        measureMent.dist_Section.Add(listDist[i - minus]);
                    }

                }
            }

            #endregion

        }

        /// <summary>
        /// 扫描线匹配：ICP算法匹配纵断面，得到断面距离
        /// </summary>
        private void ScanMatchings(MeasLine line_source, MeasLine line_target, ref double dist)
        {
            int sourceMidPiontCode = line_source.points.Count / 2;   //第一条线的中间点点号
            int matchPointNum = line_source.points.Count / 8;  //参与配准的点个数
            int marchPointStartCode = sourceMidPiontCode - matchPointNum;  //参与配准的点的起始点号
            int marchPointEndCode = sourceMidPiontCode + matchPointNum;    //参与配准的点的终止点号
            double sourceMidPiontX = line_source.points[sourceMidPiontCode].X;
            double sourceMidPiontY = line_source.points[sourceMidPiontCode].Y;

            try
            {

            }
            catch (Exception ex)
            {
                ///Function.WriteErrorLog("ScanMatchings失败：" + ex.Message);
            }


            vtkPolyData TargetPolydata = new vtkPolyData();
            vtkPolyData SourcePolydata = new vtkPolyData();
            vtkPolyData temp_s = vtkPolyData.New();
            vtkPoints sourcePoints = vtkPoints.New();
            vtkPolyData temp_t = vtkPolyData.New();
            vtkPoints targetPoints = vtkPoints.New();

            try
            {
                //给数据源赋值
                for (int i = Convert.ToInt32(line_source.points.Count * 0.1); i < Convert.ToInt32(line_source.points.Count * 0.95); i++)
                {
                    sourcePoints.InsertNextPoint(line_source.points[i].X, line_source.points[i].Y, 0);
                }
                temp_s.SetPoints(sourcePoints);
                vtkVertexGlyphFilter vertexFilter_s = vtkVertexGlyphFilter.New();
                vertexFilter_s.SetInput(temp_s);
                //vertexFilter_s.SetInputData(temp_s);
                vertexFilter_s.Update();
                SourcePolydata.ShallowCopy(vertexFilter_s.GetOutput());


                //给目标数据赋值
                for (int j = 0; j < line_target.points.Count; j++)
                {
                    targetPoints.InsertNextPoint(line_target.points[j].X, line_target.points[j].Y, 0);

                }
                temp_t.SetPoints(targetPoints);
                vtkVertexGlyphFilter vertexFilter_t = vtkVertexGlyphFilter.New();
                vertexFilter_t.SetInput(temp_t);
                //vertexFilter_t.SetInputData(temp_t);
                vertexFilter_t.Update();
                TargetPolydata.ShallowCopy(vertexFilter_t.GetOutput());

                //ICP 扫描线匹配
                vtkIterativeClosestPointTransform icp = vtkIterativeClosestPointTransform.New();
                icp.SetSource(SourcePolydata);
                icp.SetTarget(TargetPolydata);
                //icp.SetMaximumMeanDistance(0.008);
                // icp.SetMaximumNumberOfLandmarks(20);
                // icp.SetMeanDistanceModeToRMS();
                icp.GetLandmarkTransform().SetModeToRigidBody();
                icp.SetMaximumNumberOfIterations(50);
                //icp->StartByMatchingCentroidsOn();
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                icp.Modified();
                icp.Update();
                stopwatch.Stop();

                // Get the resulting transformation matrix (this matrix takes the source points to the target points)
                vtkMatrix4x4 m = icp.GetMatrix();
                double delt_x = m.GetElement(0, 3);
                double delt_y = m.GetElement(1, 3);
                if (Math.Abs(delt_y) < 0.01)
                {
                    dist = delt_x / Math.Abs(delt_x) * Math.Sqrt(delt_x * delt_x + delt_y * delt_y);
                }
                else
                {
                    dist = delt_x = m.GetElement(0, 3);
                }


                string strtmp = "";

                for (int n = 0; n < line_source.points.Count; n++)
                {
                    strtmp += line_source.points[n].X.ToString() + "\t" + line_source.points[n].Y.ToString() + "\r\n";
                }
            }
            catch (Exception ex)
            {
                ///Function.WriteErrorLog("ScanMatchings失败：" + ex.Message);
            }

        }

        /// <summary>
        /// 判断是否为车厢
        /// </summary>
        /// <param name="Measurement"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private bool IsTrainCarsMeasurement(ref MeasCar_Vol Measurement, ref double length)
        {
            //判断数量
            if (Measurement.dist_Section.Count < 100)
            {
                return false;
            }

            /*判断车厢长度是否满足*/
            double sumLength = 0;

            try
            {
                for (int i = 0; i < Measurement.dist_Section.Count; i++)
                {
                    sumLength += Measurement.dist_Section[i].dist;
                }
                length = Math.Abs(sumLength);

                /**************求平均速度,最大速度,平均速度*************/
                double maxSpeed = Math.Abs(Measurement.dist_Section[0].dist);
                double minSpeed = Math.Abs(Measurement.dist_Section[0].dist);

                for (int n = 0; n < Measurement.dist_Section.Count; n++)
                {
                    if (maxSpeed > Math.Abs(Measurement.dist_Section[n].dist))
                    {
                        maxSpeed = Math.Abs(Measurement.dist_Section[n].dist);
                    }
                    if (minSpeed < Math.Abs(Measurement.dist_Section[n].dist))
                    {
                        minSpeed = Math.Abs(Measurement.dist_Section[n].dist);
                    }
                }
                //double time = (Measurement.endScanMatchIndex - Measurement.startScanMatchIndex) / Convert.ToDouble(GlobalConfig.Frequency);
                TimeSpan ts = Measurement.EndTime - Measurement.startTime;
                double time = ts.TotalSeconds;
                Measurement.averageSpeed = length * 3.6 / time;
                Measurement.maxSpeed = maxSpeed * Convert.ToDouble(ConfigInfo_Auto.LMS_Frequency) * 3.6;
                Measurement.maxSpeed = minSpeed * Convert.ToDouble(ConfigInfo_Auto.LMS_Frequency) * 3.6;


                double fuhao = sumLength / Math.Abs(length);
                if (Math.Abs(sumLength) > 6 && !IsRailwayEngine(Measurement))//需要实测
                {
                    /**********************距离平差**************************/
                    double residual = 11.4 - Math.Abs(sumLength);
                    double average = residual / Measurement.dist_Section.Count;
                    for (int j = 0; j < Measurement.dist_Section.Count; j++)
                    {
                        if (fuhao > 0)
                        {
                            Measurement.dist_Section[j].dist = Measurement.dist_Section[j].dist + average;
                        }
                        else
                        {
                            Measurement.dist_Section[j].dist = Measurement.dist_Section[j].dist - average;
                        }

                    }


                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ///Function.WriteErrorLog("IsTrainCarsMeasurement失败：" + ex.Message);
                return false;
            }

        }

        /// <summary>
        /// (20210825)判断是否为车头
        /// </summary>
        /// <param name="measurment">待判断的一节车</param>
        /// <returns></returns>
        static public bool IsRailwayEngine(MeasCar_Vol measurment)
        {
            try
            {
                List<double> listDist = new List<double>();
                for (int i = 0; i < measurment.t_measlinelst.Count; i++)
                {
                    double sum = 0;
                    int num = 0;
                    for (int j = 0; j < measurment.t_measlinelst[i].points.Count; j++)//求每一条横断面的平均高度
                    {
                        if (measurment.t_measlinelst[i].points[j].X > -1.0 && measurment.t_measlinelst[i].points[j].X < 1.0)
                        {
                            sum += measurment.t_measlinelst[i].points[j].Y;
                            num++;
                        }
                    }
                    if (num > 0)
                    {
                        double average = sum / num;
                        listDist.Add(average);
                    }
                }

                double distyAverage = listDist.Average();//求所有断面平均高度的平均值

                if (distyAverage < 4.8)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ///Function.WriteErrorLog("IsRailwayEngine失败：" + ex.Message);
                return false;
            }
        }

        #endregion
        #endregion

        #region 相关函数
        //服务器端先初始化Socket，然后与端口绑定(bind)，对端口进行监听(listen)，调用accept阻塞，等待客户端连接。
        //在这时如果有个客户端初始化一个Socket，然后连接服务器(connect)，如果连接成功，这时客户端与服务器端的连接就建立了。
        //客户端发送数据请求，服务器端接收请求并处理请求，然后把回应数据发送给客户端，客户端读取数据，最后关闭连接，一次交互结束。
        /// <summary>
        /// 发送socket
        /// </summary>
        /// <param name="socketClient">socket客户端</param>
        /// <param name="strCommand">命令</param>
        public void Socket_Send(SocketClient socketClient, string strCommand)
        {
            byte[] bytCommand = new byte[strCommand.Length + 2];  //二进制命令
            bytCommand[0] = 0x02;
            bytCommand[strCommand.Length + 1] = 0x03;
            for (int i = 0; i < strCommand.Length; i++)
            {
                bytCommand[i + 1] = Convert.ToByte(strCommand[i]);  //命令转成二进制命令
            }
            socketClient.Send(bytCommand);  //发送命令.输出数据到Socket
        }

        /// <summary>
        /// 基于基准面计算有效断面的面积
        /// </summary>
        /// <param name="points">断面上的点集</param>
        /// <returns></returns>
        static public double CalculateSectionArea(List<MeasPoint> points)
        {
            try
            {
                double area = 0;


                for (int j = 0; j < points.Count; j++)
                {
                    if (points[j].Y > 5.6 + 0.9)//提出噪点？（此处需要调试）
                    {
                        points.RemoveAt(j);
                    }
                }

                for (int i = 0; i < points.Count - 1; i++)
                {
                    if (points[i].X > X_LeftLimit && points[i].X < X_RightLimt)//搜索范围（此处需要调试:边缘厚度分别是0.3、0.25？）
                    {
                        area += (2 * BaseHeight - points[i].Y - points[i + 1].Y) * Math.Abs(points[i + 1].X - points[i].X) / 2.0;
                    }
                }
                return area;
            }
            catch (Exception ex)
            {
                ///Function.WriteErrorLog("CalculateSectionArea失败：" + ex.Message);
                return 0;
            }
        }

        /// <summary>
        /// 计算一节车厢的体积
        /// </summary>
        /// <param name="s1">第一个断面面积</param>
        /// <param name="s2">第二个断面面积</param>
        /// <param name="interval">两个断面之间的距离</param>
        static public double CalculateVolume(double s1, double s2, double interval)
        {
            try
            {
                if (s1 < 0)
                    s1 = 0;
                if (s2 < 0)
                    s2 = 0;

                double Volume = 0;
                double delt_s = Math.Abs(s1 - s2);
                double areaDifference = 0;
                if (s1 >= s2)
                {
                    areaDifference = delt_s / s1;
                }
                else
                {
                    areaDifference = delt_s / s2;
                }
                if (areaDifference < 0.4)
                {
                    Volume = (s1 + s2) * interval / 2;
                }
                else
                {
                    Volume = (s1 + s2 + Math.Sqrt(s1 * s2)) * interval / 3;
                }
                return Volume;
            }
            catch (Exception ex)
            {
                ///Function.WriteErrorLog("CalculateVolume失败：" + ex.Message);
                return 0;
            }
        }
        #endregion

        #region 结果输出
        /// <summary>
        /// 在体积测量与计算完成后获取体积结果
        /// </summary>
        /// <returns>体积结果链表,若链表长度为0则测量取消或失败</returns>
        public List<MeasCar_Vol> GetResults()
        {
            DateTime begin = DateTime.Now;
            TimeSpan sp;
            List<MeasCar_Vol> CarVols = new List<MeasCar_Vol>();
            //先等待体积计算全部完成
            while (!bIsCalVolumFinish_MZ)
            {
                sp = DateTime.Now - begin;
                if (sp.TotalMinutes >= 3)
                    break;
            }
            //如果车厢变量里存有体积数据
            if (measTrain_MZ.meascarvollst.Count > 0)
            {
                CarVols.AddRange(measTrain_MZ.meascarvollst);
            }
            return (CarVols);

        }
        #endregion

        #region 提取质量
        /// <summary>
        /// 检查是否存在与当前时间差10分钟以内的质量测量结果
        /// </summary>
        /// <param name="filename">符合要求的文件名</param>
        /// <returns>有或没有</returns>
        bool SearchXMLZFile(ref string filename)
        {
            try
            {
                DirectoryInfo info = new DirectoryInfo("Z://");
                FileInfo[] fl = info.GetFiles();
                List<string> names = new List<string>();
                foreach (FileInfo f in fl)
                {
                    names.Add(f.Name.ToString());
                }
                string target = names.Max().Split('.')[0];
                int year = Convert.ToInt32(target.Substring(0, 4));
                int month = Convert.ToInt32(target.Substring(4, 2));
                int day = Convert.ToInt32(target.Substring(6, 2));
                int hour = Convert.ToInt32(target.Substring(8, 2));
                int minute = Convert.ToInt32(target.Substring(10, 2));
                int second = Convert.ToInt32(target.Substring(12, 2));
                DateTime dt = new DateTime(year, month, day, hour, minute, second);

                if ((DateTime.Now - dt).TotalMinutes < 10.0)
                {
                    filename = "Z://" + target + ".xml";
                    return (true);
                }
                else
                    return (false);
            }
            catch(Exception e)
            {
                Function.WriteErrorLog("SearchXMLZFile出错：" + e.Message);
                return (false);
            }

        }
        /// <summary>
        /// 解析质量数据
        /// </summary>
        /// <param name="filename">对应的xml文件名</param>
        /// <returns>重量数据链表</returns>
        bool GetWeights(string filename, ref List<double> weights, ref List<double> speed)
        {
            try
            {
                StreamReader sr = new StreamReader(filename);

                string line = "";
                int linepr = 32;
                int Nodenum = 0;
                string[] linebox;
                line = sr.ReadToEnd();
                linebox = line.Split('\n').ToArray();
                string node = linebox[17].Split('>')[1].Split('<')[0];
                Nodenum = Convert.ToInt32(node);
                int i = 0;
                for (; i < Nodenum; i++)
                {
                    node = linebox[linepr].Split('>')[1].Split('<')[0];
                    double weight = Convert.ToDouble(node);
                    weights.Add(weight);
                    speed.Add(Convert.ToDouble(linebox[linepr - 1].Split('>')[1].Split('<')[0]));
                    linepr += 24;
                }

                sr.Close();
                return (true);
            }
            catch (Exception e)
            {
                return (false);
            }
        }
        #endregion
    }
}

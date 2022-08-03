using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeMonitorApplication.FunctionClass
{
    #region DB_MengKuGradeMonitor数据库中的表结构
    /// <summary>
    /// DB_TrainMeasureResult表结构
    /// </summary>
    public class DB_TrainMeasureResult_DataStruct
    {
        //流水号
        public string sFID = "";

        //机车号
        public string sTrainName = "";

        //车箱数
        public int nTrainCount = 0;

        //体积
        public double dVolumn = 0;

        //皮重
        public double dPZ = 0;

        //毛重
        public double dMZ = 0;

        //净重
        public double dJZ = 0;

        //比重
        public double dBZ = 0;

        //品位
        public double dPW = 0;

        //货物来源
        public string sLY = "";

        //速度
        public double dSpeed = 0;

        //观测时间
        public DateTime dtMeasureTime = new DateTime();

        //完备
        public int intWB = 0;
    }

    #endregion

    #region 账号、密码、头像类
    public class DB_User_DataStruct
    {
        public string sUserName; //用户名
        public string sUserPW;
        public int bModifyPermission;  //是否有修改权限
        public string sUserPhoto;
    }
    #endregion

    /// <summary>
    /// 系统信息配置
    /// </summary>
    public class ConfigInfo_Auto
    {
        # region 扫描仪
        /// <summary>
        /// 频率
        /// </summary>
        public static int LMS_Frequency = 25;
        /// <summary>
        /// 角度分辨率
        /// </summary>
        public static double LMS_AngleResolution = 0.25f;
        public static string LMS_MZHX_IP = "10.1.8.88";//横   45  115
        public static string LMS_MZHX_PORT = "2111";
        public static int LMS_MZHX_StartAngle = 45;
        public static int LMS_MZHX_StopAngle = 115;
        public static string LMS_MZZX_IP = "10.1.8.111";//纵  30  150
        public static string LMS_MZZX_PORT = "2111";
        public static int LMS_MZZX_StartAngle = 55;
        public static int LMS_MZZX_StopAngle = 115;
        #endregion

        //继电器
        public static string JDQ_IP = "10.1.8.110";
        public static string JDQ_PORT = "6000";

        //相机
        //public static string CAM_IP = "10.1.8.250";
        //public static string CAM_PORT = "8091";

        //轨道衡
        public static string GDH_COM = "COM4";
        public static string CLCGQ_COM = "COM7";

        //数据库
        public static string DB_DataSource = "LAPTOP-2NA3IDMM";// WIN7-20140902QY\\SQLEXPRESS  01VFD2LQAYPVHSD
        public static string DB_InitialCatalog = "DB_MengKuGradeMonitor";
        public static string DB_UserID = "sa";
        public static string DB_Password = "123456";
    }

    /// <summary>
    /// 枚举：列车方向
    /// </summary>
    public enum TRAINDIRECTION
    {
        UNARRIVE,	///	车子未来
        EAST, /// 往东开（正向）
        WEST, /// 往西开（反向）
    }

    /// <summary>
    /// 点类：存储点的X、Y坐标值
    /// </summary>
    public class MeasPoint
    {
        public double X;
        public double Y;
        public MeasPoint(double _x, double _y)
        {
            X = _x;
            Y = _y;
        }
        public MeasPoint()
        {
            X = 0;
            Y = 0;
        }
    }

    /// <summary>
    /// 扫描线类：存储一条扫描线的点集、编号和时间
    /// </summary>
    public class MeasLine
    {
        /// <summary>
        /// 点集
        /// </summary>
        public List<MeasPoint> points = new List<MeasPoint>();
        /// <summary>
        /// 断面编号
        /// </summary>
        public int SectionID = 0;
        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime timeStamp = new DateTime();//时间戳
    }

    /// <summary>
    /// 断面间距离类：存储断面间距和编号
    /// </summary>
    public class ScanMatchDist
    {
        /// <summary>
        /// 断面间距离ID
        /// </summary>
        public int sectionID;
        /// <summary>
        /// 断面间距离
        /// </summary>
        public double dist;
    }

    /// <summary>
    /// 车厢计算重量类：存储计算一节车厢的重量的相关数据
    /// </summary>
    public class MeasCar_Mas
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime startTime = new DateTime();
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime = new DateTime();
        /// <summary>
        /// 存储称重读数
        /// </summary>
        public List<double> Listweight = new List<double>();
        /// <summary>
        /// 质量计算结果
        /// </summary>
        public double resultWeight = 0;
        /// <summary>
        /// 车厢照片存储地址
        /// </summary>
        public string picPath = "";
        /// <summary>
        /// 重量个数
        /// </summary>
        public int weightNum = 0;
        /// <summary>
        /// 数据中误差
        /// </summary>
        public double RMS = 0;

        /// <summary>
        /// 求中位数
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public double BubbleSort(double[] array)
        {
            double Median = 0;
            int length = array.Length;
            for (int i = 0; i <= length - 2; i++)
            {
                for (int j = length - 1; j >= 1; j--)
                {
                    //将两个元素进行交换
                    if (array[j] < array[j - 1])
                    {
                        double temp = array[j];
                        array[j] = array[j - 1];
                        array[j - 1] = temp;
                    }
                }
            }
            if (length % 2 == 0)
            {
                Median = (array[length / 2] + array[length / 2 - 1]) / 2;
            }
            else
            {
                Median = array[length / 2];
            }
            return Median;
        }
        /// <summary>
        /// 计算车厢质量
        /// </summary>
        /// <returns></returns>
        public MASSRES GetWeight()
        {
            //配置参数？
            if (Listweight.Max() - Listweight.Min() > 50)
            {
                ///Function.WriteErrorLog("重量数据不合格：加速度太大,可能是车头！");
                return MASSRES.MASSRES_ERROR_ACCELERATION_TOO_LARGE;
            }
            //配置参数？
            //if (Listweight.Count < 10)
            //{
            //    Function.WriteErrorLog("重量数据不合格：个数太少");
            //    return MASSRES.MASSRES_ERROR_SPEED_TOO_FAST;
            //}
            /***********计算中误差*********************/
            double average = Listweight.Average();
            double sum = 0;

            for (int i = 0; i < Listweight.Count; i++)
            {
                sum += (Listweight[i] - average) * (Listweight[i] - average);
            }

            RMS = Math.Sqrt(sum / (Listweight.Count - 1));
            weightNum = Listweight.Count;
            resultWeight = BubbleSort(Listweight.ToArray());
            return MASSRES.MASSRES_SUCCESS;
        }

    }

    /// <summary>
    /// 车厢计算体积类：存储计算一节车厢的体积的相关数据
    /// </summary>
    public class MeasCar_Vol
    {
        /// <summary>
        /// 车厢ID
        /// </summary>
        public string carrageID;
        /// <summary>
        /// 车厢长度
        /// </summary>
        public double carlength = 0;
        /// <summary>
        /// 过程开始时间
        /// </summary>
        public DateTime startTime = new DateTime();
        /// <summary>
        /// 过程结束时间
        /// </summary>
        public DateTime EndTime = new DateTime();
        /// <summary>
        /// 车厢扫描开始的断面ID
        /// </summary>
        public int startScanMatchIndex = 0;
        /// <summary>
        /// 车厢扫描结束断面ID
        /// </summary>
        public int endScanMatchIndex = 0;
        /// <summary>
        /// 体积测量过程ID
        /// </summary>
        public string ID = "";
        /// <summary>
        /// 存储车厢的扫描线的面积
        /// </summary>
        public List<double> linesArea = new List<double>();
        /// <summary>
        /// 两个断面之间的距离
        /// </summary>
        public List<ScanMatchDist> dist_Section = new List<ScanMatchDist>();
        /// <summary>d
        /// 测量的与基准面的体积
        /// </summary>
        public double VolumeFrombase = 0;
        /// <summary>
        /// 横断面线
        /// </summary>
        public List<MeasLine> t_measlinelst = new List<MeasLine>();
        /// <summary>
        /// 纵断面线
        /// </summary>
        public List<MeasLine> v_measlinelst = new List<MeasLine>();
        /// <summary>
        /// 测量基准面
        /// </summary>
        private double datum;
        /// <summary>
        /// 最大速度
        /// </summary>
        public double maxSpeed = 0;
        /// <summary>
        /// 平均速度
        /// </summary>
        public double averageSpeed = 0;
        /// <summary>
        /// 最小速度
        /// </summary>
        public double minSpeed = 0;
        /// <summary>
        /// 最小速度
        /// </summary>
        public int carID = 0;
        /// <summary>
        /// 计算测量过程的平均基准面
        /// </summary>
        public void GetMeanDatum()
        {
            List<double> listdatum = new List<double>();  //存储车厢的每条断面的车厢边缘的Y值
            for (int i = 0; i < t_measlinelst.Count; i++)
            {
                listdatum.Add(MeasureModule.GetDatumPlane(t_measlinelst[i].points));
            }
            datum = listdatum.Average() + 1;
        }

        /// <summary>
        /// 计算体积
        /// </summary>
        public void GetVolome()
        {
            GetMeanDatum();
            /**********计算横断面面积******************/
            for (int i = 0; i < t_measlinelst.Count; i++)
            {
                //剔除前后车厢边缘异常点（起始和终止各三条）（此处需要调试：为什么是前后各三条）
                if (i == 0 || i == 1 || i == 2 || i == (t_measlinelst.Count - 1)
                    || i == (t_measlinelst.Count - 2) || i == (t_measlinelst.Count - 3))
                {
                    for (int j = 0; j < t_measlinelst[i].points.Count; j++)
                    {
                        t_measlinelst[i].points[j].Y = datum - 1;
                    }
                }
                this.linesArea.Add(MeasureModule.CalculateSectionArea(t_measlinelst[i].points));
            }

            /**********计算体积******************/
            double volome = 0;
            for (int i = 0; i < dist_Section.Count; i++)
            {
                //按纵向扫描测算距离
                //volome += MeasureModule.CalculateVolume(linesArea[i], linesArea[i + 1], dist_Section[i].dist);
                //按平均距离
                volome += MeasureModule.CalculateVolume(linesArea[i], linesArea[i + 1], 8.0/(dist_Section.Count));

            }
            VolumeFrombase = Math.Abs(volome);
        }
    }

    /// <summary>
    /// 车厢基础信息类：存储一节车厢的基础信息
    /// </summary>
    public class MeasCarInfo
    {
        /// <summary>
        /// 体积
        /// </summary>
        public double dVolume = 0;
        /// <summary>
        /// 重量
        /// </summary>
        public double dWeight = 0;
        /// <summary>
        /// 速度
        /// </summary>
        public double dSpeed = 0;
        /// <summary>
        /// 开始测量时间
        /// </summary>
        public DateTime startTime = new DateTime();
        /// <summary>
        /// 结束测量时间
        /// </summary>
        public DateTime endTime = new DateTime();
        /// <summary>
        /// 车厢照片存储地址
        /// </summary>
        public string picPath = "";

    }

    /// <summary>
    /// 列车基础信息类：存储一列车的基础信息
    /// </summary>
    public class MeasTrainInfo
    {
        /// <summary>
        /// 测量方向：true 正向，false 反向
        /// </summary>
        public bool bMeasDirection = true;
        /// <summary>
        /// 车厢数
        /// </summary>
        public int ncarscount = 0;
        /// <summary>
        /// 每节车厢的信息
        /// </summary>
        public List<MeasCarInfo> meascarinfolst = new List<MeasCarInfo>();

        public bool bIsQualified = true;
        /// <summary>
        /// 数据反向
        /// </summary>
        public void Reverse()
        {
            meascarinfolst.Reverse();
            bMeasDirection = !bMeasDirection;
        }

        public void Clear()
        {
            ncarscount = 0;
            meascarinfolst.Clear();
        }

    }

    /// <summary>
    /// 列车测量数据类：存储一列车的测量相关数据
    /// </summary>
    public class MeasTrain
    {
        /// <summary>
        /// 所有车厢的体积列表
        /// </summary>
        public List<MeasCar_Vol> meascarvollst = new List<MeasCar_Vol>(); //只是存独立的体积数据，不管是不是和重量匹配
        /// <summary>
        /// 所有车厢的重量列表
        /// </summary>
        public List<MeasCar_Mas> meascarmaslst = new List<MeasCar_Mas>();//只是存独立的重量数据，不管是不是和体积匹配

        /// <summary>
        /// 测量方向
        /// </summary>
        public bool bMeasDirection = true; //车头进true，车尾进false
        /// <summary>
        /// 车厢数量           车厢个数应该由体积测量数据确定！！
        /// </summary>
        public int ncarcount = 0;
        /// <summary>
        /// 输出测量信息
        /// <summary>
        public MeasTrainInfo OutPutMeasInfo()//整理数据！！整理完的数据按测量顺序排列，体积重量匹配
        {
            MeasTrainInfo meastraininfo = new MeasTrainInfo();

            meastraininfo.bMeasDirection = bMeasDirection;

            meastraininfo.ncarscount = meascarvollst.Count;//体积个数为准

            for (int i = 0; i < meascarvollst.Count; i++)
            {
                MeasCarInfo meascarinfo = new MeasCarInfo(); //整理每一节数据

                meascarinfo.dVolume = meascarvollst[i].VolumeFrombase;

                meascarinfo.startTime = meascarvollst[i].startTime;

                meascarinfo.endTime = meascarvollst[i].EndTime;

                meascarinfo.dSpeed = meascarvollst[i].averageSpeed;


                if (meascarmaslst.Count < meascarvollst.Count)//重量数据少于体积数据
                {
                    meascarinfo.dWeight = 0; //表示体积错误
                    ///Function.WriteErrorLog("OutPutMeasInfo 函数中重量数据个数小于体积数据个数");
                }
                else
                {
                    if (bMeasDirection)//头进
                    {

                        meascarinfo.dWeight = meascarmaslst[meascarmaslst.Count - meascarvollst.Count + i].resultWeight;

                        meascarinfo.picPath = meascarmaslst[meascarmaslst.Count - meascarvollst.Count + i].picPath; //取照片


                    }
                    else //尾进
                    {

                        meascarinfo.dWeight = meascarmaslst[i].resultWeight;

                        meascarinfo.picPath = meascarmaslst[i].picPath; //取照片

                    }

                    meastraininfo.meascarinfolst.Add(meascarinfo);
                }

            }

            return meastraininfo;
        }
    }

    /// <summary>
    /// 定义网络开关状态信息
    /// </summary>
    public enum MASSRES
    {
        MASSRES_SUCCESS,					///	操作成功
        MASSRES_ERROR_ACCELERATION_TOO_LARGE, /// 火车加速度过大
        MASSRES_ERROR_SPEED_TOO_FAST,       /// 连接网络继电器失败
    }
}

using GradeMonitorApplication.ConfigClass;
using GradeMonitorApplication.FunctionClass;
using GradeMonitorApplication.WinForm;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms.DataVisualization.Charting;

namespace GradeMonitorApplication
{
    public partial class MainForm : Form
    {

        SoftReg softReg = new SoftReg();//软件注册对象
        public static DataBase database = new DataBase();//数据库对象
        MeasureModule measureModule;
        Thread AutoMeasure;
        public static RFIDModule rfid = new RFIDModule();
        public static IPConfigInfo ipinfo = new IPConfigInfo();
        /// <summary>
        /// 当前折线图加载数据的范围
        /// </summary>
        enum SearchType
        {   /// <summary>
            /// 当天
            /// </summary>
            Today,
            /// <summary>
            /// 最近一个有数据的自然日
            /// </summary>
            Day,
            /// <summary>
            /// 本周
            /// </summary>
            Week,
            /// <summary>
            /// 本月
            /// </summary>
            Month,
            /// <summary>
            /// 本年度
            /// </summary>
            Year
        };
        SearchType type;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainForm()
        {
            //初始化数据库
            database.InitDB(DataBase_ConfigInfo.DB_DataSource, DataBase_ConfigInfo.DB_InitialCatalog, DataBase_ConfigInfo.DB_UserID, DataBase_ConfigInfo.DB_Password);
            //初始化折线图加载数据的范围
            type = SearchType.Day;
            Control.CheckForIllegalCrossThreadCalls = false;
            
            #region 注册与登录
            //判断软件是否注册
            bool isnotretkey = true;
            //打开/创建wxf注册表。如果已存在则表示打开，如果不存在则表示创建。
            RegistryKey retkey = Registry.CurrentUser.OpenSubKey("SOFTWARE", true).CreateSubKey("wxf").CreateSubKey("wxf.INI");
            foreach (string strRNum in retkey.GetSubKeyNames())
            {
                if (strRNum == softReg.GetRNum())//如果已经注册
                {
                    try
                    {
                        //弹出登录界面
                        Login_Form login_Form = new Login_Form();
                        login_Form.ShowDialog();
                        if (login_Form.DialogResult == DialogResult.No)
                        {
                            System.Environment.Exit(System.Environment.ExitCode);
                            this.Dispose();
                            this.Close();
                            return;
                        }
                        else if (login_Form.DialogResult == DialogResult.OK)
                        {
                            isnotretkey = false;
                            break;
                        }
                    }
                    catch
                    {
                        //然后将错误写进报错日志
                        Function.WriteErrorLog("登录界面Login_Form加载出错!");
                        MessageBox.Show("请联系系统维护人员!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
            }
            if (isnotretkey)
            {
                Int32 tLong;
                try
                {
                    tLong = (Int32)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Angel", "UseTimes", 0);//检索与指定的名称和检索选项关联的值。 如果未找到名称，则返回你提供的默认值。
                    MessageBox.Show("您现在使用的是未注册版版，可以试用30次，您已经使用了" + tLong + "次！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Angel", "UseTimes", 0, RegistryValueKind.DWord);
                }
                tLong = (Int32)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Angel", "UseTimes", 0);
                if (tLong < 100)
                {
                    int tTimes = tLong + 1;
                    Registry.SetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Angel", "UseTimes", tTimes);

                    try
                    {
                        //登录连接数据库，弹出登录界面
                        Login_Form frmlog = new Login_Form();
                        frmlog.ShowDialog();
                        if (frmlog.DialogResult == DialogResult.No)
                        {
                            System.Environment.Exit(System.Environment.ExitCode);
                            this.Dispose();
                            this.Close();
                        }
                    }
                    catch
                    {
                        Function.WriteErrorLog("登录界面出错");
                        MessageBox.Show("请联系系统维护人员!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                else
                {
                    DialogResult result = MessageBox.Show("试用次数已到！您是否需要注册？", "信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        Register_Form.state = false;
                        Register_Form registerForm = new Register_Form();
                        registerForm.ShowDialog();
                        Application.Exit();
                    }
                    else
                    {
                        Application.Exit();
                    }
                }
            }
            #endregion
            

            InitializeComponent();
        }

        #region 窗口加载及布局
        /// <summary>
        /// 窗口加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private  void MainForm_Load(object sender, EventArgs e)
        {
            #region 外观初始化
            #region 头像初始化
            label_Name.TextAlign = ContentAlignment.MiddleCenter;
            //名字头像

            Function.SaveNameImage(Login_Form.dB_User_DataStruct.sUserName, Application.StartupPath + "\\UI\\" + Login_Form.dB_User_DataStruct.sUserName + ".jpg", 66, 66);
            this.pictureBox_Name.Load(Application.StartupPath + "\\UI\\" + Login_Form.dB_User_DataStruct.sUserName + ".jpg");
            Image image = this.pictureBox_Name.Image;
            Image newImage = Function.CutEllipse(image, new Rectangle(0, 0, 66, 66), new Size(60, 60));
            this.pictureBox_Name.Image = newImage;
            
            //名字
            if (Login_Form.dB_User_DataStruct.sUserName.Length > 2)
                label_Name.Text = Login_Form.dB_User_DataStruct.sUserName.Substring(0, 2);
            else
                label_Name.Text = Login_Form.dB_User_DataStruct.sUserName;
            #endregion

            #region 表格样式初始化
            //设置表格样式
            listView1.BackgroundImage = Image.FromFile(".\\UI\\表格背景图.png");
            listView1.ForeColor = Color.White;

            //设置listView的显示属性
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.View = View.Details;
            listView1.Scrollable = true;
            listView1.MultiSelect = false;
            listView1.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, 30);
            listView1.SmallImageList = imgList;

            // 针对数据库的字段名称，建立与之适应显示表头
            listView1.Columns.Add("留白", 0, HorizontalAlignment.Center);
            listView1.Columns.Add("流水号", 130, HorizontalAlignment.Center);
            listView1.Columns.Add("列车名", 130, HorizontalAlignment.Center);
            listView1.Columns.Add("车厢数量", 130, HorizontalAlignment.Center);
            listView1.Columns.Add("体积", 130, HorizontalAlignment.Center);
            listView1.Columns.Add("净重", 130, HorizontalAlignment.Center);
            listView1.Columns.Add("品位值", 130, HorizontalAlignment.Center);
            listView1.Columns.Add("测量时间", 130, HorizontalAlignment.Center);
            #endregion
            #endregion

            //初始化界面
            InitializeForm();

            //SDK初始化
            CHCNetSDK.NET_DVR_Init();
            ConnectCAM();

            //自动启动
            //button_Start.PerformClick();

            //设置定时刷新器——刷新界面内容
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 2000;//每60s自动触发一次
            timer.Tick += new EventHandler(AutoRefresh);
            timer.Start();
        }

        /// <summary>
        /// 初始化界面
        /// </summary>
        private void InitializeForm()
        {
            #region 数据、内容初始化
            #region 1、数据表格内容初始化
            //清空表头与信息
            listView1.Items.Clear();

            //最近10次测量的一列车的数据表
            List<string> trainFIDs = new List<string>();
            List<string> trainNames = new List<string>();
            List<string> trainCounts = new List<string>();
            List<double> trainVolumns = new List<double>();
            List<double> trainJZs = new List<double>();
            List<double> trainPWs = new List<double>();
            List<string> trainMeasureTimes = new List<string>();

            try
            {
                MainForm.database.SearchforList(ref trainFIDs, ref trainNames, ref trainCounts, ref trainVolumns, ref trainJZs, ref trainPWs, ref trainMeasureTimes);
            }
            catch (Exception e)
            {
                Function.WriteErrorLog("SearchforList链接数据库出错：" + e.Message);
                return;
            }

            for (int row = 0; row < trainFIDs.Count; row++)
            {
                ListViewItem item = new ListViewItem();
                item.SubItems.Clear();
                item.SubItems[0].Text = (row + 1).ToString();
                item.SubItems.Add(trainFIDs[row]);
                item.SubItems.Add(trainNames[row]);
                item.SubItems.Add(trainCounts[row]);
                item.SubItems.Add(trainVolumns[row].ToString());
                item.SubItems.Add(trainJZs[row].ToString());
                item.SubItems.Add(trainPWs[row].ToString());
                item.SubItems.Add(trainMeasureTimes[row].ToString());
                listView1.Items.Add(item);
            }
            #endregion

            #region 2、折线图内容初始化
            chart1.Series.Clear();

            //设置折线点样式
            chart1.Series.Add("");
            chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;
            chart1.Series[0].IsVisibleInLegend = false;  //不显示图例
            chart1.Series[0].MarkerSize = 15;  //折线点的大小
            chart1.Series[0].MarkerStyle = MarkerStyle.Circle; //设置点的形状
            chart1.Series[0].Color = Color.Aqua;   //折线点的颜色

            //设置标签样式
            chart1.Series[0].LabelForeColor = Color.White;
            chart1.Series[0].ToolTip = "品位值：#VALY\n时间：#VALX";

            //设置折线样式
            chart1.Series.Add("");
            chart1.Series[1].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series[1].IsVisibleInLegend = false;
            chart1.Series[1].Color = Color.Aqua;

            //设置坐标网格样式
            chart1.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.White;  //设置x轴数值为白色
            chart1.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.White;
            Color fo = Color.FromArgb(150, 100, 100, 100);
            chart1.ChartAreas[0].AxisY.MajorGrid.LineColor = fo;   //设置网格线颜色
            chart1.ChartAreas[0].AxisX.MajorGrid.LineColor = fo;
            chart1.ChartAreas[0].AxisX.LineColor = fo;
            chart1.ChartAreas[0].AxisY.LineColor = fo;
            chart1.ChartAreas[0].BackColor = Color.Transparent;  //设置背景颜色为透明

            //品位变化折线图（默认最近一天测量结果）
            if (type == SearchType.Day || type == SearchType.Today)  //若按天显示，则显示x轴数值，并在折点上显示品位值
            {
                chart1.Series[0].IsValueShownAsLabel = true;  //折线点的品位值显示
                chart1.ChartAreas[0].AxisX.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
                label_TimeTip.Text = "";
            }
            else   //若不按天显示，则不显示x轴数值，也不在折点上显示品位值。同时标签显示年、月、周。
            {
                chart1.Series[0].IsValueShownAsLabel = false;
                //chart1.ChartAreas[0].AxisX.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.False;  //不显示x轴数值

                label_TimeTip.Text = DateTime.Now.Year.ToString() + "年";
                if (type == SearchType.Week)
                {
                    DateTime yearFirstDay = new DateTime(DateTime.Now.Year, 1, 1); //当年的1月1日
                    int yearFirstDayWeek = Convert.ToInt32(yearFirstDay.DayOfWeek) - 1;
                    int week = (DateTime.Now.DayOfYear - (6 - yearFirstDayWeek)) / 7 + 2; 
                    label_TimeTip.Text += " 第" + week.ToString() + "周";
                }
                else if (type == SearchType.Month)
                {
                    label_TimeTip.Text += " " + DateTime.Now.Month.ToString() + "月";
                }
            }

            List<double> Y = new List<double>();
            List<string> X = new List<string>();
            if (!MainForm.database.SearchforChart(DateTime.Now, ref X, ref Y, Convert.ToInt32(type)))
            {
                chart1.Series[0].MarkerSize = 0;
                chart1.Series[0].IsValueShownAsLabel = false;
                chart1.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.Transparent; //x轴数值为透明色
            }

            if (type == SearchType.Day || type == SearchType.Today)
                for (int i = 0; i < X.Count; i++)
                {
                    chart1.Series[0].Points.AddXY(X[i].Split(' ')[1], Y[i]);//x值显示“小时：分钟：秒”
                    chart1.Series[1].Points.AddXY(X[i].Split(' ')[1], Y[i]);
                    //chart1.Series[0].Points.AddXY(X[i].Split(' ')[1].Split(':')[0]+':'+ X[i].Split(' ')[1].Split(':')[1], Y[i]);//x值显示“小时：分钟”
                    //chart1.Series[1].Points.AddXY(X[i].Split(' ')[1].Split(':')[0] + ':' + X[i].Split(' ')[1].Split(':')[1], Y[i]);
                }
            else
            {
                chart1.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.Transparent; //x轴数值为透明色
                for (int i = 0; i < X.Count; i++)
                {
                    chart1.Series[0].Points.AddXY(X[i].Split(' ')[1], Y[i]);
                    chart1.Series[1].Points.AddXY(X[i].Split(' ')[1], Y[i]);
                }
            }
            #endregion
            #endregion
        }

        /// <summary>
        /// 主界面布局
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            pictureBox_FullPicture.Location = new Point(110, 80);//左上角
            pictureBox_FullPicture.Height = Convert.ToInt32(pictureBox_WorkArea.Height * 0.49);
            pictureBox_FullPicture.Width = Convert.ToInt32(pictureBox_WorkArea.Width * 0.4);

            pictureBox1.Location = new Point(110, Convert.ToInt32(pictureBox_WorkArea.Height * 0.5) + 85);
            pictureBox1.Width = Convert.ToInt32(pictureBox_WorkArea.Width * 0.4);
            pictureBox1.Height = Convert.ToInt32(pictureBox_WorkArea.Height * 0.49);

            listView1.Location = new Point(120 + pictureBox_FullPicture.Width, 80);
            listView1.Width = Convert.ToInt32(pictureBox_WorkArea.Width * 0.6 - 20);
            listView1.Height = Convert.ToInt32(pictureBox_WorkArea.Height * 0.49);

            chart1.Location = new Point(120 + pictureBox1.Width, pictureBox1.Location.Y + 5);
            chart1.Width = Convert.ToInt32(pictureBox_WorkArea.Width * 0.6) - 110;
            chart1.Height = Convert.ToInt32(pictureBox_WorkArea.Height * 0.49);

            buttonRefresh.Location = new Point(chart1.Location.X + chart1.Width, chart1.Location.Y + 10);
            button_Day.Location = new Point(chart1.Location.X + chart1.Width, chart1.Location.Y + 60);
            button_Week.Location = new Point(chart1.Location.X + chart1.Width, chart1.Location.Y + 110);
            button_Month.Location = new Point(chart1.Location.X + chart1.Width, chart1.Location.Y + 160);
            button_Year.Location = new Point(chart1.Location.X + chart1.Width, chart1.Location.Y + 210);

            label_TimeTip.Location = new Point(chart1.Location.X + chart1.Width - 100, chart1.Location.Y + chart1.Height - 30);
            label_TimeTip.ForeColor = Color.White;
        }

        /// <summary>
        /// 关闭窗口提示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("确定关闭窗口，停止系统运行？", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 关闭窗口事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            MessageBox.Show("窗口已关闭，系统已停止运行");
        }
        #endregion

        #region 菜单栏
        /// <summary>
        /// 报表管理按钮单击事件（同步pictureBox_ReportManagement_Click）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_ReportManagement_Click(object sender, EventArgs e)
        {
            ReportManagement_Form reportManagement_Form = new ReportManagement_Form();
            reportManagement_Form.ShowDialog();
        }
        /// <summary>
        /// 报表管理按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox_ReportManagement_Click(object sender, EventArgs e)
        {
            ReportManagement_Form reportManagement_Form = new ReportManagement_Form();
            reportManagement_Form.ShowDialog();
        }

        /// <summary>
        /// 数据管理按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_DataManagement_Click(object sender, EventArgs e)
        {
            DataManage_Form dataManage_Form = new DataManage_Form();
            dataManage_Form.ShowDialog();
        }

        /// <summary>
        /// 配置管理按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_ConfigManagement_Click(object sender, EventArgs e)
        {
            Config_Form config_Form = new Config_Form();
            config_Form.ShowDialog();
        }

        /// <summary>
        /// 用户管理按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_UserManagement_Click(object sender, EventArgs e)
        {
            if(Login_Form.dB_User_DataStruct.bModifyPermission==1)
            {
                UserManage_Form userManage_Form = new UserManage_Form();
                userManage_Form.ShowDialog();
            }
            else
            {
                MessageBox.Show("当前用户无权限使用此功能！", "提示");
            }
        }
        #endregion

        #region 功能栏
        /// <summary>
        /// 启动按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Start_Click(object sender, EventArgs e)
        {
            ////连接车轮传感器
            //CLCGQModule cl = new CLCGQModule();
            //cl.Connect();
            ////连接RFID
            //RFIDModule rfid = new RFIDModule();
            //rfid.Connect();
            ////监控RFID信号，控制设备启动和暂停。
            //rfid.StartMonitor();
            ////监控到信号
            //Thread.Sleep(50);
            //while (true)
            //{
            //    if (cl.TrainIsCome)
            //    {
                    //开始扫描后首先重置标志变量避免重复开启
                    //cl.TrainIsCome = false;

                    button_Start.BackgroundImage = Image.FromFile(".\\UI\\正在运行128.png");

                    measureModule = new MeasureModule(button_Start);  //初始化自动测量模块
                    bool bRet = measureModule.ConnectAllDevice();  //连接所有设备
                    measureModule.StartMeasure();  //开始测量
                    //Thread.Sleep(2000);
                    //button_Start.BackgroundImage = Image.FromFile(".\\UI\\启动128.png");
            ////等待列车通过
            //Thread.Sleep(8000);
            ////停止监视器并重置变量
            //rfid.StopMonitor();
            ////重新启动monitor监视器
            //rfid.StartMonitor();

            //    }
            //}
        }

        /// <summary>
        /// 暂停按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Stop_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 通知按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Warning_Click(object sender, EventArgs e)
        {
            ErrorReport_Form erp = new ErrorReport_Form();
            erp.ShowDialog();
        }

        /// <summary>
        /// 用户按钮单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_User_Click(object sender, EventArgs e)
        {
            User_Form user_Form=new User_Form();
            user_Form.ShowDialog();
        }
        #endregion

        #region 数据展示功能栏
        /// <summary>
        /// 自动刷新界面函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoRefresh(object sender, EventArgs e)
        {
            //以重新加载的方式刷新
            InitializeForm();
        }

        /// <summary>
        /// 刷新界面按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRresh_Click(object sender, EventArgs e)
        {

            type = SearchType.Day;
            //以重新加载的方式刷新
            InitializeForm();

        }

        /// <summary>
        /// 当日数据图形显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Day_Click(object sender, EventArgs e)
        {

            type = SearchType.Today;
            InitializeForm();
        }

        /// <summary>
        /// 当周数据图形显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Week_Click(object sender, EventArgs e)
        {

            type = SearchType.Week;
            InitializeForm();
        }

        /// <summary>
        /// 当月数据图形显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Month_Click(object sender, EventArgs e)
        {
            type = SearchType.Month;
            InitializeForm();
        }

        /// <summary>
        /// 当年数据图形显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            type = SearchType.Year;
            InitializeForm();
        }
        #endregion

        #region 监控视频播放模块
        /// <summary>
        /// 重车摄像头登录id
        /// 由摄像头返回,登陆失败返回为-1
        /// </summary>
        private Int32 m_lUserIDMZ = -1;
        /// <summary>
        /// 重车视频流通道号
        /// </summary>
        private byte m_DChanMZ;
        /// <summary>
        /// 重车播放推流状态码
        /// -1为初始值.表示未设置推流方式并播放
        /// </summary>
        private Int32 m_lRealHandleMZ = -1;

        /// <summary>
        /// 空车摄像头登录id
        /// 由摄像头返回,登陆失败返回为-1
        /// </summary>
        private Int32 m_lUserIDPZ = -1;
        /// <summary>
        /// 空车视频流通道号
        /// </summary>
        private byte m_DChanPZ;
        /// <summary>
        /// 空车播放推流状态码
        /// -1为初始值.表示未设置推流方式并播放
        /// </summary>
        private Int32 m_lRealHandlePZ = -1;

        /// <summary>
        /// 链接到相机
        /// </summary>
        /// <returns></returns>
        private bool ConnectCAM()
        {
            try
            {
                if (CameraLoginMZ())

                    PlayCameraMZ();
                if(CameraLoginPZ())
                    PlayCameraPZ();

                return true;
            }
            catch (Exception socketEx)
            {
                MessageBox.Show("摄像头连接失败！");
                return false;
            }
        }

        /// <summary>
        /// 重车摄像头登录
        /// </summary>
        private bool CameraLoginMZ()
        {

            if (m_lUserIDMZ < 0)
            {
                //string DVRIPAddress = "192.168.110.152"; //设备IP地址或者域名
                string DVRIPAddress = ipinfo.CameraMZ;
                int DVRPortNumber = 8000;//设备服务端口号
                string DVRUserName = ipinfo.OtherValues["CameraUser"];//设备登录用户名
                string DVRPassword = ipinfo.OtherValues["CameraPW"];//设备登录密码

                CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();

                //登录设备 Login the device
                m_lUserIDMZ = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref DeviceInfo);
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
        /// 空车摄像头登录
        /// </summary>
        private bool CameraLoginPZ()
        {

            if (m_lUserIDPZ < 0)
            {
                //string DVRIPAddress = "192.168.110.151"; //设备IP地址或者域名
                string DVRIPAddress = ipinfo.CameraPZ;
                int DVRPortNumber = 8000;//设备服务端口号
                string DVRUserName = ipinfo.OtherValues["CameraUser"];//设备登录用户名
                string DVRPassword = ipinfo.OtherValues["CameraPW"];//设备登录密码

                CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();

                //登录设备 Login the device
                m_lUserIDPZ = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref DeviceInfo);
                if (m_lUserIDPZ < 0)
                {

                    return false;
                }
                else
                {
                    m_DChanPZ = DeviceInfo.byStartChan;//模拟通道起始号
                    //MessageBox.Show("m_lUserIDMZ:"+m_lUserIDMZ.ToString()) ;
                    return true;
                }
            }
            else
                return false;

        }

        /// <summary>
        /// 重车视频播放
        /// </summary>
        private void PlayCameraMZ()
        {

            if (m_lRealHandleMZ < 0)
            {
                //manResetEvent.WaitOne();
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                lpPreviewInfo.hPlayWnd = pictureBox_FullPicture.Handle;//预览窗口
                lpPreviewInfo.lChannel = m_DChanMZ;//预te览的设备通道
                lpPreviewInfo.dwStreamType = 0;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                lpPreviewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流

                //CHCNetSDK.REALDATACALLBACK RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数
                IntPtr pUser = new IntPtr();//用户数据

                //打开预览 Start live view 
                m_lRealHandleMZ = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserIDMZ, ref lpPreviewInfo, null/*RealData*/, pUser);
                //MessageBox.Show("m_lRealHandleMZ:"+m_lRealHandleMZ.ToString()+ "\nm_DChanMZ:" + m_DChanMZ.ToString());
                //MessageBox.Show("lastError:" + CHCNetSDK.NET_DVR_GetLastError().ToString());
            }
        }

        /// <summary>
        /// 空车视频播放
        /// </summary>
        private void PlayCameraPZ()
        {

            if (m_lRealHandlePZ < 0)
            {
                //manResetEvent.WaitOne();
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                lpPreviewInfo.hPlayWnd = pictureBox1.Handle;//预览窗口
                lpPreviewInfo.lChannel = m_DChanPZ;//预te览的设备通道
                lpPreviewInfo.dwStreamType = 0;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                lpPreviewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流

                //CHCNetSDK.REALDATACALLBACK RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数
                IntPtr pUser = new IntPtr();//用户数据

                //打开预览 Start live view 
                m_lRealHandlePZ = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserIDPZ, ref lpPreviewInfo, null/*RealData*/, pUser);
                //MessageBox.Show("m_lRealHandleMZ:"+m_lRealHandleMZ.ToString()+ "\nm_DChanMZ:" + m_DChanMZ.ToString());
                //MessageBox.Show("lastError:" + CHCNetSDK.NET_DVR_GetLastError().ToString());
            }

        }

        #endregion

        #region 其他
        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox_TitleArea_Click(object sender, EventArgs e)
        {

        }

        private void label_SoftwareName_Click(object sender, EventArgs e)
        {

        }




        #endregion

        private void button_AutoMode_Click(object sender, EventArgs e)
        {
            if (AutoMeasure==null)
            {
                AutoMeasure = new Thread(AutoStart);
                AutoMeasure.IsBackground = true;
                AutoMeasure.Start();
            }

        }
        public void AutoStart()
        {
            try
            {
                button_AutoMode.BackgroundImage = Image.FromFile(".\\UI\\正在自启动启动.png");
                button_Start.Enabled = false;
             //   //连接车轮传感器
             //   CLCGQModule cl = new CLCGQModule();
             //   cl.Connect();
             //   //连接RFID
             //   RFIDModule rfid = new RFIDModule();
             //   rfid.Connect();
             //   //监控RFID信号，控制设备启动和暂停。
                rfid.StartMonitor();
             //   //监控到信号
             //   Thread.Sleep(50);
                while (true)
                {
                    if(rfid.TrainIsCome)
                    {
                        rfid.TrainIsCome = false;
                        //变色表示测量开始
                        button_Start.BackgroundImage = Image.FromFile(".\\UI\\正在运行128.png");
                        measureModule = new MeasureModule(button_Start);  //初始化自动测量模块
                        bool bRet = measureModule.ConnectAllDevice();  //连接所有设备
                        measureModule.StartMeasure();  //开始测量
                        //停止监视器并重置变量
                        rfid.StopMonitor();
                        //等待列车通过
                        Thread.Sleep(150000);
                        
                        //重新启动monitor监视器
                        rfid.StartMonitor();
                    }
                }
            }
            catch (Exception e)
            {
                rfid.StopMonitor();
                button_Start.Enabled = true;
                button_AutoMode.BackgroundImage = Image.FromFile(".\\UI\\自启动128.png");
                MessageBox.Show(e.Message);
                return;
            }
        }

        private void button_AutoQuit_Click(object sender, EventArgs e)
        {
            if (AutoMeasure == null)
                ;
            else if (AutoMeasure.IsAlive)
            {
                AutoMeasure.Abort();
                AutoMeasure = null;
            }
        }
    }
}

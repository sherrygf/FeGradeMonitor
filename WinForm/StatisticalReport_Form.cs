using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GradeMonitorApplication.FunctionClass;
using System.Drawing.Printing;

namespace GradeMonitorApplication
{
    public partial class ReportManagement_Form : Form
    {
        List<DB_TrainMeasureResult_DataStruct> list_TrainMeasureResult = new List<DB_TrainMeasureResult_DataStruct>();//存储查询的数据
        DateTime searchTime_Day = DateTime.Now;//按天查询时间
        int weekIndex = 1;//按周查询时间
        DateTime searchTime_Month = DateTime.Now;//按月查询时间
        DateTime searchTime_Year = DateTime.Now;//按年查询时间
        DateTime searchTime_HistoryBegin = DateTime.Now;//历史数据查询开始时间
        DateTime searchTime_HistoryEnd = DateTime.Now;//历史数据查询结束时间
        int nTrains = 0; //总机车数
        int nTrainCounts = 0;  //总车厢数
        double dMZs = 0;  //总毛重
        double dJZs = 0;  //总净重
        double dVolumns = 0;   //总体积
        double dAveragePW = 0;  //平均品位
        string sSearchTime = "";  //查询时间
        PrinterClass Printer;

        public ReportManagement_Form()
        {
            InitializeComponent();
        }

        #region 窗口加载及布局
        private void ReportManagement_Form_Load(object sender, EventArgs e)
        {
            //初始化界面
            InitializeForm(sender,e);
        }

        /// <summary>
        /// 初始化界面
        /// </summary>
        private void InitializeForm(object sender, EventArgs e)
        {
            #region 界面设计
            #region 每日数据界面
            dayDataView(sender, e);  //加载当天数据
            #endregion

            #region 每周数据界面
            //添加周列表
            int weekCount = GetYearWeekCount();
            for (int i = 1; i <= weekCount; i++)
            {
                comboBox_Week.Items.Add("第" + i + "周");
            }
            DateTime yearFirstDay = new DateTime(DateTime.Now.Year, 1, 1); //当年的1月1日
            int yearFirstDayWeek = Convert.ToInt32(yearFirstDay.DayOfWeek) - 1;
            comboBox_Week.SelectedIndex = (DateTime.Now.DayOfYear - (6 - yearFirstDayWeek)) / 7 + 1;//初始化当前周
            weekIndex = comboBox_Week.SelectedIndex +1;
            weekDataView(sender, e);  //加载当周数据
            #endregion

            #region 每月数据界面
            dateTimePicker_Month.CustomFormat = "yyyy年MM月";
            dateTimePicker_Month.Format = DateTimePickerFormat.Custom;
            dateTimePicker_Month.ShowUpDown = true;
            monthDataView(sender, e);  //加载当月数据
            #endregion

            #region 每年数据界面
            dateTimePicker_Year.CustomFormat = "yyyy年";
            dateTimePicker_Year.Format = DateTimePickerFormat.Custom;
            dateTimePicker_Year.ShowUpDown = true;
            yearDataView(sender, e);  //加载当年数据
            #endregion

            #region 历史数据界面

            #endregion

            foreach (string name in PrinterSettings.InstalledPrinters)
            {
                comboBox1.Items.Add(name);
                comboBox2.Items.Add(name);
                comboBox3.Items.Add(name);
                comboBox4.Items.Add(name);
                comboBox5.Items.Add(name);
            }
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
            #endregion
        }
        #endregion

        #region 每日数据明细相关操作
        /// <summary>
        /// 前一天
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_LastDay_Click(object sender, EventArgs e)
        {
            dateTimePicker_Day.Value = dateTimePicker_Day.Value.AddDays(-1);
            searchTime_Day = dateTimePicker_Day.Value;
            dayDataView(sender, e);
        }

        /// <summary>
        /// 后一天
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_NextDay_Click(object sender, EventArgs e)
        {
            dateTimePicker_Day.Value = dateTimePicker_Day.Value.AddDays(1);
            searchTime_Day = dateTimePicker_Day.Value;
            dayDataView(sender, e);
        }

        /// <summary>
        /// 按天查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_DayQuery_Click(object sender, EventArgs e)
        {
            searchTime_Day = dateTimePicker_Day.Value;
            dayDataView(sender, e);
        }

        /// <summary>
        /// 按天打印报表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_DayReportPrint_Click(object sender, EventArgs e)
        {
            Printer = new PrinterClass(list_TrainMeasureResult);
            Printer.PrinterName = comboBox1.SelectedItem.ToString();
            Printer.Print();
        }

        /// <summary>
        /// 按天查询数据展示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dayDataView(object sender, EventArgs e)
        {
            listView_Day.Items.Clear();
            listView_DayStatistics.Items.Clear();
            nTrains = 0; //总机车数
            nTrainCounts = 0;  //总车厢数
            dMZs = 0;  //总毛重
            dJZs = 0;  //总净重
            dVolumns = 0;   //总体积
            dAveragePW = 0;  //平均品位
            sSearchTime = "";  //查询时间

            MainForm.database.GetDayData(ref list_TrainMeasureResult, searchTime_Day);

            if (list_TrainMeasureResult.Count < 1)
                return;

            nTrains = list_TrainMeasureResult.Count; //总机车数
            for (int i = 0; i < list_TrainMeasureResult.Count; i++)
            {
                nTrainCounts = nTrainCounts + list_TrainMeasureResult[i].nTrainCount;  //总车厢数
                dMZs = dMZs + list_TrainMeasureResult[i].dMZ;  //总毛重
                dJZs = dJZs+ list_TrainMeasureResult[i].dJZ;   //总净重
                dVolumns = dVolumns + list_TrainMeasureResult[i].dVolumn;  //总体积
                dAveragePW = dAveragePW + list_TrainMeasureResult[i].dPW;  //总品位

                string[] item = { "",list_TrainMeasureResult[i].sFID,list_TrainMeasureResult[i].sTrainName,
                    list_TrainMeasureResult[i].nTrainCount.ToString(),list_TrainMeasureResult[i].dVolumn.ToString("0.00"),
                    list_TrainMeasureResult[i].dMZ.ToString("0.00"),list_TrainMeasureResult[i].dJZ.ToString("0.00"),
                    list_TrainMeasureResult[i].dBZ.ToString("0.00"),list_TrainMeasureResult[i].dPW.ToString("0.00"),
                    list_TrainMeasureResult[i].dtMeasureTime.ToString()};
                ListViewItem lvi = new ListViewItem(item);//创建列表项
                listView_Day.Items.Add(lvi);//将项加入listView1列表中
            }
            dAveragePW = dAveragePW / nTrains;  //平均品位
            sSearchTime = searchTime_Day.ToString(); //查询时间

            string[] item_DayStatistics = { "",nTrains.ToString(), nTrainCounts.ToString(),dVolumns.ToString("0.00"),dMZs.ToString("0.00"),dJZs.ToString("0.00"),dAveragePW.ToString("0.00"),sSearchTime.ToString()};
            ListViewItem lvi_DayStatistics = new ListViewItem(item_DayStatistics);//创建列表项
            listView_DayStatistics.Items.Add(lvi_DayStatistics);//将项加入listView_DayStatistics列表中

            //list_TrainMeasureResult.Clear();
        }
        #endregion

        #region 每周数据明细相关操作
        /// <summary>
        /// 前一周
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_LastWeek_Click(object sender, EventArgs e)
        {
            comboBox_Week.SelectedIndex = comboBox_Week.SelectedIndex - 1;
            weekIndex = comboBox_Week.SelectedIndex + 1;
            weekDataView(sender, e);  //加载当周数据

            //控制按钮是否用
            if (comboBox_Week.SelectedIndex < 1)
                button_LastWeek.Enabled = false;
            else
                button_LastWeek.Enabled = true;
            if (comboBox_Week.SelectedIndex >= GetYearWeekCount() - 1)
                button_NextWeek.Enabled = false;
            else
                button_NextWeek.Enabled = true;
        }

        /// <summary>
        /// 后一周
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void button_NextWeek_Click(object sender, EventArgs e)
        {
            comboBox_Week.SelectedIndex = comboBox_Week.SelectedIndex + 1;
            weekIndex = comboBox_Week.SelectedIndex + 1;
            weekDataView(sender, e);  //加载当周数据

            //控制按钮是否用
            if (comboBox_Week.SelectedIndex < 1)
                button_LastWeek.Enabled = false;
            else
                button_LastWeek.Enabled = true;
            if (comboBox_Week.SelectedIndex >= GetYearWeekCount() - 1)
                button_NextWeek.Enabled = false;
            else
                button_NextWeek.Enabled = true;
        }

        /// <summary>
        /// 按周查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_WeekQuery_Click(object sender, EventArgs e)
        {
            weekIndex = comboBox_Week.SelectedIndex +1;
            weekDataView(sender, e);  //加载当周数据

            //控制按钮是否用
            if (comboBox_Week.SelectedIndex < 1)
                button_LastWeek.Enabled = false;
            else
                button_LastWeek.Enabled = true;
            if (comboBox_Week.SelectedIndex >= GetYearWeekCount() - 1)
                button_NextWeek.Enabled = false;
            else
                button_NextWeek.Enabled = true;
        }

        /// <summary>
        /// 按周打印报表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_WeekReportPrint_Click(object sender, EventArgs e)
        {
            Printer = new PrinterClass(list_TrainMeasureResult);
            Printer.PrinterName = comboBox2.SelectedItem.ToString();
            Printer.Print();
        }

        /// <summary>
        /// 按周查询数据展示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void weekDataView(object sender, EventArgs e)
        {
            listView_Week.Items.Clear();
            listView_WeekStatistics.Items.Clear();
            nTrains = 0; //总机车数
            nTrainCounts = 0;  //总车厢数
            dMZs = 0;  //总毛重
            dJZs = 0;  //总净重
            dVolumns = 0;   //总体积
            dAveragePW = 0;  //平均品位
            sSearchTime = "";  //查询时间

            MainForm.database.GetWeekData(ref list_TrainMeasureResult, weekIndex);

            if (list_TrainMeasureResult.Count < 1)
                return;

            nTrains = list_TrainMeasureResult.Count; //总机车数
            for (int i = 0; i < list_TrainMeasureResult.Count; i++)
            {
                nTrainCounts = nTrainCounts + list_TrainMeasureResult[i].nTrainCount;  //总车厢数
                dMZs = dMZs + list_TrainMeasureResult[i].dMZ;  //总毛重
                dJZs = dJZs + list_TrainMeasureResult[i].dJZ;   //总净重
                dVolumns = dVolumns + list_TrainMeasureResult[i].dVolumn;  //总体积
                dAveragePW = dAveragePW + list_TrainMeasureResult[i].dPW;  //总品位

                string[] item = { "",list_TrainMeasureResult[i].sFID,list_TrainMeasureResult[i].sTrainName,
                    list_TrainMeasureResult[i].nTrainCount.ToString(),list_TrainMeasureResult[i].dVolumn.ToString("0.00"),
                    list_TrainMeasureResult[i].dMZ.ToString("0.00"),list_TrainMeasureResult[i].dJZ.ToString("0.00"),
                    list_TrainMeasureResult[i].dBZ.ToString("0.00"),list_TrainMeasureResult[i].dPW.ToString("0.00"),
                    list_TrainMeasureResult[i].dtMeasureTime.ToString()};
                ListViewItem lvi = new ListViewItem(item);//创建列表项
                listView_Week.Items.Add(lvi);//将项加入listView1列表中
            }
            dAveragePW = dAveragePW / nTrains;  //平均品位
            sSearchTime = searchTime_Day.ToString(); //查询时间

            string[] item_WeekStatistics = { "", nTrains.ToString(), nTrainCounts.ToString(), dMZs.ToString("0.00"), dJZs.ToString("0.00"), dVolumns.ToString("0.00"), dAveragePW.ToString("0.00"), sSearchTime.ToString() };
            ListViewItem lvi_WeekStatistics = new ListViewItem(item_WeekStatistics);//创建列表项
            listView_WeekStatistics.Items.Add(lvi_WeekStatistics);//将项加入listView_DayStatistics列表中

            //list_TrainMeasureResult.Clear();
        }

        /// <summary>
        /// 计算当年多少周，并给comboBox_Week赋值
        /// </summary>
        /// <returns></returns>
        public static int GetYearWeekCount()
        {
            int countWeek = 0;
            System.DateTime fDt = DateTime.Parse(DateTime.Now.ToString("yyyy") + "-01-01");
            int k = Convert.ToInt32(fDt.DayOfWeek);//得到该年的第一天是周几 
            if (k == 1)
            {
                int countDay = fDt.AddYears(1).AddDays(-1).DayOfYear;

                countWeek = countDay / 7 + 1;
                return countWeek;
            }
            else
            {
                int countDay = fDt.AddYears(1).AddDays(-1).DayOfYear;
                countWeek = countDay / 7 + 2;
                return countWeek;
            }
        }
        #endregion

        #region 每月数据明细相关操作
        /// <summary>
        /// 前一月
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_LastMonth_Click(object sender, EventArgs e)
        {
            dateTimePicker_Month.Value = dateTimePicker_Month.Value.AddMonths(-1);
            searchTime_Month = dateTimePicker_Month.Value;
            monthDataView(sender, e);
        }
        /// <summary>
        /// 后一月
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_NextMonth_Click(object sender, EventArgs e)
        {
            dateTimePicker_Month.Value = dateTimePicker_Month.Value.AddMonths(1);
            searchTime_Month = dateTimePicker_Month.Value;
            monthDataView(sender, e);
        }
        /// <summary>
        /// 按月查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_MonthQuery_Click(object sender, EventArgs e)
        {
            searchTime_Month = dateTimePicker_Month.Value;
            monthDataView(sender, e);
        }
        /// <summary>
        /// 按月打印报表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_MonthReportPrint_Click(object sender, EventArgs e)
        {
            Printer = new PrinterClass(list_TrainMeasureResult);
            Printer.PrinterName = comboBox3.SelectedItem.ToString();
            Printer.Print();
        }

        /// <summary>
        /// 按月查询数据展示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void monthDataView(object sender, EventArgs e)
        {
            listView_Month.Items.Clear();
            listView_MonthStatistics.Items.Clear();
            nTrains = 0; //总机车数
            nTrainCounts = 0;  //总车厢数
            dMZs = 0;  //总毛重
            dJZs = 0;  //总净重
            dVolumns = 0;   //总体积
            dAveragePW = 0;  //平均品位
            sSearchTime = "";  //查询时间

            MainForm.database.GetMonthData(ref list_TrainMeasureResult, searchTime_Month);

            if (list_TrainMeasureResult.Count < 1)
                return;

            nTrains = list_TrainMeasureResult.Count; //总机车数
            for (int i = 0; i < list_TrainMeasureResult.Count; i++)
            {
                nTrainCounts = nTrainCounts + list_TrainMeasureResult[i].nTrainCount;  //总车厢数
                dMZs = dMZs + list_TrainMeasureResult[i].dMZ;  //总毛重
                dJZs = dJZs + list_TrainMeasureResult[i].dJZ;   //总净重
                dVolumns = dVolumns + list_TrainMeasureResult[i].dVolumn;  //总体积
                dAveragePW = dAveragePW + list_TrainMeasureResult[i].dPW;  //总品位

                string[] item = { "",list_TrainMeasureResult[i].sFID,list_TrainMeasureResult[i].sTrainName,
                    list_TrainMeasureResult[i].nTrainCount.ToString(),list_TrainMeasureResult[i].dVolumn.ToString("0.00"),
                    list_TrainMeasureResult[i].dMZ.ToString("0.00"),list_TrainMeasureResult[i].dJZ.ToString("0.00"),
                    list_TrainMeasureResult[i].dBZ.ToString("0.00"),list_TrainMeasureResult[i].dPW.ToString("0.00"),
                    list_TrainMeasureResult[i].dtMeasureTime.ToString()};
                ListViewItem lvi = new ListViewItem(item);//创建列表项
                listView_Month.Items.Add(lvi);//将项加入listView1列表中
            }
            dAveragePW = dAveragePW / nTrains;  //平均品位
            sSearchTime = searchTime_Day.ToString(); //查询时间

            string[] item_MonthStatistics = { "", nTrains.ToString(), nTrainCounts.ToString(), dMZs.ToString(), dJZs.ToString(), dVolumns.ToString(), dAveragePW.ToString(), sSearchTime.ToString() };
            ListViewItem lvi_MonthStatistics = new ListViewItem(item_MonthStatistics);//创建列表项
            listView_MonthStatistics.Items.Add(lvi_MonthStatistics);//将项加入listView_DayStatistics列表中

            //list_TrainMeasureResult.Clear();
        }
        #endregion

        #region 每年数据明细相关操作
        /// <summary>
        /// 前一年
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_LastYear_Click(object sender, EventArgs e)
        {
            dateTimePicker_Year.Value = dateTimePicker_Year.Value.AddYears(-1);
            searchTime_Year = dateTimePicker_Year.Value;
            yearDataView(sender, e);
        }
        /// <summary>
        /// 后一年
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_NextYear_Click(object sender, EventArgs e)
        {
            dateTimePicker_Year.Value = dateTimePicker_Year.Value.AddYears(1);
            searchTime_Year = dateTimePicker_Year.Value;
            yearDataView(sender, e);
        }
        /// <summary>
        /// 按年查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_YearQuery_Click(object sender, EventArgs e)
        {
            searchTime_Year = dateTimePicker_Year.Value;
            yearDataView(sender, e);
        }
        /// <summary>
        /// 按年打印报表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_YearReportPrint_Click(object sender, EventArgs e)
        {
            Printer = new PrinterClass(list_TrainMeasureResult);
            Printer.PrinterName = comboBox4.SelectedItem.ToString();
            Printer.Print();
        }
        /// <summary>
        /// 按年查询数据展示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void yearDataView(object sender, EventArgs e)
        {
            listView_Year.Items.Clear();
            listView_YearStatistics.Items.Clear();
            nTrains = 0; //总机车数
            nTrainCounts = 0;  //总车厢数
            dMZs = 0;  //总毛重
            dJZs = 0;  //总净重
            dVolumns = 0;   //总体积
            dAveragePW = 0;  //平均品位
            sSearchTime = "";  //查询时间

            MainForm.database.GetYearData(ref list_TrainMeasureResult, searchTime_Year);

            if (list_TrainMeasureResult.Count < 1)
                return;

            nTrains = list_TrainMeasureResult.Count; //总机车数
            for (int i = 0; i < list_TrainMeasureResult.Count; i++)
            {
                nTrainCounts = nTrainCounts + list_TrainMeasureResult[i].nTrainCount;  //总车厢数
                dMZs = dMZs + list_TrainMeasureResult[i].dMZ;  //总毛重
                dJZs = dJZs + list_TrainMeasureResult[i].dJZ;   //总净重
                dVolumns = dVolumns + list_TrainMeasureResult[i].dVolumn;  //总体积
                dAveragePW = dAveragePW + list_TrainMeasureResult[i].dPW;  //总品位

                string[] item = { "",list_TrainMeasureResult[i].sFID,list_TrainMeasureResult[i].sTrainName,
                    list_TrainMeasureResult[i].nTrainCount.ToString(),list_TrainMeasureResult[i].dVolumn.ToString("0.00"),
                    list_TrainMeasureResult[i].dMZ.ToString("0.00"),list_TrainMeasureResult[i].dJZ.ToString("0.00"),
                    list_TrainMeasureResult[i].dBZ.ToString("0.00"),list_TrainMeasureResult[i].dPW.ToString("0.00"),
                    list_TrainMeasureResult[i].dtMeasureTime.ToString()};
                ListViewItem lvi = new ListViewItem(item);//创建列表项
                listView_Year.Items.Add(lvi);//将项加入listView1列表中
            }
            dAveragePW = dAveragePW / nTrains;  //平均品位
            sSearchTime = searchTime_Day.ToString(); //查询时间

            string[] item_YearStatistics = { "", nTrains.ToString(), nTrainCounts.ToString(), dMZs.ToString("0.00"), dJZs.ToString("0.00"), dVolumns.ToString("0.00"), dAveragePW.ToString("0.00"), sSearchTime.ToString() };
            ListViewItem lvi_YearStatistics = new ListViewItem(item_YearStatistics);//创建列表项
            listView_YearStatistics.Items.Add(lvi_YearStatistics);//将项加入listView_DayStatistics列表中

            //list_TrainMeasureResult.Clear();
        }
        #endregion

        #region 历史数据明细相关操作
        /// <summary>
        /// 历史数据查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_HistoryQuery_Click(object sender, EventArgs e)
        {
            searchTime_HistoryBegin = dateTimePicker_HistoryBegin.Value;
            searchTime_HistoryEnd = dateTimePicker_HistoryEnd.Value.AddDays(1);
            historyDataView(sender, e);
        }

        /// <summary>
        /// 历史数据打印报表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_HistoryReportPrint_Click(object sender, EventArgs e)
        {
            Printer = new PrinterClass(list_TrainMeasureResult);
            Printer.PrinterName = comboBox5.SelectedItem.ToString();
            Printer.Print();
        }

        /// <summary>
        /// 历史查询数据展示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void historyDataView(object sender, EventArgs e)
        {
            listView_History.Items.Clear();
            listView_HistoryStatistics.Items.Clear();
            nTrains = 0; //总机车数
            nTrainCounts = 0;  //总车厢数
            dMZs = 0;  //总毛重
            dJZs = 0;  //总净重
            dVolumns = 0;   //总体积
            dAveragePW = 0;  //平均品位
            sSearchTime = "";  //查询时间

            MainForm.database.GetHistoryData(ref list_TrainMeasureResult, searchTime_HistoryBegin, searchTime_HistoryEnd);

            if (list_TrainMeasureResult.Count < 1)
                return;

            nTrains = list_TrainMeasureResult.Count; //总机车数
            for (int i = 0; i < list_TrainMeasureResult.Count; i++)
            {
                nTrainCounts = nTrainCounts + list_TrainMeasureResult[i].nTrainCount;  //总车厢数
                dMZs = dMZs + list_TrainMeasureResult[i].dMZ;  //总毛重
                dJZs = dJZs + list_TrainMeasureResult[i].dJZ;   //总净重
                dVolumns = dVolumns + list_TrainMeasureResult[i].dVolumn;  //总体积
                dAveragePW = dAveragePW + list_TrainMeasureResult[i].dPW;  //总品位

                string[] item = { "",list_TrainMeasureResult[i].sFID,list_TrainMeasureResult[i].sTrainName,
                    list_TrainMeasureResult[i].nTrainCount.ToString(),list_TrainMeasureResult[i].dVolumn.ToString("0.00"),
                    list_TrainMeasureResult[i].dMZ.ToString("0.00"),list_TrainMeasureResult[i].dJZ.ToString("0.00"),
                    list_TrainMeasureResult[i].dBZ.ToString("0.00"),list_TrainMeasureResult[i].dPW.ToString("0.00"),
                    list_TrainMeasureResult[i].dtMeasureTime.ToString()};
                ListViewItem lvi = new ListViewItem(item);//创建列表项
                listView_History.Items.Add(lvi);//将项加入listView1列表中
            }
            dAveragePW = dAveragePW / nTrains;  //平均品位
            sSearchTime = searchTime_Day.ToString(); //查询时间

            string[] item_HistoryStatistics = { "", nTrains.ToString(), nTrainCounts.ToString(), dMZs.ToString("0.00"), dJZs.ToString("0.00"), dVolumns.ToString("0.00"), dAveragePW.ToString("0.00"), sSearchTime.ToString() };
            ListViewItem lvi_HistoryStatistics = new ListViewItem(item_HistoryStatistics);//创建列表项
            listView_HistoryStatistics.Items.Add(lvi_HistoryStatistics);//将项加入listView_DayStatistics列表中

            //list_TrainMeasureResult.Clear();
        }


        #endregion

        private void listView_Day_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }
    }
}

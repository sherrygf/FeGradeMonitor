using GradeMonitorApplication.FunctionClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GradeMonitorApplication.WinForm
{
    public partial class ErrorReport_Form : Form
    {
        List<DB_TrainMeasureResult_DataStruct> list_TrainMeasureResult = new List<DB_TrainMeasureResult_DataStruct>();//存储查询的数据
        public ErrorReport_Form()
        {
            InitializeComponent();
        }
       
        public string  ErrorCode(int i)
        {
            if (i == 1)
                return ("未在轨道衡检测到对应的列车重量数据");
            else if (i == 3)
                return ("轨道衡数据存在一节或多节的明显偏低错误，或为空车过磅");
            else if (i == 4)
                return ("轨道衡数据可能漏节，重量个数偏少");
            else
                return "";
        }

        private void ErrorReport_Form_Load(object sender, EventArgs e)
        {
            MainForm.database.GetErrorData(ref list_TrainMeasureResult);
            listView1.Items.Clear();
            for(int pr=0;pr<list_TrainMeasureResult.Count;pr++)
            {
                string[] item = { list_TrainMeasureResult[pr].sFID, list_TrainMeasureResult[pr].dtMeasureTime.ToString(), ErrorCode(list_TrainMeasureResult[pr].intWB) };
                ListViewItem litem = new ListViewItem(item);
                listView1.Items.Add(litem);
            }
        }

        private void button_HistoryQuery_Click(object sender, EventArgs e)
        {
            DateTime begin = dateTimePicker_HistoryBegin.Value, end = dateTimePicker_HistoryEnd.Value;
            MainForm.database.GetErrorData(ref list_TrainMeasureResult, begin, end);
            listView1.Items.Clear();
            for (int pr = 0; pr < list_TrainMeasureResult.Count; pr++)
            {
                string[] item = { list_TrainMeasureResult[pr].sFID, list_TrainMeasureResult[pr].dtMeasureTime.ToString(), ErrorCode(list_TrainMeasureResult[pr].intWB) };
                ListViewItem litem = new ListViewItem(item);
                listView1.Items.Add(litem);
            }
        }
    }
}

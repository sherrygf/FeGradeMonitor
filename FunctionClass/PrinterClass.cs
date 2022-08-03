using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeMonitorApplication.FunctionClass
{
    class PrinterClass
    {
        /// <summary>
        /// 打印文档对象
        /// </summary>
        public PrintDocument printdocument;
        /// <summary>
        /// 待打印的数据集
        /// </summary>
        public List<DB_TrainMeasureResult_DataStruct> result;
        /// <summary>
        /// 默认的纸张尺寸(A4)中规定的最大可打印数据条数
        /// </summary>
        private const int LineLimit = 45;
        /// <summary>
        /// 已输出的数据条数
        /// </summary>
        private int Linepr = 0;
        /// <summary>
        /// 页码
        /// </summary>
        private int pages = 1;
        /// <summary>
        /// 指定的打印机名称
        /// </summary>
        public string PrinterName = "Microsoft Print to PDF";
        
        public PrinterClass(List<DB_TrainMeasureResult_DataStruct> dataset)
        {
            result = dataset;
            Linepr = 0;
            pages = 1;
            printdocument = new PrintDocument();
        }
        /// <summary>
        /// 毫米转英寸
        /// </summary>
        /// <param name="mm">毫米数</param>
        /// <returns>对应的英寸数</returns>
        public float mm2inch(float mm)
        {
            float inch;
            inch = 0.039370078740157f * mm * 100;
            return inch;
        }
        public bool Print()
        {
            try
            {
                printdocument.PrinterSettings.PrinterName = PrinterName;
                printdocument.PrintPage += new PrintPageEventHandler(PrintTicket);
                printdocument.Print();
                return (true);
            }
            catch(Exception e)
            {
                Function.WriteErrorLog("打印单据出错：" + e.Message);
                return (false);
            }
        }
        private void PrintTicket(object sender, PrintPageEventArgs e)
        {
            float x, y;
            float rowgap = 23;
            Font headerfont = new Font("宋体", 11, FontStyle.Bold);//列名标题字体
            Brush brush = new SolidBrush(Color.Black);//列名字体画刷
            float leftmargin = mm2inch(16.67f);
            float topmargin = mm2inch(12.44f); //顶边距 
            Font Cellfont = new Font("宋体", 11);//单元格字体


            x = leftmargin;//获取表格的左边距
            y = topmargin;//获取表格的顶边距

            //获取报表的宽度
            float delta = mm2inch(0);

            y += rowgap;//设置表格的上边线的位置



            //武钢矿业责任有限公司大冶铁矿
            Font fontrow1 = new Font("宋体", 12, FontStyle.Regular);
            string strrow1 = "八钢矿业资源有限公司蒙库铁矿";
            float xrow1 = mm2inch(80.43f);
            float yrow1 = mm2inch(14.02f) + delta;
            e.Graphics.DrawString(strrow1, fontrow1, brush, xrow1, yrow1);//绘制列标题

            //原 矿 计 量 签 证 单 据
            Font fontrow2 = new Font("宋体", 23, FontStyle.Bold);
            string strrow2 = "原 矿 计 量 签 证 单 据";
            float xrow2 = mm2inch(62.97f);
            float yrow2 = mm2inch(19.31f) + delta;
            e.Graphics.DrawString(strrow2, fontrow2, brush, xrow2, yrow2);//绘制列标题

            //画两条线
            e.Graphics.DrawLine(Pens.Black, mm2inch(62.97f), mm2inch(27) + delta, mm2inch(62.97f + 100), mm2inch(27) + delta);
            e.Graphics.DrawLine(Pens.Black, mm2inch(62.97f), mm2inch(29) + delta, mm2inch(62.97f + 100), mm2inch(29) + delta);

            //编号
            Font fontrow3 = new Font("宋体", 11, FontStyle.Regular);
            string strrow3_1 = "编号：" + DateTime.Now.ToString("yyyyMMddHHmm");
            float xrow3_1 = mm2inch(14f);
            float yrow3_1 = mm2inch(30.4f) + delta;
            e.Graphics.DrawString(strrow3_1, fontrow3, brush, xrow3_1, yrow3_1);//绘制列标题

            //  string strrow3_2 = DateTime.Now.ToString("MM/dd/yy  HH:mm:ss");
            //  float xrow3_2 = mm2inch(16.67f) + mm2inch(70f);
            //  float yrow3_2 = mm2inch(34.4f) + delta;
            //  e.Graphics.DrawString(strrow3_2, fontrow3, brush, xrow3_2, yrow3_2);//绘制列标题

            string strrow3_3 = "单位：t、m3、m/s";
            float xrow3_3 = mm2inch(16.67f) + mm2inch(150f);
            float yrow3_3 = yrow3_1;
            e.Graphics.DrawString(strrow3_3, fontrow3, brush, xrow3_3, yrow3_3);//绘制列标题

            e.Graphics.DrawLine(Pens.Black, mm2inch(14f), yrow3_3 + mm2inch(3.6f), mm2inch(14f + 185), yrow3_3 + mm2inch(3.6f));

            Font fontrow4 = new Font("宋体", 12, FontStyle.Regular);
            string strrow4_1 = "发货单位：884主溜井 ";
            float xrow4_1 = mm2inch(14f);
            float yrow4_1 = mm2inch(5f) + yrow3_1;
            e.Graphics.DrawString(strrow4_1, fontrow4, brush, xrow4_1, yrow4_1);

            string strrow4_2 = "矿石品种：原生矿";
            float xrow4_2 = xrow4_1 + mm2inch(80);
            float yrow4_2 = yrow4_1;
            e.Graphics.DrawString(strrow4_2, fontrow4, brush, xrow4_2, yrow4_2);

            string strrow4_3 = "元素：Fe";
            float xrow4_3 = xrow4_2 + mm2inch(72);
            float yrow4_3 = yrow4_2;
            e.Graphics.DrawString(strrow4_3, fontrow4, brush, xrow4_3, yrow4_3);

            e.Graphics.DrawLine(Pens.Black, mm2inch(14f), yrow4_2 + mm2inch(3.6f), mm2inch(14f + 185), yrow4_2 + mm2inch(3.6f));

            string strrow5_1 = "单据时间范围：";
            float xrow5_1 = mm2inch(14f);
            float yrow5_1 = yrow4_1 + mm2inch(5f);
            e.Graphics.DrawString(strrow5_1, fontrow4, brush, xrow5_1, yrow5_1);
            e.Graphics.DrawLine(Pens.Black, mm2inch(14f), yrow5_1 + mm2inch(3.6f), mm2inch(14f + 185), yrow5_1 + mm2inch(3.6f));
            //表头
            float width = mm2inch(19f);
            float height = mm2inch(5f);
            string strrow6_1 = "流水号";
            float xrow6_1 = mm2inch(14f);
            float yrow6_1 = yrow5_1 + height;
            e.Graphics.DrawString(strrow6_1, fontrow4, brush, xrow6_1, yrow6_1);


            strrow6_1 = "车号";
            xrow6_1 += width + mm2inch(15f);
            e.Graphics.DrawString(strrow6_1, fontrow4, brush, xrow6_1, yrow6_1);
            strrow6_1 = "车厢数";
            xrow6_1 += width;
            e.Graphics.DrawString(strrow6_1, fontrow4, brush, xrow6_1, yrow6_1);
            strrow6_1 = "体积";
            xrow6_1 += width;
            e.Graphics.DrawString(strrow6_1, fontrow4, brush, xrow6_1, yrow6_1);
            strrow6_1 = "净重";
            xrow6_1 += width;
            e.Graphics.DrawString(strrow6_1, fontrow4, brush, xrow6_1, yrow6_1);
            strrow6_1 = "比重";
            xrow6_1 += width;
            e.Graphics.DrawString(strrow6_1, fontrow4, brush, xrow6_1, yrow6_1);
            strrow6_1 = "品位";
            xrow6_1 += width;
            e.Graphics.DrawString(strrow6_1, fontrow4, brush, xrow6_1, yrow6_1);
            strrow6_1 = "测量时间";
            xrow6_1 += width;
            e.Graphics.DrawString(strrow6_1, fontrow4, brush, xrow6_1, yrow6_1);

            //写入数据
            if (Linepr == result.Count)
                Linepr = 0;
            string sFID, sTrainName, sTrainCount, sVolumn, sJZ, sBZ, sPW, sTime;
            float xPr = mm2inch(14f);
            float yPr = yrow6_1;
            int count = 0;
            double tJZ = 0,tVolumn=0;
            while (count < LineLimit && Linepr < result.Count)
            {
                sFID = result[Linepr].sFID;
                sTrainName = result[Linepr].sTrainName;
                sTrainCount = result[Linepr].nTrainCount.ToString();
                sVolumn = result[Linepr].dVolumn.ToString("0.00");
                sJZ = result[Linepr].dJZ.ToString("0.00");
                sBZ = result[Linepr].dBZ.ToString("0.00");
                sPW = result[Linepr].dPW.ToString("0.00");
                sTime = result[Linepr].dtMeasureTime.ToString();

                tJZ += result[Linepr].dJZ;
                tVolumn += result[Linepr].dVolumn;

                yPr += height;
                xPr = mm2inch(14f);

                e.Graphics.DrawString(sFID, fontrow4, brush, xPr, yPr);
                xPr += width + mm2inch(15f);
                e.Graphics.DrawString(sTrainName, fontrow4, brush, xPr, yPr);
                xPr += width;
                e.Graphics.DrawString(sTrainCount, fontrow4, brush, xPr, yPr);
                xPr += width;
                e.Graphics.DrawString(sVolumn, fontrow4, brush, xPr, yPr);
                xPr += width;
                e.Graphics.DrawString(sJZ, fontrow4, brush, xPr, yPr);
                xPr += width;
                e.Graphics.DrawString(sBZ, fontrow4, brush, xPr, yPr);
                xPr += width;
                e.Graphics.DrawString(sPW, fontrow4, brush, xPr, yPr);
                xPr += width;
                e.Graphics.DrawString(sTime, fontrow4, brush, xPr, yPr);

                count++;
                Linepr++;
            }


            e.Graphics.DrawLine(Pens.Black, mm2inch(14f), yPr + mm2inch(3.6f), mm2inch(14f + 185), yPr + mm2inch(3.6f));

            yPr += height;
            string rowC = "总车数：" + count;
            xPr = mm2inch(14f);
            e.Graphics.DrawString(rowC, fontrow4, brush, xPr, yPr);
            rowC = "总车厢数：" + count * 10;
            xPr += width + mm2inch(15f);
            e.Graphics.DrawString(rowC, fontrow4, brush, xPr, yPr);
            rowC = "总体积：" + tVolumn.ToString("0.00");
            xPr += width + width;
            e.Graphics.DrawString(rowC, fontrow4, brush, xPr, yPr);
            rowC = "总净重：" + tJZ.ToString("0.00");
            xPr += width + width;
            e.Graphics.DrawString(rowC, fontrow4, brush, xPr, yPr);
            rowC = "平均品位：" ;
            xPr += width + width;
            e.Graphics.DrawString(rowC, fontrow4, brush, xPr, yPr);

            e.Graphics.DrawLine(Pens.Black, mm2inch(14f), yPr + mm2inch(3.6f), mm2inch(14f + 185), yPr + mm2inch(3.6f));

            yPr += height;
            string time = "打印日期：" + DateTime.Now.ToString();
            e.Graphics.DrawString(time, fontrow3, brush, mm2inch(14f), yPr);//绘制列标题

            //  string strrow3_2 = DateTime.Now.ToString("MM/dd/yy  HH:mm:ss");
            //  float xrow3_2 = mm2inch(16.67f) + mm2inch(70f);
            //  float yrow3_2 = mm2inch(34.4f) + delta;
            //  e.Graphics.DrawString(strrow3_2, fontrow3, brush, xrow3_2, yrow3_2);//绘制列标题

            string name = "打印员：";
            e.Graphics.DrawString(name, fontrow3, brush, xrow3_3, yPr);//绘制列标题
            string page = "-" + pages + "-";
            e.Graphics.DrawString(page, fontrow4, brush, 370, 1123);//绘制页码
            if (Linepr < result.Count)
            {
                e.HasMorePages = true;
                pages++;
            }
            else
                e.HasMorePages = false;
        }
    }
}

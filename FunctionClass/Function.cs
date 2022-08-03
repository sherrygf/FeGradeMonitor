using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GradeMonitorApplication.FunctionClass
{
    class Function
    {
        #region 文件操作
        /// <summary>
        /// 数据写入txt文件中
        /// </summary>
        /// <param name="str">保存数据内容</param>
        /// <param name="file_name">存储路径及文件名</param>
        static public void SaveToTxt(string str, string file_name)//保存数据
        {
            try
            {
                int index = file_name.LastIndexOf('\\');
                //string strForder = file_name;
                string Forder;
                if (index == -1)
                {
                    Forder = AppDomain.CurrentDomain.BaseDirectory;
                }
                else
                {
                    Forder = file_name.Remove(index);
                }

                if (!Directory.Exists(Forder))
                {
                    Directory.CreateDirectory(Forder);
                }

                if (!File.Exists(file_name))
                {
                    //int datalenth = 0;               
                    FileStream fs = new FileStream(file_name, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("GB2312"));//通过指定字符编码方式可以实现对汉字的支持，否则在用记事本打开查看会出现乱码             
                    sw.BaseStream.Seek(0, SeekOrigin.Begin);
                    //sw.WriteLine(str + "\r\n");
                    sw.WriteLine(str);
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }
                else
                {
                    string path = file_name;
                    using (StreamWriter sw = new StreamWriter(file_name, true, System.Text.Encoding.GetEncoding("GB2312")))
                    {
                        sw.WriteLine(str);
                        sw.Flush();
                        sw.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                WriteErrorLog("CreateTxtOut函数： " + ex.Message);
            }

        }

        /// <summary>
        /// 错误信息写入日志
        /// </summary>
        /// <param name="strMeas"></param>
        static public void WriteErrorLog(string strMeas)
        {
            string str = DateTime.Now.ToString() + " # " + strMeas;
            SaveToTxt(str, AppDomain.CurrentDomain.BaseDirectory + "ErrorLog.txt");
        }

        #endregion

        #region 图片操作
        #region 姓名生成图片
        /// <summary>
        /// 获取姓名对应的颜色值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetNameColor(string name)
        {
            if (string.IsNullOrEmpty(name) || name.Length <= 0)
                throw new Exception("name不能为空");
            //获取名字第一个字,转换成 16进制图片
            string str = "";
            foreach (var item in name)
            {
                str += Convert.ToUInt16(item);
            }
            if (str.Length < 4)
            {
                str += new Random().Next(100, 1000);//返回100到1000的随机数
            }
            string color = "#" + str.Substring(1, 3);
            return color;
        }

        /// <summary>
        /// 获取姓名对应的图片 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap GetNameImage(string name, int width, int height)
        {
            Bitmap img = new Bitmap(width, height);//定义图片
            Graphics g = Graphics.FromImage(img);//绘图
            //填充颜色
            string sColor = GetNameColor(name);  //依据姓氏获取颜色
            Brush brush = new SolidBrush(ColorTranslator.FromHtml(sColor));
            g.FillRectangle(brush, 0, 0, width, height);
            //填充文字
            string sName = name.Substring(name.Length - 1);  //获取名字最后一个字
            Font font = new Font("微软雅黑", 30);  //字体颜色和大小
            SizeF firstSize = g.MeasureString(sName, font);
            g.DrawString(sName, font, Brushes.White, new PointF((img.Width - firstSize.Width) / 2, (img.Height - firstSize.Height) / 2));
            g.Dispose();
            return img;
        }

        /// <summary>
        /// 保存图片到磁盘
        /// </summary>
        /// <param name="name"></param>
        /// <param name="targetFile"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap SaveNameImage(string name, string targetFile, int width, int height)
        {
            Bitmap img = GetNameImage(name, width, height);
            img.Save(targetFile, System.Drawing.Imaging.ImageFormat.Jpeg);
            //img.Dispose();//在release中会提前释放导致报错
            return img;
        }

        /// <summary>
        /// 裁剪图片为圆形
        /// </summary>
        /// <param name="img"></param>
        /// <param name="rec"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Image CutEllipse(Image img, Rectangle rec, Size size)
        {
            Bitmap bitmap = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                using (TextureBrush br = new TextureBrush(img, System.Drawing.Drawing2D.WrapMode.Clamp, rec))
                {
                    br.ScaleTransform((float)bitmap.Width / (float)rec.Width, (float)bitmap.Height / (float)rec.Height);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.FillEllipse(br, new Rectangle(Point.Empty, size));
                }
            }
            return bitmap;
        }
        #endregion


        #endregion

    }
}

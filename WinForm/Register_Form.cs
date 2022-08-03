using GradeMonitorApplication.FunctionClass;
using Microsoft.Win32;
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
    public partial class Register_Form : Form
    {
        public static bool state = true;  //软件是否为可用状态
        SoftReg softReg = new SoftReg();
        public Register_Form()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Register_Form_Load(object sender, EventArgs e)
        {
            this.textBox_MNum.Text = softReg.GetMNum();
        }

        /// <summary>
        /// 注册按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Register_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox_RNum.Text == softReg.GetRNum())
                {
                    MessageBox.Show("注册成功！请重新启动软件！", "信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RegistryKey retkey = Registry.CurrentUser.OpenSubKey("Software", true).CreateSubKey("wxf").CreateSubKey("wxf.INI").CreateSubKey(textBox_RNum.Text);
                    retkey.SetValue("UserName", "Rsoft");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("注册码错误！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox_RNum.SelectAll();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// 退出按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Exist_Click(object sender, EventArgs e)
        {
            if (state == true)
            {
                this.Close();
            }
            else
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// 关闭窗体事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Register_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (state == true)
            {
                this.Close();
            }
            else
            {
                Application.Exit();
            }
        }
    }
}

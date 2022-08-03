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
    public partial class User_Form : Form
    {
        /// <summary>
        /// 临时存储头像路径
        /// </summary>
        private string sNewPhoto = "";

        public User_Form()
        {
            InitializeComponent();
        }

        private void User_Form_Load(object sender, EventArgs e)
        {
            //初始化头像
            this.pictureBox__UserPhoto.Load(Application.StartupPath + "\\UI\\" + Login_Form.dB_User_DataStruct.sUserName + ".jpg");
            Image image = this.pictureBox__UserPhoto.Image;
            Image newImage = Function.CutEllipse(image, new Rectangle(0, 0, 66, 66), new Size(60, 60));
            this.pictureBox__UserPhoto.Image = newImage;

            textBox_UserName.Text = Login_Form.dB_User_DataStruct.sUserName;  //初始化用户名
        }

        private void button_OKPersonlInformation_Click(object sender, EventArgs e)
        {
            if (textBox_NewUserPW.Text == Login_Form.dB_User_DataStruct.sUserName && textBox_NewUserPW.Text == Login_Form.dB_User_DataStruct.sUserPW)
            {
                if (textBox_NewUserPW.Text != "")
                {
                    if (textBox_NewUserPW.Text != textBox_PardonNewUserPW.Text)
                    {
                        MessageBox.Show("两次密码输入不一致，请重新输入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        
                    }
                }
            }
        }

        private void button_CancelPersonlInformation_Click(object sender, EventArgs e)
        {
            sNewPhoto = "";
            textBox_UserName.Text = "";
            textBox_NewUserPW.Text = "";
            textBox_PardonNewUserPW.Text = "";
            this.Close();
        }

        /// <summary>
        /// 选择头像按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_ChoseUserPhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "选择文件";
                dlg.Filter = "jpg|*.jpg|jpeg|*.jpeg|png|*.png";
                dlg.FilterIndex = 1;
                dlg.RestoreDirectory = true;
                dlg.DefaultExt = "jpg";
                DialogResult dr = dlg.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    sNewPhoto = dlg.FileName;
                    Image image = Image.FromFile(sNewPhoto);
                    Image newImage = Function.CutEllipse(image, new Rectangle(0, 0, 66, 66), new Size(60, 60));
                    this.pictureBox__UserPhoto.Image = newImage;
                }
            }
        }

        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void User_Form_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void User_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sNewPhoto != "" || textBox_NewUserPW.Text != "" || textBox_PardonNewUserPW.Text != "")
            {
                if (DialogResult.OK == MessageBox.Show("修改内容未保存，是否退出!", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                {
                    sNewPhoto = "";
                    textBox_UserName.Text = "";
                    textBox_NewUserPW.Text = "";
                    textBox_PardonNewUserPW.Text = "";
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
    }
}

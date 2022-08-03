using GradeMonitorApplication.FunctionClass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace GradeMonitorApplication.WinForm
{
    public partial class Login_Form : Form
    {
        public static DB_User_DataStruct dB_User_DataStruct = new DB_User_DataStruct();
        bool isClosed = true;  //是否直接关闭窗口
        private string sUserListXmlPath = ".\\Document\\登录界面用户列表.xml";  //存储登录界面用户列表

        public Login_Form()
        {
            InitializeComponent();
        }

        private void Login_Form_Load(object sender, EventArgs e)
        {
            List<string> list = ReadXmlFile();
            for (int i = 0;  i < list.Count; i++)
            {
                comboBox_UserID.Items.Add(list[i]);
            }
            if (comboBox_UserID.Items.Count > 0)
            {
                comboBox_UserID.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 点击登录按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Login_Click(object sender, EventArgs e)
        {
            if (MainForm.database.LoginCheck(comboBox_UserID.Text, textBox_UserPW.Text, ref dB_User_DataStruct.bModifyPermission,ref dB_User_DataStruct.sUserPhoto))
            {
                dB_User_DataStruct.sUserName = comboBox_UserID.Text;
                dB_User_DataStruct.sUserPW = textBox_UserPW.Text;
                this.DialogResult = DialogResult.OK;
                isClosed = false;
            }
            else
            {
                MessageBox.Show("您输入用户名或密码不正确，请重新输入!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            #region 记录登录名至默认下列列表
            //读取之前登录的用户文件
            List<string> list = ReadXmlFile();
            XmlDocument xmlDocument = new XmlDocument();
            XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDocument.AppendChild(xmlDeclaration);
            //将当前登录的用户名写入XML文件
            XmlElement xmlElement = xmlDocument.CreateElement("User");
            xmlDocument.AppendChild(xmlElement);
            //如果这个用户没有登录过则添加进XML文件
            if (list.Contains(comboBox_UserID.Text.Trim()))
            {
                XmlElement pName = xmlDocument.CreateElement("UserName");
                pName.InnerText = comboBox_UserID.Text.Trim();
                xmlElement.AppendChild(pName);
                list.Remove(comboBox_UserID.Text.Trim());//删除原来的
            }
            else
            {
                XmlElement pName = xmlDocument.CreateElement("UserName");
                pName.InnerText = comboBox_UserID.Text.Trim();
                xmlElement.AppendChild(pName);
            }

            //设置保存用户名的数量
            int userCount = 0;
            if (list.Count < 7)
            {
                userCount = list.Count;
            }
            else
            {
                userCount = list.Count - 1;
            }
            //将之前保存在XML中
            for (int i = 0; i < userCount; i++)
            {
                XmlElement proName = xmlDocument.CreateElement("UserName");
                proName.InnerText = list[i];
                xmlElement.AppendChild(proName);
            }
            if (File.Exists(sUserListXmlPath))
            {
                File.SetAttributes(sUserListXmlPath, FileAttributes.Normal);
            }
            xmlDocument.Save(sUserListXmlPath);
            File.SetAttributes(sUserListXmlPath, FileAttributes.Normal);
            #endregion
        }

        /// <summary>
        /// 点击退出按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Exist_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No;
        }

        /// <summary>
        /// 窗口关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (isClosed)
            {
                this.DialogResult = DialogResult.No;
            }
        }

        /// <summary>
        /// 窗口关闭前事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Login_Form_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        /// <summary>
        /// 读取XML文件
        /// </summary>
        /// <returns></returns>
        private List<string> ReadXmlFile()
        {
            List<string> list = new List<string>();
            if (File.Exists(sUserListXmlPath))
            {
                comboBox_UserID.DropDownStyle = ComboBoxStyle.DropDown;
                XmlDocument document = new XmlDocument();
                document.Load(sUserListXmlPath);
                XmlElement element = document.DocumentElement;
                XmlNodeList xmlNodeList = element.ChildNodes;
                foreach(XmlNode item in xmlNodeList)
                {
                    list.Add(item.InnerText);
                }
            }
            return list;
        }
    }
}

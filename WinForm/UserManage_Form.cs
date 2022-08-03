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
    public partial class UserManage_Form : Form
    {
        public UserManage_Form()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            comboBox1.Items.AddRange(MainForm.database.GetAllUser().ToArray());
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
                MessageBox.Show("请输入新建用户名！");
            else if (textBox2.Text == "")
                MessageBox.Show("请设置密码！");
            else if (textBox3.Text != textBox2.Text)
                MessageBox.Show("两次输入密码不一致！");
            else if (MainForm.database.CheckUser(textBox1.Text))
                MessageBox.Show("用户ID已存在！");
            else
            {
                if (MainForm.database.AddUser(textBox1.Text, textBox2.Text))
                    MessageBox.Show("创建用户成功");
                else
                    MessageBox.Show("创建用户失败");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox4.Text == "")
                MessageBox.Show("请输入新密码！");
            else if (textBox5.Text != textBox4.Text)
                MessageBox.Show("密码输入不一致");
            else
            {
                if (MainForm.database.ChangePW(comboBox1.SelectedItem.ToString(), textBox4.Text))
                    MessageBox.Show("修改成功");
                else
                    MessageBox.Show("修改失败");
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

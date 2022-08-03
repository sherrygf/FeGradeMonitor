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
    public partial class Config_Form : Form
    {
        public Config_Form()
        {
            InitializeComponent();
        }

        private void Config_Form_Load(object sender, EventArgs e)
        {
            textBoxMZ.Text = MainForm.ipinfo.CameraMZ;
            textBoxPZ.Text = MainForm.ipinfo.CameraPZ;
            textBoxHX.Text = MainForm.ipinfo.ScanerHX;
            textBoxZX.Text = MainForm.ipinfo.ScanerZX;
            textBoxReader.Text = MainForm.ipinfo.RFIDReader;
        }

        private void Button_save_Click(object sender, EventArgs e)
        {
            
            DialogResult result = MessageBox.Show("确定保存IP修改？", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if(result==DialogResult.Yes)
            {
                MainForm.ipinfo.UpdateIP(textBoxMZ.Text, textBoxPZ.Text, textBoxHX.Text, textBoxZX.Text, textBoxReader.Text);
                MessageBox.Show("修改已保存，请重新启动系统以应用更改！");
            }
        }

        private void button_Quit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}


namespace GradeMonitorApplication.WinForm
{
    partial class Config_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label_CameraMZ = new System.Windows.Forms.Label();
            this.label_CameraPZ = new System.Windows.Forms.Label();
            this.label_ScanHX = new System.Windows.Forms.Label();
            this.label_ScanZX = new System.Windows.Forms.Label();
            this.label_RFIDReader = new System.Windows.Forms.Label();
            this.textBoxMZ = new System.Windows.Forms.TextBox();
            this.textBoxPZ = new System.Windows.Forms.TextBox();
            this.textBoxHX = new System.Windows.Forms.TextBox();
            this.textBoxZX = new System.Windows.Forms.TextBox();
            this.textBoxReader = new System.Windows.Forms.TextBox();
            this.Button_save = new System.Windows.Forms.Button();
            this.button_Quit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label_CameraMZ
            // 
            this.label_CameraMZ.AutoSize = true;
            this.label_CameraMZ.Font = new System.Drawing.Font("宋体", 12F);
            this.label_CameraMZ.Location = new System.Drawing.Point(95, 58);
            this.label_CameraMZ.Name = "label_CameraMZ";
            this.label_CameraMZ.Size = new System.Drawing.Size(152, 16);
            this.label_CameraMZ.TabIndex = 0;
            this.label_CameraMZ.Text = "重车轨道摄像头IP：";
            // 
            // label_CameraPZ
            // 
            this.label_CameraPZ.AutoSize = true;
            this.label_CameraPZ.Font = new System.Drawing.Font("宋体", 12F);
            this.label_CameraPZ.Location = new System.Drawing.Point(95, 105);
            this.label_CameraPZ.Name = "label_CameraPZ";
            this.label_CameraPZ.Size = new System.Drawing.Size(152, 16);
            this.label_CameraPZ.TabIndex = 1;
            this.label_CameraPZ.Text = "空车轨道摄像头IP：";
            // 
            // label_ScanHX
            // 
            this.label_ScanHX.AutoSize = true;
            this.label_ScanHX.Font = new System.Drawing.Font("宋体", 12F);
            this.label_ScanHX.Location = new System.Drawing.Point(95, 153);
            this.label_ScanHX.Name = "label_ScanHX";
            this.label_ScanHX.Size = new System.Drawing.Size(152, 16);
            this.label_ScanHX.TabIndex = 2;
            this.label_ScanHX.Text = "横向激光扫描仪IP：";
            // 
            // label_ScanZX
            // 
            this.label_ScanZX.AutoSize = true;
            this.label_ScanZX.Font = new System.Drawing.Font("宋体", 12F);
            this.label_ScanZX.Location = new System.Drawing.Point(95, 202);
            this.label_ScanZX.Name = "label_ScanZX";
            this.label_ScanZX.Size = new System.Drawing.Size(152, 16);
            this.label_ScanZX.TabIndex = 3;
            this.label_ScanZX.Text = "纵向激光扫描仪IP：";
            // 
            // label_RFIDReader
            // 
            this.label_RFIDReader.AutoSize = true;
            this.label_RFIDReader.Font = new System.Drawing.Font("宋体", 12F);
            this.label_RFIDReader.Location = new System.Drawing.Point(95, 256);
            this.label_RFIDReader.Name = "label_RFIDReader";
            this.label_RFIDReader.Size = new System.Drawing.Size(120, 16);
            this.label_RFIDReader.TabIndex = 4;
            this.label_RFIDReader.Text = "RFID读卡器IP：";
            // 
            // textBoxMZ
            // 
            this.textBoxMZ.Font = new System.Drawing.Font("宋体", 12F);
            this.textBoxMZ.Location = new System.Drawing.Point(277, 58);
            this.textBoxMZ.Name = "textBoxMZ";
            this.textBoxMZ.Size = new System.Drawing.Size(340, 26);
            this.textBoxMZ.TabIndex = 5;
            // 
            // textBoxPZ
            // 
            this.textBoxPZ.Font = new System.Drawing.Font("宋体", 12F);
            this.textBoxPZ.Location = new System.Drawing.Point(277, 105);
            this.textBoxPZ.Name = "textBoxPZ";
            this.textBoxPZ.Size = new System.Drawing.Size(340, 26);
            this.textBoxPZ.TabIndex = 6;
            // 
            // textBoxHX
            // 
            this.textBoxHX.Font = new System.Drawing.Font("宋体", 12F);
            this.textBoxHX.Location = new System.Drawing.Point(277, 153);
            this.textBoxHX.Name = "textBoxHX";
            this.textBoxHX.Size = new System.Drawing.Size(340, 26);
            this.textBoxHX.TabIndex = 7;
            // 
            // textBoxZX
            // 
            this.textBoxZX.Font = new System.Drawing.Font("宋体", 12F);
            this.textBoxZX.Location = new System.Drawing.Point(277, 202);
            this.textBoxZX.Name = "textBoxZX";
            this.textBoxZX.Size = new System.Drawing.Size(340, 26);
            this.textBoxZX.TabIndex = 8;
            // 
            // textBoxReader
            // 
            this.textBoxReader.Font = new System.Drawing.Font("宋体", 12F);
            this.textBoxReader.Location = new System.Drawing.Point(277, 256);
            this.textBoxReader.Name = "textBoxReader";
            this.textBoxReader.Size = new System.Drawing.Size(340, 26);
            this.textBoxReader.TabIndex = 9;
            // 
            // Button_save
            // 
            this.Button_save.Font = new System.Drawing.Font("宋体", 12F);
            this.Button_save.Location = new System.Drawing.Point(185, 330);
            this.Button_save.Name = "Button_save";
            this.Button_save.Size = new System.Drawing.Size(123, 38);
            this.Button_save.TabIndex = 10;
            this.Button_save.Text = "保存修改";
            this.Button_save.UseVisualStyleBackColor = true;
            this.Button_save.Click += new System.EventHandler(this.Button_save_Click);
            // 
            // button_Quit
            // 
            this.button_Quit.Font = new System.Drawing.Font("宋体", 12F);
            this.button_Quit.Location = new System.Drawing.Point(438, 330);
            this.button_Quit.Name = "button_Quit";
            this.button_Quit.Size = new System.Drawing.Size(123, 38);
            this.button_Quit.TabIndex = 11;
            this.button_Quit.Text = "不保存";
            this.button_Quit.UseVisualStyleBackColor = true;
            this.button_Quit.Click += new System.EventHandler(this.button_Quit_Click);
            // 
            // Config_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button_Quit);
            this.Controls.Add(this.Button_save);
            this.Controls.Add(this.textBoxReader);
            this.Controls.Add(this.textBoxZX);
            this.Controls.Add(this.textBoxHX);
            this.Controls.Add(this.textBoxPZ);
            this.Controls.Add(this.textBoxMZ);
            this.Controls.Add(this.label_RFIDReader);
            this.Controls.Add(this.label_ScanZX);
            this.Controls.Add(this.label_ScanHX);
            this.Controls.Add(this.label_CameraPZ);
            this.Controls.Add(this.label_CameraMZ);
            this.Name = "Config_Form";
            this.Text = "IP配置";
            this.Load += new System.EventHandler(this.Config_Form_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label_CameraMZ;
        private System.Windows.Forms.Label label_CameraPZ;
        private System.Windows.Forms.Label label_ScanHX;
        private System.Windows.Forms.Label label_ScanZX;
        private System.Windows.Forms.Label label_RFIDReader;
        private System.Windows.Forms.TextBox textBoxMZ;
        private System.Windows.Forms.TextBox textBoxPZ;
        private System.Windows.Forms.TextBox textBoxHX;
        private System.Windows.Forms.TextBox textBoxZX;
        private System.Windows.Forms.TextBox textBoxReader;
        private System.Windows.Forms.Button Button_save;
        private System.Windows.Forms.Button button_Quit;
    }
}
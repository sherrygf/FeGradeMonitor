namespace GradeMonitorApplication.WinForm
{
    partial class Login_Form
    {
        /// <summary>
        /// Required designer variable.必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code。Windows 窗体设计器生成的代码

        /// <summary>
        /// Required method for Designer support - do not modify 设计器支持所需的方法 - 不要
        /// the contents of this method with the code editor.使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox_UserID = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_UserPW = new System.Windows.Forms.TextBox();
            this.button_Login = new System.Windows.Forms.Button();
            this.button_Exist = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(84, 62);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "用户名：";
            // 
            // comboBox_UserID
            // 
            this.comboBox_UserID.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox_UserID.FormattingEnabled = true;
            this.comboBox_UserID.ItemHeight = 20;
            this.comboBox_UserID.Location = new System.Drawing.Point(179, 59);
            this.comboBox_UserID.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox_UserID.Name = "comboBox_UserID";
            this.comboBox_UserID.Size = new System.Drawing.Size(251, 28);
            this.comboBox_UserID.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(84, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "密  码：";
            // 
            // textBox_UserPW
            // 
            this.textBox_UserPW.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_UserPW.Location = new System.Drawing.Point(179, 122);
            this.textBox_UserPW.Name = "textBox_UserPW";
            this.textBox_UserPW.Size = new System.Drawing.Size(251, 30);
            this.textBox_UserPW.TabIndex = 3;
            // 
            // button_Login
            // 
            this.button_Login.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Login.Location = new System.Drawing.Point(110, 196);
            this.button_Login.Name = "button_Login";
            this.button_Login.Size = new System.Drawing.Size(124, 43);
            this.button_Login.TabIndex = 4;
            this.button_Login.Text = "登录";
            this.button_Login.UseVisualStyleBackColor = true;
            this.button_Login.Click += new System.EventHandler(this.button_Login_Click);
            // 
            // button_Exist
            // 
            this.button_Exist.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Exist.Location = new System.Drawing.Point(297, 196);
            this.button_Exist.Name = "button_Exist";
            this.button_Exist.Size = new System.Drawing.Size(124, 43);
            this.button_Exist.TabIndex = 5;
            this.button_Exist.Text = "退出";
            this.button_Exist.UseVisualStyleBackColor = true;
            this.button_Exist.Click += new System.EventHandler(this.button_Exist_Click);
            // 
            // Login_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 268);
            this.Controls.Add(this.button_Exist);
            this.Controls.Add(this.button_Login);
            this.Controls.Add(this.textBox_UserPW);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox_UserID);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Login_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "登录";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Login_Form_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Login_Form_FormClosed);
            this.Load += new System.EventHandler(this.Login_Form_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox_UserID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_UserPW;
        private System.Windows.Forms.Button button_Login;
        private System.Windows.Forms.Button button_Exist;
    }
}
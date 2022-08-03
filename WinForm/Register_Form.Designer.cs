namespace GradeMonitorApplication.WinForm
{
    partial class Register_Form
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
            this.button_Register = new System.Windows.Forms.Button();
            this.button_Exist = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_MNum = new System.Windows.Forms.TextBox();
            this.textBox_RNum = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button_Register
            // 
            this.button_Register.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Register.Location = new System.Drawing.Point(155, 190);
            this.button_Register.Name = "button_Register";
            this.button_Register.Size = new System.Drawing.Size(122, 40);
            this.button_Register.TabIndex = 0;
            this.button_Register.Text = "注册";
            this.button_Register.UseVisualStyleBackColor = true;
            this.button_Register.Click += new System.EventHandler(this.button_Register_Click);
            // 
            // button_Exist
            // 
            this.button_Exist.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Exist.Location = new System.Drawing.Point(350, 190);
            this.button_Exist.Name = "button_Exist";
            this.button_Exist.Size = new System.Drawing.Size(122, 40);
            this.button_Exist.TabIndex = 1;
            this.button_Exist.Text = "退出";
            this.button_Exist.UseVisualStyleBackColor = true;
            this.button_Exist.Click += new System.EventHandler(this.button_Exist_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(79, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "序列号：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(79, 124);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "注册码：";
            // 
            // textBox_MNum
            // 
            this.textBox_MNum.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_MNum.Location = new System.Drawing.Point(174, 59);
            this.textBox_MNum.Name = "textBox_MNum";
            this.textBox_MNum.ReadOnly = true;
            this.textBox_MNum.Size = new System.Drawing.Size(366, 30);
            this.textBox_MNum.TabIndex = 4;
            // 
            // textBox_RNum
            // 
            this.textBox_RNum.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_RNum.Location = new System.Drawing.Point(174, 121);
            this.textBox_RNum.Name = "textBox_RNum";
            this.textBox_RNum.Size = new System.Drawing.Size(366, 30);
            this.textBox_RNum.TabIndex = 5;
            // 
            // Register_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(639, 264);
            this.Controls.Add(this.textBox_RNum);
            this.Controls.Add(this.textBox_MNum);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_Exist);
            this.Controls.Add(this.button_Register);
            this.Name = "Register_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "软件注册";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Register_Form_FormClosing);
            this.Load += new System.EventHandler(this.Register_Form_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Register;
        private System.Windows.Forms.Button button_Exist;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_MNum;
        private System.Windows.Forms.TextBox textBox_RNum;
    }
}
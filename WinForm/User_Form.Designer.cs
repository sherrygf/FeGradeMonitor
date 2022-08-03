namespace GradeMonitorApplication.WinForm
{
    partial class User_Form
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
            this.button_OKPersonlInformation = new System.Windows.Forms.Button();
            this.button_CancelPersonlInformation = new System.Windows.Forms.Button();
            this.label_UserName = new System.Windows.Forms.Label();
            this.label_UserPW = new System.Windows.Forms.Label();
            this.label_NewUserPW = new System.Windows.Forms.Label();
            this.label_PardonNewUserPW = new System.Windows.Forms.Label();
            this.textBox_UserName = new System.Windows.Forms.TextBox();
            this.textBox_UserPW = new System.Windows.Forms.TextBox();
            this.textBox_NewUserPW = new System.Windows.Forms.TextBox();
            this.textBox_PardonNewUserPW = new System.Windows.Forms.TextBox();
            this.pictureBox__UserPhoto = new System.Windows.Forms.PictureBox();
            this.button_ChoseUserPhoto = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox__UserPhoto)).BeginInit();
            this.SuspendLayout();
            // 
            // button_OKPersonlInformation
            // 
            this.button_OKPersonlInformation.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_OKPersonlInformation.Location = new System.Drawing.Point(69, 335);
            this.button_OKPersonlInformation.Name = "button_OKPersonlInformation";
            this.button_OKPersonlInformation.Size = new System.Drawing.Size(124, 43);
            this.button_OKPersonlInformation.TabIndex = 5;
            this.button_OKPersonlInformation.Text = "确定";
            this.button_OKPersonlInformation.UseVisualStyleBackColor = true;
            this.button_OKPersonlInformation.Click += new System.EventHandler(this.button_OKPersonlInformation_Click);
            // 
            // button_CancelPersonlInformation
            // 
            this.button_CancelPersonlInformation.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_CancelPersonlInformation.Location = new System.Drawing.Point(255, 335);
            this.button_CancelPersonlInformation.Name = "button_CancelPersonlInformation";
            this.button_CancelPersonlInformation.Size = new System.Drawing.Size(124, 43);
            this.button_CancelPersonlInformation.TabIndex = 6;
            this.button_CancelPersonlInformation.Text = "取消";
            this.button_CancelPersonlInformation.UseVisualStyleBackColor = true;
            this.button_CancelPersonlInformation.Click += new System.EventHandler(this.button_CancelPersonlInformation_Click);
            // 
            // label_UserName
            // 
            this.label_UserName.AutoSize = true;
            this.label_UserName.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_UserName.Location = new System.Drawing.Point(65, 174);
            this.label_UserName.Name = "label_UserName";
            this.label_UserName.Size = new System.Drawing.Size(89, 20);
            this.label_UserName.TabIndex = 7;
            this.label_UserName.Text = "用户名：";
            // 
            // label_UserPW
            // 
            this.label_UserPW.AutoSize = true;
            this.label_UserPW.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_UserPW.Location = new System.Drawing.Point(65, 210);
            this.label_UserPW.Name = "label_UserPW";
            this.label_UserPW.Size = new System.Drawing.Size(89, 20);
            this.label_UserPW.TabIndex = 8;
            this.label_UserPW.Text = "原密码：";
            // 
            // label_NewUserPW
            // 
            this.label_NewUserPW.AutoSize = true;
            this.label_NewUserPW.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_NewUserPW.Location = new System.Drawing.Point(65, 246);
            this.label_NewUserPW.Name = "label_NewUserPW";
            this.label_NewUserPW.Size = new System.Drawing.Size(89, 20);
            this.label_NewUserPW.TabIndex = 9;
            this.label_NewUserPW.Text = "新密码：";
            // 
            // label_PardonNewUserPW
            // 
            this.label_PardonNewUserPW.AutoSize = true;
            this.label_PardonNewUserPW.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_PardonNewUserPW.Location = new System.Drawing.Point(65, 278);
            this.label_PardonNewUserPW.Name = "label_PardonNewUserPW";
            this.label_PardonNewUserPW.Size = new System.Drawing.Size(89, 20);
            this.label_PardonNewUserPW.TabIndex = 10;
            this.label_PardonNewUserPW.Text = "新密码：";
            // 
            // textBox_UserName
            // 
            this.textBox_UserName.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_UserName.Location = new System.Drawing.Point(160, 167);
            this.textBox_UserName.Name = "textBox_UserName";
            this.textBox_UserName.ReadOnly = true;
            this.textBox_UserName.Size = new System.Drawing.Size(219, 30);
            this.textBox_UserName.TabIndex = 11;
            // 
            // textBox_UserPW
            // 
            this.textBox_UserPW.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_UserPW.Location = new System.Drawing.Point(160, 203);
            this.textBox_UserPW.Name = "textBox_UserPW";
            this.textBox_UserPW.Size = new System.Drawing.Size(219, 30);
            this.textBox_UserPW.TabIndex = 12;
            // 
            // textBox_NewUserPW
            // 
            this.textBox_NewUserPW.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_NewUserPW.Location = new System.Drawing.Point(160, 239);
            this.textBox_NewUserPW.Name = "textBox_NewUserPW";
            this.textBox_NewUserPW.Size = new System.Drawing.Size(219, 30);
            this.textBox_NewUserPW.TabIndex = 13;
            // 
            // textBox_PardonNewUserPW
            // 
            this.textBox_PardonNewUserPW.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox_PardonNewUserPW.Location = new System.Drawing.Point(160, 275);
            this.textBox_PardonNewUserPW.Name = "textBox_PardonNewUserPW";
            this.textBox_PardonNewUserPW.Size = new System.Drawing.Size(219, 30);
            this.textBox_PardonNewUserPW.TabIndex = 14;
            // 
            // pictureBox__UserPhoto
            // 
            this.pictureBox__UserPhoto.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox__UserPhoto.Location = new System.Drawing.Point(184, 13);
            this.pictureBox__UserPhoto.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox__UserPhoto.Name = "pictureBox__UserPhoto";
            this.pictureBox__UserPhoto.Size = new System.Drawing.Size(80, 80);
            this.pictureBox__UserPhoto.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox__UserPhoto.TabIndex = 15;
            this.pictureBox__UserPhoto.TabStop = false;
            // 
            // button_ChoseUserPhoto
            // 
            this.button_ChoseUserPhoto.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_ChoseUserPhoto.Location = new System.Drawing.Point(160, 100);
            this.button_ChoseUserPhoto.Name = "button_ChoseUserPhoto";
            this.button_ChoseUserPhoto.Size = new System.Drawing.Size(131, 28);
            this.button_ChoseUserPhoto.TabIndex = 16;
            this.button_ChoseUserPhoto.Text = "选择头像";
            this.button_ChoseUserPhoto.UseVisualStyleBackColor = true;
            this.button_ChoseUserPhoto.Click += new System.EventHandler(this.button_ChoseUserPhoto_Click);
            // 
            // User_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 434);
            this.Controls.Add(this.button_ChoseUserPhoto);
            this.Controls.Add(this.pictureBox__UserPhoto);
            this.Controls.Add(this.textBox_PardonNewUserPW);
            this.Controls.Add(this.textBox_NewUserPW);
            this.Controls.Add(this.textBox_UserPW);
            this.Controls.Add(this.textBox_UserName);
            this.Controls.Add(this.label_PardonNewUserPW);
            this.Controls.Add(this.label_NewUserPW);
            this.Controls.Add(this.label_UserPW);
            this.Controls.Add(this.label_UserName);
            this.Controls.Add(this.button_CancelPersonlInformation);
            this.Controls.Add(this.button_OKPersonlInformation);
            this.MaximizeBox = false;
            this.Name = "User_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "个人信息";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.User_Form_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.User_Form_FormClosed);
            this.Load += new System.EventHandler(this.User_Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox__UserPhoto)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_OKPersonlInformation;
        private System.Windows.Forms.Button button_CancelPersonlInformation;
        private System.Windows.Forms.Label label_UserName;
        private System.Windows.Forms.Label label_UserPW;
        private System.Windows.Forms.Label label_NewUserPW;
        private System.Windows.Forms.Label label_PardonNewUserPW;
        private System.Windows.Forms.TextBox textBox_UserName;
        private System.Windows.Forms.TextBox textBox_UserPW;
        private System.Windows.Forms.TextBox textBox_NewUserPW;
        private System.Windows.Forms.TextBox textBox_PardonNewUserPW;
        private System.Windows.Forms.PictureBox pictureBox__UserPhoto;
        private System.Windows.Forms.Button button_ChoseUserPhoto;
    }
}
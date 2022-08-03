
namespace GradeMonitorApplication.WinForm
{
    partial class ErrorReport_Form
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label6 = new System.Windows.Forms.Label();
            this.dateTimePicker_HistoryBegin = new System.Windows.Forms.DateTimePicker();
            this.button_HistoryQuery = new System.Windows.Forms.Button();
            this.dateTimePicker_HistoryEnd = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.listView1.Font = new System.Drawing.Font("宋体", 12F);
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(860, 365);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "FID";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "测量时间";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 250;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "错误信息";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 450;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 13F);
            this.label6.Location = new System.Drawing.Point(373, 400);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(26, 18);
            this.label6.TabIndex = 30;
            this.label6.Text = "至";
            // 
            // dateTimePicker_HistoryBegin
            // 
            this.dateTimePicker_HistoryBegin.Font = new System.Drawing.Font("宋体", 13F);
            this.dateTimePicker_HistoryBegin.Location = new System.Drawing.Point(210, 395);
            this.dateTimePicker_HistoryBegin.Name = "dateTimePicker_HistoryBegin";
            this.dateTimePicker_HistoryBegin.Size = new System.Drawing.Size(157, 27);
            this.dateTimePicker_HistoryBegin.TabIndex = 29;
            // 
            // button_HistoryQuery
            // 
            this.button_HistoryQuery.Location = new System.Drawing.Point(569, 389);
            this.button_HistoryQuery.Name = "button_HistoryQuery";
            this.button_HistoryQuery.Size = new System.Drawing.Size(100, 40);
            this.button_HistoryQuery.TabIndex = 28;
            this.button_HistoryQuery.Text = "查询";
            this.button_HistoryQuery.UseVisualStyleBackColor = true;
            this.button_HistoryQuery.Click += new System.EventHandler(this.button_HistoryQuery_Click);
            // 
            // dateTimePicker_HistoryEnd
            // 
            this.dateTimePicker_HistoryEnd.Font = new System.Drawing.Font("宋体", 13F);
            this.dateTimePicker_HistoryEnd.Location = new System.Drawing.Point(407, 395);
            this.dateTimePicker_HistoryEnd.Name = "dateTimePicker_HistoryEnd";
            this.dateTimePicker_HistoryEnd.Size = new System.Drawing.Size(157, 27);
            this.dateTimePicker_HistoryEnd.TabIndex = 27;
            // 
            // ErrorReport_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 450);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dateTimePicker_HistoryBegin);
            this.Controls.Add(this.button_HistoryQuery);
            this.Controls.Add(this.dateTimePicker_HistoryEnd);
            this.Controls.Add(this.listView1);
            this.Name = "ErrorReport_Form";
            this.Text = "异常数据报告";
            this.Load += new System.EventHandler(this.ErrorReport_Form_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dateTimePicker_HistoryBegin;
        private System.Windows.Forms.Button button_HistoryQuery;
        private System.Windows.Forms.DateTimePicker dateTimePicker_HistoryEnd;
    }
}
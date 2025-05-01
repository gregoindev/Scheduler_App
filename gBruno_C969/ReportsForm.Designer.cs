namespace gBruno_C969
{
    partial class ReportsForm
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
            this.dgvReports = new System.Windows.Forms.DataGridView();
            this.btnUserSchedule = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReports)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvReports
            // 
            this.dgvReports.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReports.Location = new System.Drawing.Point(146, 33);
            this.dgvReports.Name = "dgvReports";
            this.dgvReports.Size = new System.Drawing.Size(472, 173);
            this.dgvReports.TabIndex = 0;
            // 
            // btnUserSchedule
            // 
            this.btnUserSchedule.Location = new System.Drawing.Point(146, 265);
            this.btnUserSchedule.Name = "btnUserSchedule";
            this.btnUserSchedule.Size = new System.Drawing.Size(148, 57);
            this.btnUserSchedule.TabIndex = 1;
            this.btnUserSchedule.Text = "Schedule by User";
            this.btnUserSchedule.UseVisualStyleBackColor = true;
            this.btnUserSchedule.Click += new System.EventHandler(this.btnUserSchedule_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(510, 265);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(118, 57);
            this.button1.TabIndex = 2;
            this.button1.Text = "View Customers Without Appointments";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ReportsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnUserSchedule);
            this.Controls.Add(this.dgvReports);
            this.Name = "ReportsForm";
            this.Text = "ReportsForm";
            this.Load += new System.EventHandler(this.ReportsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReports)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvReports;
        private System.Windows.Forms.Button btnUserSchedule;
        private System.Windows.Forms.Button button1;
    }
}
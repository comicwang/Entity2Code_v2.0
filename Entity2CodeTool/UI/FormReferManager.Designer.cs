namespace Infoearth.Entity2CodeTool
{
    partial class FormReferManager
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.pic = new System.Windows.Forms.PictureBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnwriteProject = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ReferName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.currentVesion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ModifyTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Update = new System.Windows.Forms.DataGridViewLinkColumn();
            this.ReferPath = new DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.pic);
            this.panel1.Controls.Add(this.lblTitle);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(749, 86);
            this.panel1.TabIndex = 1;
            // 
            // pic
            // 
            this.pic.Location = new System.Drawing.Point(1, 0);
            this.pic.Name = "pic";
            this.pic.Size = new System.Drawing.Size(100, 86);
            this.pic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic.TabIndex = 1;
            this.pic.TabStop = false;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTitle.Location = new System.Drawing.Point(119, 36);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(96, 12);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "管理引用程序集";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnwriteProject);
            this.panel3.Controls.Add(this.btnCancel);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 564);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(749, 57);
            this.panel3.TabIndex = 4;
            // 
            // btnwriteProject
            // 
            this.btnwriteProject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnwriteProject.Location = new System.Drawing.Point(549, 13);
            this.btnwriteProject.Name = "btnwriteProject";
            this.btnwriteProject.Size = new System.Drawing.Size(94, 32);
            this.btnwriteProject.TabIndex = 11;
            this.btnwriteProject.Text = "更新项目(&W)";
            this.btnwriteProject.UseVisualStyleBackColor = true;
            this.btnwriteProject.Visible = false;
            this.btnwriteProject.Click += new System.EventHandler(this.btnwriteProject_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(649, 13);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 32);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "关闭";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ReferName,
            this.currentVesion,
            this.ModifyTime,
            this.Update,
            this.ReferPath});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 86);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(749, 478);
            this.dataGridView1.TabIndex = 5;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // ReferName
            // 
            this.ReferName.HeaderText = "名称";
            this.ReferName.Name = "ReferName";
            this.ReferName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ReferName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // currentVesion
            // 
            this.currentVesion.HeaderText = "当前版本";
            this.currentVesion.Name = "currentVesion";
            this.currentVesion.ReadOnly = true;
            this.currentVesion.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.currentVesion.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ModifyTime
            // 
            this.ModifyTime.HeaderText = "程序集修改时间";
            this.ModifyTime.Name = "ModifyTime";
            this.ModifyTime.ReadOnly = true;
            this.ModifyTime.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // Update
            // 
            this.Update.FillWeight = 50F;
            this.Update.HeaderText = "操作";
            this.Update.Name = "Update";
            this.Update.ReadOnly = true;
            this.Update.Text = "更新";
            // 
            // ReferPath
            // 
            this.ReferPath.HeaderText = "文件路径";
            this.ReferPath.Name = "ReferPath";
            this.ReferPath.ReadOnly = true;
            this.ReferPath.Visible = false;
            // 
            // FormReferManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(749, 621);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.Name = "FormReferManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "项目引用程序集管理";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pic)).EndInit();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pic;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnwriteProject;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReferName;
        private System.Windows.Forms.DataGridViewTextBoxColumn currentVesion;
        private System.Windows.Forms.DataGridViewTextBoxColumn ModifyTime;
        private System.Windows.Forms.DataGridViewLinkColumn Update;
        private DevComponents.DotNetBar.Controls.DataGridViewLabelXColumn ReferPath;
    }
}
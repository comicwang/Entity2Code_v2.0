namespace Infoearth.Entity2CodeTool
{
    partial class FormModelManager
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
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.rcBoxContect = new System.Windows.Forms.RichTextBox();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.ckAll = new System.Windows.Forms.CheckBox();
            this.btnKeywordView = new System.Windows.Forms.Button();
            this.btnInsertMethod = new System.Windows.Forms.Button();
            this.btnwriteProject = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.pnlMain.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(861, 86);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(118, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "管理模型内容";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 86);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pnlMain
            // 
            this.pnlMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMain.Controls.Add(this.rcBoxContect);
            this.pnlMain.Controls.Add(this.pnlLeft);
            this.pnlMain.Location = new System.Drawing.Point(0, 86);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(861, 555);
            this.pnlMain.TabIndex = 2;
            // 
            // rcBoxContect
            // 
            this.rcBoxContect.Location = new System.Drawing.Point(173, -1);
            this.rcBoxContect.Name = "rcBoxContect";
            this.rcBoxContect.Size = new System.Drawing.Size(687, 556);
            this.rcBoxContect.TabIndex = 2;
            this.rcBoxContect.Text = "";
            // 
            // pnlLeft
            // 
            this.pnlLeft.AutoScroll = true;
            this.pnlLeft.Location = new System.Drawing.Point(2, 0);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(170, 555);
            this.pnlLeft.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.ckAll);
            this.panel3.Controls.Add(this.btnKeywordView);
            this.panel3.Controls.Add(this.btnInsertMethod);
            this.panel3.Controls.Add(this.btnwriteProject);
            this.panel3.Controls.Add(this.btnSave);
            this.panel3.Controls.Add(this.btnCancel);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 643);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(861, 57);
            this.panel3.TabIndex = 3;
            // 
            // ckAll
            // 
            this.ckAll.AutoSize = true;
            this.ckAll.Location = new System.Drawing.Point(12, 24);
            this.ckAll.Name = "ckAll";
            this.ckAll.Size = new System.Drawing.Size(96, 16);
            this.ckAll.TabIndex = 11;
            this.ckAll.Text = "显示所有模型";
            this.ckAll.UseVisualStyleBackColor = true;
            this.ckAll.CheckedChanged += new System.EventHandler(this.ckAll_CheckedChanged);
            // 
            // btnKeywordView
            // 
            this.btnKeywordView.Location = new System.Drawing.Point(315, 15);
            this.btnKeywordView.Name = "btnKeywordView";
            this.btnKeywordView.Size = new System.Drawing.Size(95, 32);
            this.btnKeywordView.TabIndex = 10;
            this.btnKeywordView.Text = "<<关键字(&K)";
            this.btnKeywordView.UseVisualStyleBackColor = true;
            this.btnKeywordView.Click += new System.EventHandler(this.btnKeywordView_Click);
            // 
            // btnInsertMethod
            // 
            this.btnInsertMethod.Location = new System.Drawing.Point(421, 15);
            this.btnInsertMethod.Name = "btnInsertMethod";
            this.btnInsertMethod.Size = new System.Drawing.Size(95, 32);
            this.btnInsertMethod.TabIndex = 10;
            this.btnInsertMethod.Text = "插入方法(&I)>>";
            this.btnInsertMethod.UseVisualStyleBackColor = true;
            this.btnInsertMethod.Click += new System.EventHandler(this.btnInsertModel_Click);
            // 
            // btnwriteProject
            // 
            this.btnwriteProject.Location = new System.Drawing.Point(529, 14);
            this.btnwriteProject.Name = "btnwriteProject";
            this.btnwriteProject.Size = new System.Drawing.Size(94, 32);
            this.btnwriteProject.TabIndex = 10;
            this.btnwriteProject.Text = "更新项目(&W)";
            this.btnwriteProject.UseVisualStyleBackColor = true;
            this.btnwriteProject.Click += new System.EventHandler(this.btnwriteProject_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(635, 14);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(89, 32);
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "保存(&S)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(738, 14);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 32);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "关闭";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FormModelManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(861, 700);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.Name = "FormModelManager";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "模型管理";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.pnlMain.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnwriteProject;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnInsertMethod;
        private System.Windows.Forms.CheckBox ckAll;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Button btnKeywordView;
        private System.Windows.Forms.RichTextBox rcBoxContect;
    }
}
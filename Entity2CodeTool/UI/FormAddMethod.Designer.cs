namespace Infoearth.Entity2CodeTool.UI
{
    partial class FormAddMethod
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnInsert = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.VarName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VarType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.IsOut = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtComment = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtAuthor = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtCompany = new System.Windows.Forms.TextBox();
            this.cmbReposi = new System.Windows.Forms.ComboBox();
            this.cmbReturn = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtReComment = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "仓储：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "名称：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(207, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "返回值：";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(78, 42);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(123, 21);
            this.txtName.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnInsert);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 341);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(399, 49);
            this.panel1.TabIndex = 2;
            // 
            // btnInsert
            // 
            this.btnInsert.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInsert.Location = new System.Drawing.Point(310, 14);
            this.btnInsert.Name = "btnInsert";
            this.btnInsert.Size = new System.Drawing.Size(75, 23);
            this.btnInsert.TabIndex = 0;
            this.btnInsert.Text = "插入";
            this.btnInsert.UseVisualStyleBackColor = true;
            this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.CausesValidation = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.VarName,
            this.VarType,
            this.IsOut,
            this.Comment});
            this.dataGridView1.Location = new System.Drawing.Point(12, 155);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(375, 139);
            this.dataGridView1.TabIndex = 5;
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            // 
            // VarName
            // 
            this.VarName.HeaderText = "名称";
            this.VarName.Name = "VarName";
            // 
            // VarType
            // 
            this.VarType.HeaderText = "类型";
            this.VarType.Name = "VarType";
            this.VarType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.VarType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // IsOut
            // 
            this.IsOut.FalseValue = "0";
            this.IsOut.HeaderText = "Out";
            this.IsOut.Name = "IsOut";
            this.IsOut.TrueValue = "1";
            // 
            // Comment
            // 
            this.Comment.HeaderText = "注释";
            this.Comment.Name = "Comment";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Blue;
            this.label4.Location = new System.Drawing.Point(176, 132);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "<参数>";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(31, 112);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "注释：";
            // 
            // txtComment
            // 
            this.txtComment.Location = new System.Drawing.Point(78, 108);
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(307, 21);
            this.txtComment.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 313);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "作者：";
            // 
            // txtAuthor
            // 
            this.txtAuthor.Location = new System.Drawing.Point(78, 309);
            this.txtAuthor.Name = "txtAuthor";
            this.txtAuthor.Size = new System.Drawing.Size(131, 21);
            this.txtAuthor.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(215, 313);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "机构：";
            // 
            // txtCompany
            // 
            this.txtCompany.Location = new System.Drawing.Point(259, 309);
            this.txtCompany.Name = "txtCompany";
            this.txtCompany.Size = new System.Drawing.Size(126, 21);
            this.txtCompany.TabIndex = 1;
            // 
            // cmbReposi
            // 
            this.cmbReposi.FormattingEnabled = true;
            this.cmbReposi.Location = new System.Drawing.Point(78, 12);
            this.cmbReposi.Name = "cmbReposi";
            this.cmbReposi.Size = new System.Drawing.Size(307, 20);
            this.cmbReposi.TabIndex = 7;
            this.cmbReposi.DropDown += new System.EventHandler(this.cmbReposi_DropDown);
            this.cmbReposi.SelectedIndexChanged += new System.EventHandler(this.cmbReposi_SelectedIndexChanged);
            // 
            // cmbReturn
            // 
            this.cmbReturn.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbReturn.Enabled = false;
            this.cmbReturn.FormattingEnabled = true;
            this.cmbReturn.Location = new System.Drawing.Point(259, 43);
            this.cmbReturn.Name = "cmbReturn";
            this.cmbReturn.Size = new System.Drawing.Size(128, 22);
            this.cmbReturn.TabIndex = 3;
            this.cmbReturn.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cmbReturn_DrawItem);
            this.cmbReturn.DropDown += new System.EventHandler(this.cmbReturn_DropDown);
            this.cmbReturn.SelectedIndexChanged += new System.EventHandler(this.cmbReturn_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 79);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "返回注释：";
            // 
            // txtReComment
            // 
            this.txtReComment.Location = new System.Drawing.Point(78, 75);
            this.txtReComment.Name = "txtReComment";
            this.txtReComment.Size = new System.Drawing.Size(307, 21);
            this.txtReComment.TabIndex = 1;
            // 
            // FormAddMethod
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 390);
            this.Controls.Add(this.cmbReposi);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.cmbReturn);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.txtCompany);
            this.Controls.Add(this.txtAuthor);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtReComment);
            this.Controls.Add(this.txtComment);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAddMethod";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "插入方法";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnInsert;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtComment;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtAuthor;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtCompany;
        private System.Windows.Forms.ComboBox cmbReposi;
        private System.Windows.Forms.ComboBox cmbReturn;
        private System.Windows.Forms.DataGridViewTextBoxColumn VarName;
        private System.Windows.Forms.DataGridViewComboBoxColumn VarType;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsOut;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtReComment;
    }
}
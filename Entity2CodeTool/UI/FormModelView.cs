using Infoearth.Entity2CodeTool.Helps;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utility;
using Utility.Common;

namespace Infoearth.Entity2CodeTool.UI
{
    public partial class FormModelView : Form
    {
        public FormModelView()
        {
            InitializeComponent();
            IniDgv();
        }

        private void IniDgv()
        {
            dataGridView1.Rows.Clear();
            foreach (var kv in KeywordContainer.GetAll())
            {
                int index = this.dataGridView1.Rows.Add();
                dataGridView1[0, index].Value = kv.Key;
                dataGridView1[1, index].Value = kv.Value;            
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            //if (e.ColumnIndex == 5)
            //{
            //    //删除
            //    FileOprateHelp.RemoveKeyComment(dataGridView1[0, rowIndex].Value.ToString());
            //    ModelContainer.Remove(dataGridView1[0, rowIndex].Value.ToString());
            //    dataGridView1.Rows.RemoveAt(rowIndex);
            //    if (rowIndex == dataGridView1.Rows.Count - 1)
            //        rowIndex--;
            //}
            if (dataGridView1[0, rowIndex].Value == null)
                return;
            this.textBox1.Text = dataGridView1[0, rowIndex].Value.ToString();
            this.textBox2.Text = dataGridView1[1, rowIndex].Value == null ? String.Empty : dataGridView1[1, rowIndex].Value.ToString();
        }

        private void SetTextEnable(bool enable)
        {
            this.textBox1.ReadOnly = !enable;
            this.textBox2.ReadOnly = !enable;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText(this.textBox1.Text);
            MsgBoxHelp.ShowInfo("已拷贝到剪贴板！");
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            SetTextEnable(true);
            textBox1.Clear();
            textBox2.Clear();
            this.textBox1.Focus();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        //    if (operate == 2)
        //        return;
        //    if (_lock == false && String.Compare(textBox3.Text, oldText) != 0)
        //        operate = 1;
        //    else
        //        operate = 0;
        //    button2.Enabled = operate > 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) == true)
            {
                MsgBoxHelp.ShowWorning("关键字名称不能为空！");
                return;
            }
            if (string.IsNullOrEmpty(CommonContainer.SolutionPath))
                return;
            string xmlPath = Path.Combine(CommonContainer.SolutionPath, CommonContainer.xmlName);
            xmlManager.WriteModel(textBox1.Text, textBox2.Text,xmlPath);
            IniDgv();
        }

        private void btnRefreash_Click(object sender, EventArgs e)
        {
            IniDgv();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

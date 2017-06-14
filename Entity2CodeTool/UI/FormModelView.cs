using Infoearth.Entity2CodeTool.Helps;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Infoearth.Entity2CodeTool.UI
{
    public partial class FormModelView : Form
    {
        private int operate = 0;//0-view 1-modify 2-add
        private string oldText = string.Empty;
        private bool _lock = false;

        public FormModelView()
        {
            InitializeComponent();
            IniDgv();
        }

        private void IniDgv(int rowIndex = 0)
        {
            dataGridView1.Rows.Clear();
            foreach (ContainerModel kv in ModelContainer.Models)
            {
                int index = this.dataGridView1.Rows.Add();
                dataGridView1[0, index].Value = kv.Key;
                dataGridView1[1, index].Value = kv.Value;
                dataGridView1[2, index].Value = kv.Comment;
                dataGridView1[3, index].Value = kv.LastModifyTime;
                dataGridView1[4, index].Value = kv.ModelType;
                if (kv.ModelType == 1)
                    dataGridView1[5, index].Value = "删除";
            }
            dataGridView1.Rows.Add();
            dataGridView1_CellContentClick(null, new DataGridViewCellEventArgs(0, rowIndex));
            SetTextEnable(false);
            operate = 0;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;
            if (e.ColumnIndex == 5)
            {
                //删除
                FileOprateHelp.RemoveKeyComment(dataGridView1[0, rowIndex].Value.ToString());
                ModelContainer.Remove(dataGridView1[0, rowIndex].Value.ToString());
                dataGridView1.Rows.RemoveAt(rowIndex);
                if (rowIndex == dataGridView1.Rows.Count - 1)
                    rowIndex--;
            }
            if (dataGridView1[0, rowIndex].Value == null)
                return;
            this.textBox1.Text = dataGridView1[0, rowIndex].Value.ToString();
            this.textBox2.Text = dataGridView1[1, rowIndex].Value == null ? String.Empty : dataGridView1[1, rowIndex].Value.ToString();
            _lock = true;
            this.textBox3.Text = dataGridView1[2, rowIndex].Value == null ? String.Empty : dataGridView1[2, rowIndex].Value.ToString();
            oldText = this.textBox3.Text;
            _lock = false;
            operate = 0;
            SetTextEnable(int.Parse(dataGridView1[4, rowIndex].Value.ToString()) > 0);

        }

        private void SetTextEnable(bool enable)
        {
            this.textBox1.ReadOnly = !(operate == 2) || !enable;
            this.textBox2.ReadOnly = !enable;
            this.textBox3.ReadOnly = !enable;
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
            textBox3.Clear();
            this.textBox1.Focus();
            operate = 2;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (operate == 2)
                return;
            if (_lock == false && String.Compare(textBox3.Text, oldText) != 0)
                operate = 1;
            else
                operate = 0;
            button2.Enabled = operate > 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) == true)
            {
                MsgBoxHelp.ShowWorning("关键字名称不能为空！");
                return;
            }
            if (operate == 1)
            {
                FileOprateHelp.SetKeyComment(textBox1.Text, textBox3.Text, textBox2.Text);
                //IniDgv(dataGridView1.SelectedRows[0].Index);
                MsgBoxHelp.ShowInfo("修改成功！");
            }
            else if (operate == 2)
            {
                FileOprateHelp.SetKeyComment(textBox1.Text, textBox3.Text, textBox2.Text);
                // IniDgv(dataGridView1.Rows.Count);
                MsgBoxHelp.ShowInfo("插入成功！");
            }
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

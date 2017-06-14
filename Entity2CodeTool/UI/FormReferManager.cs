using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infoearth.Entity2CodeTool.Helps;
using Infoearth.Entity2CodeTool.Model;
using Infoearth.Entity2CodeTool.Logic.UI;
using System.IO;
using System.Reflection;

namespace Infoearth.Entity2CodeTool
{
    public partial class FormReferManager : Form
    {
        public FormReferManager()
        {
            InitializeComponent();
            pic.Image = "img.jpg".GetImageResource("Img");
            IniDgv();
        }

        private void IniDgv()
        {
            List<ReferManageArgment> models = ReferManageLogic.GetAssmblys();
           // dataGridView1.DataSource = models;
            foreach (ReferManageArgment item in models)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1[0, index].Value = item.ReferName;
                dataGridView1[1, index].Value = item.currentVesion;
                dataGridView1[2, index].Value = item.ModifyTime;
                dataGridView1[3, index].Value = "更新";
                dataGridView1[4, index].Value = item.Path;
            }
            dataGridView1.Rows.Add();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 3)
                {
                    OpenFileDialog dlg = new OpenFileDialog();
                    dlg.Filter = "应用程序集（.dll）|*.dll";
                    dlg.Multiselect = false;
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        string sourcePath = dlg.FileName;
                        int index = e.RowIndex;
                        string name = Path.GetFileNameWithoutExtension(sourcePath);
                        if (string.Compare(name, dataGridView1[0, index].Value.ToString(), true) == 0)
                        {
                            File.Copy(sourcePath, dataGridView1[4, index].Value.ToString(), true);
                            ReferManageArgment arg = ReferManageLogic.GetReferInfo(dataGridView1[4, index].Value.ToString());
                            dataGridView1[0, index].Value = arg.ReferName;
                            dataGridView1[1, index].Value = arg.currentVesion;
                            dataGridView1[2, index].Value = arg.ModifyTime;
                            dataGridView1[3, index].Value = "更新";
                            dataGridView1[4, index].Value = arg.Path;
                            dataGridView1.Refresh();
                        }
                        else
                        {
                            MsgBoxHelp.ShowWorning("引用程序集名称和源名称不一致,请选择正确的程序集");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MsgBoxHelp.ShowError("更新程序集错误", ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnwriteProject_Click(object sender, EventArgs e)
        {
            //ProjectContainer.UpdateReference(ReferManageLogic.GetAssmblys());
            
        }
    }
}

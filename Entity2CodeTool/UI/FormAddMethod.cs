using DevComponents.DotNetBar.Controls;
using Infoearth.Entity2CodeTool.Container;
using Infoearth.Entity2CodeTool.Helps;
using Infoearth.Entity2CodeTool.Logic;
using Infoearth.Entity2CodeTool.Logic.UI;
using Infoearth.Entity2CodeTool.Model;
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
    public partial class FormAddMethod : Form
    {
        private TemplateEntity _selectEntity = null;
        private List<TemplateEntity> _entitys = null;
        public FormAddMethod()
        {
            InitializeComponent();
            dataGridView1.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(dgView_EditingControlShowing);

            List<string> customs = VarCsharp.AllTypes(false);
            VarType.Items.Add("<复杂类型>");
            VarType.Items.AddRange(customs.ToArray());           
        }

        private void dgView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (this.dataGridView1.CurrentCell.OwningColumn.Name == VarType.Name && dataGridView1.CurrentCell.RowIndex != -1)
            {
                ComboBox box = e.Control as ComboBox;
                box.DrawMode = DrawMode.OwnerDrawVariable;

                box.SelectedIndexChanged -= new EventHandler(cmbReturn_SelectedIndexChanged);
                box.SelectedIndexChanged += new EventHandler(cmbReturn_SelectedIndexChanged);

                box.DrawItem -= new DrawItemEventHandler(cmbReturn_DrawItem);
                box.DrawItem += new DrawItemEventHandler(cmbReturn_DrawItem);
            }
        }


        private void cmbReposi_DropDown(object sender, EventArgs e)
        {
            try
            {
                EnvDTE.DTE dte = Model.SolutionCommon.Dte;
                bool result = LoadProjectLogic.Load(dte);
                //加载所有表信息
                if (result)
                {
                    _entitys = LoadProjectLogic.GetEntitys();
                    cmbReposi.DataSource = _entitys;
                    cmbReposi.DisplayMember = "Entity";
                    cmbReposi.ValueMember = "Data2Obj";
                    cmbReturn.Enabled = true;
                }
                else
                {
                    MsgBoxHelp.ShowWorning("加载仓储列表出错！");
                }
            }
            catch (Exception ex)
            {
                MsgBoxHelp.ShowError(ex.Message, ex);
            }
        }

        private void cmbReturn_DropDown(object sender, EventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            List<string> customs = VarCsharp.AllTypes(false);
            cmb.Items.Clear();
            cmb.Items.Add("<复杂类型>");
            cmb.Items.AddRange(customs.ToArray());
        }

        private void cmbReturn_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            //确定画布  
            Graphics g = e.Graphics;
            //绘制区域  
            Rectangle r = e.Bounds;

            Font fn = new Font("宋体", 9.0f);
            //Font fn = null;
            if (e.Index >= 0)
            {
                //设置字体、字符串格式、对齐方式  
                // fn = (Font)fontArray[e.Index];
                string s = (string)cmb.Items[e.Index];
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Near;
                //根据不同的状态用不同的颜色表示  
                if (e.State == (DrawItemState.NoAccelerator | DrawItemState.NoFocusRect))
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.White), r);
                    e.Graphics.DrawString(s, fn, e.Index == 0 ? new SolidBrush(Color.Blue) : new SolidBrush(Color.Black), r, sf);
                    e.DrawFocusRectangle();
                }
                else if (e.Index != 0)
                {
                    e.Graphics.FillRectangle(new SolidBrush(SystemColors.MenuHighlight), r);
                    e.Graphics.DrawString(s, fn, new SolidBrush(Color.White), r, sf);
                    e.DrawFocusRectangle();
                }
            }
        }

        private void cmbReturn_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            if (cmb.SelectedIndex == 0)
            {
                FormSelectArg frm = new FormSelectArg(_selectEntity.Entity, _entitys, cmb.Name != cmbReturn.Name);
                if (DialogResult.OK == frm.ShowDialog())
                {
                    if (cmbReturn.Name == cmb.Name)
                        cmb.Items.Add(frm.Code);
                    else
                    {
                        VarType.Items.Add(frm.Code);
                        cmb.Items.Add(frm.Code);
                        cmb.Refresh();
                    }
                    cmb.SelectedItem = frm.Code;
                }
            }
        }

        private void cmbReposi_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            _selectEntity = cmb.SelectedItem as TemplateEntity;
            cmbReturn.SelectedIndex = -1;
            cmbReturn.Text = "";
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void btnInsert_Click(object sender, EventArgs e)
        {

            //获取所有参数
            MethodCommon.MethodName = txtName.Text;
            MethodCommon.Organize = txtCompany.Text;
            MethodCommon.Author = txtAuthor.Text;
            MethodCommon.Return = cmbReturn.Text;
            MethodCommon.Comment = txtComment.Text;
            List<ReferArg> refs = new List<ReferArg>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Index < dataGridView1.Rows.Count - 1)
                {
                    ReferArg temp = new ReferArg();
                    temp.Comment = row.Cells[Comment.Name].Value == null ? string.Empty : row.Cells[Comment.Name].Value.ToString();
                    temp.Name = row.Cells[VarName.Name].Value == null ? string.Empty : row.Cells[VarName.Name].Value.ToString();
                    temp.IsOut = row.Cells[IsOut.Name].Value == null ? false : (row.Cells[IsOut.Name].Value.ToString() == "1" ? true : false);
                    temp.VType = row.Cells[VarType.Name].Value == null ? string.Empty : row.Cells[VarType.Name].Value.ToString();

                    refs.Add(temp);
                }
            }
            MethodCommon.InnerArgs = refs;

            StringBuilder build1 = new StringBuilder();
            StringBuilder build2 = new StringBuilder();
            StringBuilder build3 = new StringBuilder();

            foreach (ReferArg item in refs)
            {
                build1.AppendLine(string.Format("/// <param name='{0}'>{1}</param>", item.Name, item.Comment));
                build2.Append(string.Format("{0} {1},", item.VType, item.Name));
                build3.Append(item.Name + ",");
            }

            ModelContainer.Regist("$ParamComment$", build1.ToString());
            ModelContainer.Regist("$Param$", build2.ToString().Remove(build2.ToString().Length - 1));
            ModelContainer.Regist("$InnerParam$", build3.ToString().Remove(build3.ToString().Length - 1));

            string returnComment = string.Empty;
            string needReturn = string.Empty;
            string needReturn2 = string.Empty;
            if (MethodCommon.Return != "void")
            {
                returnComment = string.Format(" /// <returns>{0}</returns>", txtReComment.Text);
                needReturn = "return";
                needReturn2 = "";
            }


            ModelContainer.Regist("$Comment$", MethodCommon.Comment, "方法描述");
            ModelContainer.Regist("$Return$", MethodCommon.Return, "方法返回类型");
            ModelContainer.Regist("$Author$", MethodCommon.Author, "方法作者");
            ModelContainer.Regist("$Organize$", MethodCommon.Organize, "方法机构");
            ModelContainer.Regist("$MethodName$", MethodCommon.MethodName, "方法名称");
            ModelContainer.Regist("$ReturnComment$", returnComment);
            ModelContainer.Regist("$NeedReturn$", needReturn);
            ModelContainer.Regist("$NeedReturn2$", needReturn2);

            TemplateEntity template = new TemplateEntity();
            template.Entity = _selectEntity.Entity;
            template.Data2Obj = _selectEntity.Data2Obj.Substring(0, _selectEntity.Data2Obj.Length - 3);

            //CodeAppendManager manager = new CodeAppendManager(ConstructType.MethodApp, template);
            //build1.Clear();
            //manager.BuildTaget = build1;
            //manager.CreateCode();

            //FileOprateHelp.WriteMethod(ProjectContainer.Application, build1.ToString(), template, template.Data2Obj + "App");

            //CodeAppendManager manager1 = new CodeAppendManager(ConstructType.MethodIApp, template);
            //build1.Clear();
            //manager1.BuildTaget = build1;
            //manager1.CreateCode();

            //FileOprateHelp.WriteMethod(ProjectContainer.IApplication, build1.ToString(), template, "I" + template.Data2Obj + "App");

            //CodeAppendManager manager2 = new CodeAppendManager(ConstructType.MethodServer, template);
            //build1.Clear();
            //manager2.BuildTaget = build1;
            //manager2.CreateCode();

            //FileOprateHelp.WriteMethod(ProjectContainer.Service, build1.ToString(), template, SolutionCommon.ProjectName + "Service.svc");

            //CodeAppendManager manager3 = new CodeAppendManager(ConstructType.MethodIServer, template);
            //build1.Clear();
            //manager3.BuildTaget = build1;
            //manager3.CreateCode();

            FileOprateHelp.WriteMethod(ProjectContainer.Service, build1.ToString(), template, "I" + SolutionCommon.ProjectName + "Service");

            this.Close();
        }

    }
}

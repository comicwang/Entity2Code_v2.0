using Infoearth.Entity2CodeTool.Logic;
using Infoearth.Entity2CodeTool.Model;
using Infoearth.Entity2CodeTool;
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
using Infoearth.Entity2CodeTool.UI;

namespace Infoearth.Entity2CodeTool
{
    /// <summary>
    /// 模型管理UI
    /// </summary>
    public partial class FormModelManager : Form
    {
        #region field and attrs

        private ModelManageLogic manger = new ModelManageLogic();

        private bool _isModified = false;

        private string _oldText = string.Empty;

        private ModelManageArgment _cunrrentModel;

        private bool _isLocked = true;

        #endregion

        #region ctor

        public FormModelManager()
        {
            InitializeComponent();
            pictureBox1.Image = "img.jpg".GetImageResource("Img");
            IniLstModel(false);
            btnSave.Focus();
        }

        #endregion

        #region methods

        private void IniLstModel(bool all)
        {
            pnlLeft.Controls.Clear();
            List<ModelManageArgment> models = new List<ModelManageArgment>();
            string[] edits = new string[] { "Application.slm", "IApplication.slm", "Service.slm", "IService.slm" };
            models.AddRange(manger.GetModelList("slm"));
            if(all)
                models.AddRange(manger.GetModelList("sem"));
            if (models.Count == 0)
                return;
            int index = 0;          
            foreach (ModelManageArgment item in models)
            {
                if (!edits.Contains(item.Text)&&!all)
                    continue;
                Label lbl = new Label();
                lbl.TextAlign = ContentAlignment.MiddleRight;
                lbl.Padding = new System.Windows.Forms.Padding(5);
                lbl.MouseHover += lbl_MouseHover;
                lbl.MouseLeave += lbl_MouseLeave;
                lbl.Click += lbl_Click;
                lbl.Text = item.Text;
                lbl.AutoSize = all;
                lbl.Height = 30;
                lbl.Width = 160;
                lbl.Location = new Point(10, index * lbl.Height);
                lbl.Tag = item;
                lbl.ForeColor = Color.Gray;
                lbl.Name = item.Text + index;
                this.pnlLeft.Controls.Add(lbl);
                if (index == 0)
                {
                    lbl.BackColor = Color.FromArgb(19, 130, 206);
                    lbl.ForeColor = Color.White;

                    item.isSelected = true;
                    lbl.Tag = item;
                    lock(_oldText)
                    {
                        _oldText = manger.GetModelContent(item.Value);
                        rcBoxContect.Text = _oldText;
                        SetRichColor();
                    }
                 
                    _cunrrentModel = item;
                }
                index++;
            }
        }

        private void SetUnSelect()
        {
            foreach (Control ctrl in pnlLeft.Controls)
            {
                ModelManageArgment obj = ctrl.Tag as ModelManageArgment;
                if (null != obj && obj.isSelected)
                {
                    ctrl.BackColor = Control.DefaultBackColor;
                    ctrl.ForeColor = Color.Gray;
                    obj.isSelected = false;
                    ctrl.Tag = obj;
                }
            }
        }

        private void SetRichColor()
        {
            this.rcBoxContect.Visible = false;
            int selectionIndex = rcBoxContect.SelectionStart;
            foreach (ContainerModel model in ModelContainer.Models)
            {
                string keyword = model.Key;
                int start = 0;
                while (true)
                {
                    start = rcBoxContect.Find(keyword, start, RichTextBoxFinds.MatchCase);
                    if (start < 0)
                        break;
                    rcBoxContect.SelectionStart = start;
                    rcBoxContect.SelectionLength = keyword.Length;
                    rcBoxContect.SelectionColor = Color.YellowGreen;
                    start += keyword.Length;
                }
                rcBoxContect.SelectionStart = 0;
                rcBoxContect.SelectionLength = 0;
            }
            rcBoxContect.SelectionStart = selectionIndex;
            rcBoxContect.SelectionLength = 0;
            this.rcBoxContect.Visible = true;
        }

        private void SetRichColor(int start, int end)
        {
            int startbak = start;
            int selectionIndex = rcBoxContect.SelectionStart;
            foreach (ContainerModel model in ModelContainer.Models)
            {
                string keyword = model.Key;
                start = startbak;
                while (true)
                {
                    start = rcBoxContect.Find(keyword, start, end, RichTextBoxFinds.MatchCase);
                    if (start < 0)
                        break;
                    rcBoxContect.SelectionStart = start;
                    rcBoxContect.SelectionLength = keyword.Length;
                    rcBoxContect.SelectionColor = Color.YellowGreen;
                    start += keyword.Length;
                }
                rcBoxContect.SelectionStart = 0;
                rcBoxContect.SelectionLength = 0;
            }
            rcBoxContect.SelectionStart = selectionIndex;
            rcBoxContect.SelectionLength = 0;
        }
       
        private LineShow GetLineShow()
        {
            int section = rcBoxContect.SelectionStart;
            string[] lines = rcBoxContect.Lines;
            int index = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                if (section>=index&&section<=index+lines[i].Length)
                {
                    return new LineShow() { RowLine = i + 1, ColumnLine = section - index + 1, SectionIndex = section, SectionStart = index, SectionEnd = index + lines[i].Length };
                }
                index = index + lines[i].Length + 1;
            }
            return null;
        }

        #endregion

        #region events

        private void lbl_Click(object sender, EventArgs e)
        {
            Label lbl = (Label)sender;
            ModelManageArgment obj = lbl.Tag as ModelManageArgment;
            if (_isModified)
            {
                DialogResult dialog = MessageBox.Show("当前模型已经修改，是否保存？", "Entity2Code", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Asterisk);
                if (dialog == DialogResult.Cancel)
                    return;
                else if (dialog == DialogResult.Yes)
                {
                    FileOprateHelp.SaveFile(rcBoxContect.Text, _cunrrentModel.Value);
                    //保存
                }
                else
                {
                    //继续
                }
            }
            SetUnSelect();
            lbl.BackColor = Color.FromArgb(19, 130, 206);
            lbl.ForeColor = Color.White;
            obj.isSelected = true;
            lbl.Tag = obj;

            _cunrrentModel = obj;

            _isLocked = false;
            _oldText = manger.GetModelContent(obj.Value);
            rcBoxContect.Text = _oldText;
            SetRichColor();
            string[] strs = new string[] { "Application", "IApplication", "Service", "IService", "Map" };
            bool enable = strs.Contains(System.IO.Path.GetFileNameWithoutExtension(_cunrrentModel.Text));
            rcBoxContect.ReadOnly = !enable;
            btnSave.Enabled = enable;
            btnwriteProject.Enabled = enable;
            _isLocked = true;
        }

        private void lbl_MouseLeave(object sender, EventArgs e)
        {
            Label lbl = (Label)sender;
            ModelManageArgment obj = lbl.Tag as ModelManageArgment;
            if (obj.isSelected == false)
            {
                lbl.BackColor = Control.DefaultBackColor;
                lbl.ForeColor = Color.Gray;
            }
        }

        private void lbl_MouseHover(object sender, EventArgs e)
        {
            Label lbl = (Label)sender;
            ModelManageArgment obj = lbl.Tag as ModelManageArgment;
            if (obj.isSelected == false)
            {
                lbl.BackColor = Color.White;
                lbl.ForeColor = Color.FromArgb(19,130,206);
            }
        }

        private void rcBoxContect_TextChanged(object sender, EventArgs e)
        {
            if (_isLocked)
                _isModified = !(String.Compare(rcBoxContect.Text, _oldText) == 0);
            else
                _isModified = false;
            btnSave.Enabled = _isModified;

            LineShow line = GetLineShow();
            SetRichColor(line.SectionStart, line.SectionEnd);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_isModified == true)
            {
                FileOprateHelp.SaveFile(rcBoxContect.Text, _cunrrentModel.Value);
                MsgBoxHelp.ShowInfo("保存成功！");
                btnSave.Enabled = false;
                _isModified = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnInsertModel_Click(object sender, EventArgs e)
        {

        }

        private void btnwriteProject_Click(object sender, EventArgs e)
        {
            if (_isModified == true)
            {
                FileOprateHelp.SaveFile(rcBoxContect.Text, _cunrrentModel.Value);
                btnSave.Enabled = false;
                _isModified = false;
            }
            ModelManageLogic.WriteModel(_cunrrentModel.Text);
            //this.Close();
        }

        private void ckAll_CheckedChanged(object sender, EventArgs e)
        {
            IniLstModel(ckAll.Checked);
        }

        private void btnKeywordView_Click(object sender, EventArgs e)
        {
            FormModelView view = new FormModelView();
            view.Show(this);
        }       

        #endregion
    }
}

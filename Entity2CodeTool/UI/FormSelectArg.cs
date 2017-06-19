

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using Utility.Entity;

namespace Infoearth.Entity2CodeTool.UI
{
    public partial class FormSelectArg : Form
    {
        private List<TemplateEntity> _entitys = null;
        private string _repositoy = null;
        private bool _isReturn = false;

        private string _code = null;

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }
        public FormSelectArg()
        {
            InitializeComponent();
        }

        public FormSelectArg(string repositoy,List<TemplateEntity> entitys,bool isReurn)
            : this()
        {
            _repositoy = repositoy;
            _entitys = entitys;
            _isReturn = isReurn;
            SetCustomArg();
        }

        private void SetCustomArg()
        {
            List<string> customs = Utility.Filter.CsharpVarFilter.FilterAll(_isReturn);
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(customs.ToArray());
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(_entitys.ToArray());
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked)
                SetCustomArg();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string result = (comboBox1.SelectedItem as TemplateEntity).Data2Obj + Properties.Resources.Data2ObjEndName;

            foreach (Control item in groupBox2.Controls)
            {
                RadioButton radio = item as RadioButton;
                if (radio.Checked)
                {
                    switch (radio.Text)
                    {
                        case "List<>":
                            result = string.Format("List<{0}>", result);
                            break;
                        case "[]":
                            result = string.Format("{0}[]", result);
                            break;
                        case "[][]":
                            result = string.Format("{0}[][]", result);
                            break;
                        default:
                            break;
                    }
                    break;
                }
            }

            _code = result;

            this.DialogResult = DialogResult.OK;
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }
    }
}

using Infoearth.Entity2CodeTool.Container;
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
using Infoearth.Entity2CodeTool.Helps;
using System.IO;

namespace Infoearth.Entity2CodeTool.UI
{
    public partial class FormSelectArg : Form
    {
        private List<TemplateEntity> _entitys = null;
        private string _repositoy = null;
        private bool _reList = false;

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

        public FormSelectArg(string repositoy,List<TemplateEntity> entitys,bool reList)
            : this()
        {
            _repositoy = repositoy;
            _entitys = entitys;
            _reList = reList;
            SetCustomArg();
        }

        private void SetCustomArg()
        {
            List<string> customs = VarCsharp.AllTypes(_reList);
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(customs.ToArray());
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                //获取仓储内复杂参数
                string fileForld = ProjectContainer.DomainEntity.ToDirectory();
                string filePath = Directory.GetFiles(fileForld, _repositoy + ".cs", SearchOption.AllDirectories).FirstOrDefault();
                List<string> specials = FileOprateHelp.ReadFileFilter(filePath, _entitys);

                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(specials.ToArray());
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked)
                SetCustomArg();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string result = comboBox1.SelectedItem.ToString();

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

using Infoearth.Entity2CodeTool.Model;
using Microsoft.Data.ConnectionUI;
using System;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Infoearth.Entity2CodeTool.Helps;

namespace Infoearth.Entity2CodeTool
{
    /// <summary>
    /// 主窗体
    /// </summary>
    public partial class FormMain : Form
    {
        #region fields and attrs

        /// <summary>
        /// 进度标识
        /// </summary>
        private int _count = 0;

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName
        {
            get { return this.txtProjectName.Text; }
            set
            {
                txtDomainEntity.Text = string.Format("iTelluro.Explorer.{0}.Domain.Entities", value);
                txtData2Obj.Text = string.Format("iTelluro.Explorer.{0}.Application.DTO", value);
                txtApplica.Text = string.Format("iTelluro.Explorer.{0}.Application", value);
                txtIApplica.Text = string.Format("iTelluro.Explorer.{0}.IApplication", value);
                txtInfrastructure.Text = string.Format("iTelluro.Explorer.{0}.Infrastructure.Context", value);
                txtDomainContxt.Text = string.Format("iTelluro.Explorer.{0}.Domain.Context", value);
                txtService.Text = string.Format("iTelluro.Explorer.{0}.Service", value);
                txtContextName.Text = value + "Context";
                SolutionCommon.ProjectName = value;
            }
        }

        /// <summary>
        /// 是否新增服务
        /// </summary>
        private bool IsAddService
        {
            get { return this.chkService.Checked; }
        }

        /// <summary>
        /// 基础结构层
        /// </summary>
        private string Infrastructure
        {
            get { return this.txtInfrastructure.Text; }
        }

        /// <summary>
        /// 领域实体层
        /// </summary>
        private string DomainEntity
        {
            get { return this.txtDomainEntity.Text; }
        }

        /// <summary>
        /// 领域层
        /// </summary>
        private string DomainContxt
        {
            get { return this.txtDomainContxt.Text; }
        }

        /// <summary>
        /// 应用层
        /// </summary>
        private string Applica
        {
            get { return this.txtApplica.Text; }
        }

        /// <summary>
        /// 应用接口层
        /// </summary>
        private string IApplica
        {
            get { return this.txtIApplica.Text; }
        }

        /// <summary>
        /// 应用实体层
        /// </summary>
        private string Data2Object
        {
            get { return this.txtData2Obj.Text; }
        }

        /// <summary>
        /// 服务层
        /// </summary>
        private string Service
        {
            get { return this.txtService.Text; }
        }

        /// <summary>
        /// 不包含表
        /// </summary>
        private const string NoTables = "TableFilterInclude = new Regex(\"^$\");";

        /// <summary>
        /// 所有表
        /// </summary>
        private const string AllTables = "TableFilterInclude = null;";

        /// <summary>
        /// 正则表达式
        /// </summary>
        private Regex regex = null;

        #endregion fields and attrs

        #region ctors

        /// <summary>
        /// 构造函数
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
            //pic.Image = Infoearth.Entity2CodeTool.Properties.Resources.img;
            pic.Image = "img.jpg".GetImageResource("Img");
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="solutionName">项目名称</param>
        public FormMain(string solutionName)
            : this()
        {
            solutionName = System.IO.Path.GetFileNameWithoutExtension(solutionName);
            if (!string.IsNullOrEmpty(solutionName) && solutionName.Split('.').Length >= 3)
            {
                this.txtProjectName.Text = solutionName.Split('.')[2];
            }
        }

        #endregion ctors

        #region methods

        /// <summary>
        /// 架构各层DLL名称是否为空
        /// </summary>
        /// <returns></returns>
        private bool IsContentEmpty()
        {
            if (string.IsNullOrEmpty(IApplica) || string.IsNullOrEmpty(Infrastructure) || string.IsNullOrEmpty(DomainContxt) ||
                string.IsNullOrEmpty(DomainEntity) || string.IsNullOrEmpty(Applica) || (chkService.Checked && string.IsNullOrEmpty(Service)))
                return false;
            return true;
        }

        /// <summary>
        /// 选择的数据库表是否为空
        /// </summary>
        /// <returns></returns>
        private bool IsTableNull()
        {
            return clbDBNames.SelectedItems.Count > 0 && !string.IsNullOrEmpty(txtContextName.Text) && !string.IsNullOrEmpty(tbConStr.Text);
        }

        /// <summary>
        /// 更新正则表达式
        /// </summary>
        private void UpdateRegex()
        {
            if (clbDBNames.CheckedItems.Count == 0)
            {
                tbIncludeTables.Text = NoTables;
                return;
            }

            var sb = new StringBuilder();
            sb.Append("TableFilterInclude = new Regex(\"");
            var sb1 = new StringBuilder();

            bool first = true;
            foreach (var item in clbDBNames.CheckedItems)
            {
                if (!first)
                {
                    sb.Append("|");
                    sb1.Append("|");
                }
                else
                    first = false;

                sb.AppendFormat("^{0}$", item);
                sb1.AppendFormat("^{0}$", item);
            }

            sb.Append("\");");
            Clipboard.SetText(sb.ToString());
            tbIncludeTables.Text = sb.ToString();
            regex = new Regex(sb1.ToString());
        }

        /// <summary>
        /// 设置各个控件的状态
        /// </summary>
        private void SetEnable()
        {
            if (_count == 0)
            {
                lblTitle.Text = "配置项目名称";
                this.txtProjectName.Focus();
                btnNext.Enabled = true;
                btnPrv.Enabled = false;
                btnOk.Enabled = false;
                pnlMain.Visible = true;
                pnlMain2.Visible = false;
                pnlMain3.Visible = false;
                pnlMain4.Visible = false;
            }
            else if (_count == 1)
            {
                lblTitle.Text = "配置架构名称";
                this.txtInfrastructure.Focus();
                ProjectName = this.txtProjectName.Text;
                btnPrv.Enabled = true;
                btnNext.Enabled = IsContentEmpty();
                btnOk.Enabled = false;
                pnlMain.Visible = false;
                pnlMain2.Visible = true;
                pnlMain3.Visible = false;
                pnlMain4.Visible = false;
            }
            else if (_count == 2)
            {
                lblTitle.Text = "配置架构类型";
                this.rbtnDb.Focus();
                btnNext.Enabled = rBtnCode.Checked;
                btnPrv.Enabled = true;
                btnOk.Enabled = rbtnDb.Checked;
                pnlMain.Visible = false;
                pnlMain2.Visible = false;
                pnlMain3.Visible = true;
                pnlMain4.Visible = false;
            }
            else
            {
                lblTitle.Text = "配置架构数据源";
                this.txtContextName.Focus();
                btnNext.Enabled = false;
                btnPrv.Enabled = true;
                btnOk.Enabled = IsTableNull();
                pnlMain.Visible = false;
                pnlMain2.Visible = false;
                pnlMain3.Visible = false;
                pnlMain4.Visible = true;
            }
        }

        #endregion methods

        #region events

        /// <summary>
        /// 下一步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNext_Click(object sender, EventArgs e)
        {
            _count++;
            SetEnable();
        }

        /// <summary>
        /// 项目名称改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtProjectName_TextChanged(object sender, EventArgs e)
        {
            btnNext.Enabled = !string.IsNullOrEmpty(this.txtProjectName.Text);
            // btnOk.Enabled = !string.IsNullOrEmpty(this.txtProjectName.Text);
        }

        /// <summary>
        /// 上一步事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrv_Click(object sender, EventArgs e)
        {
            _count--;
            SetEnable();
        }

        /// <summary>
        /// 取消事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// 确定事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            SolutionCommon.ProjectName = ProjectName;
            SolutionCommon.IsAddService = IsAddService;
            SolutionCommon.Infrastructure = Infrastructure;
            SolutionCommon.IApplication = IApplica;
            SolutionCommon.Application = Applica;
            SolutionCommon.Data2Object = Data2Object;
            SolutionCommon.DomainContext = DomainContxt;
            SolutionCommon.DomainEntity = DomainEntity;
            SolutionCommon.Service = Service;
            if (rbtnDb.Checked)
                SolutionCommon.infrastryctType = InfrastructType.DbFirst;
            else
                SolutionCommon.infrastryctType = InfrastructType.CodeFirst;
            CodeFirstTools.TableFilterInclude = regex;
            CodeFirstTools.DbContextName = txtContextName.Text;
            this.DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// 是否添加服务选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkService_CheckedChanged(object sender, EventArgs e)
        {
            txtService.Enabled = chkService.Checked;
            btnNext.Enabled = IsContentEmpty();
        }

        /// <summary>
        /// 文字更改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInfrastructure_TextChanged(object sender, EventArgs e)
        {
            //if (string.IsNullOrEmpty(((Control)sender).Text))
            //    btnNext.Enabled = false;
            btnNext.Enabled = IsContentEmpty();
        }

        /// <summary>
        /// 选择架构类型事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbtnDb_CheckedChanged(object sender, EventArgs e)
        {
            btnNext.Enabled = !rbtnDb.Checked;
            btnOk.Enabled = rbtnDb.Checked;
        }

        /// <summary>
        /// 包含的表文字更改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbIncludeTables_TextChanged(object sender, EventArgs e)
        {
            btnOk.Enabled = IsTableNull();
        }

        /// <summary>
        /// 连接数据库事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            clbDBNames.Items.Clear();
            var dcd = new DataConnectionDialog();
            dcd.DataSources.Clear();
            var sqlDataSource = DataSource.SqlDataSource;
            var oracleDataSource = DataSource.OracleDataSource;

            DataProvider dp = new DataProvider("Oracle.DataAccess.Client", "Oracle.DataAccess.Client", "Oracle", "Oracle提供程序");
            oracleDataSource.Providers.Add(dp);
            oracleDataSource.DefaultProvider = dp;
            dcd.DataSources.Add(sqlDataSource);
            dcd.DataSources.Add(oracleDataSource);
            dcd.DataSources.Add(DataSource.SqlFileDataSource);
            dcd.SelectedDataSource = sqlDataSource;

            var result = DataConnectionDialog.Show(dcd);

            if (result != DialogResult.OK)
                return;
            DbProviderFactory df = DbProviderFactories.GetFactory(dcd.SelectedDataProvider.Name);
            string sql = string.Empty;

            CodeFirstTools.DataType = dcd.SelectedDataSource.Name;
            CodeFirstTools._providerName = dcd.SelectedDataProvider.Name;
            CodeFirstTools._connectionString = dcd.ConnectionString;

            if (dcd.SelectedDataSource.Name == "Oracle")
            {
                sql = @"select t.TABLE_NAME NAME from User_tables t ORDER BY t.TABLE_NAME";
            }
            else
            {
                sql = @"SELECT name AS NAME FROM sys.Tables WHERE name NOT IN ('EdmMetadata', '__MigrationHistory','sysdiagrams') ORDER BY name";
            }
            DbConnection con = df.CreateConnection();
            if (dcd.SelectedDataProvider.DisplayName == "ODPM.NET Provide")
            {
                con.ConnectionString = dcd.DisplayConnectionString;
                tbConStr.Text = dcd.DisplayConnectionString;
            }
            else
            {
                con.ConnectionString = dcd.ConnectionString;
                tbConStr.Text = dcd.ConnectionString;
            }
            con.Open();
            DbCommand com = df.CreateCommand();
            com.Connection = con;
            com.CommandText = sql;
            using (DbDataReader reader = com.ExecuteReader())
            {
                while (reader.Read())
                {
                    clbDBNames.Items.Add(reader.GetString(0));
                }
            }
            con.Close();
        }

        /// <summary>
        /// 全选事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbAll_CheckedChanged(object sender, EventArgs e)
        {
            if (rbAll.Checked)
            {
                for (var n = 0; n < clbDBNames.Items.Count; n++)
                {
                    clbDBNames.SetItemCheckState(n, CheckState.Checked);
                }
                tbIncludeTables.Text = AllTables;
                Clipboard.SetText(AllTables);
                regex = null;
            }
            else
            {
                for (var n = 0; n < clbDBNames.Items.Count; n++)
                {
                    clbDBNames.SetItemCheckState(n, CheckState.Unchecked);
                }
                tbIncludeTables.Text = NoTables;
                Clipboard.SetText(NoTables);
                regex = new Regex("^$");
            }
            btnOk.Enabled = rbAll.Checked;
        }

        /// <summary>
        /// 表名称更改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clbDBNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateRegex();
            btnOk.Enabled = IsTableNull();
        }

        #endregion events
    }
}
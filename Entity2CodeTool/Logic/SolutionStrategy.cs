using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using EnvDTE80;
using EnvDTE90;
using EnvDTE100;
using Utility.Common;
using Utility.Generate;
using Utility.Core;
using Utility.Helps;
using Utility.Base;
using Utility.CodeFirst;
using Utility;

namespace Infoearth.Entity2CodeTool
{
    /// <summary>
    /// 解决方案策略
    /// </summary>
    public class SolutionStrategy
    {
        private BackgroundWorker _work;
        private DTE _dte;
      
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dte">DTE对象</param>
        public SolutionStrategy()
        {
            _dte = CommonContainer.CommonServer;
            _work = new BackgroundWorker();
            _work.WorkerReportsProgress = true;
            _work.DoWork += _work_DoWork;
            _work.ProgressChanged += _work_ProgressChanged;
            _work.RunWorkerCompleted += _work_RunWorkerCompleted;
        }

        public void BeginStrategy()
        {
            _work.RunWorkerAsync();
        }

        private void _work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SolutionExplorHelp.CollapseAll();
            _dte.RebuildSolution();
            _dte.OutString("代码创建完成..");
        }

        private void _work_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage > 0 && e.ProgressPercentage <= 100)
                _dte.OutProcessBar(e.ProgressPercentage, e.UserState.ToString(), true);
            else
                _dte.OutString(e.UserState.ToString(), true);
        }

        private void _work_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                AddSchameLib();
                //ReferenceLogic.Add();
            }
            catch (Exception ex)
            {
                _dte.OutWindow(ex.Message);
            }
        }

        /// <summary>
        /// 添加构架应用程序
        /// </summary>
        /// <param name="ProjectCommon"></param>
        private void AddSchameLib()
        {
            try
            {
                bool allowNew = true;
                CodeGenerate generateCode = new CodeGenerate();
                TempGenerate generateTemp = new TempGenerate();
                generateTemp.TempBuild = new StringBuilder();
                generateTemp.TemplateName = "$DBContextContent$";

                TempGenerate generateProTemp = new TempGenerate();
                generateProTemp.TempBuild = new StringBuilder();
                generateProTemp.TemplateName = "$ProfileContent$";

                TempGenerate serverProTemp = new TempGenerate();
                serverProTemp.TempBuild = new StringBuilder();
                serverProTemp.TemplateName = "$ServiceContent$";

                TempGenerate iServerProTemp = new TempGenerate();
                iServerProTemp.TempBuild = new StringBuilder();
                iServerProTemp.TemplateName = "$IServiceContent$";

                TempGenerate containerTemp = new TempGenerate();
                containerTemp.TempBuild = new StringBuilder();
                containerTemp.TemplateName = "$ContainerContent$";

                Dictionary<string, string> container = new Dictionary<string, string>();
                string str = string.Empty;
                if (CodeFirstTools.DataType == "MicrosoftSqlServer")
                    str = "<add name=\"" + CodeFirstTools.DbContextName + "\" providerName=\"" + CodeFirstTools.ProviderName + "\" connectionString=\"" + CodeFirstTools.ConnectionString + ";MultipleActiveResultSets=True;Pooling=True;\"/>";
                else
                    str = "<add name=\"" + CodeFirstTools.DbContextName + "\" providerName=\"Oracle.DataAccess.Client\" connectionString=\"" + CodeFirstTools.ConnectionString + "\"/>";
                container.Add("$ConnectionString$", str);
                container.Add("$DBSchemaApp$", string.Format("<add key=\"DBSchema\" value=\"{0}\"/>", CodeFirstTools.SchemaName));

                int count = 0;
                //循环实体
                foreach (Table tbl in CodeFirstLogic.Tables.Where(t => !t.IsMapping).OrderBy(x => x.NameHumanCase))
                {
                    count++;
                    Dictionary<string, string> keycontainer = new Dictionary<string, string>();
                    keycontainer.Add("$Entity$", tbl.Name);
                    keycontainer.Add("$Data2Obj$", tbl.NameHumanCase);
                    keycontainer.Add("$Schema$", tbl.Schema);
                    keycontainer.Add("$MapPrimaryKey$", tbl.PrimaryKeyNameHumanCase());

                    StringBuilder build = new StringBuilder();
                    StringBuilder entityBuild = new StringBuilder();
                    //主键
                    foreach (Column col in tbl.Columns.Where(x => !x.Hidden).OrderBy(x => x.Ordinal))
                    {
                        build.AppendLine(col.Config);
                        entityBuild.AppendLine("        " + CodeFirstLogic.WritePocoColumn(col));
                    }
                    keycontainer.Add("$MapProperty$", build.ToString());
                    keycontainer.Add("$Property$", entityBuild.ToString());
                    build.Clear();
                    entityBuild.Clear();

                    //外键
                    if (tbl.HasForeignKey)
                    {
                        foreach (Column col in from c in tbl.Columns.OrderBy(x => x.Ordinal) where c.ConfigFk != null select c)
                        {
                            build.AppendLine(col.ConfigFk);
                        }
                        foreach (Column col in from c in tbl.Columns.OrderBy(x => x.EntityFk) where c.EntityFk != null select c)
                        {
                            entityBuild.AppendLine("        " + col.EntityFk);
                        }
                    }
                    keycontainer.Add("$MapForeignKey$", build.ToString());
                    keycontainer.Add("$ForeignKey$", entityBuild.ToString());
                    build.Clear();
                    entityBuild.Clear();
                    //导航属性
                    if (tbl.ReverseNavigationProperty.Count() > 0)
                    {
                        foreach (string s in tbl.ReverseNavigationProperty.OrderBy(x => x))
                        {
                            entityBuild.AppendLine("        " + s);
                        }
                    }
                    keycontainer.Add("$Navigation$", entityBuild.ToString());
                    entityBuild.Clear();
                    //构造函数
                    if (tbl.Columns.Where(c => c.Default != string.Empty).Count() > 0 || tbl.ReverseNavigationCtor.Count() > 0)
                    {
                        entityBuild.AppendLine("        public " + tbl.Name + "()");
                        entityBuild.AppendLine("        {");
                        foreach (Column col in tbl.Columns.OrderBy(x => x.Ordinal).Where(c => c.Default != string.Empty))
                        {
                            entityBuild.AppendLine("          " + col.PropertyName + "=" + col.Default + ";");
                        }
                        foreach (string s in tbl.ReverseNavigationCtor)
                        {
                            entityBuild.AppendLine("          " + s);
                        }
                        entityBuild.AppendLine("        }");
                    }
                    keycontainer.Add("$Constructor$", entityBuild.ToString());
                    entityBuild.Clear();

                    generateCode.Generate(CdeCmdId.DomainContextId.IRepository, allowNew, keycontainer);//IRepository
                    generateCode.Generate(CdeCmdId.InfrastructureId.Repository, allowNew, keycontainer);//Repository
                    generateCode.Generate(CdeCmdId.ApplicationId.Application, allowNew, keycontainer);//Application
                    generateCode.Generate(CdeCmdId.IApplicationId.IApplication, allowNew, keycontainer);//IApplication
                    generateCode.Generate(CdeCmdId.Data2ObjectId.Data2Obj, allowNew, keycontainer);//Data2Obj
                    generateCode.Generate(CdeCmdId.InfrastructureId.Map, allowNew, keycontainer);//Map
                    generateCode.Generate(CdeCmdId.DomainEntityId.Entity, allowNew, keycontainer);//Entity
                    generateProTemp.Generate(CdeCmdId.Data2ObjectId.Profile.UnFix, count == CodeFirstLogic.Tables.Count, keycontainer);//ProfiTemp
                    generateTemp.Generate(CdeCmdId.InfrastructureId.DBContext.UnFix, count == CodeFirstLogic.Tables.Count, keycontainer);//DbContextTemp
                    serverProTemp.Generate(CdeCmdId.ServiceId.Service.UnFix, count == CodeFirstLogic.Tables.Count, keycontainer);//ServerTemp
                    iServerProTemp.Generate(CdeCmdId.ServiceId.IService.UnFix, count == CodeFirstLogic.Tables.Count, keycontainer);//IServerTemp
                    containerTemp.Generate(CdeCmdId.ServiceId.Container.UnFix, count == CodeFirstLogic.Tables.Count, keycontainer);//ContainerTemp
                }
                
                generateCode.Generate(CdeCmdId.InfrastructureId.DbContextExtensions, allowNew, null);//DbContextExtensions
                generateCode.Generate(CdeCmdId.InfrastructureId.DBContext.Fix, allowNew, null);//DbContext
                generateCode.Generate(CdeCmdId.Data2ObjectId.Profile.Fix, allowNew, null);//DbContext
                generateCode.Generate(CdeCmdId.InfrastructureId.ContextUnit, allowNew, null);//DbContextUnit
                generateCode.Generate(CdeCmdId.ServiceId.IService.Fix, allowNew, null);//IServer
                generateCode.Generate(CdeCmdId.ServiceId.Service.Fix, allowNew, null);//Server
                generateCode.Generate(CdeCmdId.ServiceId.Container.Fix, allowNew, null);//Container
                generateCode.Generate(CdeCmdId.ServiceId.WebConfig, allowNew, null);//WebConfig
                generateCode.Generate(CdeCmdId.ServiceId.UnityInstanceProviderServiceBehavior, allowNew, null);//UnityInstanceProviderServiceBehavior
                generateCode.Generate(CdeCmdId.ServiceId.UnityInstanceProvider, allowNew, null);//UnityInstanceProvider
                generateCode.Generate(CdeCmdId.ServiceId.AttachDataSignBehavior, allowNew, null);//AttachDataSignBehavior
                generateCode.Generate(CdeCmdId.ServiceId.CodeBehind, allowNew, null);//CodeBehind

                //AssemblyInfo
                string assemblyInfoPath = Path.Combine(TemplateContainer.Resove<Project>(PrjCmdId.Service).ToDirectory(), "Properties", "AssemblyInfo.cs");
                StringBuilder build1 = FileOprateHelp.ReadTextFile(assemblyInfoPath);
                build1.AppendLine("[assembly: log4net.Config.XmlConfigurator(Watch = true)]");//日志监视
                FileOprateHelp.SaveTextFile(build1.ToString(), assemblyInfoPath);
                CommonContainer.CommonServer.SetStartup(ProjectContainer.Service);
                //保存解决方案
                if (_dte.Solution.Saved == false)
                    _dte.Solution.SaveAs(_dte.Solution.FullName);
            }
            catch (Exception ex)
            {
                Utility.Common.MsgBoxHelp.ShowError("Entity2Code Error:", ex);
            }
        }
    }


}

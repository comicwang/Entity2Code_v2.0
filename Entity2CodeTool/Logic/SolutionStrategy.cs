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
using Utility.Base;
using Utility.CodeFirst;
using Utility;
using Utility.Entity;
using Utility.Converter;

namespace Infoearth.Entity2CodeTool
{
    /// <summary>
    /// 解决方案策略
    /// </summary>
    public class SolutionStrategy
    {
        private BackgroundWorker _work;
        private DTE _dte;
        private float _totalPercent = 0;
        private int _currentTotalCount = 0;
        private int _currentCount = 0;
        private int _processTotalCount = 0;
        private int _processCurrentCount = 0;

      
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
            _dte.OutString(Properties.Resources.CollseAll);
            _dte.Solution.CollapseAllProject();
            _dte.Solution.RebuildSln();
            _dte.OutString(string.Format(Properties.Resources.BuildComplete, KeywordContainer.Resove("$ProjectName$")));
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
                NewProject();
                AddRef();
            }
            catch (Exception ex)
            {
                _dte.OutWindow(ex.Message,"Entity2CodeLog");
            }
        }

        /// <summary>
        /// 添加构架应用程序
        /// </summary>
        /// <param name="ProjectCommon"></param>
        private void NewProject()
        {
            try
            {
                bool allowNew = true;
                bool isPartService = KeywordContainer.Resove("$IsPartService$") == "true";
                GenerateContext context = new GenerateContext();
                context.GeneratedOne += context_GeneratedOne;
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

                int totalPercent = CodeFirstLogic.Tables.Count;
               
                #region 循环实体

                _processCurrentCount = 0;
                _processTotalCount = CodeFirstLogic.Tables.Count;
                _totalPercent = 0.6f;
                //循环实体
                foreach (Table tbl in CodeFirstLogic.Tables.Where(t => !t.IsMapping).OrderBy(x => x.NameHumanCase))
                {
                    _processCurrentCount++;
                    Dictionary<string, string> keycontainer = new Dictionary<string, string>();
                    keycontainer.Add("$Entity$", tbl.Name);
                    keycontainer.Add("$Data2Obj$", tbl.NameHumanCase);
                    keycontainer.Add("$Schema$", tbl.Schema);
                    keycontainer.Add("$MapPrimaryKey$", tbl.PrimaryKeyNameHumanCase());

                    string xmlPath = Path.Combine(CommonContainer.SolutionPath, CommonContainer.xmlName);
                    xmlManager.WriteEntity(tbl.Name, tbl.NameHumanCase, xmlPath);

                    StringBuilder build = new StringBuilder();
                    StringBuilder entityBuild = new StringBuilder();
                    StringBuilder dtoBuild = new StringBuilder();
                    //主键
                    foreach (Column col in tbl.Columns.Where(x => !x.Hidden).OrderBy(x => x.Ordinal))
                    {
                        build.AppendLine(col.Config);
                        entityBuild.AppendLine("        " + CodeFirstLogic.WritePocoColumn(col));
                        dtoBuild.AppendLine("        [DataMember]");
                        dtoBuild.AppendLine(string.Format("        public {0}{1} {2} {3}", col.PropertyType, CodeFirstTools.CheckNullable(col), col.PropertyName, "{get; set; }"));
                    }
                    keycontainer.Add("$MapProperty$", build.ToString());
                    keycontainer.Add("$Property$", entityBuild.ToString());
                    keycontainer.Add("$Data2ObjContent$", dtoBuild.ToString());
                    build.Clear();
                    entityBuild.Clear();
                    dtoBuild.Clear();
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

                    context.injection(generateCode, CdeCmdId.DomainContextId.IRepository, allowNew, keycontainer);//IRepository
                    context.injection(generateCode, CdeCmdId.InfrastructureId.Repository, allowNew, keycontainer);//Repositor                 
                    context.injection(generateCode, CdeCmdId.ApplicationId.Application, allowNew, keycontainer);//Application
                    context.injection(generateCode, CdeCmdId.IApplicationId.IApplication, allowNew, keycontainer);//IApplication
                    context.injection(generateCode, CdeCmdId.Data2ObjectId.Data2Obj, allowNew, keycontainer);//Data2Obj
                    context.injection(generateCode, CdeCmdId.InfrastructureId.Map, allowNew, keycontainer);//Map
                    context.injection(generateCode, CdeCmdId.DomainEntityId.Entity, allowNew, keycontainer);//Entity
                    context.injection(generateProTemp, CdeCmdId.Data2ObjectId.Profile.UnFix, _processCurrentCount == CodeFirstLogic.Tables.Count, keycontainer);//ProfiTemp
                    context.injection(generateTemp, CdeCmdId.InfrastructureId.DBContext.UnFix, _processCurrentCount == CodeFirstLogic.Tables.Count, keycontainer);//DbContextTemp
                    context.injection(serverProTemp, CdeCmdId.ServiceId.Service.UnFix, isPartService || _processCurrentCount == CodeFirstLogic.Tables.Count, keycontainer);//ServerTemp
                    context.injection(iServerProTemp, CdeCmdId.ServiceId.IService.UnFix, isPartService || _processCurrentCount == CodeFirstLogic.Tables.Count, keycontainer);//IServerTemp
                    if (isPartService)
                    {
                        keycontainer.Add("$ServiceName$", tbl.NameHumanCase);
                        context.injection(generateCode, CdeCmdId.ServiceId.IService.Fix, allowNew, keycontainer);//IServer
                        context.injection(generateCode, CdeCmdId.ServiceId.CodeBehind, allowNew, keycontainer);//CodeBehind
                        context.injection(generateCode, CdeCmdId.ServiceId.Service.Fix, allowNew, keycontainer);//Server
                    }
                    context.injection(containerTemp, CdeCmdId.ServiceId.Container.UnFix, _processCurrentCount == CodeFirstLogic.Tables.Count, keycontainer);//ContainerTemp
                    _currentTotalCount = context.Count;
                    context.Commit();
                }
                container.Add("$ConnectionString$", str);
                container.Add("$DBSchemaApp$", string.Format("<add key=\"DBSchema\" value=\"{0}\"/>", CodeFirstTools.SchemaName));


                #endregion

                context.injection(generateCode,CdeCmdId.InfrastructureId.DbContextExtensions, allowNew, container);//DbContextExtensions        
                context.injection(generateCode,CdeCmdId.InfrastructureId.DBContext.Fix, allowNew, null);//DbContext
                context.injection(generateCode,CdeCmdId.Data2ObjectId.Profile.Fix, allowNew, null);//DbContext
                context.injection(generateCode,CdeCmdId.InfrastructureId.ContextUnit, allowNew, null);//DbContextUnit
                if (!isPartService)
                {
                    container.Add("$ServiceName$", KeywordContainer.Resove("$ProjectName$"));
                    context.injection(generateCode, CdeCmdId.ServiceId.IService.Fix, allowNew, container);//IServer
                    context.injection(generateCode, CdeCmdId.ServiceId.CodeBehind, allowNew, container);//CodeBehind
                    context.injection(generateCode, CdeCmdId.ServiceId.Service.Fix, allowNew, container);//Server
                }
                context.injection(generateCode,CdeCmdId.ServiceId.Container.Fix, allowNew, null);//Container
                context.injection(generateCode,CdeCmdId.ServiceId.WebConfig, allowNew, null);//WebConfig
                context.injection(generateCode,CdeCmdId.ServiceId.UnityInstanceProviderServiceBehavior, allowNew, null);//UnityInstanceProviderServiceBehavior
                context.injection(generateCode,CdeCmdId.ServiceId.UnityInstanceProvider, allowNew, null);//UnityInstanceProvider       
                context.injection(generateCode,CdeCmdId.ServiceId.AttachDataSignBehavior, allowNew, null);//AttachDataSignBehavior      ,
               
                _totalPercent = 0.8f;
                _processTotalCount = 1;
                _processCurrentCount = 1;
                _currentTotalCount = context.Count;
                context.Commit();
                Project serverProject = TemplateContainer.Resove<Project>(PrjCmdId.Service);
                serverProject.SetLog4netWatch();

                CommonContainer.CommonServer.SetStartup(serverProject);
                //保存解决方案
                if (_dte.Solution.Saved == false)
                {
                    _dte.Solution.SaveAs(_dte.Solution.FullName);
                    KeywordContainer.SetContainer();
                }
            }
            catch (Exception ex)
            {
               Utility.Help.MsgBoxHelp.ShowError("Entity2Code Error:", ex);
            }
        }

        void context_GeneratedOne(object sender, EventGeneratedArg arg)
        {
            int currentCount = _currentTotalCount- arg.CurrentCount;
            int percent = (int)(_totalPercent * (_processCurrentCount / _processTotalCount) * (currentCount / _currentTotalCount) * 100);
            _work.ReportProgress(percent, string.Format(Properties.Resources.EndGenerate, PrjCmdId.FindProjectName(arg.GeneratedId), arg.CurrentEntity));
        }

        private void AddRef()
        {
            ReferenceContext context = new ReferenceContext();
            context.injection(PrjCmdId.Infrastructure, PrjCmdId.DomainContext, PrjCmdId.DomainEntity, "iTelluro.Explorer.Domain.CodeFirst.Seedwork.dll", "iTelluro.Explorer.Infrastruct.CodeFirst.Seedwork.dll", "EntityFramework.dll", "System.Data.Entity", "System.ComponentModel.DataAnnotations");

            context.injection(PrjCmdId.DomainContext, PrjCmdId.DomainEntity, "iTelluro.Explorer.Domain.CodeFirst.Seedwork.dll", "System.Data.Entity");

            context.injection(PrjCmdId.DomainEntity, "iTelluro.Explorer.Domain.CodeFirst.Seedwork.dll");

            context.injection(PrjCmdId.Application, PrjCmdId.Data2Object, PrjCmdId.DomainContext, PrjCmdId.DomainEntity, PrjCmdId.IApplication, "iTelluro.Explorer.Domain.CodeFirst.Seedwork.dll", "iTelluro.Explorer.Infrastruct.CodeFirst.Seedwork.dll", "iTelluro.Explorer.Application.CodeFirst.Seedwork.dll", "iTelluro.Explorer.Infrastructure.CrossCutting.dll");
            context.injection(PrjCmdId.IApplication, PrjCmdId.DomainEntity, PrjCmdId.Data2Object);
            context.injection(PrjCmdId.Data2Object, PrjCmdId.DomainEntity, "System.Runtime.Serialization", "iTelluro.Explorer.Domain.CodeFirst.Seedwork.dll", "AutoMapper.dll", "AutoMapper.Net4.dll");
            context.injection(PrjCmdId.Service, PrjCmdId.Infrastructure, PrjCmdId.Application, PrjCmdId.IApplication, PrjCmdId.DomainContext, PrjCmdId.Data2Object, "System.Runtime.Serialization", "System.ServiceModel", "EntityFramework.dll", "iTelluro.Explorer.Domain.CodeFirst.Seedwork.dll", "iTelluro.Explorer.Infrastruct.CodeFirst.Seedwork.dll", "iTelluro.Explorer.Application.CodeFirst.Seedwork.dll", "iTelluro.Explorer.Infrastructure.CrossCutting.dll", "AutoMapper.dll", "AutoMapper.Net4.dll", "iTelluro.Explorer.Infrastructure.CrossCutting.NetFramework.dll", "iTelluro.SSO.dll", "iTelluro.SSO.Common.dll", "iTelluro.SSO.WebServices.dll", "iTelluro.SYS.Entity.dll", "iTelluro.Utility.dll", "log4net.dll", "Microsoft.Practices.Unity.dll");
            context.HandledOne += context_HandledOne;
            _totalPercent = 1;
            _processCurrentCount = 1;
            _processTotalCount = 1;
            _currentTotalCount = context.Count;
            context.Commit();
        }

        void context_HandledOne(object sender, EventHandledArg arg)
        {
            int currentCount = _currentTotalCount - arg.CurrentCount;
            int percent = (int)(_totalPercent * (_processCurrentCount / _processTotalCount) * (currentCount / _currentTotalCount) * 100);
            _work.ReportProgress(percent, string.Format(Properties.Resources.BeginReference, PrjCmdId.FindProjectName(arg.HandledId)));
        }
    }

}

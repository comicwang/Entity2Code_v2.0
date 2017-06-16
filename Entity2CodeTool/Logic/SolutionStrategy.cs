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
using Utility.Entity;

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
                NewProject();
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
        private void NewProject()
        {
            try
            {
                bool allowNew = true;
                GenerateEntityContext context = new GenerateEntityContext();
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

                #region 循环实体

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
                    context.injection(generateCode, CdeCmdId.InfrastructureId.Repository, allowNew, keycontainer);//Repository
                    context.injection(generateCode, CdeCmdId.ApplicationId.Application, allowNew, keycontainer);//Application
                    context.injection(generateCode, CdeCmdId.IApplicationId.IApplication, allowNew, keycontainer);//IApplication
                    context.injection(generateCode, CdeCmdId.Data2ObjectId.Data2Obj, allowNew, keycontainer);//Data2Obj
                    context.injection(generateCode, CdeCmdId.InfrastructureId.Map, allowNew, keycontainer);//Map
                    context.injection(generateCode, CdeCmdId.DomainEntityId.Entity, allowNew, keycontainer);//Entity
                    context.injection(generateProTemp, CdeCmdId.Data2ObjectId.Profile.UnFix, count == CodeFirstLogic.Tables.Count, keycontainer);//ProfiTemp
                    context.injection(generateTemp, CdeCmdId.InfrastructureId.DBContext.UnFix, count == CodeFirstLogic.Tables.Count, keycontainer);//DbContextTemp
                    context.injection(serverProTemp, CdeCmdId.ServiceId.Service.UnFix, count == CodeFirstLogic.Tables.Count, keycontainer);//ServerTemp
                    context.injection(iServerProTemp, CdeCmdId.ServiceId.IService.UnFix, count == CodeFirstLogic.Tables.Count, keycontainer);//IServerTemp
                    context.injection(containerTemp, CdeCmdId.ServiceId.Container.UnFix, count == CodeFirstLogic.Tables.Count, keycontainer);//ContainerTemp
                    context.Commit();
                }

                #endregion

                context.injection(generateCode,CdeCmdId.InfrastructureId.DbContextExtensions, allowNew, container);//DbContextExtensions        
                context.injection(generateCode,CdeCmdId.InfrastructureId.DBContext.Fix, allowNew, null);//DbContext
                context.injection(generateCode,CdeCmdId.Data2ObjectId.Profile.Fix, allowNew, null);//DbContext
                context.injection(generateCode,CdeCmdId.InfrastructureId.ContextUnit, allowNew, null);//DbContextUnit
                context.injection(generateCode,CdeCmdId.ServiceId.IService.Fix, allowNew, null);//IServer
                context.injection(generateCode,CdeCmdId.ServiceId.Service.Fix, allowNew, null);//Server
                context.injection(generateCode,CdeCmdId.ServiceId.Container.Fix, allowNew, null);//Container
                context.injection(generateCode,CdeCmdId.ServiceId.WebConfig, allowNew, null);//WebConfig
                context.injection(generateCode,CdeCmdId.ServiceId.UnityInstanceProviderServiceBehavior, allowNew, null);//UnityInstanceProviderServiceBehavior
                context.injection(generateCode,CdeCmdId.ServiceId.UnityInstanceProvider, allowNew, null);//UnityInstanceProvider       
                context.injection(generateCode,CdeCmdId.ServiceId.AttachDataSignBehavior, allowNew, null);//AttachDataSignBehavior      ,
                context.injection(generateCode,CdeCmdId.ServiceId.CodeBehind, allowNew, null);//CodeBehind
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

        private void AddRef()
        {
            CommonContainer.CommonServer.OutString("开始添加构架程序集引用.", true);

            CommonContainer.CommonServer.OutString("添加基础结构层程序集引用.", true);
            //Infrastructure
            Project proj = CommonContainer.r PrjCmdId.Infrastructure
            .AddReferenceFromProject(ProjectContainer.DomainContext);
            ProjectContainer.Infrastructure.AddReferenceFromProject(ProjectContainer.DomainEntity);
            ProjectContainer.Infrastructure.AddReference("iTelluro.Explorer.Domain.CodeFirst.Seedwork.dll".GetFileResource("Dll"));
            ProjectContainer.Infrastructure.AddReference("iTelluro.Explorer.Infrastruct.CodeFirst.Seedwork.dll".GetFileResource("Dll"));

            ProjectContainer.Infrastructure.AddReference("EntityFramework.dll".GetFileResource("Dll"));
            ProjectContainer.Infrastructure.AddReference("System.Data.Entity");
            ProjectContainer.Infrastructure.AddReference("System.ComponentModel.DataAnnotations");

            CommonContainer.CommonServer.OutString("添加领域层程序集引用.", true);
            //DomainContext
            ProjectContainer.DomainContext.AddReferenceFromProject(ProjectContainer.DomainEntity);
            ProjectContainer.DomainContext.AddReference("iTelluro.Explorer.Domain.CodeFirst.Seedwork.dll".GetFileResource("Dll"));
            ProjectContainer.DomainEntity.AddReference("System.Data.Entity");

            CommonContainer.CommonServer.OutString("添加领域实体层程序集引用.", true);
            //DomainEntity
            ProjectContainer.DomainEntity.AddReference("iTelluro.Explorer.Domain.CodeFirst.Seedwork.dll".GetFileResource("Dll"));

            CommonContainer.CommonServer.OutString("添加应用层程序集引用.", true);
            //Application
            ProjectContainer.Application.AddReferenceFromProject(ProjectContainer.Data2Object);
            ProjectContainer.Application.AddReferenceFromProject(ProjectContainer.DomainContext);
            ProjectContainer.Application.AddReferenceFromProject(ProjectContainer.DomainEntity);
            ProjectContainer.Application.AddReferenceFromProject(ProjectContainer.IApplication);
            ProjectContainer.Application.AddReference("iTelluro.Explorer.Domain.CodeFirst.Seedwork.dll".GetFileResource("Dll"));
            ProjectContainer.Application.AddReference("iTelluro.Explorer.Infrastruct.CodeFirst.Seedwork.dll".GetFileResource("Dll"));
            ProjectContainer.Application.AddReference("iTelluro.Explorer.Application.CodeFirst.Seedwork.dll".GetFileResource("Dll"));
            ProjectContainer.Application.AddReference("iTelluro.Explorer.Infrastructure.CrossCutting.dll".GetFileResource("Dll"));

            CommonContainer.CommonServer.OutString("添加应用接口层程序集引用.", true);
            //IApplication
            ProjectContainer.IApplication.AddReferenceFromProject(ProjectContainer.DomainEntity);
            ProjectContainer.IApplication.AddReferenceFromProject(ProjectContainer.Data2Object);

            CommonContainer.CommonServer.OutString("添加应用实体层程序集引用.", true);
            //Data2Object
            ProjectContainer.Data2Object.AddReferenceFromProject(ProjectContainer.DomainEntity);
            ProjectContainer.Data2Object.AddReference("System.Runtime.Serialization");
            ProjectContainer.Data2Object.AddReference("iTelluro.Explorer.Domain.CodeFirst.Seedwork.dll".GetFileResource("Dll"));
            ProjectContainer.Data2Object.AddReference("AutoMapper.dll".GetFileResource("Dll"));
            ProjectContainer.Data2Object.AddReference("AutoMapper.Net4.dll".GetFileResource("Dll"));


            CommonContainer.CommonServer.OutString("添加服务层程序集引用.", true);

            ProjectContainer.Service.AddReferenceFromProject(ProjectContainer.Infrastructure);
            ProjectContainer.Service.AddReferenceFromProject(ProjectContainer.Application);
            ProjectContainer.Service.AddReferenceFromProject(ProjectContainer.IApplication);
            ProjectContainer.Service.AddReferenceFromProject(ProjectContainer.DomainContext);
            ProjectContainer.Service.AddReferenceFromProject(ProjectContainer.Data2Object);
            ProjectContainer.Service.AddReference("System.Runtime.Serialization");
            ProjectContainer.Service.AddReference("System.ServiceModel");
            ProjectContainer.Service.AddReference("EntityFramework.dll".GetFileResource("Dll"));
            ProjectContainer.Service.AddReference("iTelluro.Explorer.Domain.CodeFirst.Seedwork.dll".GetFileResource("Dll"));
            ProjectContainer.Service.AddReference("iTelluro.Explorer.Infrastruct.CodeFirst.Seedwork.dll".GetFileResource("Dll"));
            ProjectContainer.Service.AddReference("iTelluro.Explorer.Application.CodeFirst.Seedwork.dll".GetFileResource("Dll"));
            ProjectContainer.Service.AddReference("iTelluro.Explorer.Infrastructure.CrossCutting.dll".GetFileResource("Dll"));
            ProjectContainer.Service.AddReference("AutoMapper.dll".GetFileResource("Dll"));
            ProjectContainer.Service.AddReference("AutoMapper.Net4.dll".GetFileResource("Dll"));
            ProjectContainer.Service.AddReference("iTelluro.Explorer.Infrastructure.CrossCutting.NetFramework.dll".GetFileResource("Dll"));
            ProjectContainer.Service.AddReference("iTelluro.SSO.dll".GetFileResource("Dll"));
            ProjectContainer.Service.AddReference("iTelluro.SSO.Common.dll".GetFileResource("Dll"));
            ProjectContainer.Service.AddReference("iTelluro.SSO.WebServices.dll".GetFileResource("Dll"));
            ProjectContainer.Service.AddReference("iTelluro.SYS.Entity.dll".GetFileResource("Dll"));
            ProjectContainer.Service.AddReference("iTelluro.Utility.dll".GetFileResource("Dll"));
            ProjectContainer.Service.AddReference("log4net.dll".GetFileResource("Dll"));
            ProjectContainer.Service.AddReference("Microsoft.Practices.Unity.dll".GetFileResource("Dll"));

        }
    }

}

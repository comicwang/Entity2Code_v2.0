using Infoearth.Entity2CodeTool.Helps;
using Infoearth.Entity2CodeTool.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool.Logic
{
    public class ServiceLogic
    {
        #region methods

        public static void CreateFrame(bool overWrite)
        {
            if (ProjectContainer.Service == null)
            {
                ProjectContainer.Service = SolutionCommon.Dte.AddWebService(SolutionCommon.Service, true);
            }

            if (ProjectContainer.Service.ProjectItems.Find("web.config") != null)
                ProjectContainer.Service.ProjectItems.Find("web.config").Delete();
            if (SolutionCommon.infrastryctType == InfrastructType.DbFirst)
            {
                using (StreamReader reader = new StreamReader(Path.Combine(ProjectContainer.Infrastructure.ToDirectory(), "app.config")))
                {
                    while (reader.Peek() != -1)
                    {
                        string temp = reader.ReadLine();
                        if (temp.IndexOf(ModelContainer.Resolve("$ContextName$")) != -1)
                        {
                            ModelContainer.Regist("$ConnectionString$", temp, "数据库连接字符串");
                            break;
                        }
                    }
                }
            }
            else
            {
                string str = string.Empty;
                if (CodeFirstTools.DataType == "MicrosoftSqlServer")
                    str = "<add name=\"" + CodeFirstTools.DbContextName + "\" providerName=\"" + CodeFirstTools.ProviderName + "\" connectionString=\"" + CodeFirstTools.ConnectionString + ";MultipleActiveResultSets=True;Pooling=True;\"/>";
                else
                    str = "<add name=\"" + CodeFirstTools.DbContextName + "\" providerName=\"Oracle.DataAccess.Client\" connectionString=\"" + CodeFirstTools.ConnectionString + "\"/>";
                ModelContainer.Regist("$ConnectionString$", str, "数据库连接字符串");
            }

            if (SolutionCommon.infrastryctType == InfrastructType.CodeFirst)
                ModelContainer.Regist("$DBSchemaApp$", string.Format("<add key=\"DBSchema\" value=\"{0}\"/>", CodeFirstTools.SchemaName), "数据库命名空间");
            else
            {
                ModelContainer.Regist("$DBSchemaApp$", "", "数据库命名空间");
            }
            CodeStaticManager staticManager = new CodeStaticManager(ConstructType.WebConfig);
            staticManager.BuildTaget = new StringCodeArgment() { Encode = Encoding.UTF8, Name = "web.config", Target = ProjectContainer.Service };
            staticManager.CreateCode();

            //ServiceBehinde
            string behindCode = string.Format("<%@ ServiceHost Language=\"C#\" Debug=\"true\" Service=\"{1}.{0}Service\" CodeBehind=\"{0}Service.svc.cs\" %>", SolutionCommon.ProjectName, SolutionCommon.Service);
            ProjectContainer.Service.AddFromFileString(behindCode, SolutionCommon.ProjectName + "Service.svc", Encoding.Default);


            //AttachDataSignBehavior
            staticManager = new CodeStaticManager(ConstructType.AttachDataSignBehavior);
            staticManager.BuildTaget = new StringCodeArgment() { Folder = "InstanceProviders", Name = "AttachDataSignBehavior.cs", Target = ProjectContainer.Service };
            staticManager.CreateCode();

            //UnityInstanceProvider
            staticManager = new CodeStaticManager(ConstructType.UnityInstanceProvider);
            staticManager.BuildTaget = new StringCodeArgment() { Folder = "InstanceProviders", Name = "UnityInstanceProvider.cs", Target = ProjectContainer.Service };
            staticManager.CreateCode();

            //UnityInstanceProviderServiceBehavior
            staticManager = new CodeStaticManager(ConstructType.UnityInstanceProviderServiceBehavior);
            staticManager.BuildTaget = new StringCodeArgment() { Folder = "InstanceProviders", Name = "UnityInstanceProviderServiceBehavior.cs", Target = ProjectContainer.Service };
            staticManager.CreateCode();

            //AssemblyInfo
            string assemblyInfoPath = Path.Combine(ProjectContainer.Service.ToDirectory(), "Properties", "AssemblyInfo.cs");
            StringBuilder build = FileOprateHelp.ReadFile(assemblyInfoPath);
            build.AppendLine("[assembly: log4net.Config.XmlConfigurator(Watch = true)]");//日志监视
            FileOprateHelp.SaveFile(build.ToString(), assemblyInfoPath);

            SolutionCommon.Dte.SetStartup(ProjectContainer.Service);
        }

        public static void CreateCode(bool overWrite)
        {
            if (ProjectContainer.Service == null)
                throw new Exception("Entity2Code Service Project Can not be Find");

            if (overWrite)
            {
                CodeBuilderContainer.ServiceBuilder.Clear();
                CodeBuilderContainer.IServiceBuilder.Clear();
                CodeBuilderContainer.ContainBuilder.Clear();
                List<TemplateEntity> entitys = DomainEntityLogic.GetEntitys(false);
                for (int i = 0; i < entitys.Count; i++)
                {
                    CodeAppendManager codeManager = new CodeAppendManager(ConstructType.Service, entitys[i]);
                    codeManager.BuildTaget = CodeBuilderContainer.ServiceBuilder;
                    codeManager.CreateCode();

                    codeManager = new CodeAppendManager(ConstructType.IService, entitys[i]);
                    codeManager.BuildTaget = CodeBuilderContainer.IServiceBuilder;
                    codeManager.CreateCode();

                    codeManager = new CodeAppendManager(ConstructType.Container, entitys[i]);
                    codeManager.BuildTaget = CodeBuilderContainer.ContainBuilder;
                    codeManager.CreateCode();
                }
            }

            CodeStaticManager staticManager = new CodeStaticManager(ConstructType.Service);
            //Service
            staticManager.BuildTaget = new StringCodeArgment() { Name = SolutionCommon.ProjectName + "Service.svc.cs", Target = ProjectContainer.Service };
            staticManager.CreateCode();

            //IService
            staticManager = new CodeStaticManager(ConstructType.IService);
            staticManager.BuildTaget = new StringCodeArgment() { Name = "I" + SolutionCommon.ProjectName + "Service.cs", Target = ProjectContainer.Service };
            staticManager.CreateCode();

            //Container
            if (SolutionCommon.infrastryctType == InfrastructType.CodeFirst)
            {
                ModelContainer.Regist("$DBAppContent$", " string db= System.Configuration.ConfigurationManager.AppSettings[\"DBSchema\"];","获取数据库连接字符串的函数内容");
                ModelContainer.Regist("$DBConstr$", "new InjectionConstructor(db)","注册上下文的函数内容");
            }
            else
            {
                ModelContainer.Regist("$DBAppContent$", "", "获取数据库连接字符串的函数内容");
                ModelContainer.Regist("$DBConstr$", "", "注册上下文的函数内容");
            }
            staticManager = new CodeStaticManager(ConstructType.Container);
            staticManager.BuildTaget = new StringCodeArgment() { Folder = "InstanceProviders", Name = "Container.cs", Target = ProjectContainer.Service };
            staticManager.CreateCode();

         
        }

        #endregion
    }
}

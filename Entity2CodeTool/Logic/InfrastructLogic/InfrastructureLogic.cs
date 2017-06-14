using EnvDTE;
using Infoearth.Entity2CodeTool.Converter;
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
    public class InfrastructureLogic
    {
        #region methods
        public static void Create(bool overWrite)
        {
            StringBuilder build = new StringBuilder();
            //Infrustruture
            if (ProjectContainer.Infrastructure == null || overWrite)
            {
                ProjectContainer.Infrastructure = SolutionCommon.Dte.AddClassLibrary(SolutionCommon.Infrastructure, true);
            }
            ProjectContainer.Infrastructure.AddAdoNetEntityDataModel(SolutionCommon.ProjectName);
            ProjectContainer.Infrastructure.ProjectItems.Find(SolutionCommon.ProjectName + ".tt", true).Delete();
            string contextPath = SolutionCommon.ProjectName + ".Context.tt";
            ProjectContainer.Infrastructure.ProjectItems.Find(contextPath, true).Remove();
            ProjectContainer.Infrastructure.AddFromFile(Path.Combine(ProjectContainer.Infrastructure.ToDirectory(), contextPath)).Name = SolutionCommon.ProjectName + "Context.tt";
            ProjectContainer.Infrastructure.Save();
            ProjectItem contextItem = ProjectContainer.Infrastructure.ProjectItems.Find(SolutionCommon.ProjectName + "Context.cs", true);
            EditPoint editPoint = contextItem.FileCodeModel.CodeElements.Item(1).StartPoint.CreateEditPoint();
            editPoint.Insert(string.Format("using {0};\r\n", SolutionCommon.DomainEntity));
            //Extention  
            CodeStaticManager codeManager = new CodeStaticManager(ConstructType.DbContextExtensions);
            codeManager.BuildTaget = new StringCodeArgment() { Folder = "Extention", Name = "DbContextExtensions.cs", Target = ProjectContainer.Infrastructure };
            codeManager.CreateCode();
            //Unit

            using (StreamReader reader = new StreamReader(Path.Combine(ProjectContainer.Infrastructure.ToDirectory(), SolutionCommon.ProjectName + "Context.cs")))
            {
                string ContextName = string.Empty;
                while (reader.Peek() != -1)
                {
                    string temp = reader.ReadLine();
                    if (temp.IndexOf("class") != -1)
                    {
                        temp = temp.Substring(25);
                        ContextName = temp.Split(':')[0].Trim();
                        ModelContainer.Regist("$ContextName$", ContextName,"上下文名称");
                        break;
                    }
                }
            }

            codeManager = new CodeStaticManager(ConstructType.ContextUnit);
            codeManager.BuildTaget = new StringCodeArgment() { Name = SolutionCommon.ProjectName + "ContextUnit.cs", Encode = Encoding.Default, Target = ProjectContainer.Infrastructure };
            codeManager.CreateCode();
        }

        public static void CreateCodeFirst(bool overWrite = true)
        {
            if (ProjectContainer.Infrastructure == null)
            {
                ProjectContainer.Infrastructure = SolutionCommon.Dte.AddClassLibrary(SolutionCommon.Infrastructure, true);
            }
            CodeStaticManager codeManager = new CodeStaticManager(ConstructType.DbContextExtensions);
            codeManager.BuildTaget = new StringCodeArgment() { Folder = "Extention", Name = "DbContextExtensions.cs", Target = ProjectContainer.Infrastructure };
            codeManager.CreateCode();

            CodeFirstLogic.WriteConfigFies(overWrite);

            Tables tables = CodeFirstLogic.GetTables();

            CodeBuilderContainer.DBContextBuilder.Clear();
            foreach (Table tb in tables)
            {
                CodeAppendManager appendManager = new CodeAppendManager(ConstructType.DBContext, new TemplateEntity() { Entity = tb.Name, Data2Obj = tb.NameHumanCase });
                appendManager.BuildTaget = CodeBuilderContainer.DBContextBuilder;
                appendManager.CreateCode();
            }
            ModelContainer.Regist("$ContextName$", CodeFirstTools.DbContextName,"上下文名称");
            codeManager = new CodeStaticManager(ConstructType.DBContext);
          
            codeManager.BuildTaget = new StringCodeArgment() { Name = CodeFirstTools.DbContextName + ".cs", Target = ProjectContainer.Infrastructure };
            codeManager.CreateCode();

            codeManager = new CodeStaticManager(ConstructType.ContextUnit);
            codeManager.BuildTaget = new StringCodeArgment() { Name = CodeFirstTools.DbContextName + "Unit.cs", Encode = Encoding.Default, Target = ProjectContainer.Infrastructure };
            codeManager.CreateCode();
        }

        #endregion

    }
}

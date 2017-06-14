using Infoearth.Entity2CodeTool.Converter;
using Infoearth.Entity2CodeTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infoearth.Entity2CodeTool.Helps;

namespace Infoearth.Entity2CodeTool.Logic
{
    public class Data2ObjLogic
    {
        #region attrs and fields

        #endregion

        #region methods

        /// <summary>
        /// 创建DTO实体的代码
        /// </summary>
        /// <param name="overWrite">是否重写</param>
        public static void Create(bool overWrite)
        {
            if (ProjectContainer.Data2Object == null)
                ProjectContainer.Data2Object = SolutionCommon.Dte.AddClassLibrary(SolutionCommon.Data2Object);
            else
            {
                if (overWrite)
                {
                    ProjectContainer.Data2Object.Delete();
                    ProjectContainer.Data2Object = SolutionCommon.Dte.AddClassLibrary(SolutionCommon.Data2Object);
                }
            }

            List<TemplateEntity> entitys = DomainEntityLogic.GetEntitys(false);

            CodeBuilderContainer.ProfileBuilder.Clear();
            for (int i = 0; i < entitys.Count; i++)
            {
                // _work.ReportProgress((int)((i + 1) * 100 / entitys.Length), string.Format("添加实体类-{0}-的代码", fileEntity));
                TemplateEntity tmpEntity = entitys[i];
                Data2ObjCerateManager codeManager = new Data2ObjCerateManager(ConstructType.Data2Obj, tmpEntity);
                codeManager.IsOverWrite = overWrite;
                codeManager.BuildTaget = ProjectContainer.Data2Object;
                codeManager.CreateCode();

                CodeAppendManager appendManager = new CodeAppendManager(ConstructType.Profile, tmpEntity);
                appendManager.BuildTaget = CodeBuilderContainer.ProfileBuilder;
                appendManager.CreateCode();
            }

            CodeStaticManager staticManager = new CodeStaticManager(ConstructType.Profile);
            //Profile

            ModelPathConverter.GetCodePath(ConstructType.Profile);
            staticManager.BuildTaget = new StringCodeArgment() { Folder = "Profile", Name = SolutionCommon.ProjectName + "Profile.cs", Target = ProjectContainer.Data2Object };
            staticManager.CreateCode();
        }

        #endregion
    }
}

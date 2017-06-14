using Infoearth.Entity2CodeTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infoearth.Entity2CodeTool.Helps;

namespace Infoearth.Entity2CodeTool.Logic
{
    public class ApplicationLogic
    {
        #region methods

        /// <summary>
        /// 创建应用层的代码
        /// </summary>
        /// <param name="overWrite">是否重写</param>
        /// <param name="overReadEntity">是否重新获取实体信息</param>
        public static void Create(bool overWrite, bool overReadEntity = false)
        {
            if (ProjectContainer.Application == null)
            {
                ProjectContainer.Application = SolutionCommon.Dte.AddClassLibrary(SolutionCommon.Application, true);
            }
            if (ProjectContainer.IApplication == null)
            {
                ProjectContainer.IApplication = SolutionCommon.Dte.AddClassLibrary(SolutionCommon.IApplication, true);
            }

            List<TemplateEntity> entitys = DomainEntityLogic.GetEntitys(overReadEntity);
            
            for (int i = 0; i < entitys.Count; i++)
            {
                // _work.ReportProgress((int)((i + 1) * 100 / entitys.Length), string.Format("添加实体类-{0}-的代码", fileEntity));
                TemplateEntity tmpEntity = entitys[i];

                CodeCreateManager codeManager = new CodeCreateManager(ConstructType.Application, tmpEntity);
                codeManager.IsOverWrite = overWrite;
                codeManager.BuildTaget = ProjectContainer.Application;
                codeManager.CreateCode();

                codeManager = new CodeCreateManager(ConstructType.IApplication, tmpEntity);
                codeManager.IsOverWrite = overWrite;
                codeManager.BuildTaget = ProjectContainer.IApplication;
                codeManager.CreateCode();
            }
        }

        #endregion
    }
}

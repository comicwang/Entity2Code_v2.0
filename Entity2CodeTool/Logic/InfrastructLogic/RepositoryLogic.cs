using Infoearth.Entity2CodeTool.Converter;
using Infoearth.Entity2CodeTool.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infoearth.Entity2CodeTool.Helps;

namespace Infoearth.Entity2CodeTool.Logic
{
    public class RepositoryLogic
    {
        #region methods

        /// <summary>
        /// 创建仓储的代码
        /// </summary>
        /// <param name="overWrite">是否重写</param>
        public static void Create(bool overWrite)
        {
            if (ProjectContainer.DomainContext == null || overWrite)
            {
                ProjectContainer.DomainContext = SolutionCommon.Dte.AddClassLibrary(SolutionCommon.DomainContext, true);
            }

            List<TemplateEntity> entitys = DomainEntityLogic.GetEntitys(false);
            for (int i = 0; i < entitys.Count; i++)
            {
               // _work.ReportProgress((int)((i + 1) * 100 / entitys.Length), string.Format("添加实体类-{0}-的代码", fileEntity));
                TemplateEntity tmpEntity = entitys[i];

                CodeCreateManager codeManager = new CodeCreateManager(ConstructType.IRepository, tmpEntity);
                codeManager.IsOverWrite = overWrite;
                codeManager.BuildTaget = ProjectContainer.DomainContext;
                codeManager.CreateCode();

                codeManager = new CodeCreateManager(ConstructType.Repository, tmpEntity);
                codeManager.IsOverWrite = overWrite;
                codeManager.BuildTaget = ProjectContainer.Infrastructure;
                codeManager.CreateCode();
            }
        }

        #endregion

    }
}

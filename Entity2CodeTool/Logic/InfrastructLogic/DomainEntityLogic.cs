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
    public class DomainEntityLogic
    {
        #region fields

        /// <summary>
        /// 领域层实体信息
        /// </summary>
        private static List<TemplateEntity> _entitys;

        #endregion

        #region methods

        /// <summary>
        /// 创建领域层实体
        /// </summary>
        public static void Create(bool overWrite)
        {
            if (ProjectContainer.DomainEntity == null || overWrite)
            {
                ProjectContainer.DomainEntity = SolutionCommon.Dte.AddClassLibrary(SolutionCommon.DomainEntity, true);
            }

            CodeStaticManager codeManager = new CodeStaticManager(ConstructType.Entities);
            codeManager.BuildTaget = new StringCodeArgment() { Encode = Encoding.Default, Name = "Entities.tt", Target = ProjectContainer.DomainEntity };
            codeManager.CreateCode();

            ProjectContainer.DomainEntity.ProjectItems.Find("Entities.cs", true).Delete();
            ProjectContainer.DomainEntity.Save();
        }

        /// <summary>
        /// 获取所有领域层实体信息
        /// </summary>
        /// <param name="overRead">是否重新读</param>
        /// <returns></returns>
        public static List<TemplateEntity> GetEntitys(bool overRead)
        {
            if (_entitys == null || overRead)
            {
                _entitys = new List<TemplateEntity>();
                if (SolutionCommon.infrastryctType == InfrastructType.DbFirst)
                {
                    if (ProjectContainer.DomainEntity == null)
                        throw new Exception("Entity2Code DomainEntity Project Can not be Find");
                    string entityDir = ProjectContainer.DomainEntity.ToDirectory();
                    string[] files = Directory.GetFiles(entityDir, "*.cs");
                    if (null == files || files.Length == 0)
                        throw new Exception("Entity2Code DomainEntity ProjectItem is Null");               
                    foreach (string file in files)
                    {
                        string entity = Path.GetFileNameWithoutExtension(file);
                        _entitys.Add(new TemplateEntity() { Entity = entity, Data2Obj = ModelNameConverter.GetData2Obj(entity) });
                    }
                }
                else
                {
                    foreach (Table item in CodeFirstLogic.GetTables())
                    {
                        _entitys.Add(new TemplateEntity() { Entity = item.Name, Data2Obj = item.NameHumanCase });
                    }
                }
            }
            return _entitys;
        }

        public static void CreateCodeFirst(bool overwrite = true)
        {
            ProjectContainer.DomainEntity = SolutionCommon.Dte.AddClassLibrary(SolutionCommon.DomainEntity, true);
            CodeFirstLogic.WritePOCO(overwrite);
        }

        #endregion

    }
}

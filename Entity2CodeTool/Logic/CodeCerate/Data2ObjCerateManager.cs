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
    public class Data2ObjCerateManager : CodeManageBase
    {
        #region attrs and fields

        public override object BuildTaget { get; set; }

        private TemplateEntity _entity;

        private ConstructType _consType;

        public bool IsOverWrite = true;

        #endregion

        #region ctor

        public Data2ObjCerateManager(ConstructType consType, TemplateEntity entity)
            : base(consType, entity)
        {
            _consType = consType;
            _entity = entity;
        }

        #endregion

        #region methods

        public override void CreateCode()
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine(CodeContent);
            try
            {
                if (null == ProjectContainer.DomainEntity)
                    throw new Exception("Entity2Code DomainEntity Project Can not be Find");
                string path = Path.Combine(ProjectContainer.DomainEntity.ToDirectory(), _entity.Entity + ".cs");
                if (File.Exists(path) == false)
                    throw new Exception("Entity2Code DomainEntity Project Can not Find ProjectItem " + _entity.Entity + ".cs");
                if (SolutionCommon.infrastryctType == InfrastructType.DbFirst)
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        while (reader.Peek() != -1)
                        {
                            string line = reader.ReadLine();
                            if (line.IndexOf("get") != -1 && line.IndexOf("set") != -1 && line.IndexOf("virtual") == -1)
                            {
                                result.AppendLine("        [DataMember]");
                                result.AppendLine(line);
                            }
                        }
                    }
                }
                else
                {
                    Table tb = CodeFirstLogic.GetTables().GetTable(_entity.Entity, CodeFirstTools.SchemaName);
                    foreach (Column clm in tb.Columns)
                    {
                        result.AppendLine("        [DataMember]");
                        result.AppendLine(string.Format("        public {0}{1} {2} {3}", clm.PropertyType, CodeFirstTools.CheckNullable(clm), clm.PropertyName, "{get; set; }"));
                    }
                }
                result.AppendLine("   }");
                result.AppendLine("}");

                string targetPath = ModelPathConverter.GetCodePath(_consType, IsOverWrite, _entity);
                using (FileStream create = new FileStream(targetPath, FileMode.OpenOrCreate))
                {
                    byte[] buffer = Encoding.Default.GetBytes(result.ToString());
                    create.Write(buffer, 0, buffer.Length);
                }
                BuildResult = ProjectContainer.Data2Object.AddFromFile(targetPath);

                base.CreateCode();
            }
            catch (Exception ex)
            {
                MsgBoxHelp.ShowError("创建代码出现异常：", ex);
            }
        }

        #endregion

        #region events

        #endregion


    }
}

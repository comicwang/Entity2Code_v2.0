using EnvDTE;
using Infoearth.Entity2CodeTool.Converter;
using Infoearth.Entity2CodeTool.Helps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool.Logic
{
    /// <summary>
    /// 继承代码创建基类
    /// </summary>
    public class CodeCreateManager : CodeManageBase
    {
        #region attrs and fields

        private TemplateEntity _entity;

        private ConstructType _consType;

        public override object BuildTaget { get; set; }

        public bool IsOverWrite = true;

        #endregion

        #region ctor

        public CodeCreateManager(ConstructType consType,TemplateEntity entity)
            : base(consType,entity)
        {
            _consType = consType;
            _entity = entity;
        }

        #endregion

        public override void CreateCode()
        {
            try
            {
                string path = string.Empty;
                if (null == _entity)
                {
                    path = ModelPathConverter.GetCodePath(_consType, IsOverWrite);
                }
                else
                {
                    path = ModelPathConverter.GetCodePath(_consType, IsOverWrite, _entity);
                }
                using (FileStream create = new FileStream(path, FileMode.OpenOrCreate))
                {
                    byte[] buffer = Encoding.Default.GetBytes(CodeContent.ToString());
                    create.Write(buffer, 0, buffer.Length);
                }

                if (null == BuildTaget)
                    throw new Exception("Entity2Code Code Taget Lost");
                Project prj = BuildTaget as Project;
                if (null == prj)
                    throw new Exception("Entity2Code Code Taget Argment Error");
                BuildResult = prj.AddFromFile(path);
                base.CreateCode();
            }
            catch (Exception ex)
            {
                MsgBoxHelp.ShowError("创建代码异常了", ex);
            }
        }

        #region methods

        #endregion
    }
}

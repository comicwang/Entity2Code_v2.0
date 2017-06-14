using EnvDTE;
using Infoearth.Entity2CodeTool.Helps;
using Infoearth.Entity2CodeTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool.Logic
{
    public class CodeStaticManager : CodeManageBase
    {
        #region attrs and fields

        public override object BuildTaget { get; set; }

        private ConstructType _consType;
      
        #endregion

        #region ctor

        public CodeStaticManager(ConstructType consType)
            : base(consType)
        {
            _consType = consType;
        }

        #endregion

        #region methods

        public override void CreateCode()
        {
            try
            {
                if (null == BuildTaget)
                    throw new Exception("Entity2Code Code Taget Lost");
                StringCodeArgment fileObj = BuildTaget as StringCodeArgment;
                if (null == fileObj)
                    throw new Exception("Entity2Code Code Taget Argment Error");

                Project prj = fileObj.Target;
                BuildResult = string.IsNullOrEmpty(fileObj.Folder) ? prj.AddFromFileString(CodeContent.ToString(), fileObj.Name, fileObj.Encode) : BuildResult = prj.AddFromFileString(CodeContent.ToString(), fileObj.Folder, fileObj.Name);
                base.CreateCode();
            }
            catch (Exception ex)
            {
                MsgBoxHelp.ShowError("创建代码异常了", ex);
            }
        }

        #endregion
    }
}

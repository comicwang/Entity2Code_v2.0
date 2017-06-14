using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;
using Utility.Core;
using Utility.Base;
using System.IO;

namespace Infoearth.Entity2CodeTool.Generate
{
    public class ReplaceGenerate : CodeGenerateBase
    {
        public override object[] GetGenerateInfo(string guid, bool allowNew)
        {
            //找到项目源
            string pid = CdeCmdId.BelongId(guid);
            Project prjt = TemplateContainer.Resove<Project>(pid);
            if (prjt == null && allowNew)
            {
                string pname = PrjCmdId.FindProjectName(pid);
                if (pid != "06D57C23-4D51-430D-A8E6-9A19F38390E3")
                    prjt = CommonContainer.CommonServer.AddClassLibrary(pname);
                else
                    prjt = CommonContainer.CommonServer.AddWebService(pname);

                TemplateContainer.Regist(pid, prjt);
            }

            string targetPath=Path.Combine(CommonContainer.RootPath,TemplateContainer.Resove<string>(guid));

            return new object[] { guid, pid, prjt, targetPath };

        }

        public override void HandleGenerateContainer(Dictionary<string, string> containerArgment)
        {
            
        }

        public override bool GenerateCode(object[] info)
        {
            return false;
        }


    }
}

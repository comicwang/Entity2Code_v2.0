using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;
using Utility.Core;
using Utility.Base;
using Utility.Converter;
using Utility.Help;

namespace Utility.Generate
{
    public class CodeGenerate : CodeGenerateBase
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

            string templatePath = Path.Combine(CommonContainer.RootPath, TemplateContainer.Resove<string>(guid));

            return new object[] { guid, pid, prjt, templatePath };

        }

        public override void HandleGenerateContainer(Dictionary<string, string> containerArgment)
        {
            if (containerArgment != null)
                foreach (var item in containerArgment)
                {
                    KeywordContainer.RegistSource(item.Key, item.Value);
                }
        }

        public override bool GenerateCode(object[] info)
        {
            try
            {
                StringBuilder _tempBuild = new StringBuilder();
                using (StreamReader reader = new StreamReader(info[3].ToString()))
                {
                    while (reader.Peek() != -1)
                    {
                        string temp = reader.ReadLine();
                        temp = KeywordContainer.Replace(temp);
                        _tempBuild.AppendLine(temp);
                    }
                }
                string guid = info[0].ToString();
                Project prjt = info[2] as Project;
                string folder = string.Empty;
                if (CdeCmdId.HasForlder(guid, out folder))
                {
                    prjt.AddFromFileString(_tempBuild.ToString(), folder, StringConverter.ConvertFileName(guid));
                }
                else
                    prjt.AddFromFileString(_tempBuild.ToString(), StringConverter.ConvertFileName(guid), Encoding.Default);
                return true;
            }
            catch (Exception ex)
            {
                MsgBoxHelp.ShowError(string.Format("创建代码失败-{0}", info[0]), ex);
                return false;
            }
        }
    }
}

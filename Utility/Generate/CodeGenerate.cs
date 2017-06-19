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
    /// <summary>
    /// 生成代码文件的主要类
    /// </summary>
    public class CodeGenerate : CodeGenerateBase
    {
        /// <summary>
        /// 收集相关信息
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="allowNew">是否需要创建项目</param>
        /// <returns></returns>
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

        /// <summary>
        /// 处理容器
        /// </summary>
        /// <param name="containerArgment"></param>
        public override void HandleGenerateContainer(Dictionary<string, string> containerArgment)
        {
            if (containerArgment != null)
                foreach (var item in containerArgment)
                {
                    KeywordContainer.RegistSource(item.Key, item.Value);
                }
        }

        /// <summary>
        /// 创建代码
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
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
                Encoding encode = Encoding.Default;
                if (guid == CdeCmdId.ServiceId.WebConfig)
                    encode = Encoding.UTF8;
                if (CdeCmdId.HasForlder(guid, out folder))
                {
                    prjt.AddFromFileString(_tempBuild.ToString(), folder, StringConverter.ConvertFileName(guid),encode);
                }
                else
                    prjt.AddFromFileString(_tempBuild.ToString(), StringConverter.ConvertFileName(guid), encode);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

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

namespace Utility.Generate
{
    public class TempGenerate : CodeGenerateBase
    {
        public StringBuilder TempBuild;

        public string TemplateName;

        public override object[] GetGenerateInfo(string guid, bool key)
        {
            string templatePath = Path.Combine(CommonContainer.RootPath, TemplateContainer.Resove<string>(guid));
            return new object[] { guid,templatePath,key };
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
                using (StreamReader reader = new StreamReader(info[1].ToString()))
                {
                    while (reader.Peek() != -1)
                    {
                        string temp = reader.ReadLine();
                        temp = KeywordContainer.Replace(temp);
                        TempBuild.AppendLine(temp);
                    }
                }

                if ((bool)info[2])
                {
                    KeywordContainer.RegistSource(TemplateName, TempBuild.ToString());
                    TempBuild.Clear();
                }
                return true;
            }
            catch (Exception ex)
            {
                MsgBoxHelp.ShowError(string.Format("创建临时代码失败-{0}", info[0]), ex);
                return false;
            }
        }


    }
}

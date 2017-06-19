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
using Utility.Help;

namespace Utility.Generate
{
    /// <summary>
    /// 用于生成临时文件的类（不会创建项目项）
    /// </summary>
    public class TempGenerate : CodeGenerateBase
    {
        /// <summary>
        /// 临时的字符串容器
        /// </summary>
        public StringBuilder TempBuild;

        /// <summary>
        /// 临时字符串要用到的关键字容器的关键字名称
        /// </summary>
        public string TemplateName;

        /// <summary>
        /// 收集信息
        /// </summary>
        /// <param name="guid">ID</param>
        /// <param name="key">是否需要注入到关键字容器中</param>
        /// <returns></returns>
        public override object[] GetGenerateInfo(string guid, bool key)
        {
            string templatePath = Path.Combine(CommonContainer.RootPath, TemplateContainer.Resove<string>(guid));
            return new object[] { guid,templatePath,key };
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
        /// 生成临时代码
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
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
                throw ex;
            }
        }


    }
}

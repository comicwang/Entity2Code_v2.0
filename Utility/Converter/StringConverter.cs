using EnvDTE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;
using Utility.Core;

namespace Utility.Converter
{
    /// <summary>
    /// 提供字符串转换扩展
    /// </summary>
    public static class StringConverter
    {
        /// <summary>
        /// 转换需要生成的项目项名称（提供关键字容器的解析）
        /// </summary>
        /// <param name="guid">项目项ID</param>
        /// <returns></returns>
        public static string ConvertFileName(string guid)
        {
            string tempName = CdeCmdId.TempFileName(guid);
            if (tempName == null)
            {
                string modelPath = TemplateContainer.Resove<string>(guid);
                tempName = Path.GetFileNameWithoutExtension(modelPath) + ".cs";
            }
            else
            {
                tempName = KeywordContainer.Replace(tempName);
            }
            return tempName;
        }

        /// <summary>
        /// 获取资源文件的全路径
        /// </summary>
        /// <param name="fileName">资源文件名称</param>
        /// <returns></returns>

        public static string ConvertPath(this string fileName)
        {
            return Path.Combine(Properties.Resource.RootPath, fileName);
        }

        /// <summary>
        /// 获取类库文件的版本
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ConvertVision(this string filePath)
        {
            FileVersionInfo myFileVersion = FileVersionInfo.GetVersionInfo(filePath);
            return myFileVersion.FileVersion;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Infoearth.Entity2CodeTool.Helps;

namespace Infoearth.Entity2CodeTool.Helps
{
    /// <summary>
    /// 资源文件处理帮助
    /// </summary>
    public static class ResourceFileHelp
    {
        /// <summary>
        /// 获取图像资源文件
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <returns>图片对象</returns>
        public static Image GetImageResource(this string fileName)
        {
            string fullPath = ToModelRoot(fileName);
            if (!File.Exists(fullPath))
                FileOprateHelp.SaveFile(GetResourceStream(fileName), fullPath);
            return Image.FromFile(fullPath);
        }

        /// <summary>
        /// 获取图像资源文件(文件不在根目录)
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="folder">文件夹名称</param>
        /// <returns>图片对象</returns>
        public static Image GetImageResource(this string fileName, params string[] folder)
        {
            string sourceKey = string.Empty;
            Array.ForEach(folder, (o) => { sourceKey += o; sourceKey += "."; });
            sourceKey += fileName;
            return GetImageResource(sourceKey);
        }

        /// <summary>
        /// 获取文件资源
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <returns>文件路径</returns>
        public static string GetFileResource(this string fileName)
        {
            string fullPath = ToModelRoot(fileName);
            if (!File.Exists(fullPath))
                FileOprateHelp.SaveFile(GetResourceStream(fileName), fullPath);
            return fullPath;
        }

        /// <summary>
        /// 获取文件资源（非根目录）
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="folder">文件夹</param>
        /// <returns>文件路径</returns>
        public static string GetFileResource(this string fileName, params string[] folder)
        {
            string sourceKey = string.Empty;
            Array.ForEach(folder, (o) => { sourceKey += o; sourceKey += "."; });
            sourceKey += fileName;
            return GetFileResource(sourceKey);
        }

        /// <summary>
        /// 根据文件后缀获取资源文件
        /// </summary>
        /// <param name="directory">文件根目录</param>
        /// <param name="filter">文件后缀cs/dll等</param>
        /// <returns>所有文件路径集合</returns>
        public static List<string> GetFilesResourceByFilter(this string directory, string filter)
        {
            List<string> result = new List<string>();
            Assembly asm = Assembly.GetExecutingAssembly();
            string comparer = string.Empty;
            Array.ForEach(asm.GetManifestResourceNames(), (o) =>
            {
                if (o.EndsWith(filter))
                {
                    string fullPath = ToDllName(o);
                    FileOprateHelp.SaveFile(GetResourceStreamByFull(o), fullPath);
                    result.Add(fullPath);
                }
            });
            return result;
        }

        /// <summary>
        /// 根据完整资源命名空间获取资源流
        /// </summary>
        /// <param name="fullKey">命名空间</param>
        /// <returns>资源流</returns>
        private static Stream GetResourceStreamByFull(string fullKey)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string comparer = string.Empty;
            Array.ForEach(asm.GetManifestResourceNames(), (o) => { comparer += o; });
           // MsgBoxHelp.ShowInfo(comparer);
            if (comparer.Contains(fullKey))
                return asm.GetManifestResourceStream(fullKey);
            return null;
        }

        /// <summary>
        /// 根据文件名称和目录获取资源流
        /// </summary>
        /// <param name="sourceKey"></param>
        /// <returns></returns>
        private static Stream GetResourceStream(string sourceKey)
        {
            string fullKey = "Infoearth.Entity2CodeTool." + sourceKey;
            return GetResourceStreamByFull(fullKey);
        }

        /// <summary>
        /// 获取模型文件夹中的全目录
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <returns>文件在模型文件夹中的全路径</returns>
        private static string ToModelRoot(string fileName)
        {
            fileName = fileName.Substring(fileName.IndexOf('.') + 1);
            return Path.Combine(Infoearth.Entity2CodeTool.Converter.ModelPathConverter.RootPath, fileName);
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string ToDllName(string fileName)
        {
            fileName = fileName.Substring(fileName.IndexOf('.') + 1);
            fileName = fileName.Substring(fileName.IndexOf('.') + 1);
            fileName = fileName.Substring(fileName.IndexOf('.') + 1);
            return Path.Combine(Infoearth.Entity2CodeTool.Converter.ModelPathConverter.RootPath, fileName);
        }
    }
}

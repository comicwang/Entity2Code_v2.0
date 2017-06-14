using EnvDTE;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Utility.Common
{
    /// <summary>
    /// 用于模型关键字的替换的容器类
    /// </summary>
    public static class ModelContainer
    {
        private static Dictionary<string, string> _mContainer = new Dictionary<string, string>();

        public static void LoadContainer()
        {
            string iniPath = Path.Combine(CommonContainer.SolutionPath, Properties.Resource.ConfigName);

            Dictionary<string, string> models = xmlManager.Read(iniPath);

            foreach (var item in models)
            {
                RegistSource(item.Key, item.Value);
            }
        }


        public static void SetContainer(Dictionary<string, string> kv)
        {
            string iniPath = Path.Combine(CommonContainer.SolutionPath, Properties.Resource.ConfigName);

            foreach (var item in _mContainer)
            {
                xmlManager.Write(item.Key, item.Value, iniPath);
            }
        }

        /// <summary>
        /// 更替模型内容
        /// </summary>
        /// <param name="metaString"></param>
        public static string Replace(string metaString)
        {
            if (_mContainer != null)
            {
                foreach (var kv in _mContainer)
                {
                    if (kv.Value == null)
                        metaString.Replace(kv.Key, string.Empty);
                    else
                        metaString.Replace(kv.Key, kv.Value);
                }
            }
            return metaString;
        }


        /// <summary>
        /// 注册模型源
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        /// <param name="comment">描述</param>
        private static void RegistSource(string key, string value)
        {
            if (_mContainer.ContainsKey(key))
                _mContainer[key] = value;
            else
                _mContainer.Add(key, value);
        }     
    }
}

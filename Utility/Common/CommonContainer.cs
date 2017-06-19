using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Properties;

namespace Utility.Common
{
    /// <summary>
    /// 公用容器
    /// </summary>
    public static class CommonContainer
    {
        /// <summary>
        /// DTE类
        /// </summary>
        public static EnvDTE.DTE CommonServer { get; set; }

        /// <summary>
        /// 资源文件跟目录
        /// </summary>
        public static string RootPath
        {
            get { return Properties.Resource.RootPath; }
        }

        /// <summary>
        /// 解决方案目录
        /// </summary>

        public static string SolutionPath { get; set; }

        /// <summary>
        /// 存储xml文件路径
        /// </summary>

        public static string xmlName = Resource.ConfigName;

    }
}

using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Base
{
    /// <summary>
    /// 提供Nuget操作的扩展
    /// </summary>
    public static class NugetExtention
    {
        #region methods

        /// <summary>
        /// 安装Nuget程序包
        /// </summary>
        /// <param name="prj">项目COM</param>
        /// <param name="PackageName">安装包名称</param>
        public static void InstallNugetPackage(this Project prj, string PackageName)
        {
            DTE dte = prj.DTE;
            //设置为默认包源安装点
            dte.SetStartup(prj);
            //开始执行安装
            dte.ExecuteCommand("View.PackageManagerConsole", "Install-Package " + PackageName);
        }

        #endregion
    }
}

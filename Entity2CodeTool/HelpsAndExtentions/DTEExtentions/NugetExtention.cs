using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool.Helps
{
    static class NugetExtention
    {
        #region methods

        /// <summary>
        /// 安装Nuget程序包
        /// </summary>
        /// <param name="prj"></param>
        /// <param name="PackageName"></param>
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

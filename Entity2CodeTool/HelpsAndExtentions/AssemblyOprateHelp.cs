using EnvDTE;
using EnvDTE80;
using Infoearth.Entity2CodeTool.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool.Helps
{
    /// <summary>
    /// 提供对反射导致文件占用的操作处理解决方案
    /// </summary>
    public class AssemblyOprateHelp
    {
        #region methods

        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetVesion(string path)
        {
            FileVersionInfo myFileVersion = FileVersionInfo.GetVersionInfo(path);
            return myFileVersion.FileVersion;
            //AppDomain.CreateDomain
            //AppDomain ad = AppDomain.CreateDomain("Get DLL Vesion");
            //AppDomainNew adn = (AppDomainNew)ad.CreateInstanceFromAndUnwrap(Assembly.GetExecutingAssembly().CodeBase, "Infoearth.Entity2CodeTool.Helps.AppDomainNew");
            //string result = adn.Invoke(path);
            //AppDomain.Unload(ad);
            //adn = null;
            //return result;
        }

        #endregion
    }

    /// <summary>
    /// 定义一个能够穿透域的类
    /// </summary>
    public class AppDomainNew : MarshalByRefObject
    {
        /// <summary>
        /// 定义反射要执行的方法
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string Invoke(string path)
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.LoadFile(path);
            return asm.GetName().Version.ToString();
        }
    }
}

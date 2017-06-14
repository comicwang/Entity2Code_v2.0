using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool.Helps
{
    /// <summary>
    /// 执行ExcuteCommand的一些常用命令
    /// </summary>
    static class DTECommandExtention
    {
        /// <summary>
        /// 重新编译解决方案
        /// </summary>
        /// <param name="dte"></param>
        public static void RebuildSolution(this DTE dte)
        {
            dte.ExecuteCommand("Build.RebuildSolution");
        }
    }
}

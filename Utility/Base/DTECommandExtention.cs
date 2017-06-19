using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Base
{
    /// <summary>
    /// 执行ExcuteCommand的一些常用命令
    /// </summary>
    public static class DTECommandExtention
    {
        /// <summary>
        /// 提供DTE类执行命令
        /// </summary>
        /// <param name="dte">DTE宿主</param>
        public static void RebuildSolution(this DTE dte,string command)
        {
            dte.ExecuteCommand(command);
        }
    }
}

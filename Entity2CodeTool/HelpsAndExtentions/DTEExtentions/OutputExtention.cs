using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool.Helps
{
    /// <summary>
    /// 提供DTE输出的操作
    /// </summary>
    static class OutputExtention
    {
        #region fields and attrs

        /// <summary>
        /// 文字输出面板
        /// </summary>
        private static OutputWindowPane _outPanel;

        #endregion

        #region methods

        /// <summary>
        /// 输出文字
        /// </summary>
        /// <param name="dte"></param>
        /// <param name="strs">输出字符串</param>
        /// <param name="Inlog">是否加入输出日志</param>
        public static void OutString(this DTE dte, string strs, bool Inlog = false)
        {
            dte.StatusBar.Clear();
            dte.StatusBar.Text = strs;
            if (Inlog)
                dte.OutWindow(strs);
        }

        /// <summary>
        /// 输出进度条
        /// </summary>
        /// <param name="dte"></param>
        /// <param name="prcent">进度值</param>
        /// <param name="strs">输出字符串</param>
        /// <param name="Inlog">是否加入输出日志</param>
        public static void OutProcessBar(this DTE dte, int prcent, string strs = "", bool Inlog = false)
        {
            if (prcent > 0 && prcent < 100)
                dte.StatusBar.Progress(true, strs, prcent, 100);
            else
                dte.StatusBar.Progress(false);
            if (Inlog)
                dte.OutWindow(strs);
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="dte"></param>
        /// <param name="strs">输出字符串</param>
        public static void OutWindow(this DTE dte, string strs)
        {
            if (null == _outPanel)
            {
                DTE2 dte2 = (DTE2)dte;
                _outPanel = dte2.ToolWindows.OutputWindow.OutputWindowPanes.Add("Entity2CodeTool Log");
                _outPanel.Activate();
            }
            _outPanel.OutputString(strs + "\r\n");
        }

        #endregion
    }
}

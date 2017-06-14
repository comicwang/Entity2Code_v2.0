using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Infoearth.Entity2CodeTool.Helps
{
    /// <summary>
    /// 提供消息显示的静态方法
    /// </summary>
    public class MsgBoxHelp
    {     
        #region methods

        /// <summary>
        /// 显示错误消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="ex"></param>
        public static void ShowError(string msg, Exception ex)
        {
            MessageBox.Show(string.Format("来自Entity2Code异常：{0},异常消息：{1}", msg, ex), "Entity2Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 显示信息消息
        /// </summary>
        /// <param name="msg"></param>
        public static void ShowInfo(string msg)
        {
            MessageBox.Show(string.Format("来自Entity2Code消息：{0}", msg), "Entity2Code", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 显示提醒信息
        /// </summary>
        /// <param name="msg"></param>
        public static void ShowWorning(string msg)
        {
            MessageBox.Show(string.Format("来自Entity2Code提醒：{0}", msg), "Entity2Code", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        #endregion
    }
}

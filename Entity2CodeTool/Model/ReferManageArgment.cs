using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool.Model
{
    /// <summary>
    /// 程序集引用参数
    /// </summary>
    public class ReferManageArgment
    {
        #region attrs and fields

        /// <summary>
        /// 获取或者设置名称
        /// </summary>
        public string ReferName { get; set; }

        /// <summary>
        /// 获取或者设置路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 获取或者设置版本
        /// </summary>
        public string currentVesion { get; set; }

        /// <summary>
        /// 获取或者设置修改时间
        /// </summary>
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 获取或者设置描述
        /// </summary>
        public string Discription { get; set; }

        #endregion
    }
}

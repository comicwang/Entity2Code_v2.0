using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool
{
    /// <summary>
    /// 容器关键字实体
    /// </summary>
    public class ContainerModel
    {
        #region attrs and fields

        /// <summary>
        /// 关键字
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 关键值
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 关键字类别（0-项目用的固定关键字,不允许修改；1-用户新增的关键字）
        /// </summary>
        public int ModelType { get; set; }

        /// <summary>
        /// 最近修改时间
        /// </summary>
        public DateTime LastModifyTime { get; set; }

        #endregion
    }
}

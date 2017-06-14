using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool.Model
{
    /// <summary>
    /// 用于管理模型的实体
    /// </summary>
    public class ModelManageArgment
    {
        /// <summary>
        /// 模型ID（用于鉴别应用类别）
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 模型名称
        /// </summary>
        public string Text{get;set;}

        /// <summary>
        /// 模型全路径
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 当前模型是否被选中
        /// </summary>
        public bool isSelected { get; set; }
    }
}

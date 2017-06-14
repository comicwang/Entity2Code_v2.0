using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool
{
    /// <summary>
    /// 表示一个张表的实体信息
    /// </summary>
    public class TemplateEntity
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public string Entity { get; set; }

        /// <summary>
        /// 对应的DTO名称
        /// </summary>
        public string Data2Obj { get; set; }
    }
}

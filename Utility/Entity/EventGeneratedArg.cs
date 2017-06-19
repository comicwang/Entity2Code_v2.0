using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Entity
{
    /// <summary>
    /// 上下文生成代码的参数
    /// </summary>
    public class EventGeneratedArg:EventArgs
    {
        /// <summary>
        /// 项目项ID
        /// </summary>
        public string GeneratedId { get; internal set; }

        /// <summary>
        /// 上下文中剩余的个数
        /// </summary>
        public int CurrentCount { get; internal set; }

        /// <summary>
        /// 当前生成的实体名称
        /// </summary>
        public string CurrentEntity { get; internal set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id">项目项ID</param>
        /// <param name="count">上下文中剩余的个数</param>
        /// <param name="entity">当前生成的实体名称</param>
        public EventGeneratedArg(string id, int count, string entity)
        {
            GeneratedId = id;
            CurrentCount = count;
            CurrentEntity = entity;
        }
    }
}

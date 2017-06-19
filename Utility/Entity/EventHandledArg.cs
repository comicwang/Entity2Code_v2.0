using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Entity
{
    /// <summary>
    /// 用于传递上下文引用类库的实体
    /// </summary>
    public class EventHandledArg : EventArgs
    {
        /// <summary>
        /// 处理的项目ID
        /// </summary>
        public string HandledId { get; set; }

        /// <summary>
        /// 上下文中剩余的个数
        /// </summary>
        public int CurrentCount { get; set; }

        /// <summary>
        /// 用于传递上下文引用类库的实体构造函数
        /// </summary>
        /// <param name="id">处理的项目ID</param>
        /// <param name="currentCount">上下文中剩余的个数</param>
        public EventHandledArg(string id,int currentCount)
        {
            HandledId = id;
            CurrentCount = currentCount;
        }
    }
}

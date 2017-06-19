using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Entity
{
    /// <summary>
    /// 引用上下文用到的实体
    /// </summary>
    public class ReferenceEntity
    {
        /// <summary>
        /// 项目ID
        /// </summary>
        public string ProjectId { get;private set; }

        /// <summary>
        /// 引用参数
        /// </summary>
        public List<string> ReferenceCollection { get;private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="references"></param>
        public ReferenceEntity(string projectId, params string[] references)
        {
            ProjectId = projectId;
            ReferenceCollection = new List<string>();
            ReferenceCollection.AddRange(references);
        }
    }
}

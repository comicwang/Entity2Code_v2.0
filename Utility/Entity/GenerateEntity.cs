using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Core;

namespace Utility.Entity
{
    /// <summary>
    /// 生成代码所需参数实体
    /// </summary>
    public class GenerateEntity
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="engin">代码引擎类CodeGenerateBase</param>
        /// <param name="id">项目项ID</param>
        /// <param name="arg">参数</param>
        /// <param name="container">关键字容器</param>
        public GenerateEntity(CodeGenerateBase engin, string id, bool arg, Dictionary<string, string> container)
        {
            GenerateId = id;
            GenerateEngin = engin;
            GenerateArgment = arg;
            GenerateContainer = container;
        }
        
        /// <summary>
        /// 项目项ID
        /// </summary>
        public string GenerateId { get; set; }

        /// <summary>
        /// 代码引擎类
        /// </summary>
        public CodeGenerateBase GenerateEngin { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public bool GenerateArgment { get; set; }

        /// <summary>
        /// 关键字容器
        /// </summary>
        public Dictionary<string, string> GenerateContainer { get; set; }
    }
}

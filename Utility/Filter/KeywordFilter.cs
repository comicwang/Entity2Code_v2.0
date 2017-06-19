using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Filter
{
    /// <summary>
    /// 关键字容器不需要存储的过滤器
    /// </summary>
    public class KeywordFilter
    {
        /// <summary>
        /// 过滤器字符串
        /// </summary>
        public const string FilterCollection = "$MapProperty$,$MapPrimaryKey$，$Entity$，$Data2Obj$，$Property$，$Data2ObjContent$,$MapForeignKey$,$ForeignKey$，$Navigation$，$Constructor$，$ProfileContent$，$DBContextContent$，$ServiceContent$，$IServiceContent$，$ContainerContent$";
    }
}

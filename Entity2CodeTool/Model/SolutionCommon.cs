using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool.Model
{
    /// <summary>
    /// 解决方案的公共静态类
    /// </summary>
    static class SolutionCommon
    {

        /// <summary>
        /// 获取或者设置基础架构层名称
        /// </summary>
        public static string Infrastructure { get; set; }

        /// <summary>
        /// 获取或者设置领域实体层名称
        /// </summary>
        public static string DomainEntity { get; set; }

        /// <summary>
        /// 获取或者设置领域驱动层名称
        /// </summary>
        public static string DomainContext { get;  set; }

        /// <summary>
        /// 获取或者设置应用层名称
        /// </summary>
        public static string Application { get;  set; }

        /// <summary>
        /// 获取或者设置应用层接口名称
        /// </summary>
        public static string IApplication { get;  set; }

        /// <summary>
        /// 获取或者设置应用实体层名称
        /// </summary>
        public static string Data2Object { get;  set; }

        /// <summary>
        /// 获取或者设置服务层名称
        /// </summary>
        public static string Service { get;  set; }

        /// <summary>
        /// 获取或者设置项目名称
        /// </summary>
        public static string ProjectName { get;  set; }

        /// <summary>
        /// 获取或者设置是否添加服务层
        /// </summary>
        public static bool IsAddService { get;  set; }

        /// <summary>
        /// 获取或者设置工具生成架构类型
        /// </summary>
        public static InfrastructType infrastryctType { get;  set; }

        /// <summary>
        /// 获取或者设置DTE类
        /// </summary>
        public static DTE Dte { get;  set; }

    }
}

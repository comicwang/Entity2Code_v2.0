using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infoearth.Entity2CodeTool.Helps;

namespace Infoearth.Entity2CodeTool
{
    /// <summary>
    /// 建设类型(在Code创建时，用于区分的枚举)
    /// </summary>
    public enum ConstructType
    {
        #region 固定类别

        /// <summary>
        /// 用于创建实体转换
        /// </summary>
        Profile,

        /// <summary>
        /// 用于创建业务对象类
        /// </summary>
        Data2Obj,

        /// <summary>
        /// 用于创建上下文的扩展
        /// </summary>
        DbContextExtensions,

        /// <summary>
        /// 用于创建上下文的Dbset封装（这里是上下文的party类）
        /// </summary>
        ContextUnit,

        /// <summary>
        /// 用于创建实体的TT模板
        /// </summary>
        Entities,

        /// <summary>
        /// 用于创建服务的web.config
        /// </summary>
        WebConfig,

        /// <summary>
        /// 用于注册UntityContainer
        /// </summary>
        Container,

        /// <summary>
        /// 应用Container的类
        /// </summary>
        AttachDataSignBehavior,

        /// <summary>
        /// 应用Container的类
        /// </summary>
        UnityInstanceProvider,

        /// <summary>
        /// 应用Container的类
        /// </summary>
        UnityInstanceProviderServiceBehavior,

        #endregion

        #region 活动类别
        
        /// <summary>
        /// 用于创建应用层
        /// </summary>
        Application,

        /// <summary>
        /// 用于创建应用层接口
        /// </summary>
        IApplication,

        /// <summary>
        /// 用于创建服务
        /// </summary>
        Service,

        /// <summary>
        /// 用于创建服务接口
        /// </summary>
        IService,

        /// <summary>
        /// 用于创建仓储
        /// </summary>
        Repository,

        /// <summary>
        /// 用于创建仓储的接口
        /// </summary>
        IRepository,

        /// <summary>
        /// 代码的配置(用于代码优先)
        /// </summary>
        Map,

        /// <summary>
        /// 实体的创建（用于代码优先)
        /// </summary>
        Entity,

        /// <summary>
        /// 上下文（用于代码优先）
        /// </summary>
        DBContext,

        MethodApp,

        MethodIApp,

        MethodIServer,

        MethodServer

        #endregion

    }
}

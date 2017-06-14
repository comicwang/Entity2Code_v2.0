using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infoearth.Entity2CodeTool.Helps;
using Infoearth.Entity2CodeTool.Model;

namespace Infoearth.Entity2CodeTool
{
    /// <summary>
    /// 用于存储架构所有项目Project的静态类
    /// </summary>
    static class ProjectContainer
    {
        /// <summary>
        /// 基础结构层
        /// </summary>
        public static Project Infrastructure { get; set; }

        /// <summary>
        /// 领域实体层
        /// </summary>
        public static Project DomainEntity { get; set; }

        /// <summary>
        /// 领域逻辑层
        /// </summary>
        public static Project DomainContext { get; set; }

        /// <summary>
        /// 应用层
        /// </summary>
        public static Project Application { get; set; }

        /// <summary>
        /// 应用层接口
        /// </summary>
        public static Project IApplication { get; set; }

        /// <summary>
        /// 应用实体层
        /// </summary>
        public static Project Data2Object { get; set; }

        /// <summary>
        /// 服务层
        /// </summary>
        public static Project Service { get; set; }

        /// <summary>
        /// 根据ID获取项目
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <returns></returns>
        public static Project GetProject(int id)
        {
            switch (id)
            {
                case 0 :
                    return Infrastructure;
                case 1:
                    return DomainEntity;
                case 2:
                    return DomainContext;
                case 3:
                    return Application;
                case 4:
                    return IApplication;
                case 5:
                    return Data2Object;
                case 6:
                    return Service;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 保存所有项目
        /// </summary>
        public static void Save()
        {
            if (Infrastructure != null && Infrastructure.Saved == false)
                Infrastructure.Save();
            if (DomainEntity != null && DomainEntity.Saved == false)
                DomainEntity.Save();
            if (DomainContext != null && DomainContext.Saved == false)
                DomainContext.Save();
            if (Application != null && Application.Saved == false)
                Application.Save();
            if (IApplication != null && IApplication.Saved == false)
                IApplication.Save();
            if (Data2Object != null && Data2Object.Saved == false)
                Data2Object.Save();
            if (Service != null && Service.Saved == false)
                Service.Save();
        }

        /// <summary>
        /// 更新项目引用
        /// </summary>
        public static void UpdateReference(List<ReferManageArgment> models)
        {
            if (Infrastructure != null)
                Infrastructure.UpdateReference(models);
            if (DomainEntity != null)
                DomainEntity.UpdateReference(models);
            if (DomainContext != null)
                DomainContext.UpdateReference(models);
            if (Application != null)
                Application.UpdateReference(models);
            if (IApplication != null)
                IApplication.UpdateReference(models);
            if (Data2Object != null)
                Data2Object.UpdateReference(models);
            if (Service != null)
                Service.UpdateReference(models);
        }

    }
}

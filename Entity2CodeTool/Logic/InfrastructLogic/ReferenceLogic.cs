using Infoearth.Entity2CodeTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infoearth.Entity2CodeTool.Helps;

namespace Infoearth.Entity2CodeTool.Logic
{
    public class ReferenceLogic
    {
        #region methods

        /// <summary>
        /// 添加项目之间的引用
        /// </summary>
        public static void Add()
        {
            SolutionCommon.Dte.OutString("开始添加构架程序集引用.", true);

            SolutionCommon.Dte.OutString("添加基础结构层程序集引用.", true);
            //Infrastructure
            ProjectContainer.Infrastructure.AddReferenceFromProject(ProjectContainer.DomainContext);
            ProjectContainer.Infrastructure.AddReferenceFromProject(ProjectContainer.DomainEntity);
            ProjectContainer.Infrastructure.AddReference("iTelluro.Explorer.Domain.CodeFirst.Seedwork.dll".GetFileResource("Dll"));
            ProjectContainer.Infrastructure.AddReference("iTelluro.Explorer.Infrastruct.CodeFirst.Seedwork.dll".GetFileResource("Dll"));
            if (SolutionCommon.infrastryctType == InfrastructType.CodeFirst)
            {
                ProjectContainer.Infrastructure.AddReference("EntityFramework.dll".GetFileResource("Dll"));
                ProjectContainer.Infrastructure.AddReference("System.Data.Entity");
                ProjectContainer.Infrastructure.AddReference("System.ComponentModel.DataAnnotations");
            }

            SolutionCommon.Dte.OutString("添加领域层程序集引用.", true);
            //DomainContext
            ProjectContainer.DomainContext.AddReferenceFromProject(ProjectContainer.DomainEntity);
            ProjectContainer.DomainContext.AddReference("iTelluro.Explorer.Domain.CodeFirst.Seedwork.dll".GetFileResource("Dll"));
            ProjectContainer.DomainEntity.AddReference("System.Data.Entity");

            SolutionCommon.Dte.OutString("添加领域实体层程序集引用.", true);
            //DomainEntity
            ProjectContainer.DomainEntity.AddReference("iTelluro.Explorer.Domain.CodeFirst.Seedwork.dll".GetFileResource("Dll"));

            SolutionCommon.Dte.OutString("添加应用层程序集引用.", true);
            //Application
            ProjectContainer.Application.AddReferenceFromProject(ProjectContainer.Data2Object);
            ProjectContainer.Application.AddReferenceFromProject(ProjectContainer.DomainContext);
            ProjectContainer.Application.AddReferenceFromProject(ProjectContainer.DomainEntity);
            ProjectContainer.Application.AddReferenceFromProject(ProjectContainer.IApplication);
            ProjectContainer.Application.AddReference("iTelluro.Explorer.Domain.CodeFirst.Seedwork.dll".GetFileResource("Dll"));
            ProjectContainer.Application.AddReference("iTelluro.Explorer.Infrastruct.CodeFirst.Seedwork.dll".GetFileResource("Dll"));
            ProjectContainer.Application.AddReference("iTelluro.Explorer.Application.CodeFirst.Seedwork.dll".GetFileResource("Dll"));
            ProjectContainer.Application.AddReference("iTelluro.Explorer.Infrastructure.CrossCutting.dll".GetFileResource("Dll"));

            SolutionCommon.Dte.OutString("添加应用接口层程序集引用.", true);
            //IApplication
            ProjectContainer.IApplication.AddReferenceFromProject(ProjectContainer.DomainEntity);
            ProjectContainer.IApplication.AddReferenceFromProject(ProjectContainer.Data2Object);

            SolutionCommon.Dte.OutString("添加应用实体层程序集引用.", true);
            //Data2Object
            ProjectContainer.Data2Object.AddReferenceFromProject(ProjectContainer.DomainEntity);
            ProjectContainer.Data2Object.AddReference("System.Runtime.Serialization");
            ProjectContainer.Data2Object.AddReference("iTelluro.Explorer.Domain.CodeFirst.Seedwork.dll".GetFileResource("Dll"));
            ProjectContainer.Data2Object.AddReference("AutoMapper.dll".GetFileResource("Dll"));
            ProjectContainer.Data2Object.AddReference("AutoMapper.Net4.dll".GetFileResource("Dll"));

            //Service
            if (null != ProjectContainer.Service)
            {
                SolutionCommon.Dte.OutString("添加服务层程序集引用.", true);

                ProjectContainer.Service.AddReferenceFromProject(ProjectContainer.Infrastructure);
                ProjectContainer.Service.AddReferenceFromProject(ProjectContainer.Application);
                ProjectContainer.Service.AddReferenceFromProject(ProjectContainer.IApplication);
                ProjectContainer.Service.AddReferenceFromProject(ProjectContainer.DomainContext);
                ProjectContainer.Service.AddReferenceFromProject(ProjectContainer.Data2Object);
                ProjectContainer.Service.AddReference("System.Runtime.Serialization");
                ProjectContainer.Service.AddReference("System.ServiceModel");
                if (SolutionCommon.infrastryctType == InfrastructType.CodeFirst)
                    ProjectContainer.Service.AddReference("EntityFramework.dll".GetFileResource("Dll"));
                ProjectContainer.Service.AddReference("iTelluro.Explorer.Domain.CodeFirst.Seedwork.dll".GetFileResource("Dll"));
                ProjectContainer.Service.AddReference("iTelluro.Explorer.Infrastruct.CodeFirst.Seedwork.dll".GetFileResource("Dll"));
                ProjectContainer.Service.AddReference("iTelluro.Explorer.Application.CodeFirst.Seedwork.dll".GetFileResource("Dll"));
                ProjectContainer.Service.AddReference("iTelluro.Explorer.Infrastructure.CrossCutting.dll".GetFileResource("Dll"));
                ProjectContainer.Service.AddReference("AutoMapper.dll".GetFileResource("Dll"));
                ProjectContainer.Service.AddReference("AutoMapper.Net4.dll".GetFileResource("Dll"));
                ProjectContainer.Service.AddReference("iTelluro.Explorer.Infrastructure.CrossCutting.NetFramework.dll".GetFileResource("Dll"));
                ProjectContainer.Service.AddReference("iTelluro.SSO.dll".GetFileResource("Dll"));
                ProjectContainer.Service.AddReference("iTelluro.SSO.Common.dll".GetFileResource("Dll"));
                ProjectContainer.Service.AddReference("iTelluro.SSO.WebServices.dll".GetFileResource("Dll"));
                ProjectContainer.Service.AddReference("iTelluro.SYS.Entity.dll".GetFileResource("Dll"));
                ProjectContainer.Service.AddReference("iTelluro.Utility.dll".GetFileResource("Dll"));
                ProjectContainer.Service.AddReference("log4net.dll".GetFileResource("Dll"));
                ProjectContainer.Service.AddReference("Microsoft.Practices.Unity.dll".GetFileResource("Dll"));
            }
        }

        #endregion
    }
}

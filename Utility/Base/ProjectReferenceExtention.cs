using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;
using Utility.Core;
using Utility.Help;
using VSLangProj;

namespace Utility.Base
{
    /// <summary>
    /// 提供项目引用扩展
    /// </summary>
    public static class ProjectReferenceExtention
    {
        #region methods

        /// <summary>
        /// 添加项目引用
        /// </summary>
        /// <param name="project"></param>
        /// <param name="addProject">要引用的项目</param>
        /// <returns>添加的引用信息</returns>
        public static Reference AddReferenceFromProject(this Project project, Project addProject)
        {
            try
            {
                if (null == project || null == addProject) return null;
                VSProject vsProject = project.Object as VSProject;
                Reference refer = vsProject.References.AddProject(addProject);
                return refer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 添加新的引用
        /// </summary>
        /// <param name="project">项目宿体</param>
        /// <param name="bstrPath">引用的路径（三种形式：1. 简单的.net framework对象名称;2. .net framework文件名;3. COM库文件,这里是COM的完整路径和文件名）</param>
        /// <returns>Reference对象</returns>
        public static Reference AddReference(this Project project, string bstrPath)
        {
            try
            {
                if (null == project || string.IsNullOrEmpty(bstrPath)) return null;
                VSProject vsProject = project.Object as VSProject;
                Reference refer = vsProject.References.Add(bstrPath);
                return refer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 通过关键字来添加项目引用
        /// </summary>
        /// <param name="project"></param>
        /// <param name="referId">项目ID,系统类库或者资源类库</param>
        public static void AddReferByKey(this Project project, string referId)
        {
            try
            {
                if (referId.Count() == 36)
                {
                    Project referProject = TemplateContainer.Resove<Project>(referId);
                    if (referProject != null)
                    {
                        project.AddReferenceFromProject(referProject);
                        return;
                    }
                }
                string[] files = Directory.GetFiles(CommonContainer.RootPath, "*.dll");
                if (files != null)
                    Array.ForEach(files, t =>
                    {
                        if (t.Contains(referId))
                        {
                            project.AddReference(t);
                            return;
                        }
                    });
                project.AddReference(referId);
            }
            catch (Exception ex)
            {
                MsgBoxHelp.ShowError(string.Format(Properties.Resource.ReferError, referId), ex);
            }
        }


        /// <summary>
        /// 更新项目引用
        /// </summary>
        /// <param name="project">项目宿体</param>
    //    public static void UpdateReference(this Project project, List<ReferManageArgment> assemblyInfo)
    //    {
    //        try
    //        {
    //            if (null == project) return;
    //            VSProject vsProject = project.Object as VSProject;
    //            References refers = vsProject.References;
    //            List<ReferMini> referMinis = new List<ReferMini>();
    //            foreach (Reference refer in refers)
    //            {
    //                foreach (ReferManageArgment item in assemblyInfo)
    //                {
    //                    if (refer.Name == item.ReferName)
    //                    {
    //                        if (refer.Version == item.currentVesion)
    //                        {
    //                            ReferMini temp = new ReferMini();
    //                            temp.Id = refer.Identity;
    //                            temp.Path = item.Path;
    //                        }
    //                        break;
    //                    }
    //                }
    //            }

    //            foreach (ReferMini item in referMinis)
    //            {
    //                //删除引用
    //                refers.Find(item.Id).Remove();
    //                //添加新引用
    //                project.AddReference(item.Path);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            throw ex;
    //        }
    //    }

        #endregion
    }
}

using EnvDTE;
using Infoearth.Entity2CodeTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLangProj;

namespace Infoearth.Entity2CodeTool.Helps
{
    static class ProjectReferenceExtention
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
        /// 更新项目引用
        /// </summary>
        /// <param name="project">项目宿体</param>
        public static void UpdateReference(this Project project, List<ReferManageArgment> assemblyInfo)
        {
            try
            {
                if (null == project) return;
                VSProject vsProject = project.Object as VSProject;
                References refers = vsProject.References;
                List<ReferMini> referMinis = new List<ReferMini>();
                foreach (Reference refer in refers)
                {
                    foreach (ReferManageArgment item in assemblyInfo)
                    {
                        if (refer.Name == item.ReferName)
                        {
                            if (refer.Version == item.currentVesion)
                            {
                                ReferMini temp = new ReferMini();
                                temp.Id = refer.Identity;
                                temp.Path = item.Path;
                            }
                            break;
                        }
                    }
                }

                foreach (ReferMini item in referMinis)
                {
                    //删除引用
                    refers.Find(item.Id).Remove();
                    //添加新引用
                    project.AddReference(item.Path);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }

    public class ReferMini
    {
        public string Id;

        public string Path;
    }
}

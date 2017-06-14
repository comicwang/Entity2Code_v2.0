using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool.Helps
{
    /// <summary>
    /// 提供项目操作的扩展
    /// </summary>
    static class ProjectExtention
    {
        #region methods

        /// <summary>
        /// 添加类库项目
        /// </summary>
        /// <param name="dte">DTE宿体</param>
        /// <param name="projectName">项目名称</param>
        /// <returns>创建的项目类</returns>
        public static Project AddClassLibrary(this DTE dte, string projectName, bool overWrite = false)
        {
            try
            {
                Solution2 sln = dte.Solution as Solution2;
                Project prj = sln.Projects.Find(projectName);
                if (null != prj)
                {
                    if (overWrite == false)
                        return prj;
                    else
                        prj.Delete();
                }
                string projectTemplate = sln.GetProjectTemplate("ClassLibrary.zip", "CSharp");
                string path = Path.Combine(Path.GetDirectoryName(sln.FullName), projectName);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                prj = sln.AddFromTemplate(projectTemplate, path, projectName);
                if (null == prj)
                    prj = sln.Projects.Find(projectName);

                //移除Class1
                ProjectItem prjItem = prj.ProjectItems.Find("Class1.cs");
                if (null != prjItem)
                    prjItem.Delete();

                //更改Project目标平台为X86
                prj.SetProjectConfig("PlatformTarget", "X86");
                return prj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 添加服务项目
        /// </summary>
        /// <param name="dte">DTE宿体</param>
        /// <param name="projectName">项目名称</param>
        /// <returns>创建的项目类</returns>
        public static Project AddWebService(this DTE dte, string projectName, bool overWrite = false)
        {
            try
            {
                Solution2 sln = dte.Solution as Solution2;
                Project prj = sln.Projects.Find(projectName);
                if (null != prj)
                {
                    if (overWrite == false)
                        return prj;
                    else
                        prj.Delete();
                }
                string projectTemplate = sln.GetProjectTemplate("EmptyWebApplicationProject40.zip", "CSharp");
                string path = Path.Combine(Path.GetDirectoryName(sln.FullName), projectName);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                prj = sln.AddFromTemplate(projectTemplate, path, projectName);
                if (null == prj)
                    prj = sln.Projects.Find(projectName);
                prj.SetProjectConfig("PlatformTarget", "X86");
                return prj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 新增项目文件夹
        /// </summary>
        /// <param name="dte"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static Project AddSolutionFolder(this DTE dte, string folderName)
        {
            Project project = null;
            try
            {
                Solution2 sln = dte.Solution as Solution2;
                project = dte.Solution.Projects.Find(folderName);
                if (project == null)
                    project = sln.AddSolutionFolder(folderName);
              
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return project;
        }

        /// <summary>
        /// 获取项目的目录
        /// </summary>
        /// <param name="project">项目宿体</param>
        /// <returns></returns>
        public static string ToDirectory(this Project project)
        {
            if (null == project) return string.Empty;
            return Path.GetDirectoryName(project.FullName);
        }

        /// <summary>
        /// 根据项目名称获取项目类
        /// </summary>
        /// <param name="projects">项目集合宿体</param>
        /// <param name="projectName">项目名称</param>
        /// <returns></returns>
        public static Project Find(this Projects projects, string projectName)
        {
            try
            {
                if (null == projects || 0 == projects.Count)
                    return null;
                foreach (Project prj in projects)
                {
                    if (prj.Name == projectName)
                        return prj;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}

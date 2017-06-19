using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;
using Utility.Core;
using Utility.Help;

namespace Utility.Base
{
    /// <summary>
    /// 提供项目操作的扩展
    /// </summary>
    public static class ProjectExtention
    {
        #region methods

        /// <summary>
        /// 添加类库项目
        /// </summary>
        /// <param name="dte">DTE宿体</param>
        /// <param name="projectName">项目名称</param>
        /// <param name="overWrite">是否覆盖现有项目</param>
        /// <returns>创建的项目类</returns>
        public static Project AddClassLibrary(this DTE dte, string projectName,bool overWrite=false)
        {
            try
            {
                Solution2 sln = dte.Solution as Solution2;
                Project prj = (sln as Solution).FindProject(projectName);
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
                    prj = (sln as Solution).FindProject(projectName);

                //移除Class1
                ProjectItem prjItem = prj.ProjectItems.FindItem("Class1.cs");
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
        /// <param name="overWrite">是否覆盖现有项目</param>
        /// <returns>创建的项目类</returns>
        public static Project AddWebService(this DTE dte, string projectName, bool overWrite = false)
        {
            try
            {
                Solution2 sln = dte.Solution as Solution2;
                Project prj = (sln as Solution).FindProject(projectName);
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
                    prj = (sln as Solution).FindProject(projectName);
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
        /// <param name="folderName">文件夹名称</param>
        /// <returns></returns>
        public static Project AddSolutionFolder(this DTE dte, string folderName)
        {
            Project project = null;
            try
            {
                Solution2 sln = dte.Solution as Solution2;
                project =(dte.Solution as Solution).FindProject(folderName);
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
        /// <returns>项目根目录</returns>
        public static string GetDirectory(this Project project)
        {
            if (null == project) return string.Empty;
            return Path.GetDirectoryName(project.FullName);
        }

        /// <summary>
        /// 设置Log4net日志管理工具开始工作
        /// </summary>
        /// <param name="project"></param>
        public static void SetLog4netWatch(this Project project)
        {
            string assemblyInfoPath = Path.Combine(project.GetDirectory(), "Properties", "AssemblyInfo.cs");
            StringBuilder build1 = FileOprateHelp.ReadTextFile(assemblyInfoPath);
            build1.AppendLine("[assembly: log4net.Config.XmlConfigurator(Watch = true)]");//日志监视
            FileOprateHelp.SaveTextFile(build1.ToString(), assemblyInfoPath);
        }

        #endregion
    }
}

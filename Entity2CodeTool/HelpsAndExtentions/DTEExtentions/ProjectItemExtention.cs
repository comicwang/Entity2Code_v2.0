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
    /// 提供项目项操作的扩展
    /// </summary>
    static class ProjectItemExtention
    {
        #region methods

        /// <summary>
        /// 创建ADO.NET实体数据模型
        /// </summary>
        /// <param name="project"></param>
        /// <param name="itemNameWithoutExtion">edmx的名称（不带后缀）</param>
        /// <returns>创建完成的项目项</returns>
        public static ProjectItem AddAdoNetEntityDataModel(this Project project, string itemNameWithoutExtion)
        {
            try
            {
                ProjectItem projectItem = project.ProjectItems.Find(itemNameWithoutExtion + ".edmx");
                //项存在删除
                if (null != projectItem)
                    projectItem.Delete();
                ProjectItem appItem = project.ProjectItems.Find("app.config");
                if (null != appItem)
                    appItem.Delete();
                Solution2 sln = project.DTE.Solution as Solution2;
                string templatePath = sln.GetProjectItemTemplate("AdoNetEntityDataModelCSharp.zip", "CSharp");
                projectItem = project.ProjectItems.AddFromTemplate(templatePath, itemNameWithoutExtion + ".edmx");
                if (null == projectItem)
                    projectItem = project.ProjectItems.Item(itemNameWithoutExtion + ".edmx");
                return projectItem;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 创建ADO.NET实体数据模型（有项目文件夹）
        /// </summary>
        /// <param name="project"></param>
        /// <param name="dirName">项目项文件夹名称</param>
        /// <param name="itemNameWithoutExtion">edmx的名称（不带后缀）</param>
        /// <returns>创建完成的项目项</returns>
        public static ProjectItem AddAdoNetEntityDataModel(this Project project, string dirName, string itemNameWithoutExtion)
        {
            try
            {
                ProjectItem prjItem = null;
                string dirPath = Path.Combine(project.ToDirectory(), dirName);
                //文件夹不存在创建项目文件夹
                if (!Directory.Exists(dirPath))
                    prjItem = project.ProjectItems.AddFolder(dirName);
                //文件夹存在
                else
                {
                    //项目文件夹存在
                    prjItem = project.ProjectItems.Find(dirName);
                    //项目文件夹不存在创建
                    if (null == prjItem)
                        prjItem = project.ProjectItems.AddFromDirectory(dirPath);
                }
                if (null == prjItem)
                    throw new ArgumentException(dirName);

                ProjectItem projectItem = prjItem.ProjectItems.Find(itemNameWithoutExtion + ".edmx");
                //项存在删除
                if (null != projectItem)
                    projectItem.Delete();

                Solution2 sln = project.DTE.Solution as Solution2;
                string templatePath = sln.GetProjectItemTemplate("AdoNetEntityDataModelCSharp.zip", "CSharp");
                projectItem = prjItem.ProjectItems.AddFromTemplate(templatePath, itemNameWithoutExtion + ".edmx");
                if (null == projectItem)
                    projectItem = prjItem.ProjectItems.Find(itemNameWithoutExtion + ".edmx");
                return projectItem;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据文件和项目Copy创建项（无项目项文件夹）
        /// </summary>
        /// <param name="project">项目</param>
        /// <param name="filePath">文件路径</param>
        /// <returns>创建完成的项目项</returns>
        public static ProjectItem AddFromFileCopy(this Project project, string filePath)
        {
            if (!File.Exists(filePath))
                return null;
            try
            {
                string itemName = Path.GetFileName(filePath);
                ProjectItem projectItem = project.ProjectItems.Find(itemName);
                //项存在删除
                if (null != projectItem)
                    projectItem.Delete();
                projectItem = project.ProjectItems.AddFromFileCopy(filePath);
                if (null == projectItem)
                    projectItem = project.ProjectItems.Find(itemName);
                return projectItem;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据文件和项目创建项（无项目项文件夹）
        /// </summary>
        /// <param name="project"></param>
        /// <param name="filePath">文件路径</param>
        /// <returns>创建完成的项目项</returns>
        public static ProjectItem AddFromFile(this Project project, string filePath)
        {
            if (!File.Exists(filePath))
                return null;
            try
            {
                string itemName = Path.GetFileName(filePath);
                ProjectItem projectItem = project.ProjectItems.AddFromFile(filePath);
                if (null == projectItem)
                    projectItem = project.ProjectItems.Find(itemName);
                return projectItem;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据文件字符串创建项目项
        /// </summary>
        /// <param name="project"></param>
        /// <param name="fileString">文件字符串内容</param>
        /// <param name="itemName">文件名称（包括后缀）</param>
        /// <returns>创建完成的项目项</returns>
        public static ProjectItem AddFromFileString(this Project project, string fileString, string itemName, Encoding encoding)
        {
            try
            {
                //创建项文件
                string filePath = Path.Combine(project.ToDirectory(), itemName);
                using (FileStream create = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    byte[] buffer = encoding.GetBytes(fileString);
                    create.Write(buffer, 0, buffer.Length);
                }
                //添加项文件到项目中
                ProjectItem projectItem = project.ProjectItems.Find(itemName);
                if (null != projectItem)
                    projectItem.Remove();
                projectItem = project.ProjectItems.AddFromFile(filePath);
                if (null == projectItem)
                    projectItem = project.ProjectItems.Find(itemName);
                return projectItem;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 通过字符串来添加项目项
        /// </summary>
        /// <param name="project">项目宿体</param>
        /// <param name="fileString">字符串</param>
        /// <param name="dirName">文件夹名称</param>
        /// <param name="itemName">文件名称</param>
        /// <returns></returns>
        public static ProjectItem AddFromFileString(this Project project, string fileString, string dirName, string itemName)
        {
            try
            {
                //创建项文件
                string dirPath = Path.Combine(project.ToDirectory(), dirName);
                //文件夹不存在创建项目文件夹
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
                string filePath = Path.Combine(dirPath, itemName);
                using (FileStream create = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    byte[] buffer = Encoding.Default.GetBytes(fileString);
                    create.Write(buffer, 0, buffer.Length);
                }
                //添加项文件到项目中
                ProjectItem projectItem = project.ProjectItems.Find(itemName);
                if (null != projectItem)
                    projectItem.Delete();
                projectItem = project.ProjectItems.AddFromFile(filePath);
                if (null == projectItem)
                    projectItem = project.ProjectItems.Find(itemName);
                return projectItem;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据文件后缀名，删除项目项
        /// </summary>
        /// <param name="project"></param>
        /// <param name="fileExtention">文件后缀名（例如.cs）</param>
        /// <param name="isAllSub">是否贯穿到所有项文件夹</param>
        public static void DeleteFromExtention(this Project project, string fileExtention, bool isAllSub = false)
        {
            try
            {
                if (null == project || 0 == project.ProjectItems.Count)
                    return;
                foreach (ProjectItem item in project.ProjectItems)
                {
                    DeleteFromExtention(item, fileExtention, isAllSub);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 通过文件后缀来删除项目项
        /// </summary>
        /// <param name="projectItem"></param>
        /// <param name="fileExtention"></param>
        /// <param name="isAllSub"></param>
        private static void DeleteFromExtention(ProjectItem projectItem, string fileExtention, bool isAllSub)
        {
            if (null == projectItem)
                return;
            if (!isAllSub)
            {
                if (Path.GetExtension(projectItem.Name) == fileExtention)
                    projectItem.Delete();
            }
            else
            {
                if (Path.GetExtension(projectItem.Name) == fileExtention)
                    projectItem.Delete();
                else
                {
                    if (projectItem.ProjectItems.Count > 0)
                        foreach (ProjectItem item in projectItem.ProjectItems)
                        {
                            DeleteFromExtention(item, fileExtention, true);
                        }
                }
            }
        }

        /// <summary>
        /// 根据项目项名称获取项目项类
        /// </summary>
        /// <param name="projects">项目项宿体</param>
        /// <param name="itemName">项目项名称</param>
        /// <param name="allSub">是否递归到所有子项目（默认为false）</param>
        /// <returns></returns>
        public static ProjectItem Find(this ProjectItems projects, string itemName, bool allSub = false)
        {
            try
            {
                if (null == projects || 0 == projects.Count)
                    return null;
                if (!allSub)
                {
                    foreach (ProjectItem item in projects)
                    {
                        if (item.Name == itemName)
                            return item;
                    }
                    return null;
                }
                else
                {
                    return Find(projects, itemName);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 用于递归查询项目名称的私有方法
        /// </summary>
        /// <param name="projects">项目项集合</param>
        /// <param name="itemName">项目项名称</param>
        /// <returns></returns>
        private static ProjectItem Find(ProjectItems projects, string itemName)
        {
            foreach (ProjectItem item in projects)
            {
                if (item.Name == itemName)
                    return item;
                if (item.ProjectItems != null && item.ProjectItems.Count > 0)
                {
                    ProjectItem prj = Find(item.ProjectItems, itemName);
                    if (prj != null)
                        return prj;
                }

            }
            return null;
        }

        /// <summary>
        /// 规范化项目.cs文件
        /// </summary>
        /// <param name="projectItem"></param>
        public static void FormatDocument(this ProjectItem projectItem)
        {
            if (projectItem.Kind == Constants.vsProjectItemKindPhysicalFile && projectItem.Name.EndsWith(".cs"))
            {
                Window window = projectItem.Open(Constants.vsViewKindCode);
                window.Activate();
                projectItem.DTE.ExecuteCommand("Edit.FormatDocument");
                window.Close(vsSaveChanges.vsSaveChangesYes);
            }
            else if (projectItem.ProjectItems != null && projectItem.ProjectItems.Count > 0)
            {
                foreach (ProjectItem item in projectItem.ProjectItems)
                {
                    item.FormatDocument();
                }
            }
        }

        #endregion
    }
}

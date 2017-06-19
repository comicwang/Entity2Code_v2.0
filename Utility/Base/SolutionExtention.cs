using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Base
{
    /// <summary>
    /// 用于扩展解决方案的方法
    /// </summary>
    public static class SolutionExtention
    {
        /// <summary>
        /// 创建一个解决方案
        /// </summary>
        /// <param name="sln">解决方案创建宿主</param>
        /// <param name="slnName">名称</param>
        /// <param name="dir">文件夹目录</param>
        public static void CreateSln(this Solution sln, string slnName, string dir)
        {
            sln.Create(slnName + ".sln", slnName);
            sln.SaveAs(Path.Combine(dir, slnName));
        }

        /// <summary>
        /// 重新生成解决方案
        /// </summary>
        /// <param name="sln"></param>
        public static void RebuildSln(this Solution sln)
        {
            sln.DTE.ExecuteCommand("Build.RebuildSolution");
        }

        /// <summary>
        /// 展开解决方案的所有项目
        /// </summary>
        /// <param name="sln">解决方案COM</param>
        public static void ExpandAllProject(this Solution sln)
        {
            string solutionName = sln.Properties.Item("Name").Value.ToString();
            UIHierarchyItems items = (sln.DTE as DTE2).ToolWindows.SolutionExplorer.GetItem(solutionName).UIHierarchyItems;
            foreach (UIHierarchyItem topItem in items)
            {
                topItem.UIHierarchyItems.Expanded = true;
            }
        }

        /// <summary>
        ///  获取所有项目信息
        /// </summary>
        /// <param name="sln">解决方案COM</param>
        /// <returns>所有的项目信息集合</returns>
        public static List<UIHierarchyItem> GetProjectNodes(this Solution sln)
        {
            string solutionName = sln.Properties.Item("Name").Value.ToString();
            UIHierarchyItems topLevlItems = (sln.DTE as DTE2).ToolWindows.SolutionExplorer.GetItem(solutionName).UIHierarchyItems;

            List<UIHierarchyItem> items = new List<UIHierarchyItem>();
            foreach (UIHierarchyItem topItem in topLevlItems)
            {
                if (IsProject(topItem))
                {
                    items.Add(topItem);
                }
                else if (IsSolutionFolder(topItem))
                {
                    GetProjectNodesInSolutionFolder(topItem, ref items);
                }
            }
            return items;
        }

        /// <summary>
        /// 获取解决方案文件夹下面所有的项目信息集合
        /// </summary>
        /// <param name="topLevelItem">解决方案文件夹</param>
        /// <param name="projNodes">返回的项目信息集合</param>
        public static void GetProjectNodesInSolutionFolder(UIHierarchyItem topLevelItem, ref List<UIHierarchyItem> projNodes)
        {
            if (IsSolutionFolder(topLevelItem))
            {
                foreach (UIHierarchyItem subItem in topLevelItem.UIHierarchyItems)
                {
                    if (IsProject(subItem))
                    {
                        projNodes.Add(subItem);
                    }
                    else
                    {
                        GetProjectNodesInSolutionFolder(subItem, ref projNodes);
                    }
                }
            }
        }

        /// <summary>
        /// 判断节点是否为解决方案目录
        /// </summary>
        /// <param name="item">项目信息COM</param>
        /// <returns>是否为解决方案目录</returns>
        public static bool IsSolutionFolder(UIHierarchyItem item)
        {
            bool isFolder = false;

            //解决方案目录为顶级
            Project proj = item.Object as Project;
            if (proj != null && proj.Kind == ProjectKinds.vsProjectKindSolutionFolder)
            {
                isFolder = true;
            }
            else if (proj == null)
            {
                //解决方案目录不是顶级目录
                ProjectItem projItem = item.Object as ProjectItem;
                if (projItem != null)
                {
                    proj = projItem.Object as Project;
                    if (proj != null && proj.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                    {
                        isFolder = true;
                    }
                }
            }
            return isFolder;
        }

        /// <summary>
        /// 判断结点是否为项目
        /// </summary>
        /// <param name="item">项目信息COM</param>
        /// <returns>是否为项目</returns>
        public static bool IsProject(UIHierarchyItem item)
        {
            bool isProject = false;

            //结点为顶级项目
            Project proj = item.Object as Project;
            if (proj != null && proj.Kind != ProjectKinds.vsProjectKindSolutionFolder)
            {
                isProject = true;
            }
            else if (proj == null)
            {
                //结点为非顶级项目，即项目父节点为解决方案目录
                ProjectItem proItem = item.Object as ProjectItem;
                if (proItem != null)
                {
                    Project subPro = proItem.Object as Project;
                    if (subPro != null && subPro.Kind != ProjectKinds.vsProjectKindSolutionFolder)
                    {
                        isProject = true;
                    }
                }
            }
            return isProject;
        }

        /// <summary>
        /// 折叠全部项目
        /// </summary>
        /// <param name="sln">解决方案COM</param>
        public static void CollapseAllProject(this Solution sln)
        {
            UIHierarchy rootNode = (sln.DTE as DTE2).ToolWindows.SolutionExplorer;

            List<UIHierarchyItem> itemNodes = GetProjectNodes(sln);
            if (itemNodes != null && itemNodes.Count > 0)
            {
                for (int i = 0; i < itemNodes.Count; i++)
                {
                    Project proj = itemNodes[i].Object as Project;
                    //项目为非顶级项目
                    if (proj == null)
                    {
                        ProjectItem projItem = itemNodes[i].Object as ProjectItem;
                        if (projItem != null && itemNodes[i].UIHierarchyItems.Expanded)
                        {

                            itemNodes[i].Select(vsUISelectionType.vsUISelectionTypeSelect);
                            rootNode.DoDefaultAction();
                        }
                    }
                    else
                    {
                        //项目为顶级项目
                        itemNodes[i].UIHierarchyItems.Expanded = false;
                    }

                }
            }
            //折叠顶级项目
            string solutionName = sln.Properties.Item("Name").Value.ToString();
            UIHierarchyItems items = rootNode.GetItem(solutionName).UIHierarchyItems;
            foreach (UIHierarchyItem topItem in items)
            {
                topItem.UIHierarchyItems.Expanded = false;
            }
        }

        /// <summary>
        /// 根据项目名称获取项目COM
        /// </summary>
        /// <param name="sln">解决方案COM</param>
        /// <param name="projectName">项目名称</param>
        /// <returns>项目COM</returns>
        public static Project FindProject(this Solution sln, string projectName)
        {
            try
            {
                if (null == sln || 0 == sln.Projects.Count)
                    return null;
                foreach (Project prj in sln.Projects)
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

    }
}

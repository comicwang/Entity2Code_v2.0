using EnvDTE;
using EnvDTE80;
using Infoearth.Entity2CodeTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSLangProj;

namespace Infoearth.Entity2CodeTool.Helps
{
    public class SolutionExplorHelp
    {
         private static DTE2 _dte = null;
        private static UIHierarchy _rootNode = null;

        private static void LoadAllProjectNodes()
        {
            string solutionName = _dte.Solution.Properties.Item("Name").Value.ToString();
            UIHierarchyItems items = _rootNode.GetItem(solutionName).UIHierarchyItems;
            foreach (UIHierarchyItem topItem in items)
            {
                topItem.UIHierarchyItems.Expanded = true;
            }
        }

        private static List<UIHierarchyItem> GetProjectNodes()
        {
            string solutionName = _dte.Solution.Properties.Item("Name").Value.ToString();
            UIHierarchyItems topLevlItems = _rootNode.GetItem(solutionName).UIHierarchyItems;
            
            List<UIHierarchyItem> items = new List<UIHierarchyItem>();
            foreach (UIHierarchyItem topItem in topLevlItems)
            {
                if (IsProject(topItem))
                {
                    items.Add(topItem);
                }
                else if (IsSolutionFolder(topItem))
                {
                    GetProjectNodesInSolutionFolder(topItem,ref items);
                }
            }
            return items;
        }

        private static void GetProjectNodesInSolutionFolder(UIHierarchyItem topLevelItem, ref List<UIHierarchyItem> projNodes)
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
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool IsSolutionFolder(UIHierarchyItem item)
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
                    if(proj!=null && proj.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                    {
                        isFolder=true;
                    }
                }
            }
            return isFolder;
        }

        /// <summary>
        /// 判断结点是否为项目
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool IsProject(UIHierarchyItem item)
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
        /// 折叠全部
        /// </summary>
        public static void CollapseAll()
        {
            _dte = SolutionCommon.Dte as DTE2;
            _rootNode = _dte.ToolWindows.SolutionExplorer;

            List<UIHierarchyItem> itemNodes = GetProjectNodes();
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
                            _rootNode.DoDefaultAction();
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
            string solutionName = _dte.Solution.Properties.Item("Name").Value.ToString();
            UIHierarchyItems items = _rootNode.GetItem(solutionName).UIHierarchyItems;
            foreach (UIHierarchyItem topItem in items)
            {
                topItem.UIHierarchyItems.Expanded = false;
            }
        }

        /// <summary>
        /// 展开全部
        /// </summary>
        public static void ExpandAll()
        {
            _dte = SolutionCommon.Dte as DTE2;
            _rootNode = _dte.ToolWindows.SolutionExplorer;
            //加载所有的项目
            LoadAllProjectNodes();

            List<UIHierarchyItem> itemNodes = GetProjectNodes();
            if (itemNodes != null && itemNodes.Count > 0)
            {
                for (int i = 0; i < itemNodes.Count; i++)
                {
                    Project proj = itemNodes[i].Object as Project;
                    //项目为非顶级项目
                    if (proj == null)
                    {
                        ProjectItem projItem = itemNodes[i].Object as ProjectItem;
                        if (projItem != null && itemNodes[i].UIHierarchyItems.Expanded == false)
                        {
                            itemNodes[i].Select(vsUISelectionType.vsUISelectionTypeSelect);
                            _rootNode.DoDefaultAction();
                        }
                    }
                    else
                    {
                        //项目为顶级项目
                        itemNodes[i].UIHierarchyItems.Expanded = true;
                    }
                }
            }
        }

        /// <summary>
        /// 通过读取SolutionExplorer的UIHierarchyItems也可以得到所有的Project
        /// </summary>
        /// <param name="app"></param>
        private static List<Project> GetAllProject()
        {
            List<Project> projs = new List<Project>();

            LoadAllProjectNodes();

            List<UIHierarchyItem> itemNodes = GetProjectNodes();
            if (itemNodes != null && itemNodes.Count > 0)
            {
                for (int i = 0; i < itemNodes.Count; i++)
                {
                    Project proj = itemNodes[i].Object as Project;
                    if (proj == null)
                    {
                        ProjectItem projItem = itemNodes[i].Object as ProjectItem;
                        if (projItem != null)
                        {
                            proj = projItem.Object as Project;
                        }
                        if (proj == null)
                        {
                            continue;
                        }
                    }
                    projs.Add(proj);
                }
            }
            CollapseAll();

            //剔除projs[i].Object不是VSProject的特殊项目
            for (int i = 0; i < projs.Count; i++)
            {
                VSProject vsProj = projs[i].Object as VSProject;
                if (vsProj == null)
                {
                    projs.RemoveAt(i);
                    i--;
                }
            }

            return projs;
        }
    }
}

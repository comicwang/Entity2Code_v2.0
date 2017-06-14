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
    static class ProjectSettingExtention
    {
        #region methods

        /// <summary>
        /// 设置解决方案起始项
        /// </summary>
        /// <param name="dte">DTE宿体</param>
        /// <param name="project">要设置为启动项目的项目类</param>
        public static void SetStartup(this DTE dte, Project project)
        {
            try
            {
                Solution2 sln = dte.Solution as Solution2;
                sln.SolutionBuild.StartupProjects = Path.Combine(project.Name, Path.GetFileName(project.FullName));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 扩展项目类，提供修改项目配置的方法（configName的列表参考模型目录下的ProjectProperties.txt）
        /// </summary>
        /// <param name="prj"></param>
        /// <param name="configName">ProjectConfigurationManagerProperties的名称</param>
        /// <param name="configValue">设置的值</param>
        /// <param name="configurationName"></param>
        public static void SetProjectConfig(this Project prj, string configName, string configValue, ConfigurationName configurationName = ConfigurationName.Debug)
        {
            try
            {
                if (null == prj)
                    return;
                foreach (Configuration config in prj.ConfigurationManager)
                {
                    if (config.ConfigurationName == configurationName.ToString())
                    {
                        config.Properties.Item(configName).Value = configValue;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 扩展项目类，提供修改项目属性的方法（configName的列表参考模型目录下的ProjectProperties.txt）   
        /// </summary>
        /// <param name="prj"></param>
        /// <param name="propertyName">ProjectProperties的名称</param>
        /// <param name="value">设置的值</param>
        public static void SetProjectProperty(this Project prj, string propertyName, string value)
        {
            if(prj.Properties.Item(propertyName)!=null)
            prj.Properties.Item(propertyName).Value = value;
        }

        /// <summary>
        /// 扩展项目类,提供获取项目属性的方法（configName的列表参考模型目录下的ProjectProperties.txt） 
        /// </summary>
        /// <param name="prj"></param>
        /// <param name="propertyName">ProjectProperties的名称</param>
        /// <returns>属性值</returns>
        public static object GetProjectProperty(this Project prj, string propertyName)
        {
            if (prj.Properties.Item(propertyName) != null)
                return prj.Properties.Item(propertyName).Value;
            return null;
        }

        public enum ConfigurationName
        {
            Debug,

            Release
        }

        #endregion
    }
}

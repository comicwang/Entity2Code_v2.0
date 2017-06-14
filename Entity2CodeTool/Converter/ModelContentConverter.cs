using Infoearth.Entity2CodeTool.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool.Converter
{
    /// <summary>
    /// 模型内容转换静态类
    /// </summary>
    public class ModelContentConverter
    {
        /// <summary>
        /// 固定模板转换
        /// </summary>
        /// <param name="metaString">转换源</param>
        /// <returns></returns>
        public static string Convert(string metaString)
        {
            if (string.IsNullOrEmpty(metaString))
                return string.Empty;

            if (!string.IsNullOrEmpty(ProjectCommon.ProjectName))
                metaString = metaString.Replace("$ProjectName$", ProjectCommon.ProjectName);
            if (null != ProjectList.Infrustructure)
                metaString = metaString.Replace("$Infrustructure$", ProjectCommon.Infrustructure);
            if (null != ProjectList.DomainEntity)
                metaString = metaString.Replace("$DomainEntity$", ProjectCommon.DomainEntity);
            if (null != ProjectList.DomainContext)
                metaString = metaString.Replace("$DomainContext$", ProjectCommon.DomainContext);
            if (null != ProjectList.Application)
                metaString = metaString.Replace("$Application$", ProjectCommon.Application);
            if (null != ProjectList.IApplication)
                metaString = metaString.Replace("$IApplication$", ProjectCommon.IApplication);
            if (null != ProjectList.Data2Object)
                metaString = metaString.Replace("$Data2Object$", ProjectCommon.Data2Object);
            if (null != ProjectList.Service)
                metaString = metaString.Replace("$Service$", ProjectCommon.Service);
            if(!string.IsNullOrEmpty(ProjectBuilder.ProfileContent))
                metaString = metaString.Replace("$ProfileContent$", ProjectBuilder.ProfileContent);
            if(!string.IsNullOrEmpty(ProjectBuilder.ContainerContent))
                metaString = metaString.Replace("$ContainerContent$", ProjectBuilder.ContainerContent);
            if(!string.IsNullOrEmpty(ProjectBuilder.ServiceContent))
                metaString = metaString.Replace("$ServiceContent$", ProjectBuilder.ServiceContent);
            if(!string.IsNullOrEmpty(ProjectBuilder.IServiceContent))
                metaString = metaString.Replace("$IServiceContent$", ProjectBuilder.IServiceContent);
            return metaString;
        }

        /// <summary>
        /// 固定模板加上自定义转换字典转换
        /// </summary>
        /// <param name="metaString">转换源</param>
        /// <param name="replaceKv">转换字典</param>
        /// <returns></returns>
        public static string Convert(string metaString, Dictionary<string, string> replaceKv)
        {
            metaString = Convert(metaString);
            if (null != replaceKv && replaceKv.Count > 0)
                foreach (KeyValuePair<string, string> kv in replaceKv)
                {
                    metaString = metaString.Replace(kv.Key, kv.Value);
                }
            return metaString;
        }

        /// <summary>
        /// 活动模板转换
        /// </summary>
        /// <param name="metaEntity">活动模板实体</param>
        /// <param name="metaString">转换源</param>
        /// <returns></returns>
        public static string Convert(TemplateEntity metaEntity, string metaString)
        {
            if (string.IsNullOrEmpty(metaString))
                return string.Empty;
            if (null != metaEntity)
            {
                metaString = metaString.Replace("$Data2Obj$", metaEntity.Data2Obj);
                metaString = metaString.Replace("$Entity$", metaEntity.Entity);
            }
            return Convert(metaString);         
        }
    }
}

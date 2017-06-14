using EnvDTE;
using Infoearth.Entity2CodeTool.Helps;
using Infoearth.Entity2CodeTool.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Infoearth.Entity2CodeTool
{
    /// <summary>
    /// 用于模型关键字的替换的容器类
    /// </summary>
    public static class ModelContainer
    {
        #region attrs and fields

        private static ContainerModels _models;
        /// <summary>
        /// 容器模型的集合
        /// </summary>
        public static ContainerModels Models
        {
            get { return _models; }
            set { _models = value; }
        }

        #endregion

        #region ctor

        /// <summary>
        /// 构造函数
        /// </summary>
        static ModelContainer()
        {
            ConfigContainer();
            _models.AddRange(FileOprateHelp.ReadKeyComments());
        }

        #endregion

        #region methods

        /// <summary>
        /// 创建Config
        /// </summary>
        private static void ConfigContainer()
        {
            _models = new ContainerModels();
            RegistSource("$Data2Obj$", "", "应用层实体名称");
            RegistSource("$Entity$", "", "领域层实体名称");
            RegistSource("$ProjectName$", SolutionCommon.ProjectName, "创建的项目名称");
            RegistSource("$Infrastructure$", SolutionCommon.Infrastructure, "基础结构层项目名称");
            RegistSource("$DomainEntity$", SolutionCommon.DomainEntity, "领域实体层项目名称");
            RegistSource("$DomainContext$", SolutionCommon.DomainContext, "领域层项目名称");
            RegistSource("$Application$", SolutionCommon.Application, "应用层项目名称");
            RegistSource("$IApplication$", SolutionCommon.IApplication, "应用接口层项目名称");
            RegistSource("$Data2Object$", SolutionCommon.Data2Object, "应用实体层项目名称");
            RegistSource("$Service$", SolutionCommon.Service, "服务层项目名称");
            RegistSource("$ProfileContent$", CodeBuilderContainer.ProfileBuilder, "领域层实体转换注册内容");
            RegistSource("$ContainerContent$", CodeBuilderContainer.ContainBuilder, "服务层容器注册内容");
            RegistSource("$ServiceContent$", CodeBuilderContainer.ServiceBuilder, "服务层函数内容");
            RegistSource("$IServiceContent$", CodeBuilderContainer.IServiceBuilder, "服务层接口内容");
            RegistSource("$DBContextContent$", CodeBuilderContainer.DBContextBuilder, "数据库连接字符串内容（数据库优先）");
        }

        /// <summary>
        /// 更替模型内容
        /// </summary>
        /// <param name="metaString"></param>
        public static string Replace(string metaString)
        {
            if (_models != null)
            {
                foreach (ContainerModel kv in _models)
                {
                    if (kv.Value == null)
                        kv.Value = string.Empty;
                    if (kv.Value.GetType() == typeof(string))
                        metaString = metaString.Replace(kv.Key.ToString(), kv.Value.ToString());
                    else if (kv.Value.GetType() == typeof(StringBuilder))
                    {
                        StringBuilder build = kv.Value as StringBuilder;
                        metaString = metaString.Replace(kv.Key.ToString(), build.ToString());
                    }
                }
            }
            return metaString;
        }

        /// <summary>
        /// 解析模型关键字的值
        /// </summary>
        /// <param name="metaWord"></param>
        /// <returns></returns>
        public static string Resolve(string metaWord)
        {
            if (_models != null)
            {
                foreach (ContainerModel kv in _models)
                {
                    if (String.Compare(kv.Key, metaWord, true) == 0)
                    {
                        return kv.Value.ToString();
                    }
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 注册模型源
        /// </summary>
        /// <param name="s1">关键字</param>
        /// <param name="s2">值</param>
        /// <param name="comment">描述</param>
        private static void RegistSource(string s1, object s2, string comment)
        {
            if (_models != null)
            {
                ContainerModel model = new ContainerModel() { Key = s1, ModelType = 0, Value = s2, LastModifyTime = DateTime.MinValue, Comment = comment };
                if (_models.Contains(s1))
                    _models[s1] = model;
                else
                    _models.Add(model);
            }
        }

        /// <summary>
        /// 根据主键删除模型源
        /// </summary>
        /// <param name="key">主键</param>
        public static void Remove(string key)
        {
            if (_models != null)
            {
                if (_models.Contains(key))
                {
                    _models.Remove(_models[key]);
                }
            }
        }

        /// <summary>
        /// 注册模型关键字
        /// </summary>
        /// <param name="s1">关键字</param>
        /// <param name="s2">值</param>
        public static void Regist(string s1, object s2, string comment = "")
        {
            if (_models != null)
            {
                if (_models.Contains(s1))
                {
                    ContainerModel model = _models[s1];
                    model.Comment = comment;
                    if (model.LastModifyTime != DateTime.MinValue)
                        model.LastModifyTime = DateTime.Now;
                    model.Value = s2;
                    _models[s1] = model;
                }
                else
                {
                    ContainerModel model = new ContainerModel() { Key = s1, ModelType = 1, Value = s2, LastModifyTime = DateTime.Now };
                    _models.Add(model);
                }
            }
        }

        #endregion
    }
}

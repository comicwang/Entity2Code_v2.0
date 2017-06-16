﻿using EnvDTE;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Utility.Core;
using Utility.Filter;
using Utility.Properties;

namespace Utility.Common
{
    /// <summary>
    /// 用于模型关键字的替换的容器类
    /// </summary>
    public static class KeywordContainer
    {
        private static Dictionary<string, string> _mContainer = new Dictionary<string, string>();

        static KeywordContainer()
        {
            RegistSource("$DBAppContent$", " string db= System.Configuration.ConfigurationManager.AppSettings[\"DBSchema\"];");
            RegistSource("$DBConstr$", "new InjectionConstructor(db)");
        }

        public static Dictionary<string, string> GetAll()
        {
            return _mContainer;
        }

        public static void LoadContainer()
        {
            string xmlPath = Path.Combine(CommonContainer.SolutionPath, Resource.ConfigName);
            Dictionary<string, string> models = xmlManager.ReadModel(xmlPath);

            foreach (var item in models)
            {
                RegistSource(item.Key, item.Value);
            }
            PrjCmdId.SetProjectName(PrjCmdId.Infrastructure,KeywordContainer.Resove("$Infrastructure$"));
            PrjCmdId.SetProjectName(PrjCmdId.IApplication, KeywordContainer.Resove("$IApplication$"));
            PrjCmdId.SetProjectName(PrjCmdId.Application, KeywordContainer.Resove("$Application$"));
            PrjCmdId.SetProjectName(PrjCmdId.Data2Object, KeywordContainer.Resove("$Data2Object$"));
            PrjCmdId.SetProjectName(PrjCmdId.Service, KeywordContainer.Resove("$DomainEntity$"));
            PrjCmdId.SetProjectName(PrjCmdId.DomainContext, KeywordContainer.Resove("$DomainContext$"));
            PrjCmdId.SetProjectName(PrjCmdId.DomainEntity, KeywordContainer.Resove("$Service$"));
        }


        public static void SetContainer()
        {
            foreach (var item in _mContainer)
            {
                if (!KeywordFilter.FilterCollection.Contains(item.Key))
                {
                    xmlManager.WriteModel(item.Key, item.Value,Path.Combine(CommonContainer.SolutionPath, CommonContainer.xmlName));
                }
            }
        }

        /// <summary>
        /// 更替模型内容
        /// </summary>
        /// <param name="metaString"></param>
        public static string Replace(string metaString)
        {
            if (_mContainer != null)
            {
                foreach (var kv in _mContainer)
                {
                    if (kv.Value == null)
                        metaString = metaString.Replace(kv.Key, string.Empty);
                    else
                        metaString = metaString.Replace(kv.Key, kv.Value);
                }
            }
            return metaString;
        }


        /// <summary>
        /// 注册模型源
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="value">值</param>
        public static void RegistSource(string key, string value)
        {
            if (_mContainer.ContainsKey(key))
                _mContainer[key] = value;
            else
                _mContainer.Add(key, value);
        }

        public static string Resove(string key)
        {
            if (!_mContainer.ContainsKey(key))
                return null;
            return _mContainer[key];
        }
    }
}

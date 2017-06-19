using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Utility.Properties;

namespace Utility
{
    /// <summary>
    /// 读取xml文件
    /// </summary>
    public class xmlManager
    {
        /// <summary>
        /// 读取所有关键字的信息
        /// </summary>
        /// <param name="xmlPath">关键字文件路径</param>
        /// <returns></returns>
        public static Dictionary<string, string> ReadModel(string xmlPath)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            XmlNode memberlist = xmlDoc.SelectSingleNode(Properties.Resource.PNode);
            XmlNodeList nodelist = memberlist.ChildNodes;
            foreach (XmlNode node in nodelist)
            {
                if (node.Attributes["key"] != null)
                    result.Add(node.Attributes["key"].InnerText, node.Attributes["value"].InnerText);
            }
            return result;
        }

        /// <summary>
        /// 读取所有实体信息
        /// </summary>
        /// <param name="xmlPath">xml文件路径</param>
        /// <returns></returns>
        public static Dictionary<string, string> ReadEntities(string xmlPath)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            XmlNode memberlist = xmlDoc.SelectSingleNode(Properties.Resource.EntityPNode);
            XmlNodeList nodelist = memberlist.ChildNodes;
            foreach (XmlNode node in nodelist)
            {
                if (node.Attributes["EntityName"] != null)
                    result.Add(node.Attributes["EntityName"].InnerText, node.Attributes["DTOName"].InnerText);
            }
            return result;
        }

        /// <summary>
        /// 写入实体信息
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="dtoName"></param>
        /// <param name="path"></param>
        public static void WriteEntity(string entityName, string dtoName, string path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            if (!File.Exists(path))
            {
                //创建类型声明节点  
                XmlDeclaration xdDec = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                xmlDoc.AppendChild(xdDec);
                //创建根节点  
                XmlElement xeRoot = xmlDoc.CreateElement(Resource.PNode);
                //给节点属性赋值
                xeRoot.SetAttribute("version", "2.0");
                string user = IniManager.ReadString(Resource.IniPNodeName, Resource.UserName, "");
                xeRoot.SetAttribute("author", user);
                xmlDoc.AppendChild(xeRoot);
                xmlDoc.Save(path);
            }
            else
                xmlDoc.Load(path);
            XmlNode entitiesNode = null;
            XmlNode memberlist = xmlDoc.SelectSingleNode(Properties.Resource.PNode);
            XmlNodeList nodelist = memberlist.ChildNodes;
            foreach (XmlNode node in nodelist)
            {
                if (node.Name.Equals(Resource.EntityPNode))
                {
                    entitiesNode = node;
                    break;
                }
            }
            if (entitiesNode == null)
            {
                XmlElement entity = xmlDoc.CreateElement(Properties.Resource.EntityPNode);
                entity.SetAttribute("description", "当前项目代码生成所用到的实体");
                entitiesNode = memberlist.AppendChild(entity);
            }
            XmlElement temp = xmlDoc.CreateElement(Properties.Resource.EntityNode);
            temp.SetAttribute(Resource.TabelName, entityName);
            temp.SetAttribute(Resource.DTOName, dtoName);
            entitiesNode.AppendChild(temp);

            xmlDoc.Save(path);
            
        }


        /// <summary>
        /// 写入关键字容器信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="path"></param>
        public static void WriteModel(string key, string value, string path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            if (!File.Exists(path))
            {
                //创建类型声明节点  
                XmlDeclaration xdDec = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                xmlDoc.AppendChild(xdDec);
                //创建根节点  
                XmlElement xeRoot = xmlDoc.CreateElement(Resource.PNode);
                //给节点属性赋值
                xeRoot.SetAttribute("version", "2.0");
                string user= IniManager.ReadString(Resource.IniPNodeName, Resource.UserName,"");
                xeRoot.SetAttribute("author", user);
                xmlDoc.AppendChild(xeRoot);
                xmlDoc.Save(path);
            }
            else
             xmlDoc.Load(path);
            XmlNode memberlist = xmlDoc.SelectSingleNode(Properties.Resource.PNode);
            XmlNodeList nodelist = memberlist.ChildNodes;
            bool exist = false;
            foreach (XmlNode node in nodelist)
            {
                if (node.Attributes["key"] != null && node.Attributes["key"].InnerText.Equals(key))
                {
                    node.Attributes["value"].InnerText = value;
                    exist = true;
                    break;
                }
            }
            if (exist == false)
            {
                XmlElement entity = xmlDoc.CreateElement(Properties.Resource.Node);
                entity.SetAttribute("key", key);
                entity.SetAttribute("value", value);
                memberlist.AppendChild(entity);
            }

            xmlDoc.Save(path);
            
        }
    }
}

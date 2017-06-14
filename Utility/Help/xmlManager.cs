using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Utility
{
    public class xmlManager
    {
        public static Dictionary<string, string> Read(string xmlPath)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);
            XmlNode memberlist = xmlDoc.SelectSingleNode(Properties.Resource.EntityPNode);
            XmlNodeList nodelist = memberlist.ChildNodes;
            foreach (XmlNode node in nodelist)
            {
                result.Add(node.Attributes["key"].InnerText, node.Attributes["value"].InnerText);
              
            }
            return result;
        }

        public static void Write(string key, string value, string path)
        {
            XmlDocument xmlDoc = new XmlDocument();
            if (!File.Exists(path))
            {
                //创建类型声明节点  
                XmlDeclaration xdDec = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                xmlDoc.AppendChild(xdDec);
                //创建根节点  
                XmlElement xeRoot = xmlDoc.CreateElement(Properties.Resource.EntityPNode);
                //给节点属性赋值
                xeRoot.SetAttribute("version", "2.0");
                xeRoot.SetAttribute("author", "wangchong");
                xmlDoc.AppendChild(xeRoot);
                xmlDoc.Save(path);
            }
            else
             xmlDoc.Load(path);
            XmlNode memberlist = xmlDoc.SelectSingleNode(Properties.Resource.EntityPNode);
            XmlNodeList nodelist = memberlist.ChildNodes;
            bool exist = false;
            foreach (XmlNode node in nodelist)
            {
                if (node.Attributes["key"].InnerText.Equals(key))
                {
                    node.Attributes["value"].InnerText = value;
                    exist = true;
                    break;
                }
            }
            if (exist == false)
            {
                XmlElement entity = xmlDoc.CreateElement(Properties.Resource.EntityNode);
                entity.SetAttribute("key",key);
                entity.SetAttribute("value", value);
                memberlist.AppendChild(entity);
            }

            xmlDoc.Save(path);
            
        }
    }
}

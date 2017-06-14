using EnvDTE;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool.Helps
{
    /// <summary>
    /// 有关文件操作的帮助类
    /// </summary>
    public class FileOprateHelp
    {
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="fileString"></param>
        /// <param name="filePath"></param>
        public static void SaveFile(string fileString, string filePath)
        {
            using (FileStream create = new FileStream(filePath, FileMode.Create))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(fileString);
                create.Write(buffer, 0, buffer.Length);
            }
        }

        public static void SaveFile(Stream stream, string filePath)
        {
            using (FileStream create = new FileStream(filePath, FileMode.Create))
            {
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                stream.Dispose();
                create.Write(buffer, 0, buffer.Length);
            }   
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件的字符串</returns>
        public static StringBuilder ReadFile(string filePath)
        {
            StringBuilder result = new StringBuilder();
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (reader.Peek() != -1)
                {
                    result.AppendLine(reader.ReadLine());
                }
            }
            return result;
        }

        /// <summary>
        /// 找出符合集合内容的关键字
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public static List<string> ReadFileFilter(string filePath, List<TemplateEntity> filters)
        {
            List<string> result = new List<string>();
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (reader.Peek() != -1)
                {
                    string temp= reader.ReadLine();
                    foreach (TemplateEntity item in filters)
                    {
                        if (!result.Contains(item.Data2Obj) && temp.Contains(item.Entity))
                            result.Add(item.Data2Obj);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 读取模型关键字评论的Xml文件
        /// </summary>
        /// <returns></returns>
        public static ContainerModels ReadKeyComments()
        {
            ContainerModels result = new ContainerModels();
            DataSet dt = new DataSet();
            dt.ReadXml("KeyComment.xml".GetFileResource("Xml"));
            if (dt != null && dt.Tables.Count > 0)
            {
                foreach (DataRow item in dt.Tables[0].Rows)
                {
                    result.Add(item["name"].ToString(), item["value"].ToString(), item["comment"].ToString(), DateTime.Parse(item["time"].ToString()), 1);
                }
            }
            return result;
        }

        /// <summary>
        /// 写入模型关键字Xml
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newComment"></param>
        public static void SetKeyComment(string key, string newComment, string value)
        {
            DataSet dt = new DataSet();
            dt.ReadXml("KeyComment.xml".GetFileResource("Xml"));
            DataSet dtNew = new DataSet();
            if (dt != null && dt.Tables.Count > 0)
            {
                dtNew.Merge(dt);
                int index = 0;
                for (index = 0; index < dt.Tables[0].Rows.Count; index++)
                {
                    if (dt.Tables[0].Rows[index][0].ToString() == key)
                    {
                        dtNew.Tables[0].Rows[index][1] = value; 
                        dtNew.Tables[0].Rows[index][2] = newComment;
                        dtNew.Tables[0].Rows[index][3] = DateTime.Now;
                        break;
                    }
                }
                if (index > dt.Tables[0].Rows.Count - 1)
                {
                    dtNew.Tables[0].Rows.Add(key, value, newComment, DateTime.Now);
                }
            }
            else
            {
                dtNew.Tables[0].Rows.Add(key, value, newComment, DateTime.Now);
            }
            dtNew.WriteXml("KeyComment.xml".GetFileResource("Xml"));
            ModelContainer.Regist(key, value, newComment);
        }

        /// <summary>
        /// 删除模型关键字
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveKeyComment(string key)
        {
            DataSet dt = new DataSet();
            dt.ReadXml("KeyComment.xml".GetFileResource("Xml"));
            if (dt != null && dt.Tables.Count > 0)
            {
                DataSet dtNew = new DataSet();
                dtNew.Merge(dt);
                int index = 0;
                for (index = 0; index < dt.Tables[0].Rows.Count; index++)
                {
                    if (dt.Tables[0].Rows[index][0].ToString() == key)
                    {
                        dtNew.Tables[0].Rows.Remove(dtNew.Tables[0].Rows[index]);
                        break;
                    }
                }
                dtNew.WriteXml("KeyComment.xml".GetFileResource("Xml"));
                ModelContainer.Remove(key);
            }
        }

        public static void WriteMethod(Project prj,string content ,TemplateEntity entity,string name)
        {
            string entityDir = prj.ToDirectory();
            string filePath = Directory.GetFiles(entityDir,name + ".cs").FirstOrDefault();
            if (null == filePath )
                throw new Exception("ProjectItem not Find");

            StringBuilder build = ReadFile(filePath);

            build.Insert(build.Length - 10, content);

            SaveFile(build.ToString(), filePath);
        }
    }
}

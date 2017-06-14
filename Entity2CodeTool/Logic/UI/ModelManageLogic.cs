using Infoearth.Entity2CodeTool.Converter;
using Infoearth.Entity2CodeTool.Helps;
using Infoearth.Entity2CodeTool.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Infoearth.Entity2CodeTool.Logic
{
    public class ModelManageLogic
    {
        #region attrs and fields

        #endregion

        #region ctor

        public ModelManageLogic()
        {

        }
     
        #endregion

        #region methods

        /// <summary>
        /// 获取模型的根目录
        /// </summary>
        /// <returns></returns>
        private string GetModelRoot()
        {
            return @"D:\Entity2CodeModel";
        }

        /// <summary>
        /// 获取模型列表
        /// </summary>
        /// <returns></returns>
        public List<ModelManageArgment> GetModelList(string pattern)
        {
            List<ModelManageArgment> result = new List<ModelManageArgment>();
            string root = GetModelRoot();
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);
            string[] sems = Directory.GetFiles(root, "*." + pattern, SearchOption.AllDirectories);
            if (null == sems || sems.Length == 0)
            {
                sems = root.GetFilesResourceByFilter(pattern).ToArray();
            }
            foreach (string item in sems)
            {
                string fileName = Path.GetFileName(item);
                result.Add(new ModelManageArgment() { Text = fileName, Value = item });
            }
            return result;
        }

        /// <summary>
        /// 保存模板
        /// </summary>
        /// <param name="fileString"></param>
        /// <param name="filePath"></param>
        public static void SaveFile(string fileString, string filePath)
        {
            using (FileStream create = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(fileString);
                create.Write(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// 将模板写入项目
        /// </summary>
        public static void WriteModel(string consType)
         {
             consType = Path.GetFileNameWithoutExtension(consType);
             if (consType.ToString() == "Application" || consType.ToString() == "IApplication")
                 ApplicationLogic.Create(true);
             else if (consType.ToString() == "Service" || consType.ToString() == "IService")
                 ServiceLogic.CreateCode(true);
             else if (consType.ToString() == "Map" && SolutionCommon.infrastryctType == InfrastructType.CodeFirst)
                 InfrastructureLogic.CreateCodeFirst(true);
            //其他固定模板

         }

        /// <summary>
        /// 获取模型内容
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public string GetModelContent(string modelName)
        {
            StringBuilder build = new StringBuilder();

            using (StreamReader reader = new StreamReader(modelName))
            {
                while (reader.Peek() != -1)
                {
                    string temp = reader.ReadLine();
                    build.AppendLine(temp);
                }
            }
            return build.ToString();
        }


        #endregion
    }
}

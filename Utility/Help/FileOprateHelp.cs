using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
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
        public static void SaveTextFile(string fileString, string filePath)
        {
            using (FileStream create = new FileStream(filePath, FileMode.Create))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(fileString);
                create.Write(buffer, 0, buffer.Length);
            }
        }

        public static void SaveStreamFile(Stream stream, string filePath)
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
        public static StringBuilder ReadTextFile(string filePath)
        {
            StringBuilder result = new StringBuilder();

            using (StreamReader reader = new StreamReader(filePath, Encoding.Default))
            {
                while (reader.Peek() != -1)
                {
                    result.AppendLine(reader.ReadLine());
                }
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Utility
{
    /// <summary>
    /// 配置文件读取类
    /// </summary>
    public class IniManager
    {
        private static string IniFilePath = Path.Combine(Properties.Resource.RootPath, "Entity2Code.ini");


        [DllImport("kernel32")]
        internal static extern bool WritePrivateProfileString(byte[] section, byte[] key, byte[] val, string filePath);
        [DllImport("kernel32")]
        internal static extern int GetPrivateProfileString(byte[] section, byte[] key, byte[] def, byte[] retVal, int size, string filePath);
        //与ini交互必须统一编码格式
        private static byte[] getBytes(string s, string encodingName)
        {
            return null == s ? null : Encoding.GetEncoding(encodingName).GetBytes(s);
        }

        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="section">配置节点</param>
        /// <param name="key">配置名称</param>
        /// <param name="value"></param>
        /// <param name="encodingName">编码</param>
        /// <param name="size">大小</param>
        /// <returns></returns>
        public static string ReadString(string section, string key, string def, string encodingName = "utf-8", int size = 1024)
        {
            byte[] buffer = new byte[size];
            int count = GetPrivateProfileString(getBytes(section, encodingName), getBytes(key, encodingName), getBytes(def, encodingName), buffer, size, IniFilePath);
            return Encoding.GetEncoding(encodingName).GetString(buffer, 0, count).Trim();
        }

        /// <summary>
        /// 写入配置文件
        /// </summary>
        /// <param name="section">配置节点</param>
        /// <param name="key">配置名称</param>
        /// <param name="value"></param>
        /// <param name="encodingName">编码</param>
        /// <returns></returns>
        public static bool WriteString(string section, string key, string value,string encodingName = "utf-8")
        {
            return WritePrivateProfileString(getBytes(section, encodingName), getBytes(key, encodingName), getBytes(value, encodingName), IniFilePath);
        }

    }
}

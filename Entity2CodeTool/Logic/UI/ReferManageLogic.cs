using Infoearth.Entity2CodeTool.Helps;
using Infoearth.Entity2CodeTool.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Infoearth.Entity2CodeTool.Logic.UI
{
    /// <summary>
    /// 引用程序集管理的逻辑
    /// </summary>
    public class ReferManageLogic
    {
        #region methods

        /// <summary>
        /// 根据文件路径获取程序集信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ReferManageArgment GetReferInfo(string path)
        {
            ReferManageArgment temp = new ReferManageArgment();
            temp.ReferName = Path.GetFileNameWithoutExtension(path);
            temp.Path = path;
            temp.currentVesion = AssemblyOprateHelp.GetVesion(path);
            temp.ModifyTime = File.GetLastWriteTime(path);
            return temp;
        }

        /// <summary>
        /// 获取所有的程序集信息
        /// </summary>
        /// <returns></returns>
        public static List<ReferManageArgment> GetAssmblys()
        {
            List<ReferManageArgment> result = new List<ReferManageArgment>();
            string[] dlls = Directory.GetFiles(Infoearth.Entity2CodeTool.Converter.ModelPathConverter.RootPath, "*.dll", SearchOption.AllDirectories);
            if (null == dlls || dlls.Length == 0)
            {
                dlls = Infoearth.Entity2CodeTool.Converter.ModelPathConverter.RootPath.GetFilesResourceByFilter("dll").ToArray();
            }
            foreach (string item in dlls)
            {
                result.Add(GetReferInfo(item));
            }
            return result;
        }

        #endregion
    }
}

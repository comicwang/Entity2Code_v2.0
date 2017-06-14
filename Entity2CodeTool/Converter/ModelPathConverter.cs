using Infoearth.Entity2CodeTool.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Infoearth.Entity2CodeTool.Helps;

namespace Infoearth.Entity2CodeTool.Converter
{
    /// <summary>
    /// 模型路径获取的静态类
    /// </summary>
    public class ModelPathConverter
    {
        /// <summary>
        /// 获取模型文件根目录
        /// </summary>
        public static string RootPath
        {
            get
            {
                string root = @"D:\Entity2CodeModel";
                if (!Directory.Exists(root))
                    Directory.CreateDirectory(root);
                return root;
            }
        }

        #region methods

        /// <summary>
        /// 根据模型类别获取创建代码的路径
        /// </summary>
        /// <param name="modelType"></param>
        /// <param name="data2Obj"></param>
        /// <param name="overWrite">是否覆盖原有的文件</param>
        /// <returns></returns>
        public static string GetCodePath(ConstructType modelType, bool overWrite = true, TemplateEntity entity=null)
        {
            string path = string.Empty;
            string dir = string.Empty;
            switch (modelType)
            {
                case ConstructType.Repository:
                    dir = Path.Combine(ProjectContainer.Infrastructure.ToDirectory(), "Repository");
                    if (Directory.Exists(dir) == false)
                        Directory.CreateDirectory(dir);
                    path = Path.Combine(dir, entity.Data2Obj + "Repository.cs");
                    if (File.Exists(path) && overWrite)
                        File.Delete(path);
                    break;
                case ConstructType.IRepository:
                    dir = Path.Combine(ProjectContainer.DomainContext.ToDirectory(), entity.Data2Obj);
                    if (Directory.Exists(dir) == false)
                        Directory.CreateDirectory(dir);
                    path = Path.Combine(dir, "I" + entity.Data2Obj + "Repository.cs");
                    if (File.Exists(path) && overWrite)
                        File.Delete(path);
                    break;
                case ConstructType.Application:
                    path = Path.Combine(ProjectContainer.Application.ToDirectory(), entity.Data2Obj + "App.cs");
                    if (File.Exists(path) && overWrite)
                        File.Delete(path);
                    break;
                case ConstructType.IApplication:
                    path = Path.Combine(ProjectContainer.IApplication.ToDirectory(), "I" + entity.Data2Obj + "App.cs");
                    if (File.Exists(path) && overWrite)
                        File.Delete(path);
                    break;
                case ConstructType.Data2Obj:
                    path = Path.Combine(ProjectContainer.Data2Object.ToDirectory(), entity.Data2Obj + "DTO.cs");
                    if (File.Exists(path) && overWrite)
                        File.Delete(path);
                    break;
                case ConstructType.Profile:
                    dir = Path.Combine(ProjectContainer.Data2Object.ToDirectory(), "Profile");
                    if (Directory.Exists(dir) == false)
                        Directory.CreateDirectory(dir);
                    path = Path.Combine(dir, SolutionCommon.ProjectName + "Profile.cs");
                    if (File.Exists(path) && overWrite)
                        File.Delete(path);
                    break;
                case ConstructType.Map:
                    dir = Path.Combine(ProjectContainer.Infrastructure.ToDirectory(), "Map");
                    if (Directory.Exists(dir) == false)
                        Directory.CreateDirectory(dir);
                    path = Path.Combine(dir, entity.Entity + "Map.cs");
                    if (File.Exists(path) && overWrite)
                        File.Delete(path);
                    break;
                case ConstructType.Entity:
                     path = Path.Combine(ProjectContainer.DomainEntity.ToDirectory(), entity.Entity + ".cs");
                    if (File.Exists(path) && overWrite)
                        File.Delete(path);
                    break;
                default:
                    break;
            }
            return path;
        }

        #endregion
    }
}

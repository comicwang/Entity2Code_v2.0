using EnvDTE;
using Infoearth.Entity2CodeTool.Converter;
using Infoearth.Entity2CodeTool.Helps;
using Infoearth.Entity2CodeTool.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool.Logic
{
    /// <summary>
    /// 代码创建基类（1.直接根据实体模型创建代码（文件或者字符串）2.根据实体模型生成临时字符串3.根据模型名称寻找实体类成员）
    /// </summary>
    public abstract class CodeManageBase 
    {
        #region attrs and fields

        /// <summary>
        /// 代码创建的宿体
        /// </summary>
        public abstract object BuildTaget { get; set; }

        /// <summary>
        /// 创建完成的项目项（Append子类为空）
        /// </summary>
        protected ProjectItem BuildResult { get; set; }

        /// <summary>
        /// 获取DTE类
        /// </summary>
        public EnvDTE.DTE Dte
        {
            get
            {
                if (SolutionCommon.Dte == null)
                {
                    throw new Exception("Entity2Code Lost DTE!");
                }
                return SolutionCommon.Dte;
            }
        }

        /// <summary>
        /// 获取是否是固定模板（true为固定）
        /// </summary>
        protected bool IsStaticModel { get; private set; }

        /// <summary>
        /// 获取模板生成的代码内容
        /// </summary>
        protected string CodeContent { get;private set; }

        /// <summary>
        /// 获取活动模板的实体信息（固定模板返回空）
        /// </summary>
        protected TemplateEntity ModelEntity { get; private set; }

        private ConstructType Construct;

        #endregion

        #region ctor

        /// <summary>
        /// 固定模板的抽象类构造函数
        /// </summary>
        /// <param name="consType">创建类别</param>
        public CodeManageBase(ConstructType consType)
        {
            IsStaticModel = false;
            ModelEntity = null;
            Construct = consType;
            StringBuilder result = new StringBuilder();
            using (StreamReader reader = new StreamReader(string.Format("{0}.sem", consType.ToString()).GetFileResource("Code")))
            {
                while (reader.Peek() != -1)
                {
                    string temp = reader.ReadLine();
                    temp = ModelContainer.Replace(temp);
                    result.AppendLine(temp);
                }
            }
            CodeContent = result.ToString();
        }

        /// <summary>
        /// 活动模板的抽象类构造函数
        /// </summary>
        /// <param name="consType">创建类别</param>
        /// <param name="entity">活动模板的实体信息</param>
        public CodeManageBase(ConstructType consType, TemplateEntity entity)
        {
            IsStaticModel = true;
            ModelEntity = entity;
            Construct = consType;
            StringBuilder result = new StringBuilder();
            ModelContainer.Regist("$Data2Obj$", entity.Data2Obj, "实体的数据库名称");
            ModelContainer.Regist("$Entity$", entity.Entity, "实体的应用名称");
            using (StreamReader reader = new StreamReader(string.Format("{0}.slm", consType.ToString()).GetFileResource("Code")))
            {
                while (reader.Peek() != -1)
                {
                    string temp = reader.ReadLine();
                    temp = ModelContainer.Replace(temp);
                    result.AppendLine(temp);
                }
            }
            CodeContent = result.ToString();
        }

        #endregion

        #region methods

        /// <summary>
        /// 创建代码虚类
        /// </summary>
        public virtual void CreateCode()
        {
            //整理文档
            if (null != BuildResult)
            {
                BuildResult.FormatDocument();
            }
            if (null != Dte)
            {
                if (IsStaticModel)
                {
                    Dte.OutString(string.Format("创建目标{0}的实体{1}代码完成....", Construct, ModelEntity.Entity));
                }
                else
                {
                    Dte.OutString(string.Format("创建目标{0}的代码完成....", Construct));
                }
            }         
        }

        #endregion
    }
}

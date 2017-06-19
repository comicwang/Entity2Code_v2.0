using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Common;
using Utility.Base;
using Utility.Properties;
using Utility.Entity;

namespace Utility.Core
{
    /// <summary>
    /// 代码创建的基类
    /// </summary>
    public abstract class CodeGenerateBase
    {
        /// <summary>
        /// 开始创建
        /// </summary>
        public virtual void BeginGenerate()
        {
            //if (CommonContainer.CommonServer != null)
            //    CommonContainer.CommonServer.OutString(Resource.BeginGenerate);
        }

        /// <summary>
        /// 收集生成的信息
        /// </summary>
        /// <param name="guid">项目项ID</param>
        /// <param name="arg">参数</param>
        /// <returns></returns>

        public abstract object[] GetGenerateInfo(string guid,bool arg);

        /// <summary>
        /// 处理关键字容器信息
        /// </summary>
        /// <param name="containerArgment"></param>
        public abstract void HandleGenerateContainer(Dictionary<string, string> containerArgment);

        /// <summary>
        /// 生成的临时方法，可选
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual bool GenerateTemp(object[] info)
        {
            return true;
        }

        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public abstract bool GenerateCode(object[] info);

        /// <summary>
        /// 完成生成代码
        /// </summary>
        /// <param name="result"></param>
        public virtual void EndGenerate(bool result)
        {
            //if (CommonContainer.CommonServer != null)
            //    CommonContainer.CommonServer.OutString(string.Format("{0}{1}", Resource.GenerateOutput, result ? Resource.StateSuccess : Resource.StateFail));
        }

        /// <summary>
        /// 生成代码的关键方法（串联步骤）
        /// </summary>
        /// <param name="id">项目项ID</param>
        /// <param name="allowNew">参数</param>
        /// <param name="containerArgment">关键字容器</param>
        public void Generate(string id,bool allowNew,Dictionary<string,string> containerArgment)
        {
            object[] info = GetGenerateInfo(id,allowNew);
            HandleGenerateContainer(containerArgment);
            BeginGenerate();
            bool result = GenerateTemp(info);
            result &= GenerateCode(info);
            EndGenerate(result);
        }
    }
}

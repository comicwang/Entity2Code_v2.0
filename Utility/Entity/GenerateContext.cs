using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;
using Utility.Core;
using Utility.Help;

namespace Utility.Entity
{
    /// <summary>
    /// 生成代码的上下文
    /// </summary>
    public class GenerateContext
    {
        /// <summary>
        /// 需要生成的个数
        /// </summary>
        public int Count { get { return _gEntityQueue.Count(); } }

        /// <summary>
        /// 处理完一个事件代理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        public delegate void GeneratedEventHandle(object sender, EventGeneratedArg arg);

        /// <summary>
        /// 处理完一个之后的事件
        /// </summary>
        public event GeneratedEventHandle GeneratedOne;

        private static Queue<GenerateEntity> _gEntityQueue = new Queue<GenerateEntity>();

        /// <summary>
        /// 将生成代码的参数注入到上下文中
        /// </summary>
        /// <param name="gEntity">参数实体</param>
        public void injection(GenerateEntity gEntity)
        {
            _gEntityQueue.Enqueue(gEntity);
        }

        /// <summary>
        /// 将生成代码的参数注入到上下文中
        /// </summary>
        /// <param name="engin">生成代码的引擎类</param>
        /// <param name="id">项目项ID</param>
        /// <param name="argment">参数</param>
        /// <param name="container">关键字容器</param>
        public void injection(CodeGenerateBase engin, string id, bool argment, Dictionary<string, string> container)
        {
            _gEntityQueue.Enqueue(new GenerateEntity(engin, id, argment, container));
        }

        /// <summary>
        /// 开始执行上下文中所有的内容
        /// </summary>
        public void Commit()
        {
            while (_gEntityQueue.Count() > 0)
            {
                GenerateEntity queue = _gEntityQueue.Dequeue();
                try
                {
                    queue.GenerateEngin.Generate(queue.GenerateId, queue.GenerateArgment, queue.GenerateContainer);
                }
                catch (Exception ex)
                {
                    MsgBoxHelp.ShowError(string.Format(Properties.Resource.GenerateError, PrjCmdId.FindProjectName(queue.GenerateId)), ex);
                }
                finally
                {
                    if (GeneratedOne != null)
                        GeneratedOne(this, new EventGeneratedArg(queue.GenerateId, Count, KeywordContainer.Resove("$Entity$")));
                }
            }
        }

    }
}

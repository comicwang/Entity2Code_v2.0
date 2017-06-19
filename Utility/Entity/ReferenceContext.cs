using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Core;
using Utility.Help;
using Utility.Base;

namespace Utility.Entity
{
    /// <summary>
    /// 引用类库的上下文
    /// </summary>
    public class ReferenceContext
    {
        /// <summary>
        /// 需要引用的个数
        /// </summary>
        public int Count
        {
            get
            {
               return _rEntityQueue.Count;
            }
        }

        /// <summary>
        /// 引用后触发事件代理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        public delegate void HandledEventHandler(object sender, EventHandledArg arg);

        /// <summary>
        /// 引用完一个之后事件
        /// </summary>
        public event HandledEventHandler HandledOne;

        private static Queue<ReferenceEntity> _rEntityQueue = new Queue<ReferenceEntity>();

        /// <summary>
        /// 将引用注入上下文
        /// </summary>
        /// <param name="gEntity"></param>
        public void injection(ReferenceEntity gEntity)
        {
            _rEntityQueue.Enqueue(gEntity);
        }

        /// <summary>
        /// 将引用注入上下文
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="references">需要引用的参数：三种</param>
        public void injection(string projectId,params string[] references)
        {
            _rEntityQueue.Enqueue(new ReferenceEntity(projectId, references));
        }

        /// <summary>
        /// 执行上下文
        /// </summary>

        public void Commit()
        {
            while (_rEntityQueue.Count() > 0)
            {
                ReferenceEntity queue = _rEntityQueue.Dequeue();
                try
                {
                    Project proj = TemplateContainer.Resove<Project>(queue.ProjectId);
                    List<string> refers = queue.ReferenceCollection;
                    if (refers != null && refers.Count > 0)
                        refers.ForEach(t =>
                        {
                            proj.AddReferByKey(t);
                        });
                }
                catch (Exception ex)
                {
                    MsgBoxHelp.ShowError(string.Format(Properties.Resource.ReferError, PrjCmdId.FindProjectName(queue.ProjectId)), ex);
                }
                finally
                {
                    if (HandledOne != null)
                        HandledOne(this, new EventHandledArg(queue.ProjectId, Count));
                }
            }
        }
    }
}

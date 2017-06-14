using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using EnvDTE80;
using EnvDTE90;
using EnvDTE100;
using Infoearth.Entity2CodeTool.Model;
using Infoearth.Entity2CodeTool.Converter;
using Infoearth.Entity2CodeTool.Logic;
using Infoearth.Entity2CodeTool.Helps;

namespace Infoearth.Entity2CodeTool
{
    /// <summary>
    /// 解决方案策略
    /// </summary>
    public class SolutionStrategy
    {
        private BackgroundWorker _work;
        private DTE _dte;
      
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dte">DTE对象</param>
        public SolutionStrategy(DTE dte)
        {
            _dte = dte;
            CodeBuilderContainer.Clear();
            _work = new BackgroundWorker();
            _work.WorkerReportsProgress = true;
            _work.DoWork += _work_DoWork;
            _work.ProgressChanged += _work_ProgressChanged;
            _work.RunWorkerCompleted += _work_RunWorkerCompleted;
        }

        public void BeginStrategy()
        {
            _work.RunWorkerAsync();
        }

        private void _work_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SolutionExplorHelp.CollapseAll();
            _dte.RebuildSolution();
            _dte.OutString("创建完成..");
        }

        private void _work_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage > 0 && e.ProgressPercentage <= 100)
                _dte.OutProcessBar(e.ProgressPercentage, e.UserState.ToString(), true);
            else
                _dte.OutString(e.UserState.ToString(), true);
        }

        private void _work_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                AddSchameLib(true);
                ReferenceLogic.Add();
            }
            catch (Exception ex)
            {
                _dte.OutWindow(ex.Message);
            }
        }

        /// <summary>
        /// 添加构架应用程序
        /// </summary>
        /// <param name="ProjectCommon"></param>
        private void AddSchameLib(bool overWrite = true)
        {
            try
            {
                if (SolutionCommon.infrastryctType == InfrastructType.CodeFirst)
                {
                    InfrastructureLogic.CreateCodeFirst(overWrite);
                    DomainEntityLogic.CreateCodeFirst(overWrite);
                }
                else
                {
                    InfrastructureLogic.Create(overWrite);
                    DomainEntityLogic.Create(overWrite);
                }
                RepositoryLogic.Create(overWrite);
                Data2ObjLogic.Create(overWrite);
                ApplicationLogic.Create(overWrite);
                if (SolutionCommon.IsAddService)
                {
                    ServiceLogic.CreateFrame(overWrite);
                    ServiceLogic.CreateCode(overWrite);
                }
                else
                    ProjectContainer.Service = null;

                //保存解决方案
                if (_dte.Solution.Saved == false)
                    _dte.Solution.SaveAs(_dte.Solution.FullName);
            }
            catch (Exception ex)
            {
                MsgBoxHelp.ShowError("Entity2Code Error:", ex);
            }
        }
    }


}

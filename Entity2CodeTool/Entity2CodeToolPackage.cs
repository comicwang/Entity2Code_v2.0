using System;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using EnvDTE80;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using Infoearth.Entity2CodeTool.Model;
using Infoearth.Entity2CodeTool.Helps;
using Infoearth.Entity2CodeTool.Logic;
using Infoearth.Entity2CodeTool.UI;
using Infoearth.Entity2CodeTool;

namespace Infoearth.Entity2CodeTool
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidEntity2CodeToolPkgString)]
    public sealed class Entity2CodeToolPackage : Package
    {
        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>

        public Entity2CodeToolPackage()
        {

        }

        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Utility.ConfirmResource.Copy();

            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidEntity2CodeMain, (int)PkgCmdIDList.guidEntity2CodeMain);
                MenuCommand menuItem = new MenuCommand(CreateCode, menuCommandID);
                mcs.AddCommand(menuItem);

                CommandID menuCommandID3 = new CommandID(GuidList.guidEntity2CodeAddMethod, (int)PkgCmdIDList.addMethodCommad);
                MenuCommand menuItem3 = new MenuCommand(AddMethod, menuCommandID3);
                mcs.AddCommand(menuItem3);

                CommandID menuCommandID1 = new CommandID(GuidList.guidEntity2CodeModelManager, (int)PkgCmdIDList.modelManageCommand);
                MenuCommand menuItem1 = new MenuCommand(ModelManage, menuCommandID1);
                mcs.AddCommand(menuItem1);

                CommandID menuCommandID2 = new CommandID(GuidList.guidEntity2CodeReferManager, (int)PkgCmdIDList.referManageCommand);
                MenuCommand menuItem2 = new MenuCommand(ReferManage, menuCommandID2);
                mcs.AddCommand(menuItem2);
            }
        }

        private void AddMethod(object sender, EventArgs e)
        {
            if (SolutionCommon.Dte == null)
                SolutionCommon.Dte = (DTE)(GetService(typeof(DTE)));

            FormAddMethod frmAdd = new FormAddMethod();
            frmAdd.Show();
        }

        /// <summary>
        /// 引用管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReferManage(object sender, EventArgs e)
        {
            if (SolutionCommon.Dte == null)
                SolutionCommon.Dte = (DTE)(GetService(typeof(DTE)));

            FormReferManager frmRefer = new FormReferManager();
            frmRefer.Show();
        }
        #endregion

        /// <summary>
        /// 模型管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModelManage(object sender, EventArgs e)
        {
            if (SolutionCommon.Dte == null)
                SolutionCommon.Dte = (DTE)(GetService(typeof(DTE)));

            FormModelManager frmModel = new FormModelManager();
            frmModel.Show();
        }

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void CreateCode(object sender, EventArgs e) 
        {
            DTE dte = (DTE)(GetService(typeof(DTE)));
            //Test(dte);
            dte.OutString("开始收集构架应用程序名称信息.", true);
            FormMain frm = new FormMain(dte.Solution.FullName);
            if (frm.ShowDialog() != DialogResult.OK)
                return;
            Solution sln = dte.Solution;
            if (string.IsNullOrEmpty(sln.FullName))
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.RootFolder = Environment.SpecialFolder.MyComputer;
                dialog.Description = "项目存储路径";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    sln.Create(string.Format("iTelluro.Explorer.{0}.sln", SolutionCommon.ProjectName), string.Format("iTelluro.Explorer.{0}", SolutionCommon.ProjectName));
                    dte.Solution.SaveAs(Path.Combine(dialog.SelectedPath, string.Format("iTelluro.Explorer.{0}.sln", SolutionCommon.ProjectName)));
                }
                else
                    return;
            }

            SolutionCommon.Dte = dte;
            SolutionStrategy strategy = new SolutionStrategy(dte);
            strategy.BeginStrategy();
        }
         
    }
}

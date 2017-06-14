using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Base
{
    static class CodeClassExtention
    {
        #region methods

        /// <summary>
        /// 找到项目项的代码对象
        /// </summary>
        /// <param name="projectItem"></param>
        /// <returns>该项目的代码对象</returns>
        public static CodeClass FindCodeClass(this ProjectItem projectItem)
        {
            TextSelection selection = (TextSelection)projectItem.Document.Selection;
            CodeClass codeClass = (CodeClass)selection.TopPoint.get_CodeElement(vsCMElement.vsCMElementClass);
            return codeClass;
        }

        #endregion
    }
}

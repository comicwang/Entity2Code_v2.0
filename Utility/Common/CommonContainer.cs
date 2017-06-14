using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Common
{
    public static class CommonContainer
    {
        public static EnvDTE.DTE CommonServer { get; set; }

        public static string RootPath
        {
            get { return Properties.Resource.RootPath; }
        }

        public static string SolutionPath { get; set; }
    }
}

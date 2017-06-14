using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool.Model
{
    static class MethodCommon
    {
        public static string MethodName { get; set; }

        public static string Return { get; set; }

        public static string Comment { get; set; }

        public static string Author { get; set; }

        public static string Organize { get; set; }

        public static string ReturnComment { get; set; }

        public static List<ReferArg> InnerArgs { get; set; }
    }
}

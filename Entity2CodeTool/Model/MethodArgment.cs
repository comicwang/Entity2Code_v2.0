using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool.Model
{
    public class MethodArgment
    {
        public string MethodName { get; set; }

        public string Return { get; set; }

        public string Comment { get; set; }

        public string Author { get; set; }

        public string Organize { get; set; }

        public List<ReferArg> InnerArgs { get; set; }
    }

    public class ReferArg
    {
        public string Name { get; set; }

        public string VType { get; set; }

        public string Comment { get; set; }

        public bool IsOut { get; set; }
    }
}

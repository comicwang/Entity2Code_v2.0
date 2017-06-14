using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool.Model
{
    public class StringCodeArgment
    {
        public string Folder { get; set; }

        public string Name { get; set; }

        public Project Target { get; set; }

        public Encoding Encode = Encoding.Default;
    }
}

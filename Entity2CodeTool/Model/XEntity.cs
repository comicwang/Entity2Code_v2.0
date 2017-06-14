using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool.Model
{
    public abstract class XEntity
    {
        public string XName { get; set; }

        public abstract void SetXName();
    }
}

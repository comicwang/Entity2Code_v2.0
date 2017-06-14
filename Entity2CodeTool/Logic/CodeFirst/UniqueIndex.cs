using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool
{

    /// <summary>
    /// 索引类
    /// </summary>
    public class UniqueIndex
    {
        public string Schema;
        public string TableName;
        public string IndexName;
        public byte KeyOrdinal;
        public string Column;
        public int ColumnCount;
    }
}

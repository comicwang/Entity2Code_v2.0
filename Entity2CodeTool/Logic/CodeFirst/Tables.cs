using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool
{
    /// <summary>
    /// 表集合类
    /// </summary>
    public class Tables : List<Table>
    {
        public Table GetTable(string tableName, string schema)
        {
            return this.SingleOrDefault(x =>
                String.Compare(x.Name, tableName, StringComparison.OrdinalIgnoreCase) == 0 &&
                String.Compare(x.Schema, schema, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public void SetPrimaryKeys()
        {
            foreach (var tbl in this)
            {
                tbl.SetPrimaryKeys();
            }
        }

        public void IdentifyMappingTables(List<ForeignKey> fkList, bool useCamelCase, string collectionType, bool checkForFkNameClashes, bool includeComments, bool isSqlCE)
        {
            foreach (var tbl in this.Where(x => x.HasForeignKey))
            {
                tbl.IdentifyMappingTable(fkList, this, useCamelCase, collectionType, checkForFkNameClashes, includeComments, isSqlCE);
            }
        }

        public void ResetNavigationProperties()
        {
            foreach (var tbl in this)
            {
                tbl.ResetNavigationProperties();
            }
        }
    }
}

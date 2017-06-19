using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.CodeFirst
{
    /// <summary>
    /// 表集合类
    /// </summary>
    public class Tables : List<Table>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        public Table GetTable(string tableName, string schema)
        {
            return this.SingleOrDefault(x =>
                String.Compare(x.Name, tableName, StringComparison.OrdinalIgnoreCase) == 0 &&
                String.Compare(x.Schema, schema, StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetPrimaryKeys()
        {
            foreach (var tbl in this)
            {
                tbl.SetPrimaryKeys();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fkList"></param>
        /// <param name="useCamelCase"></param>
        /// <param name="collectionType"></param>
        /// <param name="checkForFkNameClashes"></param>
        /// <param name="includeComments"></param>
        /// <param name="isSqlCE"></param>
        public void IdentifyMappingTables(List<ForeignKey> fkList, bool useCamelCase, string collectionType, bool checkForFkNameClashes, bool includeComments, bool isSqlCE)
        {
            foreach (var tbl in this.Where(x => x.HasForeignKey))
            {
                tbl.IdentifyMappingTable(fkList, this, useCamelCase, collectionType, checkForFkNameClashes, includeComments, isSqlCE);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetNavigationProperties()
        {
            foreach (var tbl in this)
            {
                tbl.ResetNavigationProperties();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utility.CodeFirst
{
    /// <summary>
    /// 数据库架构读取类SchemaReader
    /// </summary>
    public abstract class SchemaReader
    {
        /// <summary>
        /// 数据库执行器
        /// </summary>
        protected readonly DbCommand Cmd;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="factory"></param>
        protected SchemaReader(DbConnection connection, DbProviderFactory factory)
        {
            Cmd = factory.CreateCommand();
            if (Cmd != null)
                Cmd.Connection = connection;
        }

        /// <summary>
        /// 
        /// </summary>
        public object Outer;

        /// <summary>
        /// 读取表空间
        /// </summary>
        /// <param name="tableFilterExclude"></param>
        /// <param name="columnFilterExclude"></param>
        /// <param name="useCamelCase"></param>
        /// <param name="prependSchemaName"></param>
        /// <param name="includeComments"></param>
        /// <param name="includeExtendedPropertyComments"></param>
        /// <param name="tableRename"></param>
        /// <param name="schemaNameFilter"></param>
        /// <param name="updateColumn"></param>
        /// <returns></returns>
        public abstract Tables ReadSchema(Regex tableFilterExclude, Regex columnFilterExclude, bool useCamelCase, bool prependSchemaName, bool includeComments, ExtendedPropertyCommentsStyle includeExtendedPropertyComments, Func<string, string, string> tableRename, string schemaNameFilter, Func<Column, Table, Column> updateColumn);
        /// <summary>
        /// 读取存储过程
        /// </summary>
        /// <param name="storedProcedureFilterExclude"></param>
        /// <param name="useCamelCase"></param>
        /// <param name="prependSchemaName"></param>
        /// <param name="StoredProcedureRename"></param>
        /// <param name="schemaNameFilter"></param>
        /// <returns></returns>
        public abstract List<StoredProcedure> ReadStoredProcs(Regex storedProcedureFilterExclude, bool useCamelCase, bool prependSchemaName, Func<string, string, string> StoredProcedureRename, string schemaNameFilter);
        /// <summary>
        /// 读取外键信息
        /// </summary>
        /// <param name="tableRename"></param>
        /// <returns></returns>
        public abstract List<ForeignKey> ReadForeignKeys(Func<string, string, string> tableRename);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fkList"></param>
        /// <param name="tables"></param>
        /// <param name="useCamelCase"></param>
        /// <param name="prependSchemaName"></param>
        /// <param name="collectionType"></param>
        /// <param name="checkForFkNameClashes"></param>
        /// <param name="includeComments"></param>
        public abstract void ProcessForeignKeys(List<ForeignKey> fkList, Tables tables, bool useCamelCase, bool prependSchemaName, string collectionType, bool checkForFkNameClashes, bool includeComments);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fkList"></param>
        /// <param name="tables"></param>
        public abstract void IdentifyForeignKeys(List<ForeignKey> fkList, Tables tables);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        public abstract void ReadUniqueIndexes(Tables tables);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tables"></param>
        public abstract void ReadExtendedProperties(Tables tables);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>

        protected void WriteLine(string o)
        {
            Console.WriteLine(o);
        }
    }
}

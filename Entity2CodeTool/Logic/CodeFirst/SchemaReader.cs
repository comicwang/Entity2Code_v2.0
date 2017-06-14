using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool
{
    /// <summary>
    /// 数据库架构读取类SchemaReader
    /// </summary>
    public abstract class SchemaReader
    {
        protected readonly DbCommand Cmd;

        protected SchemaReader(DbConnection connection, DbProviderFactory factory)
        {
            Cmd = factory.CreateCommand();
            if (Cmd != null)
                Cmd.Connection = connection;
        }

        public object Outer;
        public abstract Tables ReadSchema(Regex tableFilterExclude, Regex columnFilterExclude, bool useCamelCase, bool prependSchemaName, bool includeComments, ExtendedPropertyCommentsStyle includeExtendedPropertyComments, Func<string, string, string> tableRename, string schemaNameFilter, Func<Column, Table, Column> updateColumn);
        public abstract List<StoredProcedure> ReadStoredProcs(Regex storedProcedureFilterExclude, bool useCamelCase, bool prependSchemaName, Func<string, string, string> StoredProcedureRename, string schemaNameFilter);
        public abstract List<ForeignKey> ReadForeignKeys(Func<string, string, string> tableRename);
        public abstract void ProcessForeignKeys(List<ForeignKey> fkList, Tables tables, bool useCamelCase, bool prependSchemaName, string collectionType, bool checkForFkNameClashes, bool includeComments);
        public abstract void IdentifyForeignKeys(List<ForeignKey> fkList, Tables tables);
        public abstract void ReadUniqueIndexes(Tables tables);
        public abstract void ReadExtendedProperties(Tables tables);

        protected void WriteLine(string o)
        {
            Console.WriteLine(o);
        }
    }
}

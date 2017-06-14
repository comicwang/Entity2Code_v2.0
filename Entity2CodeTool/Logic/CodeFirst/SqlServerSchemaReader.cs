using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool
{
    /// <summary>
    /// SQL数据库架构读取类
    /// </summary>
    public class SqlServerSchemaReader : SchemaReader
    {
        private const string TableSQL = @"
SELECT  c.TABLE_SCHEMA AS SchemaName,
        c.TABLE_NAME AS TableName,
        t.TABLE_TYPE AS TableType,
        c.ORDINAL_POSITION AS Ordinal,
        c.COLUMN_NAME AS ColumnName,
        CAST(CASE WHEN IS_NULLABLE = 'YES' THEN 1
                  ELSE 0
             END AS BIT) AS IsNullable,
        DATA_TYPE AS TypeName,
        ISNULL(CHARACTER_MAXIMUM_LENGTH, 0) AS [MaxLength],
        CAST(ISNULL(NUMERIC_PRECISION, 0) AS INT) AS [Precision],
        ISNULL(COLUMN_DEFAULT, '') AS [Default],
        CAST(ISNULL(DATETIME_PRECISION, 0) AS INT) AS DateTimePrecision,
        ISNULL(NUMERIC_SCALE, 0) AS Scale,
        CAST(COLUMNPROPERTY(OBJECT_ID(QUOTENAME(c.TABLE_SCHEMA) + '.' + QUOTENAME(c.TABLE_NAME)), c.COLUMN_NAME, 'IsIdentity') AS BIT) AS IsIdentity,
        CAST(CASE WHEN COLUMNPROPERTY(OBJECT_ID(QUOTENAME(c.TABLE_SCHEMA) + '.' + QUOTENAME(c.TABLE_NAME)), c.COLUMN_NAME, 'IsIdentity') = 1 THEN 1
                  WHEN COLUMNPROPERTY(OBJECT_ID(QUOTENAME(c.TABLE_SCHEMA) + '.' + QUOTENAME(c.TABLE_NAME)), c.COLUMN_NAME, 'IsComputed') = 1 THEN 1
                  WHEN DATA_TYPE = 'TIMESTAMP' THEN 1
                  ELSE 0
             END AS BIT) AS IsStoreGenerated,
        CAST(CASE WHEN pk.ORDINAL_POSITION IS NULL THEN 0
                  ELSE 1
             END AS BIT) AS PrimaryKey,
        ISNULL(pk.ORDINAL_POSITION, 0) PrimaryKeyOrdinal,
        CAST(CASE WHEN fk.COLUMN_NAME IS NULL THEN 0
                  ELSE 1
             END AS BIT) AS IsForeignKey
FROM    INFORMATION_SCHEMA.COLUMNS c
        LEFT OUTER JOIN (SELECT u.TABLE_SCHEMA,
                                u.TABLE_NAME,
                                u.COLUMN_NAME,
                                u.ORDINAL_POSITION
                         FROM   INFORMATION_SCHEMA.KEY_COLUMN_USAGE u
                                INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                                    ON u.TABLE_SCHEMA = tc.CONSTRAINT_SCHEMA
                                       AND u.TABLE_NAME = tc.TABLE_NAME
                                       AND u.CONSTRAINT_NAME = tc.CONSTRAINT_NAME
                         WHERE  CONSTRAINT_TYPE = 'PRIMARY KEY') pk
            ON c.TABLE_SCHEMA = pk.TABLE_SCHEMA
               AND c.TABLE_NAME = pk.TABLE_NAME
               AND c.COLUMN_NAME = pk.COLUMN_NAME
        LEFT OUTER JOIN (SELECT DISTINCT
                                u.TABLE_SCHEMA,
                                u.TABLE_NAME,
                                u.COLUMN_NAME
                         FROM   INFORMATION_SCHEMA.KEY_COLUMN_USAGE u
                                INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                                    ON u.TABLE_SCHEMA = tc.CONSTRAINT_SCHEMA
                                       AND u.TABLE_NAME = tc.TABLE_NAME
                                       AND u.CONSTRAINT_NAME = tc.CONSTRAINT_NAME
                         WHERE  CONSTRAINT_TYPE = 'FOREIGN KEY') fk
            ON c.TABLE_SCHEMA = fk.TABLE_SCHEMA
               AND c.TABLE_NAME = fk.TABLE_NAME
               AND c.COLUMN_NAME = fk.COLUMN_NAME
        INNER JOIN INFORMATION_SCHEMA.TABLES t
            ON c.TABLE_SCHEMA = t.TABLE_SCHEMA
               AND c.TABLE_NAME = t.TABLE_NAME
WHERE c.TABLE_NAME NOT IN ('EdmMetadata', '__MigrationHistory','sysdiagrams')";

        private const string ForeignKeySQL = @"
SELECT  FK.name AS FK_Table,
        FkCol.name AS FK_Column,
        PK.name AS PK_Table,
        PkCol.name AS PK_Column,
        OBJECT_NAME(f.object_id) AS Constraint_Name,
        SCHEMA_NAME(FK.schema_id) AS fkSchema,
        SCHEMA_NAME(PK.schema_id) AS pkSchema,
        PkCol.name AS primarykey,
        k.constraint_column_id AS ORDINAL_POSITION,
		case when f.delete_referential_action_desc='CASCADE' then 1 else 0 end as [CASCADE]
FROM    sys.objects AS PK
        INNER JOIN sys.foreign_keys AS f
            INNER JOIN sys.foreign_key_columns AS k
                ON k.constraint_object_id = f.object_id
            INNER JOIN sys.indexes AS i
                ON f.referenced_object_id = i.object_id
                   AND f.key_index_id = i.index_id
            ON PK.object_id = f.referenced_object_id
        INNER JOIN sys.objects AS FK
            ON f.parent_object_id = FK.object_id
        INNER JOIN sys.columns AS PkCol
            ON f.referenced_object_id = PkCol.object_id
               AND k.referenced_column_id = PkCol.column_id
        INNER JOIN sys.columns AS FkCol
            ON f.parent_object_id = FkCol.object_id
               AND k.parent_column_id = FkCol.column_id
ORDER BY FK_Table, FK_Column
";

        private const string ExtendedPropertySQL = @"
SELECT  s.name AS [schema],
        t.name AS [table],
        c.name AS [column],
        value AS [property]
FROM    sys.extended_properties AS ep
        INNER JOIN sys.tables AS t
            ON ep.major_id = t.object_id
        INNER JOIN sys.schemas AS s
            ON s.schema_id = t.schema_id
        INNER JOIN sys.columns AS c
            ON ep.major_id = c.object_id
               AND ep.minor_id = c.column_id
WHERE   class = 1
ORDER BY t.name";

        private const string ExtendedPropertyTableExistsSQLCE = @"
SELECT  1
FROM    INFORMATION_SCHEMA.TABLES
WHERE   TABLE_NAME = '__ExtendedProperties';";

        private const string ExtendedPropertySQLCE = @"
SELECT  '' AS [schema],
        [ObjectName] AS [column],
        [ParentName] AS [table],
        [Value] AS [property]
FROM    [__ExtendedProperties];";

        private const string TableSQLCE = @"
SELECT  '' AS SchemaName,
        c.TABLE_NAME AS TableName,
        'BASE TABLE' AS TableType,
        c.ORDINAL_POSITION AS Ordinal,
        c.COLUMN_NAME AS ColumnName,
        CAST(CASE WHEN c.IS_NULLABLE = N'YES' THEN 1
                  ELSE 0
             END AS BIT) AS IsNullable,
        c.DATA_TYPE AS TypeName,
        CASE WHEN c.CHARACTER_MAXIMUM_LENGTH IS NOT NULL THEN c.CHARACTER_MAXIMUM_LENGTH
             ELSE 0
        END AS MaxLength,
        CASE WHEN c.NUMERIC_PRECISION IS NOT NULL THEN c.NUMERIC_PRECISION
             ELSE 0
        END AS Precision,
        c.COLUMN_DEFAULT AS [Default],
        CASE WHEN c.DATA_TYPE = N'datetime' THEN 0
             ELSE 0
        END AS DateTimePrecision,
        CASE WHEN c.DATA_TYPE = N'datetime' THEN 0
             WHEN c.NUMERIC_SCALE IS NOT NULL THEN c.NUMERIC_SCALE
             ELSE 0
        END AS Scale,
        CAST(CASE WHEN c.AUTOINC_INCREMENT > 0 THEN 1
                  ELSE 0
             END AS BIT) AS IsIdentity,
        CAST(CASE WHEN c.DATA_TYPE = N'rowversion' THEN 1
                  ELSE 0
             END AS BIT) AS IsStoreGenerated,
        CAST(CASE WHEN u.TABLE_NAME IS NULL THEN 0
                  ELSE 1
             END AS BIT) AS PrimaryKey,
        0 AS PrimaryKeyOrdinal,
        0 as IsForeignKey
FROM    INFORMATION_SCHEMA.COLUMNS c
        INNER JOIN INFORMATION_SCHEMA.TABLES t
            ON c.TABLE_NAME = t.TABLE_NAME
        LEFT JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS cons
            ON cons.TABLE_NAME = c.TABLE_NAME
        LEFT JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS u
            ON cons.CONSTRAINT_NAME = u.CONSTRAINT_NAME
               AND u.TABLE_NAME = c.TABLE_NAME
               AND u.COLUMN_NAME = c.COLUMN_NAME
WHERE   t.TABLE_TYPE <> N'SYSTEM TABLE'
        AND cons.CONSTRAINT_TYPE = 'PRIMARY KEY'
ORDER BY c.TABLE_NAME,
        c.COLUMN_NAME,
        c.ORDINAL_POSITION;";

        private const string ForeignKeySQLCE = @"
SELECT DISTINCT
        FK.TABLE_NAME AS FK_Table,
        FK.COLUMN_NAME AS FK_Column,
        PK.TABLE_NAME AS PK_Table,
        PK.COLUMN_NAME AS PK_Column,
        FK.CONSTRAINT_NAME AS Constraint_Name,
        '' AS fkSchema,
        '' AS pkSchema,
        PT.COLUMN_NAME AS primarykey,
        FK.ORDINAL_POSITION
FROM    INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS AS C
        INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS FK
            ON FK.CONSTRAINT_NAME = C.CONSTRAINT_NAME
        INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS PK
            ON PK.CONSTRAINT_NAME = C.UNIQUE_CONSTRAINT_NAME
               AND PK.ORDINAL_POSITION = FK.ORDINAL_POSITION
        INNER JOIN (
                    SELECT  i1.TABLE_NAME,
                            i2.COLUMN_NAME
                    FROM    INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1
                            INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2
                                ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME
                    WHERE   i1.CONSTRAINT_TYPE = 'PRIMARY KEY'
                   ) PT
            ON PT.TABLE_NAME = PK.TABLE_NAME
WHERE   PT.COLUMN_NAME = PK.COLUMN_NAME
ORDER BY FK.TABLE_NAME, FK.COLUMN_NAME;";

        private const string StoredProcedureSQL = @"
SELECT  P.SPECIFIC_SCHEMA,
        P.SPECIFIC_NAME,
        P.ORDINAL_POSITION,
        P.PARAMETER_MODE,
        P.PARAMETER_NAME,
        P.DATA_TYPE,
        ISNULL(P.CHARACTER_MAXIMUM_LENGTH, 0) AS CHARACTER_MAXIMUM_LENGTH,
        ISNULL(P.NUMERIC_PRECISION, 0) AS NUMERIC_PRECISION,
        ISNULL(P.NUMERIC_SCALE, 0) AS NUMERIC_SCALE,
        ISNULL(P.DATETIME_PRECISION, 0) AS DATETIME_PRECISION
FROM    INFORMATION_SCHEMA.PARAMETERS P
        INNER JOIN INFORMATION_SCHEMA.ROUTINES R
            ON P.SPECIFIC_SCHEMA = R.SPECIFIC_SCHEMA
               AND P.SPECIFIC_NAME = R.SPECIFIC_NAME
               AND R.ROUTINE_TYPE = 'PROCEDURE'
WHERE   P.IS_RESULT = 'NO'
        AND P.SPECIFIC_SCHEMA + P.SPECIFIC_NAME IN (
            SELECT  SCHEMA_NAME(sp.schema_id) + sp.name
            FROM    sys.all_objects AS sp
                    LEFT OUTER JOIN sys.all_sql_modules AS sm
                        ON sm.object_id = sp.object_id
            WHERE   sp.type = 'P'
                    AND (
                         CAST(CASE WHEN sp.is_ms_shipped = 1 THEN 1
                                   WHEN (SELECT major_id
                                         FROM   sys.extended_properties
                                         WHERE  major_id = sp.object_id
                                                AND minor_id = 0
                                                AND class = 1
                                                AND name = N'microsoft_database_tools_support') IS NOT NULL THEN 1
                                   ELSE 0
                              END AS BIT) = 0
                        )
        )
ORDER BY P.SPECIFIC_SCHEMA,
        P.SPECIFIC_NAME,
        P.ORDINAL_POSITION";

        private const string UniqueIndexSQL = @"
SELECT  SCHEMA_NAME(t.schema_id) AS TableSchema,
        t.name AS TableName,
        ind.name AS IndexName,
        ic.key_ordinal AS KeyOrdinal,
        col.name AS ColumnName,
        (SELECT COUNT(1)
         FROM   sys.index_columns i
         WHERE  i.object_id = ind.object_id
                AND i.index_id = ind.index_id) AS ColumnCount
FROM    sys.tables t
        INNER JOIN sys.indexes ind
            ON ind.object_id = t.object_id
        INNER JOIN sys.index_columns ic
            ON ind.object_id = ic.object_id
               AND ind.index_id = ic.index_id
        INNER JOIN sys.columns col
            ON ic.object_id = col.object_id
               AND ic.column_id = col.column_id
WHERE   t.is_ms_shipped = 0
        AND (
             ind.is_unique = 1
             OR ind.is_primary_key = 1
             OR ind.is_unique_constraint = 1
            )
        AND ind.ignore_dup_key = 0";

        private bool IncludeQueryTraceOn9481Flag;

        public SqlServerSchemaReader(DbConnection connection, DbProviderFactory factory, bool includeQueryTraceOn9481Flag)
            : base(connection, factory)
        {
            IncludeQueryTraceOn9481Flag = includeQueryTraceOn9481Flag;
        }

        private string IncludeQueryTraceOn9481()
        {
            if (IncludeQueryTraceOn9481Flag)
                return @" 
OPTION (QUERYTRACEON 9481)";
            return string.Empty;
        }

        public override Tables ReadSchema(Regex tableFilterExclude, Regex columnFilterExclude, bool useCamelCase, bool prependSchemaName, bool includeComments, ExtendedPropertyCommentsStyle includeExtendedPropertyComments, Func<string, string, string> tableRename, string schemaNameFilter, Func<Column, Table, Column> updateColumn)
        {
            var result = new Tables();
            if (Cmd == null)
                return result;

            Cmd.CommandText = TableSQL + IncludeQueryTraceOn9481();
            if (Cmd.GetType().Name == "SqlCeCommand")
                Cmd.CommandText = TableSQLCE;
            else
                Cmd.CommandTimeout = 600;

            using (DbDataReader rdr = Cmd.ExecuteReader())
            {
                var rxClean = new Regex("^(event|Equals|GetHashCode|GetType|ToString|repo|Save|IsNew|Insert|Update|Delete|Exists|SingleOrDefault|Single|First|FirstOrDefault|Fetch|Page|Query)$");
                var lastTable = string.Empty;
                Table table = null;
                while (rdr.Read())
                {
                    string tableName = rdr["TableName"].ToString().Trim();
                    if (tableFilterExclude != null && tableFilterExclude.IsMatch(tableName))
                        continue;

                    string schema = rdr["SchemaName"].ToString().Trim();
                    if (schemaNameFilter != null && !schema.Equals(schemaNameFilter, StringComparison.CurrentCultureIgnoreCase))
                        continue;

                    if (lastTable != tableName || table == null)
                    {
                        // The data from the database is not sorted
                        table = result.Find(x => x.Name == tableName && x.Schema == schema);
                        if (table == null)
                        {
                            table = new Table
                            {
                                Name = tableName,
                                Schema = schema,
                                IsView = String.Compare(rdr["TableType"].ToString().Trim(), "View", StringComparison.OrdinalIgnoreCase) == 0,

                                // Will be set later
                                HasForeignKey = false,
                                HasNullableColumns = false
                            };

                            tableName = tableRename(tableName, schema);
                            CodeFirstTools.SchemaName = schema;
                            table.CleanName = CodeFirstTools.CleanUp(tableName);
                            table.ClassName = Inflector.MakeSingular(table.CleanName);
                            string singular = Inflector.MakeSingular(tableName);
                            table.NameHumanCase = (useCamelCase ? Inflector.ToTitleCase(singular) : singular).Replace(" ", "").Replace("$", "");
                            //if ((string.Compare(table.Schema, "dbo", StringComparison.OrdinalIgnoreCase) != 0) && prependSchemaName)
                            //    table.NameHumanCase = table.Schema + "_" + table.NameHumanCase;

                            // Check for table or C# name clashes
                            if (CodeFirstTools.ReservedKeywords.Contains(table.NameHumanCase) ||
                                (useCamelCase && result.Find(x => x.NameHumanCase == table.NameHumanCase) != null))
                            {
                                table.NameHumanCase += "1";
                            }

                            result.Add(table);
                        }
                    }

                    var col = CreateColumn(rdr, rxClean, table, useCamelCase, columnFilterExclude, updateColumn);
                    if (col != null)
                        table.Columns.Add(col);
                }
            }

            // Check for property name clashes in columns
            foreach (Column c in result.SelectMany(tbl => tbl.Columns.Where(c => tbl.Columns.FindAll(x => x.PropertyNameHumanCase == c.PropertyNameHumanCase).Count > 1)))
            {
                c.PropertyNameHumanCase = c.PropertyName;
            }

            if (includeExtendedPropertyComments != ExtendedPropertyCommentsStyle.None)
                ReadExtendedProperties(result);

            ReadUniqueIndexes(result);

            foreach (Table tbl in result)
            {
                tbl.Columns.ForEach(x => x.SetupEntityAndConfig(includeComments, includeExtendedPropertyComments));
            }

            return result;
        }

        public override List<ForeignKey> ReadForeignKeys(Func<string, string, string> tableRename)
        {
            var fkList = new List<ForeignKey>();
            if (Cmd == null)
                return fkList;

            Cmd.CommandText = ForeignKeySQL + IncludeQueryTraceOn9481();
            if (Cmd.GetType().Name == "SqlCeCommand")
                Cmd.CommandText = ForeignKeySQLCE;
            else
                Cmd.CommandTimeout = 600;

            using (DbDataReader rdr = Cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    string fkTableName = rdr["FK_Table"].ToString();
                    string fkSchema = rdr["fkSchema"].ToString();
                    string pkTableName = rdr["PK_Table"].ToString();
                    string pkSchema = rdr["pkSchema"].ToString();
                    string fkColumn = rdr["FK_Column"].ToString().Replace(" ", "");
                    string pkColumn = rdr["PK_Column"].ToString().Replace(" ", "");
                    string constraintName = rdr["Constraint_Name"].ToString().Replace(" ", "");
                    int ordinal = (int)rdr["ORDINAL_POSITION"];
                    int cascade = decimal.ToInt32(decimal.Parse(rdr["CASCADE"].ToString()));

                    string fkTableNameFiltered = tableRename(fkTableName, fkSchema);
                    string pkTableNameFiltered = tableRename(pkTableName, pkSchema);

                    fkList.Add(new ForeignKey(fkTableName, fkSchema, pkTableName, pkSchema, fkColumn, pkColumn, constraintName, fkTableNameFiltered, pkTableNameFiltered, ordinal,cascade));
                }
            }

            return fkList;
        }

        // When a table has no primary keys, all the NOT NULL columns are set as being the primary key.
        // This function reads the unique indexes for a table, and correctly sets the columns being used as primary keys.
        public override void ReadUniqueIndexes(Tables tables)
        {
            if (Cmd == null)
                return;

            if (Cmd.GetType().Name == "SqlCeCommand")
                return;

            Cmd.CommandText = UniqueIndexSQL + IncludeQueryTraceOn9481();

            var list = new List<UniqueIndex>();
            using (DbDataReader rdr = Cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    var uniqueIndex = new UniqueIndex();

                    uniqueIndex.Schema = rdr["TableSchema"].ToString().Trim();
                    uniqueIndex.TableName = rdr["TableName"].ToString().Trim();
                    uniqueIndex.IndexName = rdr["IndexName"].ToString().Trim();
                    uniqueIndex.KeyOrdinal = (byte)rdr["KeyOrdinal"];
                    uniqueIndex.Column = rdr["ColumnName"].ToString().Trim();
                    uniqueIndex.ColumnCount = (int)rdr["ColumnCount"];

                    list.Add(uniqueIndex);
                }
            }

            Table t = null;
            var indexes = list
                .Select(x => new { x.Schema, x.TableName, x.IndexName })
                .Distinct()
                .OrderBy(o => o.Schema)
                .ThenBy(o => o.TableName)
                .ThenBy(o => o.IndexName);

            foreach (var index in indexes)
            {
                if (t == null || t.Name != index.TableName || t.Schema != index.Schema)
                    t = tables.Find(x => x.Name == index.TableName && x.Schema == index.Schema);

                if (t != null && !t.PrimaryKeys.Any())
                {
                    // Table has no primary keys
                    var uniqueIndexKeys =
                        list.Where(x => x.Schema == index.Schema && x.TableName == index.TableName && x.IndexName == index.IndexName)
                            .Select(x => new { x.IndexName, x.KeyOrdinal, x.Column, x.ColumnCount })
                            .OrderBy(o => o.ColumnCount)
                            .ThenBy(o => o.IndexName);

                    // Process only the first index with the lowest unique column count
                    string indexName = null;
                    foreach (var key in uniqueIndexKeys)
                    {
                        if (indexName == null)
                            indexName = key.IndexName;

                        if (indexName != key.IndexName)
                            break;  // First unique index with lowest column count has been processed, exit.

                        var col = t.Columns.Find(x => x.Name == key.Column);
                        if (col != null && !col.IsNullable && !col.Hidden)
                        {
                            col.IsPrimaryKey = true;
                            col.IsPrimaryKeyViaUniqueIndex = true;
                            col.UniqueIndexName = indexName;
                        }
                    }
                }
            }
        }

        public override void ReadExtendedProperties(Tables tables)
        {
            if (Cmd == null)
                return;

            Cmd.CommandText = ExtendedPropertySQL + IncludeQueryTraceOn9481();

            if (Cmd.GetType().Name == "SqlCeCommand")
            {
                Cmd.CommandText = ExtendedPropertyTableExistsSQLCE;
                var obj = Cmd.ExecuteScalar();
                if (obj == null)
                    return;

                Cmd.CommandText = ExtendedPropertySQLCE;
            }

            using (DbDataReader rdr = Cmd.ExecuteReader())
            {
                Table t = null;
                while (rdr.Read())
                {
                    string schema = rdr["schema"].ToString().Trim();
                    string tableName = rdr["table"].ToString().Trim();
                    string column = rdr["column"].ToString().Trim();
                    string extendedProperty = rdr["property"].ToString().Trim();

                    if (extendedProperty == string.Empty)
                        continue;

                    if (t == null || t.Name != tableName || t.Schema != schema)
                        t = tables.Find(x => x.Name == tableName && x.Schema == schema);

                    if (t != null)
                    {
                        var col = t.Columns.Find(x => x.Name == column);
                        if (col != null)
                        {
                            extendedProperty = extendedProperty.Replace("\n", " ").Replace("\r", " ");
                            col.ExtendedProperty = extendedProperty;
                        }
                    }
                }
            }
        }

        public override List<StoredProcedure> ReadStoredProcs(Regex spFilterExclude, bool useCamelCase, bool prependSchemaName, Func<string, string, string> StoredProcedureRename, string schemaNameFilter)
        {
            var result = new List<StoredProcedure>();
            if (Cmd == null)
                return result;

            Cmd.CommandText = StoredProcedureSQL + IncludeQueryTraceOn9481();
            if (Cmd.GetType().Name == "SqlCeCommand")
                return result;
            Cmd.CommandTimeout = 600;

            using (DbDataReader rdr = Cmd.ExecuteReader())
            {
                var lastSp = string.Empty;
                StoredProcedure sp = null;
                while (rdr.Read())
                {
                    string spName = rdr["SPECIFIC_NAME"].ToString().Trim();
                    if (spFilterExclude != null && spFilterExclude.IsMatch(spName))
                        continue;

                    string schema = rdr["SPECIFIC_SCHEMA"].ToString().Trim();
                    if (schemaNameFilter != null && !schema.Equals(schemaNameFilter, StringComparison.CurrentCultureIgnoreCase))
                        continue;

                    if (lastSp != spName || sp == null)
                    {
                        lastSp = spName;
                        sp = new StoredProcedure
                        {
                            Name = spName,
                            NameHumanCase = (useCamelCase ? Inflector.ToTitleCase(spName) : spName).Replace(" ", "").Replace("$", ""),
                            Schema = schema
                        };
                        if ((string.Compare(schema, "dbo", StringComparison.OrdinalIgnoreCase) != 0) && prependSchemaName)
                            sp.NameHumanCase = schema + "_" + sp.NameHumanCase;

                        sp.NameHumanCase = StoredProcedureRename(sp.NameHumanCase, schema);

                        result.Add(sp);
                    }

                    string typename = rdr["DATA_TYPE"].ToString().Trim().ToLower();
                    var scale = (int)rdr["NUMERIC_SCALE"];
                    var precision = (int)((byte)rdr["NUMERIC_PRECISION"]);
                    var parameterMode = rdr["PARAMETER_MODE"].ToString().Trim().ToUpper();

                    var param = new StoredProcedureParameter
                    {
                        Ordinal = (int)rdr["ORDINAL_POSITION"],
                        Mode = (parameterMode == "IN") ? StoredProcedureParameterMode.In : StoredProcedureParameterMode.InOut,
                        Name = rdr["PARAMETER_NAME"].ToString().Trim(),
                        SqlDbType = GetSqlDbType(typename, scale, precision),
                        PropertyType = GetPropertyType(typename, scale, precision),
                        DateTimePrecision = (Int16)rdr["DATETIME_PRECISION"],
                        MaxLength = (int)rdr["CHARACTER_MAXIMUM_LENGTH"],
                        Precision = precision,
                        Scale = scale
                    };

                    var clean = CodeFirstTools.CleanUp(param.Name.Replace("@", ""));
                    param.NameHumanCase = Inflector.MakeInitialLower((useCamelCase ? Inflector.ToTitleCase(clean) : clean).Replace(" ", ""));

                    if (CodeFirstTools.ReservedKeywords.Contains(param.NameHumanCase))
                    {
                        param.NameHumanCase = "@" + param.NameHumanCase;
                    }

                    sp.Parameters.Add(param);
                }
            }
            return result;
        }

        public void ReadStoredProcReturnObject(string connectionString, StoredProcedure proc)
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append("SET FMTONLY OFF; SET FMTONLY ON; \n");

                sb.Append(String.Format("exec [{0}].[{1}] ", proc.Schema, proc.Name));

                var count = 1;
                int paramCount = proc.Parameters.Count;
                foreach (var param in proc.Parameters)
                {
                    sb.Append(String.Format("{0}=null", param.Name));
                    if (count < paramCount)
                        sb.Append(", ");
                    count++;
                }

                sb.Append("\n SET FMTONLY OFF; SET FMTONLY OFF;");

                var ds = new DataSet();
                using (var sqlConnection = new SqlConnection(connectionString))
                {
                    using (var sqlAdapter = new SqlDataAdapter(sb.ToString(), sqlConnection))
                    {
                        if (sqlConnection.State != ConnectionState.Open)
                            sqlConnection.Open();
                        sqlAdapter.SelectCommand.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo);
                        sqlConnection.Close();
                        sqlAdapter.FillSchema(ds, SchemaType.Source, "MyTable");
                    }
                }

                if (ds.Tables.Count < 1)
                    return;

                proc.ReturnColumns = ds.Tables[0].Columns.Cast<DataColumn>().ToList();
                // Tidy up
                foreach (var col in proc.ReturnColumns)
                {
                    col.ColumnName = Regex.Replace(col.ColumnName, @"[^A-Za-z0-9\s]*", "");
                    col.ColumnName = Inflector.ToTitleCase(col.ColumnName).Replace(" ", "");
                }
                foreach (var p in proc.Parameters)
                {
                    p.NameHumanCase = Regex.Replace(p.NameHumanCase, @"[^A-Za-z0-9\s]*", "");
                    //p.NameHumanCase = Inflector.ToTitleCase(p.NameHumanCase).Replace(" ", "");
                }
            }
            catch (Exception)
            {
                // Stored procedure does not have a return type
            }
        }

        public override void ProcessForeignKeys(List<ForeignKey> fkList, Tables tables, bool useCamelCase, bool prependSchemaName, string collectionType, bool checkForFkNameClashes, bool includeComments)
        {
            var constraints = fkList.Select(x => x.ConstraintName).Distinct();
            foreach (var constraint in constraints)
            {
                var localConstraint = constraint;
                var foreignKeys = fkList.Where(x => x.ConstraintName == localConstraint).ToList();
                var casete = fkList.Where(x => x.ConstraintName == localConstraint).Select(t => t.Cascade).First();
                var foreignKey = foreignKeys.First();

                Table fkTable = tables.GetTable(foreignKey.FkTableName, foreignKey.FkSchema);
                if (fkTable == null || fkTable.IsMapping || !fkTable.HasForeignKey)
                    continue;

                Table pkTable = tables.GetTable(foreignKey.PkTableName, foreignKey.PkSchema);
                if (pkTable == null || pkTable.IsMapping)
                    continue;

                var fkCols = foreignKeys.Select(x => new
                {
                    fkOrdinal = x.Ordinal,
                    col = fkTable.Columns.Find(n => n.PropertyName == x.FkColumn || n.PropertyName == x.FkColumn + "_")
                })
                    .Where(x => x != null)
                    .ToList();

                var pkCols = foreignKeys.Select(x => pkTable.Columns.Find(n => n.PropertyName == x.PkColumn)).Where(x => x != null).OrderBy(o => o.Ordinal).ToList();
                if (!pkCols.Any())
                    pkCols = foreignKeys.Select(x => pkTable.Columns.Find(n => n.PropertyName == x.PkColumn + "_")).Where(x => x != null).OrderBy(o => o.Ordinal).ToList();

                var fkCol = fkCols.First();
                var pkCol = pkCols.First();

                if (!pkCol.IsPrimaryKey)
                    continue;

                var relationship = CodeFirstTools.CalcRelationship(pkTable, fkTable, fkCol.col, pkCol);

                string pkTableHumanCase = foreignKey.PkTableHumanCase(useCamelCase, prependSchemaName);
                string pkPropName = fkTable.GetUniqueColumnPropertyName(pkTableHumanCase, foreignKey, useCamelCase, checkForFkNameClashes, true);
                bool fkMakePropNameSingular = (relationship == Relationship.OneToOne);
                string fkPropName = pkTable.GetUniqueColumnPropertyName(fkTable.NameHumanCase, foreignKey, useCamelCase, checkForFkNameClashes, fkMakePropNameSingular);

                fkCol.col.EntityFk = string.Format("public virtual {0} {1} {2}{3}", pkTable.Name, pkPropName, "{ get; set; }", includeComments ? " // " + foreignKey.ConstraintName : string.Empty);

                string manyToManyMapping;
                if (foreignKeys.Count > 1)
                    manyToManyMapping = string.Format("c => new {{ {0} }}", string.Join(", ", fkCols.OrderBy(o => o.fkOrdinal).Select(x => "c." + x.col.PropertyNameHumanCase).ToArray()));
                else
                    manyToManyMapping = string.Format("c => c.{0}", fkCol.col.PropertyNameHumanCase);

                fkCol.col.ConfigFk = string.Format("            {0};{1}", GetRelationship(relationship, fkCol.col, pkCol, pkPropName, fkPropName, manyToManyMapping, casete), includeComments ? " // " + foreignKey.ConstraintName : string.Empty);
               
                pkTable.AddReverseNavigation(relationship, pkTableHumanCase, fkTable, fkPropName, string.Format("{0}.{1}", fkTable.Name, foreignKey.ConstraintName), collectionType, includeComments);
            }
        }

        public override void IdentifyForeignKeys(List<ForeignKey> fkList, Tables tables)
        {
            foreach (var foreignKey in fkList)
            {
                Table fkTable = tables.GetTable(foreignKey.FkTableName, foreignKey.FkSchema);
                if (fkTable == null)
                    continue;   // Could be filtered out

                Table pkTable = tables.GetTable(foreignKey.PkTableName, foreignKey.PkSchema);
                if (pkTable == null)
                    continue;   // Could be filtered out

                Column fkCol = fkTable.Columns.Find(n => n.PropertyName == foreignKey.FkColumn);
                if (fkCol == null)
                    continue;   // Could not find fk column

                Column pkCol = pkTable.Columns.Find(n => n.PropertyName == foreignKey.PkColumn);
                if (pkCol == null)
                    continue;   // Could not find pk column

                fkTable.HasForeignKey = true;
            }
        }

        private static string GetRelationship(Relationship relationship, Column fkCol, Column pkCol, string pkPropName, string fkPropName, string manyToManyMapping, int casete)
        {
            string temp = string.Empty;
            if (casete == 1)
            {
                temp = ".WillCascadeOnDelete(true)";
            }
            else
            {
                temp = ".WillCascadeOnDelete(false)";
            }
            return string.Format("Has{0}(a => a.{1}){2}{3}", GetHasMethod(relationship, fkCol, pkCol), pkPropName, GetWithMethod(relationship, fkCol, fkPropName, manyToManyMapping), temp);
        }

        // HasOptional
        // HasRequired
        // HasMany
        private static string GetHasMethod(Relationship relationship, Column fkCol, Column pkCol)
        {
            bool withMany = false;
            switch (relationship)
            {
                case Relationship.ManyToOne:
                case Relationship.ManyToMany:
                    withMany = true;
                    break;
            }

            if (withMany || pkCol.IsPrimaryKey)
                return fkCol.IsNullable ? "Optional" : "Required";

            return "Many";
        }

        // WithOptional
        // WithRequired
        // WithMany
        // WithRequiredPrincipal
        // WithRequiredDependent
        private static string GetWithMethod(Relationship relationship, Column fkCol, string fkPropName, string manyToManyMapping)
        {
            switch (relationship)
            {
                case Relationship.OneToOne:
                    return string.Format(".WithOptional(b => b.{0})", fkPropName);

                case Relationship.OneToMany:
                    return string.Format(".WithRequiredDependent(b => b.{0})", fkPropName);

                case Relationship.ManyToOne:
                    if (!fkCol.Hidden)
                        return string.Format(".WithMany(b => b.{0}).HasForeignKey(c => c.{1})", fkPropName, fkCol.PropertyName);   // Foreign Key Association
                    return string.Format(".WithMany(b => b.{0}).Map(c => c.MapKey(\"{1}\"))", fkPropName, fkCol.Name);  // Independent Association

                case Relationship.ManyToMany:
                    return string.Format(".WithMany(b => b.{0}).HasForeignKey({1})", fkPropName, manyToManyMapping);

                default:
                    throw new ArgumentOutOfRangeException("relationship");
            }
        }

        private static Column CreateColumn(IDataRecord rdr, Regex rxClean, Table table, bool useCamelCase, Regex columnFilterExclude, Func<Column, Table, Column> updateColumn)
        {
            if (rdr == null)
                throw new ArgumentNullException("rdr");

            string typename = rdr["TypeName"].ToString().Trim().ToLower();
            var scale = (int)rdr["Scale"];
            var precision = (int)rdr["Precision"];

            var col = new Column
            {
                Name = rdr["ColumnName"].ToString().Trim(),
                TypeName = typename,
                PropertyType = GetPropertyType(typename, scale, precision),
                MaxLength = (int)rdr["MaxLength"],
                Precision = precision,
                Default = rdr["Default"].ToString().Trim(),
                DateTimePrecision = (int)rdr["DateTimePrecision"],
                Scale = scale,
                Ordinal = (int)rdr["Ordinal"],
                IsIdentity = rdr["IsIdentity"].ToString().Trim().ToLower() == "true",
                IsNullable = rdr["IsNullable"].ToString().Trim().ToLower() == "true",
                IsStoreGenerated = rdr["IsStoreGenerated"].ToString().Trim().ToLower() == "true",
                IsPrimaryKey = rdr["PrimaryKey"].ToString().Trim().ToLower() == "true",
                PrimaryKeyOrdinal = (int)rdr["PrimaryKeyOrdinal"],
                IsForeignKey = rdr["IsForeignKey"].ToString().Trim().ToLower() == "true"
            };

            if (columnFilterExclude != null && !col.IsPrimaryKey && columnFilterExclude.IsMatch(col.Name))
                col.Hidden = true;

            col.IsFixedLength = (typename == "char" || typename == "nchar");
            col.IsUnicode = !(typename == "char" || typename == "varchar" || typename == "text");

            col.IsRowVersion = col.IsStoreGenerated && !col.IsNullable && typename == "timestamp";
            if (col.IsRowVersion)
                col.MaxLength = 8;

            col.CleanUpDefault();
            col.PropertyName = CodeFirstTools.CleanUp(col.Name);
            col.PropertyName = rxClean.Replace(col.PropertyName, "_$1");

            // Make sure property name doesn't clash with class name
            if (col.PropertyName == table.NameHumanCase)
                col.PropertyName = col.PropertyName + "_";

            col.PropertyNameHumanCase = (useCamelCase ? Inflector.ToTitleCase(col.PropertyName) : col.PropertyName).Replace(" ", "");
            if (col.PropertyNameHumanCase == string.Empty)
                col.PropertyNameHumanCase = col.PropertyName;

            // Make sure property name doesn't clash with class name
            if (col.PropertyNameHumanCase == table.NameHumanCase)
                col.PropertyNameHumanCase = col.PropertyNameHumanCase + "_";

            if (char.IsDigit(col.PropertyNameHumanCase[0]))
                col.PropertyNameHumanCase = "_" + col.PropertyNameHumanCase;

            if (CodeFirstTools.CheckNullable(col) != string.Empty)
                table.HasNullableColumns = true;

            col = updateColumn(col, table);

            // If PropertyType is empty, return null. Most likely ignoring a column due to legacy (such as OData not supporting spatial types)
            if (string.IsNullOrEmpty(col.PropertyType))
                return null;

            return col;
        }

        private static string GetSqlDbType(string sqlType, int scale, int precision)
        {
            string sysType = "VarChar";
            switch (sqlType)
            {
                case "bigint":
                    sysType = "BigInt";
                    break;

                case "binary":
                    sysType = "Binary";
                    break;

                case "bit":
                    sysType = "Bit";
                    break;

                case "char":
                    sysType = "Char";
                    break;

                case "datetime":
                    sysType = "DateTime";
                    break;

                case "decimal":
                    sysType = "Decimal";
                    break;

                case "float":
                    sysType = "Float";
                    break;

                case "image":
                    sysType = "Image";
                    break;

                case "int":
                    sysType = "Int";
                    break;

                case "money":
                    sysType = "Money";
                    break;

                case "nchar":
                    sysType = "NChar";
                    break;

                case "ntext":
                    sysType = "NText";
                    break;

                case "nvarchar":
                    sysType = "NVarChar";
                    break;

                case "real":
                    sysType = "Real";
                    break;

                case "uniqueidentifier":
                    sysType = "UniqueIdentifier";
                    break;

                case "smalldatetime":
                    sysType = "SmallDateTime";
                    break;

                case "smallint":
                    sysType = "SmallInt";
                    break;

                case "smallmoney":
                    sysType = "SmallMoney";
                    break;

                case "text":
                    sysType = "Text";
                    break;

                case "timestamp":
                    sysType = "Timestamp";
                    break;

                case "tinyint":
                    sysType = "TinyInt";
                    break;

                case "varbinary":
                    sysType = "VarBinary";
                    break;

                case "varchar":
                    sysType = "VarChar";
                    break;

                case "variant":
                    sysType = "Variant";
                    break;

                case "xml":
                    sysType = "Xml";
                    break;

                case "udt":
                    sysType = "Udt";
                    break;

                case "structured":
                    sysType = "Structured";
                    break;

                case "date":
                    sysType = "Date";
                    break;

                case "time":
                    sysType = "Time";
                    break;

                case "datetime2":
                    sysType = "DateTime2";
                    break;

                case "datetimeoffset":
                    sysType = "DateTimeOffset";
                    break;
            }
            return sysType;
        }

        private static string GetPropertyType(string sqlType, int scale, int precision)
        {
            string sysType = "string";
            switch (sqlType)
            {
                case "bigint":
                    sysType = "long";
                    break;
                case "smallint":
                    sysType = "short";
                    break;
                case "int":
                    sysType = "int";
                    break;
                case "uniqueidentifier":
                    sysType = "Guid";
                    break;
                case "smalldatetime":
                case "datetime":
                case "datetime2":
                case "date":
                    sysType = "DateTime";
                    break;
                case "datetimeoffset":
                    sysType = "DateTimeOffset";
                    break;
                case "time":
                    sysType = "TimeSpan";
                    break;
                case "float":
                    sysType = "double";
                    break;
                case "real":
                    sysType = "float";
                    break;
                case "numeric":
                case "smallmoney":
                case "decimal":
                case "money":
                    sysType = "decimal";
                    break;
                case "tinyint":
                    sysType = "byte";
                    break;
                case "bit":
                    sysType = "bool";
                    break;
                case "image":
                case "binary":
                case "varbinary":
                case "varbinary(max)":
                case "timestamp":
                    sysType = "byte[]";
                    break;
                case "geography":
                    if (CodeFirstTools.DisableGeographyTypes)
                        sysType = "";
                    else
                        sysType = "System.Data.Entity.Spatial.DbGeography";
                    break;
                case "geometry":
                    if (CodeFirstTools.DisableGeographyTypes)
                        sysType = "";
                    else
                        sysType = "System.Data.Entity.Spatial.DbGeometry";
                    break;
            }
            return sysType;
        }
    }

}

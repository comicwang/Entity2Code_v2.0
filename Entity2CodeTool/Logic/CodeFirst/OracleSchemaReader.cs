using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool
{
    /// <summary>
    /// Oracle数据库架构读取类
    /// </summary>
    public class OracleSchemaReader : SchemaReader
    {
        private const string TableSQL = @" select (select distinct(dt.OWNER) from user_constraints dt where dt.constraint_type='P' and dt.TABLE_NAME=t.TABLE_NAME) SchemaName,t.TABLE_NAME TableName,'BASE TABLE' TableType,c.COLUMN_ID Ordinal,
       c.COLUMN_NAME ColumnName,CASE WHEN c.NULLABLE = 'Y' THEN 1 ELSE 0 END AS IsNullable,c.DATA_TYPE TypeName, 
		    case when c.DATA_TYPE like '%CLOB%' then 0 when c.DATA_TYPE='RAW' then NVL(c.DATA_LENGTH ,0) else NVL(c.CHAR_COL_DECL_LENGTH ,0) end MaxLength ,NVL(c.DATA_PRECISION,0) Precision,NVL(c.DEFAULT_LENGTH, '') ""Default"", 
			 case when c.DATA_TYPE like '%TIMESTAMP%' then c.DATA_SCALE else 0 end DateTimePrecision, case when c.DATA_TYPE like '%TIMESTAMP%' then 0 else NVL(c.DATA_SCALE,0) end Scale,0 IsIdentity,0 IsStoreGenerated,CASE WHEN pk.position IS NULL THEN 0 ELSE 1 END  PrimaryKey,
			 NVL(pk.position,0) PrimaryKeyOrdinal,CASE WHEN fk.r_constraint_name IS NULL THEN 0 ELSE 1 END  IsForeignKey from USER_TABLES t 
			 inner join USER_TAB_COLUMNS c on t.TABLE_NAME=c.TABLE_NAME left outer join  (select col.table_name,col.column_name,col.position 
			 from user_constraints con, user_cons_columns col where con.constraint_name = col.constraint_name and con.constraint_type='P' 
			 and con.table_name=col.table_name) pk on c.TABLE_NAME=pk.table_name and c.COLUMN_NAME=pk.column_name 
			 LEFT OUTER JOIN (select DISTINCT(colf.column_name),r.table_name,r.r_constraint_name from user_constraints conf,user_cons_columns colf,
			  (select t2.table_name,t2.column_name,t1.r_constraint_name from user_constraints t1,user_cons_columns t2  where t1.constraint_name=t2.constraint_name 
			  and t1.table_name=t2.table_name) r where conf.constraint_name=colf.constraint_name and conf.r_constraint_name=r.r_constraint_name) fk on 
			  c.TABLE_NAME=fk.table_name and c.COLUMN_NAME=fk.column_name";

        private const string ForeignKeySQL = @"
select distinct(col.column_name) FK_Column,r.fktb FK_Table,r.Table_Name PK_Table,r.column_name PK_Column,
r.constraint_name Constraint_Name,r.column_name PrimaryKey,r.position ORDINAL_POSITION, 
case when con.delete_rule='CASCADE' then 1 else 0 end CASCADE,r.Fkschema,r.Pkschema from user_constraints con,user_cons_columns col,(select FT.table_name FKTB,PT.table_name,PT.column_name,FT.r_constraint_name,FT.constraint_name,PT.position,FT.owner FKSchema,PT.owner PKSchema  from user_constraints FT,user_cons_columns PT where FT.r_constraint_name=PT.constraint_name and FT.constraint_type='R') r
where con.constraint_name=col.constraint_name and con.r_constraint_name=r.r_constraint_name and col.constraint_name=r.constraint_name";

        private const string ExtendedPropertySQL = @"
select t.owner ""schema"",c.table_name ""table"",c.column_name ""column"",c.comments ""property"" from user_col_comments c inner join user_constraints t on c.table_name=t.TABLE_NAME where c.comments is not null  and t.constraint_type='P' order by c.table_name
";

        private string StoredProcedureSQL = string.Empty;

        private const string UniqueIndexSQL = @"
select ui.table_owner TableSchema,i.TABLE_NAME TableName,i.INDEX_NAME IndexName,i.COLUMN_POSITION KeyOrdinal,
i.COLUMN_NAME ColumnName,(select count(1) from user_ind_columns ci where ci.INDEX_NAME=i.INDEX_NAME and ci.TABLE_NAME=i.TABLE_NAME) 
ColumnCount from user_ind_columns i,user_indexes ui 
where i.INDEX_NAME=ui.index_name and i.TABLE_NAME=ui.table_name and ui.uniqueness='UNIQUE' and ui.index_type='NORMAL'  
order by i.TABLE_NAME asc,i.COLUMN_POSITION asc
";

        public OracleSchemaReader(DbConnection connection, DbProviderFactory factory)
            : base(connection, factory)
        {

        }

        public override Tables ReadSchema(Regex tableFilterExclude, Regex columnFilterExclude, bool useCamelCase, bool prependSchemaName, bool includeComments, ExtendedPropertyCommentsStyle includeExtendedPropertyComments, Func<string, string, string> tableRename, string schemaNameFilter, Func<Column, Table, Column> updateColumn)
        {
            var result = new Tables();
            if (Cmd == null)
                return result;

            Cmd.CommandText = TableSQL;
            if (Cmd.GetType().Name == "SqlCeCommand")
                Cmd.CommandText = string.Empty;
            else
                Cmd.CommandTimeout = 600;

            using (DbDataReader rdr = Cmd.ExecuteReader())
            {
                var rxClean = new Regex("^(event|Equals|GetHashCode|GetType|ToString|repo|Save|IsNew|Insert|Update|Delete|Exists|SingleOrDefault|Single|First|FirstOrDefault|Fetch|Page|Query)$");
                var lastTable = string.Empty;
                Table table = null;
                while (rdr.Read())
                {
                    string tableName = rdr["TABLENAME"].ToString().Trim();
                    if (tableFilterExclude != null && tableFilterExclude.IsMatch(tableName))
                        continue;

                    string schema = rdr["SCHEMANAME"].ToString().Trim();
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
                                IsView = String.Compare(rdr["TABLETYPE"].ToString().Trim(), "View", StringComparison.OrdinalIgnoreCase) == 0,

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

            Cmd.CommandText = ForeignKeySQL;
            if (Cmd.GetType().Name == "SqlCeCommand")
                Cmd.CommandText = string.Empty;
            else
                Cmd.CommandTimeout = 600;

            using (DbDataReader rdr = Cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    string fkTableName = rdr["FK_TABLE"].ToString();
                    string fkSchema = rdr["FKSCHEMA"].ToString();
                    string pkTableName = rdr["PK_TABLE"].ToString();
                    string pkSchema = rdr["PKSCHEMA"].ToString();
                    string fkColumn = rdr["FK_COLUMN"].ToString().Replace(" ", "");
                    string pkColumn = rdr["PK_COLUMN"].ToString().Replace(" ", "");
                    string constraintName = rdr["CONSTRAINT_NAME"].ToString().Replace(" ", "");
                    int ordinal = decimal.ToInt32(decimal.Parse(rdr["ORDINAL_POSITION"].ToString()));
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

            Cmd.CommandText = UniqueIndexSQL;

            var list = new List<UniqueIndex>();
            using (DbDataReader rdr = Cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    var uniqueIndex = new UniqueIndex();

                    uniqueIndex.Schema = rdr["TABLESCHEMA"].ToString().Trim();
                    uniqueIndex.TableName = rdr["TABLENAME"].ToString().Trim();
                    uniqueIndex.IndexName = rdr["INDEXNAME"].ToString().Trim();
                    uniqueIndex.KeyOrdinal =decimal.ToByte(decimal.Parse(rdr["KEYORDINAL"].ToString()));
                    uniqueIndex.Column = rdr["COLUMNNAME"].ToString().Trim();
                    uniqueIndex.ColumnCount = decimal.ToInt32(decimal.Parse(rdr["COLUMNCOUNT"].ToString()));

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

            Cmd.CommandText = ExtendedPropertySQL;

            using (DbDataReader rdr = Cmd.ExecuteReader())
            {
                Table t = null;
                while (rdr.Read())
                {
                    string schema = rdr["SCHEMA"].ToString().Trim();
                    string tableName = rdr["TABLE"].ToString().Trim();
                    string column = rdr["COLUMN"].ToString().Trim();
                    string extendedProperty = rdr["PROPERTY"].ToString().Trim();

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
            throw new Exception();
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
                var casete = fkList.Where(x => x.ConstraintName == localConstraint).Select(t=>t.Cascade).First();
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
                    manyToManyMapping = string.Format("c => new {{ {0} }}", string.Join(", ", fkCols.OrderBy(o => o.fkOrdinal).Select(x => "c." + x.col.PropertyName).ToArray()));
                else
                    manyToManyMapping = string.Format("c => c.{0}", fkCol.col.PropertyName);

                fkCol.col.ConfigFk = string.Format("           {0};{1}", GetRelationship(relationship, fkCol.col, pkCol, pkPropName, fkPropName, manyToManyMapping,casete), includeComments ? " // " + foreignKey.ConstraintName : string.Empty);
               
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
            try
            {
                if (rdr == null)
                    throw new ArgumentNullException("rdr");

                string typename = rdr["TYPENAME"].ToString().Trim().ToLower();
                var scale = decimal.ToInt32(decimal.Parse(rdr["SCALE"].ToString()));
                var precision = decimal.ToInt32(decimal.Parse(rdr["PRECISION"].ToString()));
                if (typename.ToLower() == "number" && scale == 0 && precision == 0)
                {
                    scale = int.MinValue;
                    precision = 38;
                }
                var col = new Column
                {
                    Name = rdr["COLUMNNAME"].ToString().Trim(),
                    TypeName = typename,
                    PropertyType = GetPropertyType(typename,ref scale, precision),
                    MaxLength = decimal.ToInt32(decimal.Parse(rdr["MAXLENGTH"].ToString())),
                    Precision = precision,
                    Default = rdr["Default"].ToString().Trim(),
                    DateTimePrecision = decimal.ToInt32(decimal.Parse(rdr["DATETIMEPRECISION"].ToString())),
                    Scale = scale,
                    Ordinal = decimal.ToInt32(decimal.Parse(rdr["ORDINAL"].ToString())),
                    IsIdentity = rdr["ISIDENTITY"].ToString().Trim().ToLower() == "1",
                    IsNullable = rdr["ISNULLABLE"].ToString().Trim().ToLower() == "1",
                    IsStoreGenerated = rdr["ISSTOREGENERATED"].ToString().Trim().ToLower() == "1",
                    IsPrimaryKey = rdr["PRIMARYKEY"].ToString().Trim().ToLower() == "1",
                    PrimaryKeyOrdinal = decimal.ToInt32(decimal.Parse(rdr["PRIMARYKEYORDINAL"].ToString())),
                    IsForeignKey = rdr["ISFOREIGNKEY"].ToString().Trim().ToLower() == "1"
                };
                Assembly asm = Assembly.Load("EntityFramework");
                Version v = new Version("5.0.0.0");

                if (typename.ToLower() == "float" && asm.GetName().Version > v)
                {
                    if (precision == 126)
                    {
                        col.Precision = 38;
                    }
                    else if (precision == 63)
                    {
                        col.Precision = 19;
                    }
                }
               
                if (columnFilterExclude != null && !col.IsPrimaryKey && columnFilterExclude.IsMatch(col.Name))
                    col.Hidden = true;

                col.IsFixedLength = (typename.ToLower() == "char" || typename.ToLower() == "nchar");
                col.IsUnicode = !(typename.ToLower() == "char" || typename.ToLower() == "varchar2" || typename.ToLower() == "clob" || typename.ToLower() == "long");

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
            catch (Exception ee)
            {
                throw ee;
            }
        }

        private static string GetSqlDbType(string sqlType, int scale, int precision)
        {
            string sysType = "VarChar";
            switch (sqlType.ToLower())
            {
                case "char":
                    sysType = "Char";
                    break;

                case "date":
                    sysType = "Date";
                    break;

                case "nvarchar2":
                    sysType = "NVarChar2";
                    break;
                case "varchar2":
                    sysType = "VarChar2";
                    break;
                case "long":
                    sysType = "Long";
                    break;
                case "timestamp":
                    sysType = "Timestamp";
                    break;
                case "blob":
                    sysType = "Blob";
                    break;
                case "raw":
                    sysType = "Raw";
                    break;
                case "clob":
                    sysType = "Clob";
                    break;
                case "nclob":
                    sysType = "NColb";
                    break;
                case "number":
                    sysType = "Number";
                    break;
                case "binary_double":
                    sysType = "Binary_double";
                    break;
                case "binary_float":
                    sysType = "Binary_float";
                    break;
                case "timestamp with time zone":
                    sysType = "Timestamp With Time Zone";
                    break;
            }
            return sysType;
        }

        private static string GetPropertyType(string sqlType,ref int scale, int precision)
        {
            string sysType = "string";
            switch (sqlType.ToLower())
            {
                case "char":
                    sysType = "string";
                    break;
                case "nchar":
                    sysType = "string";
                    break;
                case "date":
                    sysType = "DateTime";
                    break;
                case "nvarchar2":
                    sysType = "string";
                    break;
                case "varchar2":
                    sysType = "string";
                    break;
                case "long":
                    sysType = "string";
                    break;
                case "timestamp":
                    sysType = "DateTime";
                    break;
                case "blob":
                case "raw":
                case "bfile":
                    sysType = "byte[]";
                    break;
                case "clob":
                    sysType = "string";
                    break;
                case "nclob":
                    sysType = "string";
                    break;
                case "number":
                    if (scale != 0)
                    {
                        sysType = "decimal";
                        if (scale == int.MinValue)
                        {
                            scale = 0;
                        }
                    }
                    else
                    {
                        if (precision <= 4)
                        {
                            sysType = "short";
                        }
                        else if (precision > 4 && precision < 10)
                        {
                            sysType = "int";
                        }
                        else if (precision >= 10 && precision < 19)
                        {
                            sysType = "long";
                        }
                        else
                        {
                            sysType = "decimal";
                        }
                    }
                    break;
                case "binary_double":
                    sysType = "double";
                    break;
                case "binary_float":
                    sysType = "float";
                    break;
                case "float":
                    sysType = "decimal";
                    break;
                case "real":
                    sysType = "decimal";
                    break;
                case "timestamp with time zone":
                    sysType = "DateTimeOffset";
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
                default: 
                    if(sqlType.StartsWith("timestamp"))
                    {
                        sysType = "DateTime";
                    }
                    break;
            }
            return sysType;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.CodeFirst
{
    /// <summary>
    /// 表类
    /// </summary>
    public class Table
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name;
        /// <summary>
        /// 
        /// </summary>
        public string NameHumanCase;
        /// <summary>
        /// 
        /// </summary>
        public string Schema;
        /// <summary>
        /// 
        /// </summary>
        public string Type;
        /// <summary>
        /// 
        /// </summary>
        public string ClassName;
        /// <summary>
        /// 
        /// </summary>
        public string CleanName;
        /// <summary>
        /// 
        /// </summary>
        public bool IsMapping;
        /// <summary>
        /// 
        /// </summary>
        public bool IsView;
        /// <summary>
        /// 
        /// </summary>
        public bool HasForeignKey;
        /// <summary>
        /// 
        /// </summary>
        public bool HasNullableColumns;

        /// <summary>
        /// 
        /// </summary>
        public List<Column> Columns;
        /// <summary>
        /// 
        /// </summary>
        public List<string> ReverseNavigationProperty;
        /// <summary>
        /// 
        /// </summary>
        public List<string> MappingConfiguration;
        /// <summary>
        /// 
        /// </summary>
        public List<string> ReverseNavigationCtor;
        /// <summary>
        /// 
        /// </summary>
        public List<string> ReverseNavigationUniquePropName;
        /// <summary>
        /// 
        /// </summary>
        public List<string> ReverseNavigationUniquePropNameClashes;

        /// <summary>
        /// 
        /// </summary>
        public Table()
        {
            Columns = new List<Column>();
            ResetNavigationProperties();
            ReverseNavigationUniquePropNameClashes = new List<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetNavigationProperties()
        {
            MappingConfiguration = new List<string>();
            ReverseNavigationProperty = new List<string>();
            ReverseNavigationCtor = new List<string>();
            ReverseNavigationUniquePropName = new List<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Column> PrimaryKeys
        {
            get { return Columns.Where(x => x.IsPrimaryKey).OrderBy(x => x.Ordinal).ToList(); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string PrimaryKeyNameHumanCase()
        {
            var data = PrimaryKeys.Select(x => "x." + x.PropertyName).ToList();
            int n = data.Count();
            if (n == 0)
                return string.Empty;
            if (n == 1)
                return "x => " + data.First();
            // More than one primary key
            return string.Format("x => new {{ {0} }}", string.Join(", ", data));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public Column this[string columnName]
        {
            get { return GetColumn(columnName); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>

        public Column GetColumn(string columnName)
        {
            return Columns.SingleOrDefault(x => String.Compare(x.Name, columnName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableNameHumanCase"></param>
        /// <param name="foreignKey"></param>
        /// <param name="useCamelCase"></param>
        /// <param name="checkForFkNameClashes"></param>
        /// <param name="makeSingular"></param>
        /// <returns></returns>
        public string GetUniqueColumnPropertyName(string tableNameHumanCase, ForeignKey foreignKey, bool useCamelCase, bool checkForFkNameClashes, bool makeSingular)
        {
            if (ReverseNavigationUniquePropName.Count == 0)
            {
                ReverseNavigationUniquePropName.Add(NameHumanCase);
                ReverseNavigationUniquePropName.AddRange(Columns.Select(c => c.PropertyName));
            }

            if (!makeSingular)
                tableNameHumanCase = Inflector.MakePlural(tableNameHumanCase);

            if (checkForFkNameClashes && ReverseNavigationUniquePropName.Contains(tableNameHumanCase) && !ReverseNavigationUniquePropNameClashes.Contains(tableNameHumanCase))
                ReverseNavigationUniquePropNameClashes.Add(tableNameHumanCase); // Name clash

            // Try without appending foreign key name
            if (!ReverseNavigationUniquePropNameClashes.Contains(tableNameHumanCase) && !ReverseNavigationUniquePropName.Contains(tableNameHumanCase))
            {
                ReverseNavigationUniquePropName.Add(tableNameHumanCase);
                return tableNameHumanCase;
            }

            // Append foreign key name
            string fkName = (useCamelCase ? Inflector.ToTitleCase(foreignKey.FkColumn) : foreignKey.FkColumn);
            string col = tableNameHumanCase + "_" + fkName.Replace(" ", "").Replace("$", "");

            if (checkForFkNameClashes && ReverseNavigationUniquePropName.Contains(col) && !ReverseNavigationUniquePropNameClashes.Contains(col))
                ReverseNavigationUniquePropNameClashes.Add(col); // Name clash

            if (!ReverseNavigationUniquePropNameClashes.Contains(col) && !ReverseNavigationUniquePropName.Contains(col))
            {
                ReverseNavigationUniquePropName.Add(col);
                return col;
            }

            for (int n = 1; n < 99; ++n)
            {
                col = tableNameHumanCase + n;

                if (ReverseNavigationUniquePropName.Contains(col))
                    continue;

                ReverseNavigationUniquePropName.Add(col);
                return col;
            }

            // Give up
            return tableNameHumanCase;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relationship"></param>
        /// <param name="fkName"></param>
        /// <param name="fkTable"></param>
        /// <param name="propName"></param>
        /// <param name="constraint"></param>
        /// <param name="collectionType"></param>
        /// <param name="includeComments"></param>
        public void AddReverseNavigation(Relationship relationship, string fkName, Table fkTable, string propName, string constraint, string collectionType, bool includeComments)
        {
            switch (relationship)
            {
                case Relationship.OneToOne:
                    ReverseNavigationProperty.Add(string.Format("public virtual {0} {1} {{ get; set; }}{2}", fkTable.Name, propName, includeComments ? " // " + constraint : string.Empty));
                    break;

                case Relationship.OneToMany:
                    ReverseNavigationProperty.Add(string.Format("public virtual {0} {1} {{ get; set; }}{2}", fkTable.Name, propName, includeComments ? " // " + constraint : string.Empty));
                    break;

                case Relationship.ManyToOne:
                    ReverseNavigationProperty.Add(string.Format("public virtual ICollection<{0}> {1} {{ get; set; }}{2}", fkTable.Name, propName, includeComments ? " // " + constraint : string.Empty));
                    ReverseNavigationCtor.Add(string.Format("{0} = new {1}<{2}>();", propName, collectionType, fkTable.Name));
                    break;

                case Relationship.ManyToMany:
                    ReverseNavigationProperty.Add(string.Format("public virtual ICollection<{0}> {1} {{ get; set; }}{2}", fkTable.Name, propName, includeComments ? " // Many to many mapping" : string.Empty));
                    ReverseNavigationCtor.Add(string.Format("{0} = new {1}<{2}>();", propName, collectionType, fkTable.Name));
                    break;

                default:    
                    throw new ArgumentOutOfRangeException("relationship");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="useCamelCase"></param>
        /// <param name="leftPropName"></param>
        /// <param name="rightPropName"></param>
        /// <param name="isSqlCE"></param>
        public void AddMappingConfiguration(ForeignKey left, ForeignKey right, bool useCamelCase, string leftPropName, string rightPropName, bool isSqlCE)
        {
            MappingConfiguration.Add(string.Format(@"HasMany(t => t.{0}).WithMany(t => t.{1}).Map(m => 
            {{
                m.ToTable(""{2}""{5});
                m.MapLeftKey(""{3}"");
                m.MapRightKey(""{4}"");
            }});", leftPropName, rightPropName, left.FkTableName, left.FkColumn, right.FkColumn, isSqlCE ? string.Empty : ", schema"));
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetPrimaryKeys()
        {
            if (PrimaryKeys.Any())
                return; // Table has at least one primary key

            // This table is not allowed in EntityFramework as it does not have a primary key.
            // Therefore generate a composite key from all non-null fields.
            foreach (var col in Columns.Where(x => !x.IsNullable && !x.Hidden))
            {
                col.IsPrimaryKey = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fkList"></param>
        /// <param name="tables"></param>
        /// <param name="useCamelCase"></param>
        /// <param name="collectionType"></param>
        /// <param name="checkForFkNameClashes"></param>
        /// <param name="includeComments"></param>
        /// <param name="isSqlCE"></param>
        public void IdentifyMappingTable(List<ForeignKey> fkList, Tables tables, bool useCamelCase, string collectionType, bool checkForFkNameClashes, bool includeComments, bool isSqlCE)
        {
            IsMapping = false;

            // Must have only 2 columns to be a mapping table
            if (Columns.Count != 2)
                return;

            // All columns must be primary keys
            if (PrimaryKeys.Count() != 2)
                return;

            // No columns should be nullable
            if (Columns.Any(x => x.IsNullable))
                return;

            // Find the foreign keys for this table
            var foreignKeys = fkList.Where(x =>
                                           String.Compare(x.FkTableName, Name, StringComparison.OrdinalIgnoreCase) == 0 &&
                                           String.Compare(x.FkSchema, Schema, StringComparison.OrdinalIgnoreCase) == 0)
                                    .ToList();

            // Each column must have a foreign key, therefore check column and foreign key counts match
            if (foreignKeys.Select(x => x.FkColumn).Distinct().Count() != 2)
                return;

            ForeignKey left = foreignKeys[0];
            ForeignKey right = foreignKeys[1];

            Table leftTable = tables.GetTable(left.PkTableName, left.PkSchema);
            if (leftTable == null)
                return;

            Table rightTable = tables.GetTable(right.PkTableName, right.PkSchema);
            if (rightTable == null)
                return;

            if (leftTable == rightTable)
                return;

            var leftPropName = leftTable.GetUniqueColumnPropertyName(rightTable.NameHumanCase, right, useCamelCase, checkForFkNameClashes, false);
            var rightPropName = rightTable.GetUniqueColumnPropertyName(leftTable.NameHumanCase, left, useCamelCase, checkForFkNameClashes, false);
            leftTable.AddMappingConfiguration(left, right, useCamelCase, leftPropName, rightPropName, isSqlCE);

            IsMapping = true;
            rightTable.AddReverseNavigation(Relationship.ManyToMany, rightTable.NameHumanCase, leftTable, rightPropName, null, collectionType, includeComments);
            leftTable.AddReverseNavigation(Relationship.ManyToMany, leftTable.NameHumanCase, rightTable, leftPropName, null, collectionType, includeComments);
        }
    }
}

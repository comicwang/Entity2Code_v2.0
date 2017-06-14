using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool
{
    /// <summary>
    /// 外键类
    /// </summary>
    public class ForeignKey
    {
        public string FkTableName { get; private set; }
        public string FkTableNameFiltered { get; private set; }
        public string FkSchema { get; private set; }
        public string PkTableName { get; private set; }
        public string PkTableNameFiltered { get; private set; }
        public string PkSchema { get; private set; }
        public string FkColumn { get; private set; }
        public string PkColumn { get; private set; }
        public string ConstraintName { get; private set; }
        public int Ordinal { get; private set; }

        public int Cascade { get; set; }

        public ForeignKey(string fkTableName, string fkSchema, string pkTableName, string pkSchema, string fkColumn, string pkColumn, string constraintName, string fkTableNameFiltered, string pkTableNameFiltered, int ordinal, int cascade)
        {
            ConstraintName = constraintName;
            PkColumn = pkColumn;
            FkColumn = fkColumn;
            PkSchema = pkSchema;
            PkTableName = pkTableName;
            FkSchema = fkSchema;
            FkTableName = fkTableName;
            FkTableNameFiltered = fkTableNameFiltered;
            PkTableNameFiltered = pkTableNameFiltered;
            Ordinal = ordinal;
            Cascade = cascade;
        }

        public string PkTableHumanCase(bool useCamelCase, bool prependSchemaName)
        {
            string singular = Inflector.MakeSingular(PkTableNameFiltered);
            string pkTableHumanCase = (useCamelCase ? Inflector.ToTitleCase(singular) : singular).Replace(" ", "").Replace("$", "");
            //if (string.Compare(PkSchema, "dbo", StringComparison.OrdinalIgnoreCase) != 0 && prependSchemaName)
            //    pkTableHumanCase = PkSchema + "_" + pkTableHumanCase;
            return pkTableHumanCase;
        }
    }
}

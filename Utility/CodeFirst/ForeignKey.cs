using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.CodeFirst
{
    /// <summary>
    /// 外键类
    /// </summary>
    public class ForeignKey
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public string FkTableName { get; private set; }
        /// <summary>
        /// 表名称过滤
        /// </summary>
        public string FkTableNameFiltered { get; private set; }
        /// <summary>
        /// 表空间
        /// </summary>
        public string FkSchema { get; private set; }
        /// <summary>
        /// 主键名称
        /// </summary>
        public string PkTableName { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string PkTableNameFiltered { get; private set; }
        /// <summary>
        /// 主键空间
        /// </summary>
        public string PkSchema { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string FkColumn { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string PkColumn { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string ConstraintName { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public int Ordinal { get; private set; }
        /// <summary>
        /// 
        /// </summary>

        public int Cascade { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fkTableName"></param>
        /// <param name="fkSchema"></param>
        /// <param name="pkTableName"></param>
        /// <param name="pkSchema"></param>
        /// <param name="fkColumn"></param>
        /// <param name="pkColumn"></param>
        /// <param name="constraintName"></param>
        /// <param name="fkTableNameFiltered"></param>
        /// <param name="pkTableNameFiltered"></param>
        /// <param name="ordinal"></param>
        /// <param name="cascade"></param>
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

        /// <summary>
        /// 获取表外键驼峰名称
        /// </summary>
        /// <param name="useCamelCase"></param>
        /// <param name="prependSchemaName"></param>
        /// <returns></returns>
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

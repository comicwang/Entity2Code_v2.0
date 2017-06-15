using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Pluralization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.CodeFirst;
using Utility.Core;
using Utility.Generate;

namespace Utility.Core
{
    public  class CodeFirstLogic
    {
        private static Tables _tables;

        public static Tables Tables
        {
            get
            {
                if (_tables == null)
                    _tables = GetTables();
                return CodeFirstLogic._tables;
            }
        }

        #region methods

        public static Tables GetTables()
        {

            CodeFirstTools.TableRename = (name, schema) => name;   // Do nothing by default
            CodeFirstTools.UpdateColumn = (Column column, Table table) => column; // Do nothing by default
            CodeFirstTools.StoredProcedureRename = (name, schema) => name;   // Do nothing by default
            Inflector.PluralizationService = new EnglishPluralizationService(new[]
           {
                 // Create custom ("Singular", "Plural") forms for one-off words as needed
               new CustomPluralizationEntry("LiveQuiz", "LiveQuizzes"),
               new CustomPluralizationEntry("Course", "Courses"),
              new CustomPluralizationEntry("CustomerStatus", "CustomerStatus"), // Use same value to prevent pluralisation
              new CustomPluralizationEntry("EmployeeStatus", "EmployeeStatus")
           });
            DbProviderFactory dbf = CodeFirstTools.GetDbProviderFactory();
            return CodeFirstTools.LoadTables(dbf);
        }

        Action<Table> WritePocoClassAttributes = t =>
        {
            // Do nothing by default
            // Example:
            // if(t.ClassName.StartsWith("Order"))
            //     WriteLine("    [SomeAttribute]");
        };

        // Writes optional base classes
        static Func<Table, string> WritePocoBaseClasses = t =>
        {
            //if (t.ClassName == "User")
            //    return ": IdentityUser<int, CustomUserLogin, CustomUserRole, CustomUserClaim>";
            return "";
        };

        // Writes any boilerplate stuff
        Action<Table> WritePocoBaseClassBody = t =>
        {
            // Do nothing by default
            // Example:
            // WriteLine("        // " + t.ClassName);
        };

        public static Func<Column, string> WritePocoColumn = c => c.Entity;

        Func<StoredProcedure, string> WriteStoredProcFunctionName = sp =>
            string.Format("{0}{1}", (sp.Schema.ToLower() != "dbo") ? sp.Schema + "_" : string.Empty, sp.NameHumanCase);

        Func<StoredProcedure, string> WriteStoredProcFunctionParams = sp =>
        {
            var sb = new StringBuilder();
            int n = 1;
            int count = sp.Parameters.Count;
            foreach (var p in sp.Parameters.OrderBy(x => x.Ordinal))
            {
                sb.AppendFormat("{0}{1} {2}{3}",
                    p.Mode == StoredProcedureParameterMode.In ? "" : "out ",
                    p.PropertyType,
                    p.NameHumanCase,
                    (n++ < count) ? ", " : string.Empty);
            }
            if (sp.ReturnColumns.Count > 0)
                sb.AppendFormat(", out int procResult");
            return sb.ToString();
        };

        Func<StoredProcedure, string> WriteStoredProcFunctionSqlAtParams = sp =>
        {
            var sb = new StringBuilder();
            int n = 1;
            int count = sp.Parameters.Count;
            foreach (var p in sp.Parameters.OrderBy(x => x.Ordinal))
            {
                sb.AppendFormat("{0}{1}{2}",
                    p.Name,
                    p.Mode == StoredProcedureParameterMode.In ? string.Empty : " OUTPUT",
                    (n++ < count) ? ", " : string.Empty);
            }
            return sb.ToString();
        };

        Func<StoredProcedure, string> WriteStoredProcFunctionDeclareSqlParameter = sp =>
        {
            var sb = new StringBuilder();
            foreach (var p in sp.Parameters.OrderBy(x => x.Ordinal))
            {
                sb.AppendLine(string.Format("            var {0}Param = new SqlParameter {{ ParameterName = \"{1}\", SqlDbType = SqlDbType.{2}, Direction = ParameterDirection.{3}{4}{5} }};",
                    p.NameHumanCase,
                    p.Name,
                    p.SqlDbType,
                    p.Mode == StoredProcedureParameterMode.In ? "Input" : "Output",
                    p.Mode == StoredProcedureParameterMode.In ? ", Value = " + p.NameHumanCase : string.Empty,
                    p.MaxLength > 0 ? ", Size = " + p.MaxLength : string.Empty));
            }
            sb.AppendLine("            var procResultParam = new SqlParameter { ParameterName = \"@procResult\", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };");
            return sb.ToString();
        };

        Func<StoredProcedure, string> WriteStoredProcFunctionSqlParameterAnonymousArray = sp =>
        {
            var sb = new StringBuilder();
            foreach (var p in sp.Parameters.OrderBy(x => x.Ordinal))
            {
                sb.AppendLine(string.Format("                {0}Param,", p.NameHumanCase));
            }
            sb.AppendLine("                procResultParam");
            return sb.ToString();
        };

        Func<StoredProcedure, bool, string> WriteStoredProcFunctionSetSqlParameters = (sp, isFake) =>
        {
            var sb = new StringBuilder();
            foreach (var p in sp.Parameters.Where(x => x.Mode != StoredProcedureParameterMode.In).OrderBy(x => x.Ordinal))
            {
                if (isFake)
                    sb.AppendLine(string.Format("            {0} = default({1});", p.NameHumanCase, p.PropertyType));
                else
                    sb.AppendLine(string.Format("            {0} = ({1}) {2}Param.Value;", p.NameHumanCase, p.PropertyType, p.NameHumanCase));
            }
            return sb.ToString();
        };

        Func<StoredProcedure, string> WriteStoredProcReturnModelName = sp =>
            string.Format("{0}{1}ReturnModel", (sp.Schema.ToLower() != "dbo") ? sp.Schema + "_" : string.Empty, sp.NameHumanCase);

        Func<DataColumn, string> WriteStoredProcReturnColumn = col =>
            string.Format("public {0}{1}{2} {{ get; set; }}",
                col.DataType.Name, StoredProcedure.CheckNullable(col), col.ColumnName);

        #endregion
    }
}

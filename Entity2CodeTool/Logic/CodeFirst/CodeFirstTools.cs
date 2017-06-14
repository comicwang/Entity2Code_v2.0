using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Pluralization;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Infoearth.Entity2CodeTool
{
    /// <summary>
    /// 注释的位置
    /// </summary>
    [Flags]
    public enum ExtendedPropertyCommentsStyle
    {
        None,//没有注释
        InSummaryBlock,//Summary注释，三斜杠的那种
        AtEndOfField  // 尾部添加注释
    }

    /// <summary>
    /// 允许生成的代码原素
    /// </summary>
    [Flags]
    public enum Elements
    {
        None = 0,//不生成
        Poco = 1,//POCO实体类
        Context = 2,//上下文
        UnitOfWork = 4,// 单元模式
        PocoConfiguration = 8,// CodeFirst配置
        StoredProcedures = 16// 存储过程
    }

    /// <summary>
    /// 关系枚举
    /// </summary>
    public enum Relationship
    {
        OneToOne,//一对一
        OneToMany,//一对多
        ManyToOne,//多对一
        ManyToMany,//多对多
    }

    public class CodeFirstTools
    {
        // 参数配置
        public static string DataType = "Oracle";

        public static string ConnectionStringName = "MyDbContext";        	//连接字符串的名称	

        public static bool IncludeViews = true; //是否包含视图

        public static bool AddUnitTestingDbContext = true; //输出的时候是否添加测试的上下文

        public static string DbContextName = "MyDbContext"; //上下文的类名称

        public static string DbContextBaseClass = "DbContext"; //上下文的基础类名,这个基本上不变

        public static string ConfigurationClassName = "Map";//配置类名，这个基本上不变

        public static string CollectionType = "List"; //集合类型，由于导航属性

        public static string CollectionTypeNamespace = ""; //集合类型的命名空间，"ObservableCollection" 此类型需要命名空间"System.Collections.ObjectModel"

        public static bool MakeClassesPartial = true; //是否将类标记为部分类

        public static bool GenerateSeparateFiles = false; //是否生成多个文件,一个类一个文件

        public static string FileExtension = ".cs"; //文件的扩展名

        //This will rename the tables & fields to use CamelCase(骆驼拼写法,比如，backColor这个复合词，color的第一个字母采用大写).
        // If false table & field names will be left alone.
        public static bool UseCamelCase = true;

        public static bool IncludeComments = true; //是否包含注释

        public static bool IncludeQueryTraceOn9481Flag = false; //是否包含查询跟踪

        public static ExtendedPropertyCommentsStyle IncludeExtendedPropertyComments = ExtendedPropertyCommentsStyle.InSummaryBlock;//注释位置

        public static bool AddWcfDataAttributes = false; //是否添加WCF的数据契约

        public static string ExtraWcfDataContractAttributes = ""; //额外的数据契约信息

        public static string SchemaName = null; //表空间名称

        public  static bool DisableGeographyTypes = false; //是否使用 System.Data.Entity.Spatial.DbGeometry 类型，Odata不支持此类型

        public static bool PrependSchemaName = true;// 预先考虑表空间名称，当表空间与表名一至的时候

        public static Regex TableFilterExclude = null;// 不包含的表
        public static Regex TableFilterInclude = null;// 包含的表

        public static Regex StoredProcedureFilterExclude = null;//不包含的存储过程
        public static Regex StoredProcedureFilterInclude = null;//包含的存储过程

        public static Regex ColumnFilterExclude = null;// 不包含的表列

        public static string[] ConfigFilenameSearchOrder = null;// 配置文件名，用于搜索
        public static string[] AdditionalNamespaces = null;// 额外的命名空间

        public static string _connectionString = "";//连接字符串

        public static string _providerName = "";//提供程序

        public static string _configFilePath = "";// 配置文件路径

        public static Func<string, string, string> TableRename;// 改表名的委托
        public static Func<string, string, string> StoredProcedureRename;//改存储过程的委托
        public static Func<Column, Table, Column> UpdateColumn;// 更新表列的委托

        //数据迁移的相关配置
        public static string MigrationConfigurationFileName = null;// 
        public static string MigrationStrategy = "MigrateDatabaseToLatestVersion";//
        public static bool AutomaticMigrationsEnabled = true;//
        public static bool AutomaticMigrationDataLossAllowed = true;//

        //特性表识
        public static string CodeGeneratedAttribute = "[GeneratedCodeAttribute(\"CodeFirstTools\", \"1.0.0.0\")]";//

        //生成的元素包括
        public static Elements ElementsToGenerate = Elements.Poco | Elements.Context | Elements.UnitOfWork | Elements.PocoConfiguration | Elements.StoredProcedures;//
        //各种命名空间
        public static string PocoNamespace, ContextNamespace, UnitOfWorkNamespace, PocoConfigurationNamespace = "";

        // Settings to allow TargetFramework checks
        public static string TargetFrameworkVersion;//NET版本

        //检查是否支持当前的版本
        public static Func<string, bool> IsSupportedFrameworkVersion = (string frameworkVersion) =>
        {
            if (!string.IsNullOrEmpty(TargetFrameworkVersion))
            {
                return String.Compare(TargetFrameworkVersion, frameworkVersion) >= 0;
            }
            return true;
        };

        public static string DataDirectory = "|DataDirectory|";//

       public static string[] NotNullable = new string[]
        {
            "string", 
            "byte[]", 
            "Microsoft.SqlServer.Types.SqlGeography", 
            "Microsoft.SqlServer.Types.SqlGeometry",
            "System.Data.Entity.Spatial.DbGeography",
            "System.Data.Entity.Spatial.DbGeometry"
        };

        //系统预留的关键字
       public static string[] ReservedKeywords = new string[]
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char",
            "checked", "class", "const", "continue", "decimal", "default", "delegate", "do",
            "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", 
            "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface",
            "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator",
            "out", "override", "params", "private", "protected", "public", "readonly", "ref",
            "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string",
            "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong",
            "unchecked", "unsafe", "ushort", "using", "virtual", "volatile", "void", "while"
        };

        //清空
        public static readonly Regex RxCleanUp = new Regex(@"[^\w\d_]", RegexOptions.Compiled);

        public static readonly Func<string, string> CleanUp = (str) =>
        {
            // Replace punctuation and symbols in variable names as these are not allowed.
            int len = str.Length;
            if (len == 0)
                return str;
            var sb = new StringBuilder();
            bool replacedCharacter = false;
            for (int n = 0; n < len; ++n)
            {
                char c = str[n];
                if (c != '_' && (char.IsSymbol(c) || char.IsPunctuation(c)))
                {
                    int ascii = c;
                    sb.AppendFormat("{0}", ascii);
                    replacedCharacter = true;
                    continue;
                }
                sb.Append(c);
            }
            if (replacedCharacter)
                str = sb.ToString();

            // Remove non alphanumerics
            str = RxCleanUp.Replace(str, "");
            if (char.IsDigit(str[0]))
                str = "C" + str;

            return str;
        };

        //连接字符串
        public static string ConnectionString
        {
            get
            {
                return _connectionString;
            }
        }
        //提供程序
        public static string ProviderName
        {
            get
            {
                return _providerName;
            }
        }

        //检查参数是否为空
        public static void ArgumentNotNull<T>(T arg, string name) where T : class
        {
            if (arg == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        //检查表列是否为空
        public static string CheckNullable(Column col)
        {
            string result = "";
            if (col.IsNullable && !NotNullable.Contains(col.PropertyType, StringComparer.InvariantCultureIgnoreCase))
                result = "?";
            return result;
        }

        //获取连接字符串，提供程序，配置路径
        public static string GetConnectionString(ref string connectionStringName, out string providerName, out string configFilePath)
        {
            providerName = null;
            configFilePath = String.Empty;
            string result = "";
            var paths = new string[] { Path.Combine(Application.StartupPath, "Infoearth.Entity2CodeTool.exe.config") };

            // Find a configuration file with the named connection string
            foreach (var path in paths)
            {
                var configFile = new ExeConfigurationFileMap { ExeConfigFilename = path };
                var config = ConfigurationManager.OpenMappedExeConfiguration(configFile, ConfigurationUserLevel.None);
                var connSection = config.ConnectionStrings;

                if (string.IsNullOrEmpty(connectionStringName))
                    continue;

                // Get the named connection string
                try
                {
                    result = connSection.ConnectionStrings[connectionStringName].ConnectionString;
                    providerName = connSection.ConnectionStrings[connectionStringName].ProviderName;
                    configFilePath = path;
                    return result;  // found it
                }
                catch
                {
                    result = "There is no connection string name called '" + connectionStringName + "'";
                }
            }
            return result;
        }

        //初始化连接字符串，提供程序，配置路径
        public static void InitConnectionString()
        {
            if (!String.IsNullOrEmpty(_connectionString))
                return;

            _connectionString = GetConnectionString(ref ConnectionStringName, out _providerName, out _configFilePath);

            if (!_connectionString.Contains(DataDirectory))
                return;
        }


        //控制密码
        public static string ZapPassword(string connectionString)
        {
            var rx = new Regex("password=.*;", RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            return rx.Replace(connectionString, "password=**zapped**;");
        }

        //获取数据库提供程序工厂
        public static DbProviderFactory GetDbProviderFactory()
        {
            //InitConnectionString();

            string solutionPath = string.Empty;
            Console.WriteLine("// Do not make changes directly to this file - edit the template instead.");
            Console.WriteLine("// This file was automatically generated.");
            Console.WriteLine("// ");
            Console.WriteLine("// The following connection settings were used to generate this file");
            Console.WriteLine("// ");
            //Console.WriteLine("//     Configuration file:     \"{0}\"", _configFilePath.Replace(solutionPath, String.Empty));
            Console.WriteLine("//     Connection String Name: \"{0}\"", ConnectionStringName);
            Console.WriteLine("//     Connection String:      \"{0}\"", ZapPassword(ConnectionString));
            Console.WriteLine("");

            try
            {
                return DbProviderFactories.GetFactory(ProviderName);
            }
            catch (Exception x)
            {
                string error = x.Message.Replace("\r\n", "\n").Replace("\n", " ");
                Console.WriteLine(string.Format("Failed to load provider \"{0}\" - {1}", ProviderName, error));
                Console.WriteLine("");
                Console.WriteLine("// -----------------------------------------------------------------------------------------");
                Console.WriteLine("// Failed to load provider \"{0}\" - {1}", ProviderName, error);
                Console.WriteLine("// -----------------------------------------------------------------------------------------");
                Console.WriteLine("");
                return null;
            }
        }

        public static DbProviderFactory TryGetDbProviderFactory()
        {
            try
            {
                return DbProviderFactories.GetFactory(ProviderName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        //获取表
        public static Tables LoadTables(DbProviderFactory factory)
        {
            if (factory == null || !(ElementsToGenerate.HasFlag(Elements.Poco) ||
                                    ElementsToGenerate.HasFlag(Elements.Context) ||
                                    ElementsToGenerate.HasFlag(Elements.UnitOfWork) ||
                                    ElementsToGenerate.HasFlag(Elements.PocoConfiguration)))
                return new Tables();

            try
            {
                using (DbConnection conn = factory.CreateConnection())
                {
                    conn.ConnectionString = ConnectionString;
                    conn.Open();

                    bool isSqlCE = false;
                    if (conn.GetType().Name == "SqlCeConnection")
                    {
                        PrependSchemaName = false;
                        isSqlCE = true;
                    }

                    SchemaReader reader = null;
                    if(CodeFirstTools.DataType.ToLower()=="oracle")
                    {
                        reader = new OracleSchemaReader(conn, factory);
                    }
                    else
                    {
                        reader = new SqlServerSchemaReader(conn, factory, IncludeQueryTraceOn9481Flag) { Outer = null };
                    }
                  
                    var tables = reader.ReadSchema(TableFilterExclude, ColumnFilterExclude, UseCamelCase, PrependSchemaName, IncludeComments, IncludeExtendedPropertyComments, TableRename, SchemaName, UpdateColumn);
                    tables.SetPrimaryKeys();

                    // Remove unrequired tables/views
                    for (int i = tables.Count - 1; i >= 0; i--)
                    {
                        if (SchemaName != null && String.Compare(tables[i].Schema, SchemaName, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            tables.RemoveAt(i);
                            continue;
                        }
                        if (!IncludeViews && tables[i].IsView)
                        {
                            tables.RemoveAt(i);
                            continue;
                        }
                        if (TableFilterInclude != null && !TableFilterInclude.IsMatch(tables[i].Name))
                        {
                            tables.RemoveAt(i);
                            continue;
                        }
                        if (!tables[i].IsView && string.IsNullOrEmpty(tables[i].PrimaryKeyNameHumanCase()))
                        {
                            tables.RemoveAt(i);
                        }
                    }

                    // Must be done in this order
                    var fkList = reader.ReadForeignKeys(TableRename);
                    reader.IdentifyForeignKeys(fkList, tables);
                    reader.ProcessForeignKeys(fkList, tables, UseCamelCase, PrependSchemaName, CollectionType, true, IncludeComments);
                    tables.IdentifyMappingTables(fkList, UseCamelCase, CollectionType, true, IncludeComments, isSqlCE);

                    tables.ResetNavigationProperties();
                    reader.ProcessForeignKeys(fkList, tables, UseCamelCase, PrependSchemaName, CollectionType, false, IncludeComments);
                    tables.IdentifyMappingTables(fkList, UseCamelCase, CollectionType, false, IncludeComments, isSqlCE);

                    // Remove views that only consist of all nullable fields.
                    // I.e. they do not contain any primary key, and therefore cannot be used by EF
                    for (int i = tables.Count - 1; i >= 0; i--)
                    {
                        if (string.IsNullOrEmpty(tables[i].PrimaryKeyNameHumanCase()))
                        {
                            tables.RemoveAt(i);
                        }
                    }

                    conn.Close();
                    return tables;
                }
            }
            catch (Exception x)
            {
                string error = x.Message.Replace("\r\n", "\n").Replace("\n", " ");
                Console.WriteLine(string.Format("Failed to read database schema - {0}", error));
                Console.WriteLine("");
                Console.WriteLine("// -----------------------------------------------------------------------------------------");
                Console.WriteLine("// Failed to read database schema - {0}", error);
                Console.WriteLine("// -----------------------------------------------------------------------------------------");
                Console.WriteLine("");
                //return new Tables();
                throw x;
            }
        }

        //获取存储过程
        private static List<StoredProcedure> LoadStoredProcs(DbProviderFactory factory)
        {
            if (factory == null || !ElementsToGenerate.HasFlag(Elements.StoredProcedures))
                return new List<StoredProcedure>();

            try
            {
                using (DbConnection conn = factory.CreateConnection())
                {
                    conn.ConnectionString = ConnectionString;
                    conn.Open();

                    if (conn.GetType().Name == "SqlCeConnection")
                        return new List<StoredProcedure>();

                    var reader = new SqlServerSchemaReader(conn, factory, IncludeQueryTraceOn9481Flag) { Outer = null };
                    var storedProcs = reader.ReadStoredProcs(StoredProcedureFilterExclude, UseCamelCase, PrependSchemaName, StoredProcedureRename, SchemaName);

                    // Remove unrequired stored procs
                    for (int i = storedProcs.Count - 1; i >= 0; i--)
                    {
                        if (SchemaName != null && String.Compare(storedProcs[i].Schema, SchemaName, StringComparison.OrdinalIgnoreCase) != 0)
                        {
                            storedProcs.RemoveAt(i);
                            continue;
                        }
                        if (StoredProcedureFilterInclude != null && !StoredProcedureFilterInclude.IsMatch(storedProcs[i].Name))
                        {
                            storedProcs.RemoveAt(i);
                            continue;
                        }
                    }

                    foreach (var proc in storedProcs)
                        reader.ReadStoredProcReturnObject(ConnectionString, proc);

                    conn.Close();
                    return storedProcs;
                }
            }
            catch (Exception x)
            {
                string error = x.Message.Replace("\r\n", "\n").Replace("\n", " ");
                Console.WriteLine(string.Format("Failed to read database schema for stored procedures - {0}", error));
                Console.WriteLine("");
                Console.WriteLine("// -----------------------------------------------------------------------------------------");
                Console.WriteLine("// Failed to read database schema for stored procedures - {0}", error);
                Console.WriteLine("// -----------------------------------------------------------------------------------------");
                Console.WriteLine("");
                return new List<StoredProcedure>();
            }
        }

        //获取表与表之间的关系
        public static Relationship CalcRelationship(Table pkTable, Table fkTable, Column fkCol, Column pkCol)
        {
            bool fkTableSinglePrimaryKey = (fkTable.PrimaryKeys.Count() == 1);
            bool pkTableSinglePrimaryKey = (pkTable.PrimaryKeys.Count() == 1);

            // 1:1
            if (fkCol.IsPrimaryKey && pkCol.IsPrimaryKey && fkTableSinglePrimaryKey && pkTableSinglePrimaryKey)
                return Relationship.OneToOne;

            // 1:n
            if (fkCol.IsPrimaryKey && !pkCol.IsPrimaryKey && fkTableSinglePrimaryKey)
                return Relationship.OneToMany;

            // n:1
            if (!fkCol.IsPrimaryKey && pkCol.IsPrimaryKey && pkTableSinglePrimaryKey)
                return Relationship.ManyToOne;

            // n:n
            return Relationship.ManyToMany;
        }

    }

    #region Inflector类

    /// <summary>
    /// Summary for the Inflector class
    /// </summary>
    public static class Inflector
    {
        static public IPluralizationService PluralizationService = null;

        /// <summary>
        /// Makes the plural.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static string MakePlural(string word)
        {
            try
            {
                return (PluralizationService == null) ? word : PluralizationService.Pluralize(word);
            }
            catch (Exception)
            {
                return word;
            }
        }

        /// <summary>
        /// Makes the singular.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static string MakeSingular(string word)
        {
            try
            {
                return (PluralizationService == null) ? word : PluralizationService.Singularize(word);
            }
            catch (Exception)
            {
                return word;
            }
        }

        /// <summary>
        /// Converts the string to title case.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static string ToTitleCase(string word)
        {
            string s = Regex.Replace(ToHumanCase(AddUnderscores(word)), @"\b([a-z])", match => match.Captures[0].Value.ToUpper());
            bool digit = false;
            string a = string.Empty;
            foreach (char c in s)
            {
                if (Char.IsDigit(c))
                {
                    digit = true;
                    a = a + c;
                }
                else
                {
                    if (digit && Char.IsLower(c))
                        a = a + Char.ToUpper(c);
                    else
                        a = a + c;
                    digit = false;
                }
            }
            return a;
        }

        /// <summary>
        /// Converts the string to human case.
        /// </summary>
        /// <param name="lowercaseAndUnderscoredWord">The lowercase and underscored word.</param>
        /// <returns></returns>
        public static string ToHumanCase(string lowercaseAndUnderscoredWord)
        {
            return MakeInitialCaps(Regex.Replace(lowercaseAndUnderscoredWord, @"_", " "));
        }


        /// <summary>
        /// Adds the underscores.
        /// </summary>
        /// <param name="pascalCasedWord">The pascal cased word.</param>
        /// <returns></returns>
        public static string AddUnderscores(string pascalCasedWord)
        {
            return
                Regex.Replace(Regex.Replace(Regex.Replace(pascalCasedWord, @"([A-Z]+)([A-Z][a-z])", "$1_$2"), @"([a-z\d])([A-Z])", "$1_$2"), @"[-\s]", "_").ToLower();
        }

        /// <summary>
        /// Makes the initial caps.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static string MakeInitialCaps(string word)
        {
            return String.Concat(word.Substring(0, 1).ToUpper(), word.Substring(1).ToLower());
        }

        /// <summary>
        /// Makes the initial character lowercase.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        public static string MakeInitialLower(string word)
        {
            return String.Concat(word.Substring(0, 1).ToLower(), word.Substring(1));
        }
    }

    #endregion

    #region 存储过程类

    public class StoredProcedure
    {
        public string Schema;
        public string Name;
        public string NameHumanCase;
        public List<StoredProcedureParameter> Parameters;
        public List<DataColumn> ReturnColumns;

        public StoredProcedure()
        {
            Parameters = new List<StoredProcedureParameter>();
            ReturnColumns = new List<DataColumn>();
        }

        public static string CheckNullable(DataColumn col)
        {
            string result = " ";
            if (col.AllowDBNull && !CodeFirstTools.NotNullable.Contains(col.DataType.Name, StringComparer.InvariantCultureIgnoreCase))
                result = "? ";
            return result;
        }
    }

    public enum StoredProcedureParameterMode
    {
        In,
        InOut,
        Out
    };

    public class StoredProcedureParameter
    {
        public int Ordinal;
        public StoredProcedureParameterMode Mode;
        public string Name;
        public string NameHumanCase;
        public string SqlDbType;
        public string PropertyType;
        public int DateTimePrecision;
        public int MaxLength;
        public int Precision;
        public int Scale;
    }

    #endregion
}

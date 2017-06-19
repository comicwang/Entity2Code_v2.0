using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.CodeFirst
{
    /// <summary>
    /// 表列Column类
    /// </summary>
    public class Column
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 类型
        /// </summary>
        public string TypeName;
        /// <summary>
        /// 时间精度
        /// </summary>
        public int DateTimePrecision;
        /// <summary>
        /// 默认项
        /// </summary>
        public string Default;
        /// <summary>
        /// 最大长度
        /// </summary>
        public int MaxLength;
        /// <summary>
        /// 精度
        /// </summary>
        public int Precision;
        /// <summary>
        /// 属性名称
        /// </summary>
        public string PropertyName;
        /// <summary>
        /// 属性驼峰名称
        /// </summary>
        public string PropertyNameHumanCase;
        /// <summary>
        /// 属性类型
        /// </summary>
        public string PropertyType;
        /// <summary>
        /// 属性大小
        /// </summary>
        public int Scale;
        public int Ordinal;
        /// <summary>
        /// 主键
        /// </summary>
        public int PrimaryKeyOrdinal;
        /// <summary>
        /// 扩展属性
        /// </summary>
        public string ExtendedProperty;
        /// <summary>
        /// 索引名称
        /// </summary>
        public string UniqueIndexName;

        /// <summary>
        /// 是否唯一
        /// </summary>
        public bool IsIdentity;
        /// <summary>
        /// 是否可空
        /// </summary>
        public bool IsNullable;
        /// <summary>
        /// 是否为主键
        /// </summary>
        public bool IsPrimaryKey;
        /// <summary>
        /// 是否索引
        /// </summary>
        public bool IsPrimaryKeyViaUniqueIndex;
        /// <summary>
        /// 是否存储过程
        /// </summary>
        public bool IsStoreGenerated;
    
        public bool IsRowVersion;
        /// <summary>
        /// 是否固定长度
        /// </summary>
        public bool IsFixedLength;
        /// <summary>
        /// 是字符串
        /// </summary>
        public bool IsUnicode;
        /// <summary>
        /// 是否隐藏属性
        /// </summary>
        public bool Hidden;
        /// <summary>
        /// 是否外键
        /// </summary>
        public bool IsForeignKey;

        /// <summary>
        /// 配置
        /// </summary>
        public string Config;
        /// <summary>
        /// 配置外键
        /// </summary>
        public string ConfigFk;
        /// <summary>
        /// 实体
        /// </summary>
        public string Entity;
        /// <summary>
        /// 实体外键
        /// </summary>
        public string EntityFk;

        /// <summary>
        /// 设置实体
        /// </summary>
        /// <param name="includeComments"></param>
        /// <param name="includeExtendedPropertyComments"></param>
        private void SetupEntity(bool includeComments, ExtendedPropertyCommentsStyle includeExtendedPropertyComments)
        {
            string comments;
            if (includeComments)
            {
                comments = " // " + Name;
                if (IsPrimaryKey)
                {
                    if (IsPrimaryKeyViaUniqueIndex)
                        comments += " (Primary key via unique index " + UniqueIndexName + ")";
                    else
                        comments += " (Primary key)";
                }
            }
            else
            {
                comments = string.Empty;
            }

            if (includeExtendedPropertyComments == ExtendedPropertyCommentsStyle.AtEndOfField && !string.IsNullOrEmpty(ExtendedProperty))
            {
                if (string.IsNullOrEmpty(comments))
                    comments = " // " + ExtendedProperty;
                else
                    comments += ". " + ExtendedProperty;
            }

            if (IsPrimaryKey)
            {
                StringBuilder build = new StringBuilder();
                build.AppendLine(string.Format("private {0} {1} _{2};", PropertyType, CodeFirstTools.CheckNullable(this), PropertyNameHumanCase.ToLower()));
                build.AppendLine(string.Format("public {0} {1} {2} {3} {4}", PropertyType, CodeFirstTools.CheckNullable(this), PropertyName, "{ get { return _" + PropertyNameHumanCase.ToLower() + " ; } set { this.Id = value; this._" + PropertyNameHumanCase.ToLower() + " = value; }}", comments));
                Entity = build.ToString();
            }
            else
            {
                Entity = string.Format("public {0}{1} {2} {3}{4}", PropertyType, CodeFirstTools.CheckNullable(this), PropertyName, IsStoreGenerated ? "{ get; internal set; }" : "{ get; set; }", comments);
            }
           
        }

        /// <summary>
        /// 设置配置
        /// </summary>
        private void SetupConfig()
        {
            bool hasDatabaseGeneratedOption = false;
            string propertyType = PropertyType.ToLower();
            switch (propertyType)
            {
                case "long":
                case "short":
                case "int":
                case "double":
                case "float":
                case "decimal":
                case "string":
                    hasDatabaseGeneratedOption = true;
                    break;
            }
            string databaseGeneratedOption = string.Empty;
            if (hasDatabaseGeneratedOption)
            {
                if (IsIdentity)
                    databaseGeneratedOption = ".HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)";
                if (IsStoreGenerated)
                    databaseGeneratedOption = ".HasDatabaseGeneratedOption(DatabaseGeneratedOption.Computed)";
                if (IsPrimaryKey && !IsIdentity && !IsStoreGenerated)
                    databaseGeneratedOption = ".HasDatabaseGeneratedOption(DatabaseGeneratedOption.None)";
            }

            Config = string.Format("            Property(x => x.{0}).HasColumnName(\"{1}\"){2}{3}{4}{5}{6}{7}{8}{9}{10}{11};", PropertyName, Name,
                                      (IsNullable) ? ".IsOptional()" : ".IsRequired()",
                                      (IsFixedLength || IsRowVersion) ? ".IsFixedLength()" : "",
                                      (IsUnicode) ? string.Empty : ".IsUnicode(false)",
                                      (TypeName.ToLower() == "long" || TypeName.ToLower() == "float" || TypeName.ToLower() == "bfile") ? ".HasColumnType(\"" + TypeName + "\")" : string.Empty,
                                      (MaxLength > 0) ? ".HasMaxLength(" + MaxLength + ")" : string.Empty,
                                      (DateTimePrecision > 0) ? ".HasPrecision(" + DateTimePrecision + ")" : string.Empty,
                                      (Precision > 0) ? (Scale <= 0 && propertyType != "short" && propertyType != "int" && propertyType != "long" && propertyType != "byte") ? ".HasPrecision(" + Precision + "," + 0 + ")" : string.Empty : string.Empty,
                                      (Scale > 0) ? ".HasPrecision(" + Precision + "," + Scale + ")" : string.Empty,
                                      (IsRowVersion) ? ".IsRowVersion()" : string.Empty,
                                      databaseGeneratedOption);
        }

        /// <summary>
        /// 设置实体和配置
        /// </summary>
        /// <param name="includeComments"></param>
        /// <param name="includeExtendedPropertyComments"></param>
        public void SetupEntityAndConfig(bool includeComments, ExtendedPropertyCommentsStyle includeExtendedPropertyComments)
        {
            SetupEntity(includeComments, includeExtendedPropertyComments);
            SetupConfig();
        }

        /// <summary>
        /// 清除默认项
        /// </summary>
        public void CleanUpDefault()
        {
            if (string.IsNullOrEmpty(Default))
                return;

            while (Default.First() == '(' && Default.Last() == ')' && Default.Length > 2)
            {
                Default = Default.Substring(1, Default.Length - 2);
            }

            if (Default.First() == '\'' && Default.Last() == '\'' && Default.Length >= 2)
                Default = string.Format("\"{0}\"", Default.Substring(1, Default.Length - 2));

            string lower = Default.ToLower();
            string lowerPropertyType = PropertyType.ToLower();

            // Cleanup default
            switch (lowerPropertyType)
            {
                case "bool":
                    Default = (Default == "0") ? "false" : "true";
                    break;

                case "string":
                case "datetime":
                case "timespan":
                case "datetimeoffset":
                    if (Default.First() != '"')
                        Default = string.Format("\"{0}\"", Default);
                    if (Default.Contains('\\'))
                        Default = "@" + Default;
                    break;

                case "long":
                case "short":
                case "int":
                case "double":
                case "float":
                case "decimal":
                case "byte":
                case "guid":
                    if (Default.First() == '\"' && Default.Last() == '\"' && Default.Length > 2)
                        Default = Default.Substring(1, Default.Length - 2);
                    break;

                case "byte[]":
                case "System.Data.Entity.Spatial.DbGeography":
                case "System.Data.Entity.Spatial.DbGeometry":
                    Default = string.Empty;
                    break;
            }

            if (string.IsNullOrWhiteSpace(Default))
                return;

            // Validate default
            switch (lowerPropertyType)
            {
                case "long":
                    long l;
                    if (!long.TryParse(Default, out l))
                        Default = string.Empty;
                    break;

                case "short":
                    short s;
                    if (!short.TryParse(Default, out s))
                        Default = string.Empty;
                    break;

                case "int":
                    int i;
                    if (!int.TryParse(Default, out i))
                        Default = string.Empty;
                    break;

                case "datetime":
                    DateTime dt;
                    if (!DateTime.TryParse(Default, out dt))
                        Default = lower.Contains("getdate()") ? "System.DateTime.Now" : lower.Contains("getutcdate()") ? "System.DateTime.UtcNow" : string.Empty;
                    else
                        Default = string.Format("System.DateTime.Parse({0})", Default);
                    break;

                case "datetimeoffset":
                    DateTimeOffset dto;
                    if (!DateTimeOffset.TryParse(Default, out dto))
                        Default = lower.Contains("sysdatetimeoffset") ? "System.DateTimeOffset.Now" : lower.Contains("sysutcdatetime") ? "System.DateTimeOffset.UtcNow" : string.Empty;
                    else
                        Default = string.Format("System.DateTimeOffset.Parse({0})", Default);
                    break;

                case "timespan":
                    TimeSpan ts;
                    if (!TimeSpan.TryParse(Default, out ts))
                        Default = string.Empty;
                    else
                        Default = string.Format("System.TimeSpan.Parse({0})", Default);
                    break;

                case "double":
                    double d;
                    if (!double.TryParse(Default, out d))
                        Default = string.Empty;
                    break;

                case "float":
                    float f;
                    if (!float.TryParse(Default, out f))
                        Default = string.Empty;
                    break;

                case "decimal":
                    decimal dec;
                    if (!decimal.TryParse(Default, out dec))
                        Default = string.Empty;
                    else
                        Default += "m";
                    break;

                case "byte":
                    byte b;
                    if (!byte.TryParse(Default, out b))
                        Default = string.Empty;
                    break;

                case "bool":
                    bool x;
                    if (!bool.TryParse(Default, out x))
                        Default = string.Empty;
                    break;

                case "string":
                    if (lower.Contains("newid()") || lower.Contains("newsequentialid()"))
                        Default = "Guid.NewGuid().ToString()";
                    break;

                case "guid":
                    if (lower.Contains("newid()") || lower.Contains("newsequentialid()"))
                        Default = "System.Guid.NewGuid()";
                    else
                        Default = string.Format("Guid.Parse(\"{0}\")", Default);
                    break;
            }
        }
    }
}

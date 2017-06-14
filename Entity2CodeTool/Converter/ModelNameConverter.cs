using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool.Converter
{
    public class ModelNameConverter
    {
        #region attrs and fields

        private static System.Globalization.CultureInfo _cultureInfo;

        #endregion

        #region methods

        /// <summary>
        /// 根据实体名称转换得DTO名称
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string GetData2Obj(string entity)
        {
            string result = string.Empty;
            if (string.IsNullOrEmpty(entity))
                return string.Empty;
            string[] strs = entity.ToLower().Split('_');

            for (int i = 0; i < strs.Length; i++)
            {
                result += ToHeadUpper(strs[i]);
            }

            //if (strs.Length == 2)
            //{
            //    result = ToHeadUpper(strs[1]);
            //}
            //else if (strs.Length == 1)
            //{
            //    result = ToHeadUpper(entity);
            //}
            //else
            //{
            //    result = ToHeadUpper(strs[1]) + ToHeadUpper(strs[2]);
            //}
            return result;
        }

        private static string ToHeadUpper(string str)
        {
            if (_cultureInfo == null)
                _cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            return _cultureInfo.TextInfo.ToTitleCase(str);
        }

        #endregion
    }
}

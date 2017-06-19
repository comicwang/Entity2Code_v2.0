using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Filter
{
    /// <summary>
    /// C#参数类型
    /// </summary>
    public class CsharpVarFilter
    {
        private const string VoidArg = "void";

        private const string EnumTypeCons = "int,string,double,float,DateTime,char,long,byte,short,uint,ushort,ulong,sbyte,decimal,bool";

        /// <summary>
        /// 获取所有的C#参数类型
        /// </summary>
        /// <param name="isReturn">是否为返回参数</param>
        /// <returns></returns>
        public static List<string> FilterAll(bool isReturn)
        {
            List<string> result = EnumTypeCons.Split(',').OrderBy(t => t).ToList();

            if (isReturn)
                result.Add(VoidArg);

            return result;
        }
    }
}
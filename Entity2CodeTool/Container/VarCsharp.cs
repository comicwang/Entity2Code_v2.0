using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool.Container
{
    public class VarCsharp
    {
        private const string UnTypeCons = "void";

        private const string EnumTypeCons = "int,string,double,float,DateTime,char,long,byte,short,uint,ushort,ulong,sbyte,decimal,bool";

        public static List<string> AllTypes(bool reList)
        {
            List<string> result = EnumTypeCons.Split(',').ToList();

            if (!reList)
                result.Add(UnTypeCons);

            return result;
        }
    }
}
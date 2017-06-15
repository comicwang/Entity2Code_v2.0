using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;
using Utility.Core;

namespace Utility.Converter
{
    public class StringConverter
    {
        public static string ConvertFileName(string guid)
        {
            string tempName = CdeCmdId.TempFileName(guid);
            if (tempName == null)
            {
                string modelPath = TemplateContainer.Resove<string>(guid);
                tempName = Path.GetFileNameWithoutExtension(modelPath) + ".cs";
            }
            else
            {
                tempName = KeywordContainer.Replace(tempName);
            }
            return tempName;
        }

        public static string ConvertPath(string fileName)
        {
            return Path.Combine(Properties.Resource.RootPath, fileName);
        }
    }
}

using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility.Common;
using Utility.Base;
using Utility.Properties;

namespace Utility.Core
{
    public abstract class CodeGenerateBase
    {
        public virtual void BeginGenerate()
        {
            if (CommonContainer.CommonServer != null)
                CommonContainer.CommonServer.OutString(Resource.BeginGenerate);
        }

        public abstract object[] GetGenerateInfo(string guid,bool allowNew);

        public virtual bool GenerateTemp(object[] info)
        {
            return true;
        }

        public abstract bool GenerateCode(object[] info);

        public virtual void EndGenerate(bool result)
        {
            if (CommonContainer.CommonServer != null)
                CommonContainer.CommonServer.OutString(string.Format("{0}{1}", Resource.GenerateOutput, result ? Resource.StateSuccess : Resource.StateFail));
        }

        public void Generate(string id,bool allowNew)
        {
            object[] info = GetGenerateInfo(id,allowNew);

            BeginGenerate();

            bool result = GenerateTemp(info);

            result &= GenerateCode(info);

            EndGenerate(result);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool.Logic
{
    public class CodeAppendManager : CodeManageBase
    {
        #region attrs and fields

        public override object BuildTaget { get; set; }

        private TemplateEntity _entity;

        private ConstructType _consType;

        #endregion

        #region ctor

        public CodeAppendManager(ConstructType consType, TemplateEntity entity)
            : base(consType, entity)
        {
            _consType = consType;
            _entity = entity;
        }

        #endregion

        #region methods

        public override void CreateCode()
        {
            if (null == BuildTaget)
                throw new Exception("Entity2Code Code Taget Lost");
            StringBuilder build = BuildTaget as StringBuilder;
            if (null == build)
                throw new Exception("Entity2Code Code Taget Argment Error");

            build.AppendLine(CodeContent);
            build.AppendLine();

            base.CreateCode();
        }

        #endregion
    }
}

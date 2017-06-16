using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Core;

namespace Utility.Entity
{
    public class GenerateEntity
    {
        public GenerateEntity(CodeGenerateBase engin, string id, bool arg, Dictionary<string, string> container)
        {
            GenerateId = id;
            GenerateEngin = engin;
            GenerateArgment = arg;
            GenerateContainer = container;
        }

        public string GenerateId { get; set; }

        public CodeGenerateBase GenerateEngin { get; set; }

        public bool GenerateArgment { get; set; }

        public Dictionary<string, string> GenerateContainer { get; set; }
    }
}

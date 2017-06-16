using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Core;

namespace Utility.Entity
{
    public class GenerateEntityContext
    {
        private static Queue<GenerateEntity> _gEntityQueue = new Queue<GenerateEntity>();
        public  void injection(GenerateEntity gEntity)
        {
            _gEntityQueue.Enqueue(gEntity);
        }

        public void injection(CodeGenerateBase engin, string id, bool argment, Dictionary<string, string> container)
        {
            _gEntityQueue.Enqueue(new GenerateEntity(engin, id, argment, container));
        }

        public void Commit()
        {
            while (_gEntityQueue.Count() > 0)
            {
                GenerateEntity queue = _gEntityQueue.Dequeue();
                queue.GenerateEngin.Generate(queue.GenerateId, queue.GenerateArgment, queue.GenerateContainer);
            }
        }

    }
}

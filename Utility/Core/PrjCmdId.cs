using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Core
{
    public class PrjCmdId
    {
        private static Dictionary<string, string> _pContainer = new Dictionary<string, string>();

        public static string FindProjectName(string pid)
        {
            return _pContainer[pid];
        }

        public static void SetProjectName(string pid, string pname)
        {
            if (!_pContainer.ContainsKey(pid))
                _pContainer.Add(pid, pname);
            else
                _pContainer[pid] = pname;
        }

        public const string Infrastructure = "3B1FC4ED-4158-4556-89FC-0D52D2C05750";

        public const string DomainEntity = "574E6625-41B8-4FA2-A6CD-C6B2AA4BF71E";

        public const string DomainContext = "10CEB805-03F0-43CC-8B49-9DD7AA08CE94";

        public const string Application = "BD0ED2FB-2496-4629-8EBA-65051828731F";

        public const string IApplication = "9F278124-C9EC-412E-820B-E8DDA72F5405";

        public const string Data2Object = "8F2A0B70-4A4A-4B1A-83B9-8BE891D35AF3";

        public const string Service = "06D57C23-4D51-430D-A8E6-9A19F38390E3";

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Core
{
    /// <summary>
    /// 项目信息
    /// </summary>
    public class PrjCmdId
    {
        private static Dictionary<string, string> _pContainer = new Dictionary<string, string>();

        /// <summary>
        /// 根据项目ID获取项目的名称
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static string FindProjectName(string pid)
        {
            if (!_pContainer.ContainsKey(pid))
                return null;
            return _pContainer[pid];
        }

        /// <summary>
        /// 设置项目名称到容器
        /// </summary>
        /// <param name="pid">ID</param>
        /// <param name="pname">项目名称</param>
        public static void SetProjectName(string pid, string pname)
        {
            if (!_pContainer.ContainsKey(pid))
                _pContainer.Add(pid, pname);
            else
                _pContainer[pid] = pname;
        }

        /// <summary>
        /// 
        /// </summary>
        public const string Infrastructure = "3B1FC4ED-4158-4556-89FC-0D52D2C05750";

        /// <summary>
        /// 
        /// </summary>
        public const string DomainEntity = "574E6625-41B8-4FA2-A6CD-C6B2AA4BF71E";

        /// <summary>
        /// 
        /// </summary>
        public const string DomainContext = "10CEB805-03F0-43CC-8B49-9DD7AA08CE94";

        /// <summary>
        /// 
        /// </summary>
        public const string Application = "BD0ED2FB-2496-4629-8EBA-65051828731F";

        /// <summary>
        /// 
        /// </summary>
        public const string IApplication = "9F278124-C9EC-412E-820B-E8DDA72F5405";

        /// <summary>
        /// 
        /// </summary>
        public const string Data2Object = "8F2A0B70-4A4A-4B1A-83B9-8BE891D35AF3";

        /// <summary>
        /// 
        /// </summary>
        public const string Service = "06D57C23-4D51-430D-A8E6-9A19F38390E3";

    }
}

using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility.Core
{
    public class ProjectContainer : IDisposable
    {

        private static Dictionary<string, Project> _container = null;

        public static void Regist(string guid, Project prj)
        {
            if (_container == null)
                _container = new Dictionary<string, Project>();
            if (_container.ContainsKey(guid))
                _container[guid] = prj;
            else
                _container.Add(guid, prj);

        }


        public static Project Resove(string guid)
        {
            if (_container == null||!_container.ContainsKey(guid))
                return null;

            return _container[guid];
        }


        public void Dispose()
        {
            _container = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infoearth.Entity2CodeTool
{
    static class CodeBuilderContainer
    {
        public static StringBuilder ProfileBuilder;

        public static StringBuilder ServiceBuilder;

        public static StringBuilder IServiceBuilder;

        public static StringBuilder ContainBuilder;

        public static StringBuilder DBContextBuilder;

        public static string ProfileContent
        {
            get
            {
                if (null != ProfileBuilder)
                    return ProfileBuilder.ToString();
                else
                    return string.Empty;
            }

        }

        public static string ServiceContent
        {
            get
            {
                if (null != ServiceBuilder)
                    return ServiceBuilder.ToString();
                else
                    return string.Empty;
            }

        }

        public static string IServiceContent
        {
            get
            {
                if (null != IServiceBuilder)
                    return IServiceBuilder.ToString();
                else
                    return string.Empty;
            }

        }

        public static string ContainerContent
        {
            get
            {
                if (null != ContainBuilder)
                    return ContainBuilder.ToString();
                else
                    return string.Empty;
            }
        }

        public static string DBContextContent
        {
            get
            {
                if (null != DBContextBuilder)
                    return DBContextBuilder.ToString();
                else
                    return string.Empty;
            }
        }

        public static void Clear()
        {
            ProfileBuilder = new StringBuilder();
            ServiceBuilder = new StringBuilder();
            IServiceBuilder = new StringBuilder();
            ContainBuilder = new StringBuilder();
            DBContextBuilder = new StringBuilder();
        }
    }
}

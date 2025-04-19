using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Bilten
{
    static class ConfigurationParameters
    {
        public static int ItemsPerPage
        {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["ItemsPerPage"]);
                }
                catch
                {
                    return 20;
                }
            }
        }

        public static string DatabaseFile
        {
            get { return "BiltenPodaci.sdf"; }
        }

        public static string Password
        {
            get { return ""; }
        }

        public static string ConnectionString
        {
            get { return String.Format("Data Source={0}", DatabaseFile); }
        }
    }
}

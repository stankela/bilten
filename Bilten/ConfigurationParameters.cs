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
    }
}

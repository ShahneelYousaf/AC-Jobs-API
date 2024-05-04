using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AC_Jobs_API_Service_Layer.Models.Healper
{
    public static class Helper
    {
        public static DateTime GetCurrentDate()
        {
            return DateTime.Now;
        }

        public static bool IsNullOrEmpty<T>(T obj)
        {
            return obj is null;
        }

    }
}

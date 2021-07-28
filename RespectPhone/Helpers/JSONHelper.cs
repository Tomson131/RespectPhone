using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RespectPhone.Helpers
{
    public class JSONHelper
    {
        public static int GetInt(JToken j)
        {
            if (j == null) return 0;
            int.TryParse(j.Value<string>(), out int x);
            return x;
        }
        public static long GetLong(JToken j)
        {
            if (j == null) return 0;
            long.TryParse(j.Value<string>(), out long x);
            return x;
        }
        public static decimal GetDecimal(JToken j)
        {
            if (j == null) return 0;
            if (decimal.TryParse(j.Value<string>(), out decimal x))
            {
                return x;
            }
            else
            {
                if (decimal.TryParse(j.Value<string>().Replace(",", "."), out decimal x2))
                {
                    return x2;
                }
                if (decimal.TryParse(j.Value<string>().Replace(".", ","), out decimal x3))
                {
                    return x3;
                }
            }
            return 0;
        }
        public static double GetDouble(JToken j)
        {
            if (j == null) return 0;
            if (double.TryParse(j.Value<string>(), out double x))
            {
                return x;
            }
            else
            {
                if (double.TryParse(j.Value<string>().Replace(",", "."), out double x2))
                {
                    return x2;
                }
                if (double.TryParse(j.Value<string>().Replace(".", ","), out double x3))
                {
                    return x3;
                }
            }
            return x;
        }

        public static DateTime GetDate(JToken j)
        {
            if (j == null) return DateTime.MinValue;
            DateTime.TryParse(j.Value<string>(), out DateTime x);
            return x;
        }
        public static string GetString(JToken j)
        {
            if (j == null) return "";
            return j.Value<string>() == null ? "" : j.Value<string>();
        }
        public static bool GetBool(JToken j)
        {
            if (j == null) return false;
            bool.TryParse(j.Value<string>(), out bool x);
            return x;
        }
    }
}

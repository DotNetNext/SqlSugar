using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar.OceanBaseForOracle
{
    /// <summary>
    ///Common Extensions for external users
    /// </summary>
    public static class UtilExtensions
    {
        public static string ToLower(this string value, bool isLower) 
        {
            if (isLower) 
            {
                return value.ObjToString().ToLower();
            }
            return value.ObjToString();
        }
        public static string ToUpper(this string value, bool isAutoToUpper)
        {
            if (value == null) return null;
            if (isAutoToUpper == false) return value;
            return value.ToUpper();
        }

        public static string GetN(this SqlSugarProvider Context) 
        {
            var N = "N";
            if (Context.CurrentConnectionConfig.MoreSettings != null && Context.CurrentConnectionConfig.MoreSettings.DisableNvarchar)
            {
                N = "";
            }
            return N;
        }
    }
}

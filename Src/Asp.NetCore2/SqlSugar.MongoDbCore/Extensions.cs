using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar.MongoDb
{
    public static class Extensions
    {
        public static bool EqualCase(this string thisValue, string equalValue)
        {
            if (thisValue != null && equalValue != null)
            {
                return thisValue.ToLower() == equalValue.ToLower();
            }
            else
            {
                return thisValue == equalValue;
            }
        }
    }
}

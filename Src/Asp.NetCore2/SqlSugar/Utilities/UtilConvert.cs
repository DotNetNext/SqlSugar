﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    internal static class UtilConvert
    {
        public static int ObjToInt(this object thisValue)
        {
            int reval = 0;
            if (thisValue == null) return 0;
            if (thisValue is Enum)
            {
                return Convert.ToInt32(thisValue);
            }
            if (thisValue != null && thisValue != DBNull.Value && int.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return reval;
        }

        public static long ObjToLong(this object thisValue)
        {
            long reval = 0;
            if (thisValue == null) return 0;
            if (thisValue is Enum)
            {
                return Convert.ToInt64(thisValue);
            }
            if (thisValue != null && thisValue != DBNull.Value && long.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return reval;
        }

        public static int ObjToInt(this object thisValue, int errorValue)
        {
            int reval = 0;
            if (thisValue is Enum)
            {
                return (int)thisValue;
            }
            if (thisValue != null && thisValue != DBNull.Value && int.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return errorValue;
        }

        public static double ObjToMoney(this object thisValue)
        {
            double reval = 0;
            if (thisValue != null && thisValue != DBNull.Value && double.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return 0;
        }

        public static double ObjToMoney(this object thisValue, double errorValue)
        {
            double reval = 0;
            if (thisValue != null && thisValue != DBNull.Value && double.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return errorValue;
        }
        public static bool EqualCase(this string thisValue,string equalValue) 
        {
            if ( thisValue!=null && equalValue != null)
            {
                return thisValue.ToLower() == equalValue.ToLower();
            }
            else 
            {
                return thisValue == equalValue;
            }
        }
        public static string ObjToString(this object thisValue,Func<DateTime,string> formatTime)
        {
            if (formatTime != null&&thisValue is DateTime)
            {
                var dt = Convert.ToDateTime(thisValue);
                return formatTime(dt);
            }
            else 
            {
                return thisValue+string.Empty;
            }
        }
        public static string ObjToString(this object thisValue)
        {
            if (thisValue != null) return thisValue.ToString().Trim();
            return "";
        }
        public static string ObjToStringNoTrim(this object thisValue)
        {
            if (thisValue != null) return thisValue.ToString();
            return "";
        }
        public static string ObjToStringNew(this object thisValue)
        {
            if (thisValue != null && thisValue is byte[])
            {
                return string.Join("|",thisValue as byte[]);
            }
            if (thisValue != null) return thisValue.ToString().Trim();
            return "";
        }

        public static string ObjToString(this object thisValue, string errorValue)
        {
            if (thisValue != null) return thisValue.ToString().Trim();
            return errorValue;
        }

        public static Decimal ObjToDecimal(this object thisValue)
        {
            Decimal reval = 0;
            if (thisValue != null && thisValue != DBNull.Value && decimal.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return 0;
        }

        public static Decimal ObjToDecimal(this object thisValue, decimal errorValue)
        {
            Decimal reval = 0;
            if (thisValue != null && thisValue != DBNull.Value && decimal.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return errorValue;
        }

        public static DateTime ObjToDate(this object thisValue)
        {
            if (thisValue is DateTime)
            {
                return (DateTime)thisValue;
            }
            DateTime reval = DateTime.MinValue;
            if (thisValue != null && thisValue != DBNull.Value && DateTime.TryParse(thisValue.ToString(), out reval))
            {
                reval = Convert.ToDateTime(thisValue);
            }
            return reval;
        }

        public static DateTime ObjToDate(this object thisValue, DateTime errorValue)
        {
            if (thisValue is DateTime)
            {
                return (DateTime)thisValue;
            }
            DateTime reval = DateTime.MinValue;
            if (thisValue != null && thisValue != DBNull.Value && DateTime.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return errorValue;
        }

        public static bool ObjToBool(this object thisValue)
        {
            bool reval = false;
            if (thisValue != null && thisValue != DBNull.Value && bool.TryParse(thisValue.ToString(), out reval))
            {
                return reval;
            }
            return reval;
        }

        internal static MemberExpression ToMemberExpression(Expression parentIdExpression)
        {
            if (parentIdExpression is UnaryExpression)
            {
                return (parentIdExpression as UnaryExpression).Operand as MemberExpression;
            }
            else
            {
                return parentIdExpression as MemberExpression;
            }
        }
    }
}

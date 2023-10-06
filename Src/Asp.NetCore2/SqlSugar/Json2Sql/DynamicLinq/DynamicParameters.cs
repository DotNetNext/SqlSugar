using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar 
{
    public class DynamicParameters
    { 
        public static Dictionary<string, Type> Create(string parameterName, Type parameterType1, string parameterName2, Type parameterType2)
        {
            return new Dictionary<string, Type>() { { parameterName, parameterType1 }, { parameterName2, parameterType2 } };
        }

        public static Dictionary<string, Type> Create(string parameterName, Type parameterType1, string parameterName2, Type parameterType2, string parameterName3, Type parameterType3)
        {
            return new Dictionary<string, Type>() { { parameterName, parameterType1 }, { parameterName2, parameterType2 }, { parameterName3, parameterType3 } };
        }

        public static Dictionary<string, Type> Create(string parameterName, Type parameterType1, string parameterName2, Type parameterType2, string parameterName3, Type parameterType3, string parameterName4, Type parameterType4)
        {
            return new Dictionary<string, Type>() { { parameterName, parameterType1 }, { parameterName2, parameterType2 }, { parameterName3, parameterType3 }, { parameterName4, parameterType4 } };
        }

        public static Dictionary<string, Type> Create(string parameterName, Type parameterType1, string parameterName2, Type parameterType2, string parameterName3, Type parameterType3, string parameterName4, Type parameterType4, string parameterName5, Type parameterType5)
        {
            return new Dictionary<string, Type>() { { parameterName, parameterType1 }, { parameterName2, parameterType2 }, { parameterName3, parameterType3 }, { parameterName4, parameterType4 }, { parameterName5, parameterType5 } };
        }

        public static Dictionary<string, Type> Create(string parameterName, Type parameterType1, string parameterName2, Type parameterType2, string parameterName3, Type parameterType3, string parameterName4, Type parameterType4, string parameterName5, Type parameterType5, string parameterName6, Type parameterType6)
        {
            return new Dictionary<string, Type>() { { parameterName, parameterType1 }, { parameterName2, parameterType2 }, { parameterName3, parameterType3 }, { parameterName4, parameterType4 }, { parameterName5, parameterType5 }, { parameterName6, parameterType6 } };
        }

        public static Dictionary<string, Type> Create(string parameterName, Type parameterType1, string parameterName2, Type parameterType2, string parameterName3, Type parameterType3, string parameterName4, Type parameterType4, string parameterName5, Type parameterType5, string parameterName6, Type parameterType6, string parameterName7, Type parameterType7)
        {
            return new Dictionary<string, Type>() { { parameterName, parameterType1 }, { parameterName2, parameterType2 }, { parameterName3, parameterType3 }, { parameterName4, parameterType4 }, { parameterName5, parameterType5 }, { parameterName6, parameterType6 }, { parameterName7, parameterType7 } };
        }

        public static Dictionary<string, Type> Create(string parameterName, Type parameterType1, string parameterName2, Type parameterType2, string parameterName3, Type parameterType3, string parameterName4, Type parameterType4, string parameterName5, Type parameterType5, string parameterName6, Type parameterType6, string parameterName7, Type parameterType7, string parameterName8, Type parameterType8)
        {
            return new Dictionary<string, Type>() { { parameterName, parameterType1 }, { parameterName2, parameterType2 }, { parameterName3, parameterType3 }, { parameterName4, parameterType4 }, { parameterName5, parameterType5 }, { parameterName6, parameterType6 }, { parameterName7, parameterType7 }, { parameterName8, parameterType8 } };
        }

        public static Dictionary<string, Type> Create(string parameterName, Type parameterType1, string parameterName2, Type parameterType2, string parameterName3, Type parameterType3, string parameterName4, Type parameterType4, string parameterName5, Type parameterType5, string parameterName6, Type parameterType6, string parameterName7, Type parameterType7, string parameterName8, Type parameterType8, string parameterName9, Type parameterType9)
        {
            return new Dictionary<string, Type>() { { parameterName, parameterType1 }, { parameterName2, parameterType2 }, { parameterName3, parameterType3 }, { parameterName4, parameterType4 }, { parameterName5, parameterType5 }, { parameterName6, parameterType6 }, { parameterName7, parameterType7 }, { parameterName8, parameterType8 }, { parameterName9, parameterType9 } };
        }

        public static Dictionary<string, Type> Create(string parameterName, Type parameterType1, string parameterName2, Type parameterType2, string parameterName3, Type parameterType3, string parameterName4, Type parameterType4, string parameterName5, Type parameterType5, string parameterName6, Type parameterType6, string parameterName7, Type parameterType7, string parameterName8, Type parameterType8, string parameterName9, Type parameterType9, string parameterName10, Type parameterType10)
        {
            return new Dictionary<string, Type>() { { parameterName, parameterType1 }, { parameterName2, parameterType2 }, { parameterName3, parameterType3 }, { parameterName4, parameterType4 }, { parameterName5, parameterType5 }, { parameterName6, parameterType6 }, { parameterName7, parameterType7 }, { parameterName8, parameterType8 }, { parameterName9, parameterType9 }, { parameterName10, parameterType10 } };
        }
    }
}

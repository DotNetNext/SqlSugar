using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SqlSugar
{
    /// <summary>
    /// SqlSugarTool局部类与Core有差异的部分
    /// </summary>
    public partial class SqlSugarTool
    {
        private static void FillValueTypeToDictionary<T>(Type type, IDataReader dr, List<T> strReval)
        {
            using (IDataReader re = dr)
            {
                Dictionary<string, string> reval = new Dictionary<string, string>();
                while (re.Read())
                {
                    if (SqlSugarTool.DicOO == type)
                    {
                        var kv = new KeyValuePair<object, object>((object)Convert.ChangeType(re.GetValue(0), typeof(object)), (int)Convert.ChangeType(re.GetValue(1), typeof(object)));
                        strReval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<object, object>)));
                    }
                    else if (SqlSugarTool.Dicii == type)
                    {
                        var kv = new KeyValuePair<int, int>((int)Convert.ChangeType(re.GetValue(0), typeof(int)), (int)Convert.ChangeType(re.GetValue(1), typeof(int)));
                        strReval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<int, int>)));
                    }
                    else if (SqlSugarTool.DicSi == type)
                    {
                        var kv = new KeyValuePair<string, int>((string)Convert.ChangeType(re.GetValue(0), typeof(string)), (int)Convert.ChangeType(re.GetValue(1), typeof(int)));
                        strReval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, int>)));
                    }
                    else if (SqlSugarTool.DicSo == type)
                    {
                        var kv = new KeyValuePair<string, object>((string)Convert.ChangeType(re.GetValue(0), typeof(string)), (object)Convert.ChangeType(re.GetValue(1), typeof(object)));
                        strReval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, object>)));
                    }
                    else if (SqlSugarTool.DicSS == type)
                    {
                        var kv = new KeyValuePair<string, string>((string)Convert.ChangeType(re.GetValue(0), typeof(string)), (string)Convert.ChangeType(re.GetValue(1), typeof(string)));
                        strReval.Add((T)Convert.ChangeType(kv, typeof(KeyValuePair<string, string>)));
                    }
                    else
                    {
                        Check.Exception(true, "暂时不支持该类型的Dictionary 你可以试试 Dictionary<string ,string>或者联系作者！！");
                    }
                }
            }
        }
        private static void FillValueTypeToArray<T>(Type type, IDataReader dr, List<T> strReval)
        {
            using (IDataReader re = dr)
            {
                int count = dr.FieldCount;
                var childType = type.GetElementType();
                while (re.Read())
                {
                    object[] array = new object[count];
                    for (int i = 0; i < count; i++)
                    {
                        array[i] = Convert.ChangeType(re.GetValue(i), childType);
                    }
                    if (childType == SqlSugarTool.StringType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => (string)it).ToArray(), type));
                    else if (childType == SqlSugarTool.ObjType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => (object)it).ToArray(), type));
                    else if (childType == SqlSugarTool.BoolType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => (bool)it).ToArray(), type));
                    else if (childType == SqlSugarTool.ByteType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => (byte)it).ToArray(), type));
                    else if (childType == SqlSugarTool.DecType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => (decimal)it).ToArray(), type));
                    else if (childType == SqlSugarTool.GuidType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => (Guid)it).ToArray(), type));
                    else if (childType == SqlSugarTool.DateType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => (DateTime)it).ToArray(), type));
                    else if (childType == SqlSugarTool.IntType)
                        strReval.Add((T)Convert.ChangeType(array.Select(it => (int)it).ToArray(), type));
                    else
                        Check.Exception(true, "暂时不支持该类型的Array 你可以试试 object[] 或者联系作者！！");
                }
            }
        }
    }
}

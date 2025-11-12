using SqlSugar;
using System;
using System.Collections.Generic;

namespace OrmTest
{
    /// <summary>
    /// UtilConvert utility class tests
    /// UtilConvert 实用工具类测试
    /// </summary>
    public class UnitUtilConvert
    {
        public static void Init()
        {
            Console.WriteLine("=== UtilConvert Tests / UtilConvert 测试 ===");

            // Test ObjToInt / 测试 ObjToInt
            TestObjToInt();

            // Test ObjToLong / 测试 ObjToLong
            TestObjToLong();

            // Test ObjToString / 测试 ObjToString
            TestObjToString();

            // Test ObjToDecimal / 测试 ObjToDecimal
            TestObjToDecimal();

            // Test ObjToMoney / 测试 ObjToMoney
            TestObjToMoney();

            // Test ObjToDate / 测试 ObjToDate
            TestObjToDate();

            // Test ObjToBool / 测试 ObjToBool
            TestObjToBool();

            // Test EqualCase / 测试 EqualCase
            TestEqualCase();

            // Test Edge Cases / 测试边界情况
            TestEdgeCases();

            Console.WriteLine("=== All UtilConvert Tests Passed! / 所有 UtilConvert 测试通过！ ===");
        }

        #region ObjToInt Tests / ObjToInt 测试

        private static void TestObjToInt()
        {
            // Test 1: Valid integer / 有效整数
            object value1 = 123;
            Check.Exception(value1.ObjToInt() == 123, "ObjToInt with valid integer");

            // Test 2: Valid string / 有效字符串
            object value2 = "456";
            Check.Exception(value2.ObjToInt() == 456, "ObjToInt with valid string");

            // Test 3: Null value / 空值
            object value3 = null;
            Check.Exception(value3.ObjToInt() == 0, "ObjToInt with null");

            // Test 4: DBNull value / DBNull 值
            object value4 = DBNull.Value;
            Check.Exception(value4.ObjToInt() == 0, "ObjToInt with DBNull");

            // Test 5: Invalid string / 无效字符串
            object value5 = "invalid";
            Check.Exception(value5.ObjToInt() == 0, "ObjToInt with invalid string");

            // Test 6: Enum value / 枚举值
            object value6 = DayOfWeek.Monday;
            Check.Exception(value6.ObjToInt() == 1, "ObjToInt with enum");

            // Test 7: With error value - valid / 带错误值 - 有效
            object value7 = 789;
            Check.Exception(value7.ObjToInt(-1) == 789, "ObjToInt with error value (valid)");

            // Test 8: With error value - invalid / 带错误值 - 无效
            object value8 = "invalid";
            Check.Exception(value8.ObjToInt(-1) == -1, "ObjToInt with error value (invalid)");

            // Test 9: Max value / 最大值
            object value9 = int.MaxValue.ToString();
            Check.Exception(value9.ObjToInt() == int.MaxValue, "ObjToInt with max value");

            // Test 10: Min value / 最小值
            object value10 = int.MinValue.ToString();
            Check.Exception(value10.ObjToInt() == int.MinValue, "ObjToInt with min value");
        }

        #endregion

        #region ObjToLong Tests / ObjToLong 测试

        private static void TestObjToLong()
        {
            // Test 1: Valid long / 有效长整数
            object value1 = 9876543210L;
            Check.Exception(value1.ObjToLong() == 9876543210L, "ObjToLong with valid long");

            // Test 2: Valid string / 有效字符串
            object value2 = "1234567890";
            Check.Exception(value2.ObjToLong() == 1234567890L, "ObjToLong with valid string");

            // Test 3: Null value / 空值
            object value3 = null;
            Check.Exception(value3.ObjToLong() == 0L, "ObjToLong with null");

            // Test 4: Enum value / 枚举值
            object value4 = DayOfWeek.Friday;
            Check.Exception(value4.ObjToLong() == 5L, "ObjToLong with enum");

            // Test 5: Max value / 最大值
            object value5 = long.MaxValue.ToString();
            Check.Exception(value5.ObjToLong() == long.MaxValue, "ObjToLong with max value");
        }

        #endregion

        #region ObjToString Tests / ObjToString 测试

        private static void TestObjToString()
        {
            // Test 1: Valid string with whitespace / 带空格的有效字符串
            object value1 = "  Hello World  ";
            Check.Exception(value1.ObjToString() == "Hello World", "ObjToString with whitespace");

            // Test 2: Null value / 空值
            object value2 = null;
            Check.Exception(value2.ObjToString() == string.Empty, "ObjToString with null");

            // Test 3: Integer / 整数
            object value3 = 12345;
            Check.Exception(value3.ObjToString() == "12345", "ObjToString with integer");

            // Test 4: With error value - valid / 带错误值 - 有效
            object value4 = "test";
            Check.Exception(value4.ObjToString("default") == "test", "ObjToString with error value (valid)");

            // Test 5: With error value - null / 带错误值 - 空值
            object value5 = null;
            Check.Exception(value5.ObjToString("default") == "default", "ObjToString with error value (null)");

            // Test 6: No trim / 不修剪
            object value6 = "  Hello  ";
            Check.Exception(value6.ObjToStringNoTrim() == "  Hello  ", "ObjToStringNoTrim preserves whitespace");

            // Test 7: Empty string / 空字符串
            object value7 = string.Empty;
            Check.Exception(value7.ObjToString() == string.Empty, "ObjToString with empty string");
        }

        #endregion

        #region ObjToDecimal Tests / ObjToDecimal 测试

        private static void TestObjToDecimal()
        {
            // Test 1: Valid decimal / 有效小数
            object value1 = 123.45m;
            Check.Exception(value1.ObjToDecimal() == 123.45m, "ObjToDecimal with valid decimal");

            // Test 2: Valid string / 有效字符串
            object value2 = "678.90";
            Check.Exception(value2.ObjToDecimal() == 678.90m, "ObjToDecimal with valid string");

            // Test 3: Null value / 空值
            object value3 = null;
            Check.Exception(value3.ObjToDecimal() == 0m, "ObjToDecimal with null");

            // Test 4: DBNull value / DBNull 值
            object value4 = DBNull.Value;
            Check.Exception(value4.ObjToDecimal() == 0m, "ObjToDecimal with DBNull");

            // Test 5: With error value / 带错误值
            object value5 = "invalid";
            Check.Exception(value5.ObjToDecimal(-1.5m) == -1.5m, "ObjToDecimal with error value");

            // Test 6: Max value / 最大值
            object value6 = decimal.MaxValue;
            Check.Exception(value6.ObjToDecimal() == decimal.MaxValue, "ObjToDecimal with max value");

            // Test 7: Negative value / 负值
            object value7 = -123.45m;
            Check.Exception(value7.ObjToDecimal() == -123.45m, "ObjToDecimal with negative value");
        }

        #endregion

        #region ObjToMoney Tests / ObjToMoney 测试

        private static void TestObjToMoney()
        {
            // Test 1: Valid double / 有效双精度
            object value1 = 999.99;
            Check.Exception(value1.ObjToMoney() == 999.99, "ObjToMoney with valid double");

            // Test 2: Valid string / 有效字符串
            object value2 = "1234.56";
            Check.Exception(value2.ObjToMoney() == 1234.56, "ObjToMoney with valid string");

            // Test 3: Null value / 空值
            object value3 = null;
            Check.Exception(value3.ObjToMoney() == 0.0, "ObjToMoney with null");

            // Test 4: With error value / 带错误值
            object value4 = "invalid";
            Check.Exception(value4.ObjToMoney(-100.0) == -100.0, "ObjToMoney with error value");

            // Test 5: Zero value / 零值
            object value5 = 0.0;
            Check.Exception(value5.ObjToMoney() == 0.0, "ObjToMoney with zero");
        }

        #endregion

        #region ObjToDate Tests / ObjToDate 测试

        private static void TestObjToDate()
        {
            // Test 1: Valid DateTime / 有效日期时间
            var expectedDate = new DateTime(2024, 1, 15);
            object value1 = expectedDate;
            Check.Exception(value1.ObjToDate() == expectedDate, "ObjToDate with valid DateTime");

            // Test 2: Valid string / 有效字符串
            object value2 = "2024-06-30";
            Check.Exception(value2.ObjToDate() == new DateTime(2024, 6, 30), "ObjToDate with valid string");

            // Test 3: Null value / 空值
            object value3 = null;
            Check.Exception(value3.ObjToDate() == DateTime.MinValue, "ObjToDate with null");

            // Test 4: DBNull value / DBNull 值
            object value4 = DBNull.Value;
            Check.Exception(value4.ObjToDate() == DateTime.MinValue, "ObjToDate with DBNull");

            // Test 5: With error value / 带错误值
            object value5 = "invalid";
            var errorDate = new DateTime(2000, 1, 1);
            Check.Exception(value5.ObjToDate(errorDate) == errorDate, "ObjToDate with error value");
        }

        #endregion

        #region ObjToBool Tests / ObjToBool 测试

        private static void TestObjToBool()
        {
            // Test 1: True string / True 字符串
            object value1 = "true";
            Check.Exception(value1.ObjToBool() == true, "ObjToBool with 'true' string");

            // Test 2: False string / False 字符串
            object value2 = "false";
            Check.Exception(value2.ObjToBool() == false, "ObjToBool with 'false' string");

            // Test 3: True boolean / True 布尔值
            object value3 = true;
            Check.Exception(value3.ObjToBool() == true, "ObjToBool with true boolean");

            // Test 4: False boolean / False 布尔值
            object value4 = false;
            Check.Exception(value4.ObjToBool() == false, "ObjToBool with false boolean");

            // Test 5: Null value / 空值
            object value5 = null;
            Check.Exception(value5.ObjToBool() == false, "ObjToBool with null");

            // Test 6: Invalid string / 无效字符串
            object value6 = "invalid";
            Check.Exception(value6.ObjToBool() == false, "ObjToBool with invalid string");
        }

        #endregion

        #region EqualCase Tests / EqualCase 测试

        private static void TestEqualCase()
        {
            // Test 1: Same case / 相同大小写
            string value1 = "Hello";
            string value2 = "Hello";
            Check.Exception(value1.EqualCase(value2) == true, "EqualCase with same case");

            // Test 2: Different case / 不同大小写
            string value3 = "Hello";
            string value4 = "hello";
            Check.Exception(value3.EqualCase(value4) == true, "EqualCase with different case");

            // Test 3: Mixed case / 混合大小写
            string value5 = "HeLLo WoRLd";
            string value6 = "hello world";
            Check.Exception(value5.EqualCase(value6) == true, "EqualCase with mixed case");

            // Test 4: Different strings / 不同字符串
            string value7 = "Hello";
            string value8 = "World";
            Check.Exception(value7.EqualCase(value8) == false, "EqualCase with different strings");

            // Test 5: Both null / 都为空
            string value9 = null;
            string value10 = null;
            Check.Exception(value9.EqualCase(value10) == true, "EqualCase with both null");

            // Test 6: One null / 一个为空
            string value11 = "Hello";
            string value12 = null;
            Check.Exception(value11.EqualCase(value12) == false, "EqualCase with one null");
        }

        #endregion

        #region Edge Cases / 边界情况测试

        private static void TestEdgeCases()
        {
            // Test 1: Large numbers / 大数字
            object largeInt = int.MaxValue;
            Check.Exception(largeInt.ObjToInt() == int.MaxValue, "Edge case: int.MaxValue");

            // Test 2: Small numbers / 小数字
            object smallInt = int.MinValue;
            Check.Exception(smallInt.ObjToInt() == int.MinValue, "Edge case: int.MinValue");

            // Test 3: Very long number / 非常长的数字
            object longValue = long.MaxValue;
            Check.Exception(longValue.ObjToLong() == long.MaxValue, "Edge case: long.MaxValue");

            // Test 4: Very large decimal / 非常大的小数
            object decimalValue = decimal.MaxValue;
            Check.Exception(decimalValue.ObjToDecimal() == decimal.MaxValue, "Edge case: decimal.MaxValue");

            // Test 5: Negative decimal / 负小数
            object negativeDecimal = -999.99m;
            Check.Exception(negativeDecimal.ObjToDecimal() == -999.99m, "Edge case: negative decimal");

            // Test 6: Zero values / 零值
            object zeroInt = 0;
            Check.Exception(zeroInt.ObjToInt() == 0, "Edge case: zero int");

            object zeroDecimal = 0m;
            Check.Exception(zeroDecimal.ObjToDecimal() == 0m, "Edge case: zero decimal");
        }

        #endregion
    }
}

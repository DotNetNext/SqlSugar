using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OrmTest
{
    /// <summary>
    /// Comprehensive UtilMethods utility class tests
    /// UtilMethods 实用工具类综合测试
    /// </summary>
    public class UnitUtilMethods
    {
        public static void Init()
        {
            Console.WriteLine("=== UtilMethods Comprehensive Tests / UtilMethods 综合测试 ===");

            // Array and List Tests / 数组和列表测试
            TestIsArrayOrList();
            TestConvertToArray();

            // String Manipulation Tests / 字符串操作测试
            TestToUnderLine();
            TestEscapeLikeValue();
            TestReplaceFirstMatch();
            TestStringCheckFirstAndLast();

            // Type Checking Tests / 类型检查测试
            TestIsKeyValuePairType();
            TestIsTuple();
            TestIsValueTypeArray();
            TestIsNumberArray();

            // Conversion Tests / 转换测试
            TestConvertToObjectList();
            TestConvertToListOfObjects();

            // Utility Tests / 实用工具测试
            TestCountSubstringOccurrences();
            TestIsDefaultValue();
            TestGetMinDate();
            TestGetUnderType();
            TestGetDefaultValue();

            // Parameter Validation Tests / 参数验证测试
            TestNoErrorParameter();
            TestIsErrorParameterName();

            // Data Structure Tests / 数据结构测试
            TestDataRowToDictionary();
            TestBuildTree();

            // Edge Cases / 边界情况测试
            TestEdgeCases();

            Console.WriteLine("=== All UtilMethods Tests Passed! / 所有 UtilMethods 测试通过！ ===");
        }

        #region Array and List Tests / 数组和列表测试

        private static void TestIsArrayOrList()
        {
            Console.WriteLine("Testing IsArrayOrList...");

            // Test arrays / 测试数组
            Check.Exception(UtilMethods.IsArrayOrList(typeof(int[])) == true, "int[] should be array");
            Check.Exception(UtilMethods.IsArrayOrList(typeof(string[])) == true, "string[] should be array");

            // Test lists / 测试列表
            Check.Exception(UtilMethods.IsArrayOrList(typeof(List<int>)) == true, "List<int> should be list");
            Check.Exception(UtilMethods.IsArrayOrList(typeof(List<string>)) == true, "List<string> should be list");

            // Test non-collections / 测试非集合类型
            Check.Exception(UtilMethods.IsArrayOrList(typeof(int)) == false, "int should not be array/list");
            Check.Exception(UtilMethods.IsArrayOrList(typeof(string)) == false, "string should not be array/list");
            Check.Exception(UtilMethods.IsArrayOrList(null) == false, "null should return false");

            Console.WriteLine("IsArrayOrList tests passed!");
        }

        private static void TestConvertToArray()
        {
            Console.WriteLine("Testing ConvertToArray...");

            // Test int array conversion / 测试整数数组转换
            var intArray = UtilMethods.ConvertToArray("[1,2,3,4,5]", typeof(int[])) as int[];
            Check.Exception(intArray != null && intArray.Length == 5, "Should convert to int array");
            Check.Exception(intArray[0] == 1 && intArray[4] == 5, "Array values should match");

            // Test string array conversion / 测试字符串数组转换
            var strArray = UtilMethods.ConvertToArray("[\"a\",\"b\",\"c\"]", typeof(string[])) as string[];
            Check.Exception(strArray != null && strArray.Length == 3, "Should convert to string array");
            Check.Exception(strArray[0] == "a" && strArray[2] == "c", "String array values should match");

            // Test List<int> conversion / 测试 List<int> 转换
            var intList = UtilMethods.ConvertToArray("[10,20,30]", typeof(List<int>)) as List<int>;
            Check.Exception(intList != null && intList.Count == 3, "Should convert to List<int>");
            Check.Exception(intList[0] == 10 && intList[2] == 30, "List values should match");

            // Test empty input / 测试空输入
            var emptyArray = UtilMethods.ConvertToArray("", typeof(int[])) as int[];
            Check.Exception(emptyArray != null && emptyArray.Length == 0, "Empty input should return empty array");

            Console.WriteLine("ConvertToArray tests passed!");
        }

        #endregion

        #region String Manipulation Tests / 字符串操作测试

        private static void TestToUnderLine()
        {
            Console.WriteLine("Testing ToUnderLine...");

            // Test camelCase to snake_case / 测试驼峰转下划线
            Check.Exception(UtilMethods.ToUnderLine("UserName") == "user_name", "UserName should convert to user_name");
            Check.Exception(UtilMethods.ToUnderLine("userId") == "user_id", "userId should convert to user_id");
            Check.Exception(UtilMethods.ToUnderLine("ID") == "i_d", "ID should convert to i_d");

            // Test already underscored / 测试已有下划线
            Check.Exception(UtilMethods.ToUnderLine("user_name") == "user_name", "user_name should stay the same");

            // Test single word / 测试单词
            Check.Exception(UtilMethods.ToUnderLine("User") == "user", "User should convert to user");

            Console.WriteLine("ToUnderLine tests passed!");
        }

        private static void TestEscapeLikeValue()
        {
            Console.WriteLine("Testing EscapeLikeValue...");

            // Test basic escaping / 测试基本转义
            string result1 = UtilMethods.EscapeLikeValue("test%value");
            Check.Exception(result1.Contains("\\%") || result1.Contains("[%]"), "Should escape % character");

            string result2 = UtilMethods.EscapeLikeValue("test_value");
            Check.Exception(result2.Contains("\\_") || result2.Contains("[_]"), "Should escape _ character");

            // Test multiple special chars / 测试多个特殊字符
            string result3 = UtilMethods.EscapeLikeValue("test%_value");
            Check.Exception(result3.Length > "test%_value".Length, "Should escape both % and _");

            // Test normal string / 测试普通字符串
            string result4 = UtilMethods.EscapeLikeValue("normalvalue");
            Check.Exception(result4 == "normalvalue", "Normal string should not change");

            Console.WriteLine("EscapeLikeValue tests passed!");
        }

        private static void TestReplaceFirstMatch()
        {
            Console.WriteLine("Testing ReplaceFirstMatch...");

            // Test basic replacement / 测试基本替换
            string result1 = UtilMethods.ReplaceFirstMatch("hello world hello", "hello", "hi");
            Check.Exception(result1 == "hi world hello", "Should replace only first occurrence");

            // Test no match / 测试无匹配
            string result2 = UtilMethods.ReplaceFirstMatch("hello world", "goodbye", "hi");
            Check.Exception(result2 == "hello world", "Should return original if no match");

            // Test empty strings / 测试空字符串
            string result3 = UtilMethods.ReplaceFirstMatch("", "test", "replace");
            Check.Exception(result3 == "", "Empty string should stay empty");

            Console.WriteLine("ReplaceFirstMatch tests passed!");
        }

        private static void TestStringCheckFirstAndLast()
        {
            Console.WriteLine("Testing StringCheckFirstAndLast...");

            // Test matching first and last / 测试首尾匹配
            Check.Exception(UtilMethods.StringCheckFirstAndLast("(test)", '(', ')') == true, "Should match parentheses");
            Check.Exception(UtilMethods.StringCheckFirstAndLast("[test]", '[', ']') == true, "Should match brackets");
            Check.Exception(UtilMethods.StringCheckFirstAndLast("\"test\"", '"', '"') == true, "Should match quotes");

            // Test non-matching / 测试不匹配
            Check.Exception(UtilMethods.StringCheckFirstAndLast("(test]", '(', ')') == false, "Should not match mismatched brackets");
            Check.Exception(UtilMethods.StringCheckFirstAndLast("test", '(', ')') == false, "Should not match without brackets");

            Console.WriteLine("StringCheckFirstAndLast tests passed!");
        }

        #endregion

        #region Type Checking Tests / 类型检查测试

        private static void TestIsKeyValuePairType()
        {
            Console.WriteLine("Testing IsKeyValuePairType...");

            // Test KeyValuePair types / 测试 KeyValuePair 类型
            Check.Exception(UtilMethods.IsKeyValuePairType(typeof(KeyValuePair<int, string>)) == true, 
                "KeyValuePair<int, string> should be recognized");
            Check.Exception(UtilMethods.IsKeyValuePairType(typeof(KeyValuePair<string, object>)) == true, 
                "KeyValuePair<string, object> should be recognized");

            // Test non-KeyValuePair types / 测试非 KeyValuePair 类型
            Check.Exception(UtilMethods.IsKeyValuePairType(typeof(Dictionary<int, string>)) == false, 
                "Dictionary should not be KeyValuePair");
            Check.Exception(UtilMethods.IsKeyValuePairType(typeof(int)) == false, 
                "int should not be KeyValuePair");

            Console.WriteLine("IsKeyValuePairType tests passed!");
        }

        private static void TestIsTuple()
        {
            Console.WriteLine("Testing IsTuple...");

            // Test tuple types / 测试元组类型
            Check.Exception(UtilMethods.IsTuple(typeof(Tuple<int>)) == true, "Tuple<int> should be tuple");
            Check.Exception(UtilMethods.IsTuple(typeof(Tuple<int, string>)) == true, "Tuple<int, string> should be tuple");
            Check.Exception(UtilMethods.IsTuple(typeof(ValueTuple<int, string>)) == true, "ValueTuple should be tuple");

            // Test non-tuple types / 测试非元组类型
            Check.Exception(UtilMethods.IsTuple(typeof(int)) == false, "int should not be tuple");
            Check.Exception(UtilMethods.IsTuple(typeof(List<int>)) == false, "List should not be tuple");

            Console.WriteLine("IsTuple tests passed!");
        }

        private static void TestIsValueTypeArray()
        {
            Console.WriteLine("Testing IsValueTypeArray...");

            // Test value type arrays / 测试值类型数组
            Check.Exception(UtilMethods.IsValueTypeArray(typeof(int[])) == true, "int[] should be value type array");
            Check.Exception(UtilMethods.IsValueTypeArray(typeof(double[])) == true, "double[] should be value type array");
            Check.Exception(UtilMethods.IsValueTypeArray(typeof(bool[])) == true, "bool[] should be value type array");

            // Test reference type arrays / 测试引用类型数组
            Check.Exception(UtilMethods.IsValueTypeArray(typeof(string[])) == false, "string[] should not be value type array");
            Check.Exception(UtilMethods.IsValueTypeArray(typeof(object[])) == false, "object[] should not be value type array");

            // Test non-arrays / 测试非数组
            Check.Exception(UtilMethods.IsValueTypeArray(typeof(int)) == false, "int should not be array");

            Console.WriteLine("IsValueTypeArray tests passed!");
        }

        private static void TestIsNumberArray()
        {
            Console.WriteLine("Testing IsNumberArray...");

            // Test number arrays / 测试数字数组
            Check.Exception(UtilMethods.IsNumberArray(typeof(int[])) == true, "int[] should be number array");
            Check.Exception(UtilMethods.IsNumberArray(typeof(double[])) == true, "double[] should be number array");
            Check.Exception(UtilMethods.IsNumberArray(typeof(decimal[])) == true, "decimal[] should be number array");
            Check.Exception(UtilMethods.IsNumberArray(typeof(long[])) == true, "long[] should be number array");

            // Test non-number arrays / 测试非数字数组
            Check.Exception(UtilMethods.IsNumberArray(typeof(string[])) == false, "string[] should not be number array");
            Check.Exception(UtilMethods.IsNumberArray(typeof(bool[])) == false, "bool[] should not be number array");

            Console.WriteLine("IsNumberArray tests passed!");
        }

        #endregion

        #region Conversion Tests / 转换测试

        private static void TestConvertToObjectList()
        {
            Console.WriteLine("Testing ConvertToObjectList...");

            // Test array to object list / 测试数组转对象列表
            var intArray = new int[] { 1, 2, 3, 4, 5 };
            var objectList = UtilMethods.ConvertToObjectList(intArray);
            Check.Exception(objectList != null && objectList.Count == 5, "Should convert array to object list");
            Check.Exception((int)objectList[0] == 1, "First element should be 1");

            // Test list to object list / 测试列表转对象列表
            var stringList = new List<string> { "a", "b", "c" };
            var objectList2 = UtilMethods.ConvertToObjectList(stringList);
            Check.Exception(objectList2 != null && objectList2.Count == 3, "Should convert list to object list");
            Check.Exception((string)objectList2[0] == "a", "First element should be 'a'");

            Console.WriteLine("ConvertToObjectList tests passed!");
        }

        private static void TestConvertToListOfObjects()
        {
            Console.WriteLine("Testing ConvertToListOfObjects...");

            // Test array conversion / 测试数组转换
            var intArray = new int[] { 10, 20, 30 };
            var result = UtilMethods.ConvertToListOfObjects(intArray);
            Check.Exception(result != null && result.Count == 3, "Should convert to list of objects");
            Check.Exception((int)result[1] == 20, "Second element should be 20");

            // Test list conversion / 测试列表转换
            var doubleList = new List<double> { 1.1, 2.2, 3.3 };
            var result2 = UtilMethods.ConvertToListOfObjects(doubleList);
            Check.Exception(result2 != null && result2.Count == 3, "Should convert list to list of objects");

            Console.WriteLine("ConvertToListOfObjects tests passed!");
        }

        #endregion

        #region Utility Tests / 实用工具测试

        private static void TestCountSubstringOccurrences()
        {
            Console.WriteLine("Testing CountSubstringOccurrences...");

            // Test counting occurrences / 测试计数出现次数
            Check.Exception(UtilMethods.CountSubstringOccurrences("hello world hello", "hello") == 2, 
                "Should count 2 occurrences of 'hello'");
            Check.Exception(UtilMethods.CountSubstringOccurrences("test test test", "test") == 3, 
                "Should count 3 occurrences of 'test'");
            Check.Exception(UtilMethods.CountSubstringOccurrences("no match here", "xyz") == 0, 
                "Should count 0 occurrences when no match");

            // Test empty strings / 测试空字符串
            Check.Exception(UtilMethods.CountSubstringOccurrences("", "test") == 0, 
                "Empty string should have 0 occurrences");

            Console.WriteLine("CountSubstringOccurrences tests passed!");
        }

        private static void TestIsDefaultValue()
        {
            Console.WriteLine("Testing IsDefaultValue...");

            // Test default values / 测试默认值
            Check.Exception(UtilMethods.IsDefaultValue(0) == true, "0 should be default for int");
            Check.Exception(UtilMethods.IsDefaultValue(false) == true, "false should be default for bool");
            Check.Exception(UtilMethods.IsDefaultValue(null) == true, "null should be default");
            Check.Exception(UtilMethods.IsDefaultValue(default(DateTime)) == true, "default DateTime should be default");

            // Test non-default values / 测试非默认值
            Check.Exception(UtilMethods.IsDefaultValue(1) == false, "1 should not be default");
            Check.Exception(UtilMethods.IsDefaultValue(true) == false, "true should not be default");
            Check.Exception(UtilMethods.IsDefaultValue("test") == false, "non-empty string should not be default");

            Console.WriteLine("IsDefaultValue tests passed!");
        }

        private static void TestGetMinDate()
        {
            Console.WriteLine("Testing GetMinDate...");

            // Test getting min date / 测试获取最小日期
            var minDate = UtilMethods.GetMinDate(DbType.SqlServer);
            Check.Exception(minDate != null, "Should return a min date");
            Check.Exception(minDate.Year == 1753, "SQL Server min date should be 1753-01-01");

            Console.WriteLine("GetMinDate tests passed!");
        }

        private static void TestGetUnderType()
        {
            Console.WriteLine("Testing GetUnderType...");

            // Test nullable types / 测试可空类型
            Check.Exception(UtilMethods.GetUnderType(typeof(int?)) == typeof(int), 
                "int? should return int");
            Check.Exception(UtilMethods.GetUnderType(typeof(DateTime?)) == typeof(DateTime), 
                "DateTime? should return DateTime");

            // Test non-nullable types / 测试非可空类型
            Check.Exception(UtilMethods.GetUnderType(typeof(int)) == typeof(int), 
                "int should return int");
            Check.Exception(UtilMethods.GetUnderType(typeof(string)) == typeof(string), 
                "string should return string");

            Console.WriteLine("GetUnderType tests passed!");
        }

        private static void TestGetDefaultValue()
        {
            Console.WriteLine("Testing GetDefaultValue...");

            // Test default values for various types / 测试各种类型的默认值
            Check.Exception((int)UtilMethods.GetDefaultValue(typeof(int)) == 0, "int default should be 0");
            Check.Exception((bool)UtilMethods.GetDefaultValue(typeof(bool)) == false, "bool default should be false");
            Check.Exception(UtilMethods.GetDefaultValue(typeof(string)) == null, "string default should be null");

            Console.WriteLine("GetDefaultValue tests passed!");
        }

        #endregion

        #region Parameter Validation Tests / 参数验证测试

        private static void TestNoErrorParameter()
        {
            Console.WriteLine("Testing NoErrorParameter...");

            // Test valid parameters / 测试有效参数
            Check.Exception(UtilMethods.NoErrorParameter("validParam") == true, "validParam should be valid");
            Check.Exception(UtilMethods.NoErrorParameter("param123") == true, "param123 should be valid");
            Check.Exception(UtilMethods.NoErrorParameter("_param") == true, "_param should be valid");

            // Test invalid parameters / 测试无效参数
            Check.Exception(UtilMethods.NoErrorParameter("param with space") == false, "param with space should be invalid");
            Check.Exception(UtilMethods.NoErrorParameter("param(test)") == false, "param with parentheses should be invalid");
            Check.Exception(UtilMethods.NoErrorParameter("param.test") == false, "param with dot should be invalid");
            Check.Exception(UtilMethods.NoErrorParameter(null) == false, "null should be invalid");

            Console.WriteLine("NoErrorParameter tests passed!");
        }

        private static void TestIsErrorParameterName()
        {
            Console.WriteLine("Testing IsErrorParameterName...");

            // Test error parameter names / 测试错误参数名
            Check.Exception(UtilMethods.IsErrorParameterName("param with space") == true, 
                "param with space should be error");
            Check.Exception(UtilMethods.IsErrorParameterName("param(test)") == true, 
                "param with parentheses should be error");

            // Test valid parameter names / 测试有效参数名
            Check.Exception(UtilMethods.IsErrorParameterName("validParam") == false, 
                "validParam should not be error");
            Check.Exception(UtilMethods.IsErrorParameterName("param123") == false, 
                "param123 should not be error");

            Console.WriteLine("IsErrorParameterName tests passed!");
        }

        #endregion

        #region Data Structure Tests / 数据结构测试

        private static void TestDataRowToDictionary()
        {
            Console.WriteLine("Testing DataRowToDictionary...");

            // Create test DataTable / 创建测试 DataTable
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Age", typeof(int));

            DataRow row = dt.NewRow();
            row["Id"] = 1;
            row["Name"] = "Test User";
            row["Age"] = 25;
            dt.Rows.Add(row);

            // Test conversion / 测试转换
            var dict = UtilMethods.DataRowToDictionary(row);
            Check.Exception(dict != null && dict.Count == 3, "Should convert DataRow to dictionary");
            Check.Exception((int)dict["Id"] == 1, "Id should be 1");
            Check.Exception((string)dict["Name"] == "Test User", "Name should be 'Test User'");
            Check.Exception((int)dict["Age"] == 25, "Age should be 25");

            Console.WriteLine("DataRowToDictionary tests passed!");
        }

        private static void TestBuildTree()
        {
            Console.WriteLine("Testing BuildTree...");

            // Create test data / 创建测试数据
            var items = new List<TreeNode>
            {
                new TreeNode { Id = 1, ParentId = 0, Name = "Root" },
                new TreeNode { Id = 2, ParentId = 1, Name = "Child1" },
                new TreeNode { Id = 3, ParentId = 1, Name = "Child2" },
                new TreeNode { Id = 4, ParentId = 2, Name = "GrandChild" }
            };

            // Test tree building / 测试树构建
            var tree = UtilMethods.BuildTree(items, 
                item => item.Id, 
                item => item.ParentId, 
                0, 
                (parent, children) => parent.Children = children);

            Check.Exception(tree != null && tree.Count > 0, "Should build tree structure");
            Check.Exception(tree[0].Id == 1, "Root should be first");
            Check.Exception(tree[0].Children != null && tree[0].Children.Count == 2, "Root should have 2 children");

            Console.WriteLine("BuildTree tests passed!");
        }

        // Helper class for tree testing / 树测试辅助类
        private class TreeNode
        {
            public int Id { get; set; }
            public int ParentId { get; set; }
            public string Name { get; set; }
            public List<TreeNode> Children { get; set; }
        }

        #endregion

        #region Edge Cases / 边界情况测试

        private static void TestEdgeCases()
        {
            Console.WriteLine("Testing Edge Cases...");

            // Test null handling / 测试空值处理
            Check.Exception(UtilMethods.IsArrayOrList(null) == false, "null type should return false");
            Check.Exception(UtilMethods.NoErrorParameter(null) == false, "null parameter should be invalid");

            // Test empty string handling / 测试空字符串处理
            Check.Exception(UtilMethods.CountSubstringOccurrences("", "test") == 0, 
                "Empty string should have 0 occurrences");
            Check.Exception(UtilMethods.ReplaceFirstMatch("", "old", "new") == "", 
                "Empty string replacement should return empty");

            // Test type edge cases / 测试类型边界情况
            Check.Exception(UtilMethods.GetUnderType(typeof(int)) == typeof(int), 
                "Non-nullable type should return itself");
            Check.Exception(UtilMethods.IsDefaultValue(default(int)) == true, 
                "Default int should be recognized");

            Console.WriteLine("Edge cases tests passed!");
        }

        #endregion
    }
}

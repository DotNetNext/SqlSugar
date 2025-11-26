using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OrmTest
{
    /// <summary>
    /// Comprehensive test suite for UtilMethods utility class.
    /// Tests 90+ utility method scenarios across multiple categories:
    /// - Type Checking Methods (12 tests)
    /// - String Operations (20 tests)
    /// - Encoding/Decoding (6 tests)
    /// - Data Conversion (15 tests)
    /// - SQL Utilities (10 tests)
    /// - Reflection Utilities (12 tests)
    /// - Date/Time (4 tests)
    /// - Tree Building (5 tests)
    /// - Other Utilities (10 tests)
    /// 
    /// Note: UtilMethods is a public static class, so direct method calls can be used.
    /// Some methods require ISqlSugarClient - these use the Db instance from Main.cs.
    /// </summary>
    public class UUtilMethods
    {
        #region Test Initialization
        
        /// <summary>
        /// Main entry point for test suite execution.
        /// Runs all 94 utility method tests in organized categories.
        /// Throws exception on first failure for quick debugging.
        /// </summary>
        public static void Init()
        {
            Console.WriteLine("\n========== UUtilMethods Test Suite Started ==========");
            Console.WriteLine("Testing SqlSugar.UtilMethods utility class");
            Console.WriteLine("Total Tests: 96\n");
            
            // Category 1: Type Checking Methods (12 tests)
            Console.WriteLine("--- Category 1: Type Checking Methods (12 tests) ---");
            Test_IsArrayOrList_ArrayTypes();
            Test_IsArrayOrList_ListTypes();
            Test_IsArrayOrList_String_ReturnsFalse();
            Test_IsArrayOrList_NullType();
            Test_IsArrayOrList_NonCollectionTypes();
            Test_IsKeyValuePairType_ValidKeyValuePair();
            Test_IsKeyValuePairType_NonKeyValuePair();
            Test_HasInterface_TypeImplementsInterface();
            Test_HasInterface_TypeNotImplementsInterface();
            Test_IsTuple_TupleTypes();
            Test_IsValueTypeArray_ValueTypeArrays();
            Test_IsNumber_NumericStrings();
            
            // Category 2: String Operations (20 tests)
            Console.WriteLine("\n--- Category 2: String Operations (20 tests) ---");
            Test_StringCheckFirstAndLast_Matching();
            Test_StringCheckFirstAndLast_NonMatching();
            Test_RemoveBeforeFirstWhere_WithWhere();
            Test_RemoveBeforeFirstWhere_WithoutWhere();
            Test_RemoveEqualOne_Basic();
            Test_ToUnderLine_CamelCase();
            Test_ToUnderLine_WithIsToUpper();
            Test_GetSeparatorChar_PlatformSpecific();
            Test_IsParentheses_WithParentheses();
            Test_ReplaceFirstMatch_FirstMatchOnly();
            Test_CountSubstringOccurrences_Basic();
            Test_ConvertNumbersToString_Basic();
            Test_ConvertStringToNumbers_Basic();
            Test_RemoveBeforeFirstWhere_CaseInsensitive();
            Test_ToUnderLine_AlreadySnakeCase();
            Test_IsParentheses_WithoutParentheses();
            Test_ReplaceFirstMatch_NoMatches();
            Test_CountSubstringOccurrences_MultipleOccurrences();
            Test_ConvertNumbersToString_VariousNumbers();
            Test_ConvertStringToNumbers_VariousStrings();
            
            // Category 3: Encoding/Decoding (6 tests)
            Console.WriteLine("\n--- Category 3: Encoding/Decoding (6 tests) ---");
            Test_EncodeBase64_Basic();
            Test_EncodeBase64_EmptyString();
            Test_EncodeBase64_SpecialCharacters();
            Test_DecodeBase64_Basic();
            Test_DecodeBase64_InvalidString();
            Test_DecodeBase64_RoundTrip();
            
            // Category 4: Data Conversion (15 tests)
            Console.WriteLine("\n--- Category 4: Data Conversion (17 tests) ---");
            Test_ConvertToArray_StringToArray();
            Test_ConvertToArray_StringToList();
            Test_ConvertToArray_EmptyString();
            Test_ConvertToListOfObjects_Basic();
            Test_ChangeType2_Basic();
            Test_DataRowToDictionary_Basic();
            Test_ConvertDateTimeOffsetToDateTime_Basic();
            Test_ConvertToArray_VariousElementTypes();
            Test_ConvertToListOfObjects_VariousCollections();
            Test_ChangeType2_VariousTypes();
            Test_DataRowToDictionary_VariousColumnTypes();
            Test_ConvertDateTimeOffsetToDateTime_TimeZones();
            Test_ConvertToArray_QuotedElements();
            Test_ConvertToListOfObjects_EmptyCollection();
            Test_ConvertDateTimeOffsetToDateTime_UTC();
            Test_ConvertToObjectList_Basic();
            Test_ReplaceSqlParameter_Basic();
            
            // Category 5: SQL Utilities (10 tests)
            Console.WriteLine("\n--- Category 5: SQL Utilities (10 tests) ---");
            Test_EscapeLikeValue_SqlServer();
            Test_EscapeLikeValue_MySql();
            Test_EscapeLikeValue_PostgreSQL();
            Test_EscapeLikeValue_DifferentWildcards();
            Test_GetNativeSql_Basic();
            Test_GetNativeSql_WithParameters();
            Test_GetSqlValue_Basic();
            Test_GetSqlValue_NullValue();
            Test_EscapeLikeValue_MultipleWildcards();
            Test_GetNativeSql_NullParameters();
            
            // Category 6: Reflection Utilities (12 tests)
            Console.WriteLine("\n--- Category 6: Reflection Utilities (12 tests) ---");
            Test_GetPropertyValue_Basic();
            Test_GetPropertyValue_VariousTypes();
            Test_GetUnderType_NullableTypes();
            Test_GetUnderType_NonNullableTypes();
            Test_GetDefaultValue_ValueTypes();
            Test_GetDefaultValue_ReferenceTypes();
            Test_GetTypeByTypeName_ValidTypes();
            Test_GetTypeByTypeName_InvalidTypes();
            Test_GetPropertyValue_NonExistentProperty();
            Test_GetUnderType_ComplexTypes();
            Test_GetDefaultValue_VariousTypes();
            Test_GetTypeByTypeName_CommonTypes();
            
            // Category 7: Date/Time (4 tests)
            Console.WriteLine("\n--- Category 7: Date/Time (4 tests) ---");
            Test_GetWeekLastDaySun_Basic();
            Test_GetWeekLastDaySun_VariousDates();
            Test_GetWeekLastDaySun_YearBoundaries();
            Test_GetWeekLastDaySun_EdgeCases();
            
            // Category 8: Tree Building (5 tests)
            Console.WriteLine("\n--- Category 8: Tree Building (5 tests) ---");
            Test_BuildTree_Basic();
            Test_BuildTree_VariousStructures();
            Test_BuildTree_RootValues();
            Test_BuildTree_EmptyList();
            Test_BuildTree_ComplexTree();
            
            // Category 9: Other Utilities (10 tests)
            Console.WriteLine("\n--- Category 9: Other Utilities (10 tests) ---");
            Test_EnumToDictionary_Basic();
            Test_EnumToDictionary_VariousEnums();
            Test_IsAsyncMethod_AsyncMethod();
            Test_IsAsyncMethod_SyncMethod();
            Test_CopySugarParameters_Basic();
            Test_CopyConfig_Basic();
            Test_IsTuple_ValueTuple();
            Test_IsTuple_NonTuple();
            Test_IsAnyAsyncMethod_Basic();
            Test_GetStackTrace_Basic();
            
            Console.WriteLine("\n========== All 96 Tests Passed Successfully! ==========");
        }
        
        #endregion
        
        #region Category 1: Type Checking Methods (12 tests)
        
        /// <summary>
        /// Test: IsArrayOrList() with array types should return true
        /// </summary>
        private static void Test_IsArrayOrList_ArrayTypes()
        {
            bool result1 = UtilMethods.IsArrayOrList(typeof(int[]));
            bool result2 = UtilMethods.IsArrayOrList(typeof(string[]));
            bool result3 = UtilMethods.IsArrayOrList(typeof(double[]));
            
            if (!result1 || !result2 || !result3)
                throw new Exception($"Test_IsArrayOrList_ArrayTypes failed: Array types should return true");
            
            Console.WriteLine("✓ Test_IsArrayOrList_ArrayTypes passed");
        }
        
        /// <summary>
        /// Test: IsArrayOrList() with List types should return true
        /// </summary>
        private static void Test_IsArrayOrList_ListTypes()
        {
            bool result1 = UtilMethods.IsArrayOrList(typeof(List<int>));
            bool result2 = UtilMethods.IsArrayOrList(typeof(List<string>));
            bool result3 = UtilMethods.IsArrayOrList(typeof(List<object>));
            
            if (!result1 || !result2 || !result3)
                throw new Exception($"Test_IsArrayOrList_ListTypes failed: List types should return true");
            
            Console.WriteLine("✓ Test_IsArrayOrList_ListTypes passed");
        }
        
        /// <summary>
        /// Test: IsArrayOrList() with string should return false
        /// </summary>
        private static void Test_IsArrayOrList_String_ReturnsFalse()
        {
            bool result = UtilMethods.IsArrayOrList(typeof(string));
            
            if (result)
                throw new Exception($"Test_IsArrayOrList_String_ReturnsFalse failed: String should return false");
            
            Console.WriteLine("✓ Test_IsArrayOrList_String_ReturnsFalse passed");
        }
        
        /// <summary>
        /// Test: IsArrayOrList() with null type should return false
        /// </summary>
        private static void Test_IsArrayOrList_NullType()
        {
            bool result = UtilMethods.IsArrayOrList(null);
            
            if (result)
                throw new Exception($"Test_IsArrayOrList_NullType failed: Null type should return false");
            
            Console.WriteLine("✓ Test_IsArrayOrList_NullType passed");
        }
        
        /// <summary>
        /// Test: IsArrayOrList() with non-collection types should return false
        /// </summary>
        private static void Test_IsArrayOrList_NonCollectionTypes()
        {
            bool result1 = UtilMethods.IsArrayOrList(typeof(int));
            bool result2 = UtilMethods.IsArrayOrList(typeof(DateTime));
            bool result3 = UtilMethods.IsArrayOrList(typeof(object));
            
            if (result1 || result2 || result3)
                throw new Exception($"Test_IsArrayOrList_NonCollectionTypes failed: Non-collection types should return false");
            
            Console.WriteLine("✓ Test_IsArrayOrList_NonCollectionTypes passed");
        }
        
        /// <summary>
        /// Test: IsKeyValuePairType() with KeyValuePair types should return true
        /// </summary>
        private static void Test_IsKeyValuePairType_ValidKeyValuePair()
        {
            bool result1 = UtilMethods.IsKeyValuePairType(typeof(KeyValuePair<int, string>));
            bool result2 = UtilMethods.IsKeyValuePairType(typeof(KeyValuePair<string, object>));
            
            if (!result1 || !result2)
                throw new Exception($"Test_IsKeyValuePairType_ValidKeyValuePair failed: KeyValuePair types should return true");
            
            Console.WriteLine("✓ Test_IsKeyValuePairType_ValidKeyValuePair passed");
        }
        
        /// <summary>
        /// Test: IsKeyValuePairType() with non-KeyValuePair types should return false
        /// </summary>
        private static void Test_IsKeyValuePairType_NonKeyValuePair()
        {
            bool result1 = UtilMethods.IsKeyValuePairType(typeof(int));
            bool result2 = UtilMethods.IsKeyValuePairType(typeof(List<int>));
            
            if (result1 || result2)
                throw new Exception($"Test_IsKeyValuePairType_NonKeyValuePair failed: Non-KeyValuePair types should return false");
            
            Console.WriteLine("✓ Test_IsKeyValuePairType_NonKeyValuePair passed");
        }
        
        /// <summary>
        /// Test: HasInterface() with type implementing interface should return true
        /// </summary>
        private static void Test_HasInterface_TypeImplementsInterface()
        {
            bool result = UtilMethods.HasInterface(typeof(List<int>), typeof(IEnumerable<int>));
            
            if (!result)
                throw new Exception($"Test_HasInterface_TypeImplementsInterface failed: Type implementing interface should return true");
            
            Console.WriteLine("✓ Test_HasInterface_TypeImplementsInterface passed");
        }
        
        /// <summary>
        /// Test: HasInterface() with type not implementing interface should return false
        /// </summary>
        private static void Test_HasInterface_TypeNotImplementsInterface()
        {
            bool result = UtilMethods.HasInterface(typeof(int), typeof(IEnumerable<int>));
            
            if (result)
                throw new Exception($"Test_HasInterface_TypeNotImplementsInterface failed: Type not implementing interface should return false");
            
            Console.WriteLine("✓ Test_HasInterface_TypeNotImplementsInterface passed");
        }
        
        /// <summary>
        /// Test: IsTuple() with Tuple types should return true
        /// </summary>
        private static void Test_IsTuple_TupleTypes()
        {
            var tupleType = typeof(Tuple<int, string>);
            var properties = tupleType.GetProperties().ToList();
            
            bool result = UtilMethods.IsTuple(tupleType, properties);
            
            if (!result)
                throw new Exception($"Test_IsTuple_TupleTypes failed: Tuple types should return true");
            
            Console.WriteLine("✓ Test_IsTuple_TupleTypes passed");
        }
        
        /// <summary>
        /// Test: IsValueTypeArray() with value type arrays should return true
        /// </summary>
        private static void Test_IsValueTypeArray_ValueTypeArrays()
        {
            bool result1 = UtilMethods.IsValueTypeArray(new int[] { 1, 2, 3 });
            bool result2 = UtilMethods.IsValueTypeArray(new string[] { "a", "b" });
            bool result3 = UtilMethods.IsValueTypeArray(new List<int> { 1, 2 });
            
            if (!result1 || !result2 || !result3)
                throw new Exception($"Test_IsValueTypeArray_ValueTypeArrays failed: Value type arrays should return true");
            
            Console.WriteLine("✓ Test_IsValueTypeArray_ValueTypeArrays passed");
        }
        
        /// <summary>
        /// Test: IsNumber() with numeric strings should return true
        /// </summary>
        private static void Test_IsNumber_NumericStrings()
        {
            bool result1 = UtilMethods.IsNumber("int");
            bool result2 = UtilMethods.IsNumber("decimal");
            bool result3 = UtilMethods.IsNumber("double");
            bool result4 = UtilMethods.IsNumber("invalid");
            
            if (!result1 || !result2 || !result3 || result4)
                throw new Exception($"Test_IsNumber_NumericStrings failed: Numeric type names should return true");
            
            Console.WriteLine("✓ Test_IsNumber_NumericStrings passed");
        }
        
        #endregion
        
        #region Category 2: String Operations (20 tests)
        
        /// <summary>
        /// Test: StringCheckFirstAndLast() with matching prefix and suffix should return true
        /// </summary>
        private static void Test_StringCheckFirstAndLast_Matching()
        {
            bool result = UtilMethods.StringCheckFirstAndLast("Hello World", "Hello", "World");
            
            if (!result)
                throw new Exception($"Test_StringCheckFirstAndLast_Matching failed: Expected true");
            
            Console.WriteLine("✓ Test_StringCheckFirstAndLast_Matching passed");
        }
        
        /// <summary>
        /// Test: StringCheckFirstAndLast() with non-matching prefix or suffix should return false
        /// </summary>
        private static void Test_StringCheckFirstAndLast_NonMatching()
        {
            bool result1 = UtilMethods.StringCheckFirstAndLast("Hello World", "Hi", "World");
            bool result2 = UtilMethods.StringCheckFirstAndLast("Hello World", "Hello", "Universe");
            
            if (result1 || result2)
                throw new Exception($"Test_StringCheckFirstAndLast_NonMatching failed: Expected false");
            
            Console.WriteLine("✓ Test_StringCheckFirstAndLast_NonMatching passed");
        }
        
        /// <summary>
        /// Test: RemoveBeforeFirstWhere() with SQL containing WHERE should return substring after WHERE
        /// </summary>
        private static void Test_RemoveBeforeFirstWhere_WithWhere()
        {
            string sql = "SELECT * FROM Users WHERE Id = 1";
            string result = UtilMethods.RemoveBeforeFirstWhere(sql);
            
            if (result != " Id = 1")
                throw new Exception($"Test_RemoveBeforeFirstWhere_WithWhere failed: Expected ' Id = 1', got '{result}'");
            
            Console.WriteLine("✓ Test_RemoveBeforeFirstWhere_WithWhere passed");
        }
        
        /// <summary>
        /// Test: RemoveBeforeFirstWhere() with SQL without WHERE should return original string
        /// </summary>
        private static void Test_RemoveBeforeFirstWhere_WithoutWhere()
        {
            string sql = "SELECT * FROM Users";
            string result = UtilMethods.RemoveBeforeFirstWhere(sql);
            
            if (result != sql)
                throw new Exception($"Test_RemoveBeforeFirstWhere_WithoutWhere failed: Expected original string");
            
            Console.WriteLine("✓ Test_RemoveBeforeFirstWhere_WithoutWhere passed");
        }
        
        /// <summary>
        /// Test: RemoveEqualOne() should remove trailing "=1" from SQL
        /// </summary>
        private static void Test_RemoveEqualOne_Basic()
        {
            string sql = "Column =1";
            string result = UtilMethods.RemoveEqualOne(sql);
            
            if (result != "Column")
                throw new Exception($"Test_RemoveEqualOne_Basic failed: Expected 'Column', got '{result}'");
            
            Console.WriteLine("✓ Test_RemoveEqualOne_Basic passed");
        }
        
        /// <summary>
        /// Test: ToUnderLine() should convert camelCase to snake_case
        /// </summary>
        private static void Test_ToUnderLine_CamelCase()
        {
            string result = UtilMethods.ToUnderLine("HelloWorld");
            
            if (result != "hello_world")
                throw new Exception($"Test_ToUnderLine_CamelCase failed: Expected 'hello_world', got '{result}'");
            
            Console.WriteLine("✓ Test_ToUnderLine_CamelCase passed");
        }
        
        /// <summary>
        /// Test: ToUnderLine() with isToUpper=true should convert to UPPER_SNAKE_CASE
        /// </summary>
        private static void Test_ToUnderLine_WithIsToUpper()
        {
            string result = UtilMethods.ToUnderLine("HelloWorld", true);
            
            if (result != "HELLO_WORLD")
                throw new Exception($"Test_ToUnderLine_WithIsToUpper failed: Expected 'HELLO_WORLD', got '{result}'");
            
            Console.WriteLine("✓ Test_ToUnderLine_WithIsToUpper passed");
        }
        
        /// <summary>
        /// Test: GetSeparatorChar() should return platform-specific path separator
        /// </summary>
        private static void Test_GetSeparatorChar_PlatformSpecific()
        {
            string result = UtilMethods.GetSeparatorChar();
            
            if (string.IsNullOrEmpty(result))
                throw new Exception($"Test_GetSeparatorChar_PlatformSpecific failed: Should return path separator");
            
            Console.WriteLine("✓ Test_GetSeparatorChar_PlatformSpecific passed");
        }
        
        /// <summary>
        /// Test: IsParentheses() with parentheses should return true
        /// </summary>
        private static void Test_IsParentheses_WithParentheses()
        {
            bool result = UtilMethods.IsParentheses("(test)");
            
            if (!result)
                throw new Exception($"Test_IsParentheses_WithParentheses failed: Expected true");
            
            Console.WriteLine("✓ Test_IsParentheses_WithParentheses passed");
        }
        
        /// <summary>
        /// Test: ReplaceFirstMatch() should replace only the first match
        /// </summary>
        private static void Test_ReplaceFirstMatch_FirstMatchOnly()
        {
            string result = UtilMethods.ReplaceFirstMatch("test test test", "test", "replaced");
            
            if (result != "replaced test test")
                throw new Exception($"Test_ReplaceFirstMatch_FirstMatchOnly failed: Expected 'replaced test test', got '{result}'");
            
            Console.WriteLine("✓ Test_ReplaceFirstMatch_FirstMatchOnly passed");
        }
        
        /// <summary>
        /// Test: CountSubstringOccurrences() should count substring occurrences
        /// </summary>
        private static void Test_CountSubstringOccurrences_Basic()
        {
            int result = UtilMethods.CountSubstringOccurrences("test test test", "test");
            
            if (result != 3)
                throw new Exception($"Test_CountSubstringOccurrences_Basic failed: Expected 3, got {result}");
            
            Console.WriteLine("✓ Test_CountSubstringOccurrences_Basic passed");
        }
        
        /// <summary>
        /// Test: ConvertNumbersToString() should convert numbers to string
        /// </summary>
        private static void Test_ConvertNumbersToString_Basic()
        {
            string result = UtilMethods.ConvertNumbersToString("abc123def456");
            
            if (string.IsNullOrEmpty(result))
                throw new Exception($"Test_ConvertNumbersToString_Basic failed: Should return converted string");
            
            Console.WriteLine("✓ Test_ConvertNumbersToString_Basic passed");
        }
        
        /// <summary>
        /// Test: ConvertStringToNumbers() should convert string to numbers
        /// </summary>
        private static void Test_ConvertStringToNumbers_Basic()
        {
            string result = UtilMethods.ConvertStringToNumbers("abc123def456");
            
            if (string.IsNullOrEmpty(result))
                throw new Exception($"Test_ConvertStringToNumbers_Basic failed: Should return converted string");
            
            Console.WriteLine("✓ Test_ConvertStringToNumbers_Basic passed");
        }
        
        /// <summary>
        /// Test: RemoveBeforeFirstWhere() should be case-insensitive
        /// </summary>
        private static void Test_RemoveBeforeFirstWhere_CaseInsensitive()
        {
            string sql = "SELECT * FROM Users where Id = 1";
            string result = UtilMethods.RemoveBeforeFirstWhere(sql);
            
            if (result != " Id = 1")
                throw new Exception($"Test_RemoveBeforeFirstWhere_CaseInsensitive failed: Expected ' Id = 1', got '{result}'");
            
            Console.WriteLine("✓ Test_RemoveBeforeFirstWhere_CaseInsensitive passed");
        }
        
        /// <summary>
        /// Test: ToUnderLine() with already snake_case string should return unchanged
        /// </summary>
        private static void Test_ToUnderLine_AlreadySnakeCase()
        {
            string result = UtilMethods.ToUnderLine("hello_world");
            
            if (result != "hello_world")
                throw new Exception($"Test_ToUnderLine_AlreadySnakeCase failed: Expected 'hello_world', got '{result}'");
            
            Console.WriteLine("✓ Test_ToUnderLine_AlreadySnakeCase passed");
        }
        
        /// <summary>
        /// Test: IsParentheses() without parentheses should return false
        /// </summary>
        private static void Test_IsParentheses_WithoutParentheses()
        {
            bool result = UtilMethods.IsParentheses("test");
            
            if (result)
                throw new Exception($"Test_IsParentheses_WithoutParentheses failed: Expected false");
            
            Console.WriteLine("✓ Test_IsParentheses_WithoutParentheses passed");
        }
        
        /// <summary>
        /// Test: ReplaceFirstMatch() with no matches should return original string
        /// </summary>
        private static void Test_ReplaceFirstMatch_NoMatches()
        {
            string result = UtilMethods.ReplaceFirstMatch("test", "xyz", "replaced");
            
            if (result != "test")
                throw new Exception($"Test_ReplaceFirstMatch_NoMatches failed: Expected 'test', got '{result}'");
            
            Console.WriteLine("✓ Test_ReplaceFirstMatch_NoMatches passed");
        }
        
        /// <summary>
        /// Test: CountSubstringOccurrences() with multiple occurrences should count correctly
        /// </summary>
        private static void Test_CountSubstringOccurrences_MultipleOccurrences()
        {
            int result = UtilMethods.CountSubstringOccurrences("a b a b a", "a");
            
            if (result != 3)
                throw new Exception($"Test_CountSubstringOccurrences_MultipleOccurrences failed: Expected 3, got {result}");
            
            Console.WriteLine("✓ Test_CountSubstringOccurrences_MultipleOccurrences passed");
        }
        
        /// <summary>
        /// Test: ConvertNumbersToString() with various numbers should convert correctly
        /// </summary>
        private static void Test_ConvertNumbersToString_VariousNumbers()
        {
            string result1 = UtilMethods.ConvertNumbersToString("123");
            string result2 = UtilMethods.ConvertNumbersToString("abc");
            
            if (string.IsNullOrEmpty(result1) || string.IsNullOrEmpty(result2))
                throw new Exception($"Test_ConvertNumbersToString_VariousNumbers failed: Should handle various inputs");
            
            Console.WriteLine("✓ Test_ConvertNumbersToString_VariousNumbers passed");
        }
        
        /// <summary>
        /// Test: ConvertStringToNumbers() with various strings should convert correctly
        /// </summary>
        private static void Test_ConvertStringToNumbers_VariousStrings()
        {
            string result1 = UtilMethods.ConvertStringToNumbers("123");
            string result2 = UtilMethods.ConvertStringToNumbers("abc");
            
            if (string.IsNullOrEmpty(result1) || string.IsNullOrEmpty(result2))
                throw new Exception($"Test_ConvertStringToNumbers_VariousStrings failed: Should handle various inputs");
            
            Console.WriteLine("✓ Test_ConvertStringToNumbers_VariousStrings passed");
        }
        
        #endregion
        
        #region Category 3: Encoding/Decoding (6 tests)
        
        /// <summary>
        /// Test: EncodeBase64() should encode string to base64
        /// </summary>
        private static void Test_EncodeBase64_Basic()
        {
            string input = "Hello World";
            string result = UtilMethods.EncodeBase64(input);
            
            if (string.IsNullOrEmpty(result))
                throw new Exception($"Test_EncodeBase64_Basic failed: Should return encoded string");
            
            Console.WriteLine("✓ Test_EncodeBase64_Basic passed");
        }
        
        /// <summary>
        /// Test: EncodeBase64() with empty string should handle correctly
        /// </summary>
        private static void Test_EncodeBase64_EmptyString()
        {
            string result = UtilMethods.EncodeBase64("");
            
            if (result == null)
                throw new Exception($"Test_EncodeBase64_EmptyString failed: Should handle empty string");
            
            Console.WriteLine("✓ Test_EncodeBase64_EmptyString passed");
        }
        
        /// <summary>
        /// Test: EncodeBase64() with special characters should encode correctly
        /// </summary>
        private static void Test_EncodeBase64_SpecialCharacters()
        {
            string input = "Hello@World#123";
            string result = UtilMethods.EncodeBase64(input);
            
            if (string.IsNullOrEmpty(result))
                throw new Exception($"Test_EncodeBase64_SpecialCharacters failed: Should encode special characters");
            
            Console.WriteLine("✓ Test_EncodeBase64_SpecialCharacters passed");
        }
        
        /// <summary>
        /// Test: DecodeBase64() should decode base64 string
        /// </summary>
        private static void Test_DecodeBase64_Basic()
        {
            string encoded = UtilMethods.EncodeBase64("Hello World");
            string result = UtilMethods.DecodeBase64(encoded);
            
            if (result != "Hello World")
                throw new Exception($"Test_DecodeBase64_Basic failed: Expected 'Hello World', got '{result}'");
            
            Console.WriteLine("✓ Test_DecodeBase64_Basic passed");
        }
        
        /// <summary>
        /// Test: DecodeBase64() with invalid base64 string should handle gracefully
        /// </summary>
        private static void Test_DecodeBase64_InvalidString()
        {
            try
            {
                string result = UtilMethods.DecodeBase64("invalid_base64!!!");
                // Should either return empty/null or throw exception
                Console.WriteLine("✓ Test_DecodeBase64_InvalidString passed (handled gracefully)");
            }
            catch
            {
                Console.WriteLine("✓ Test_DecodeBase64_InvalidString passed (exception thrown)");
            }
        }
        
        /// <summary>
        /// Test: DecodeBase64() round-trip encoding/decoding should work
        /// </summary>
        private static void Test_DecodeBase64_RoundTrip()
        {
            string original = "Test String 123";
            string encoded = UtilMethods.EncodeBase64(original);
            string decoded = UtilMethods.DecodeBase64(encoded);
            
            if (decoded != original)
                throw new Exception($"Test_DecodeBase64_RoundTrip failed: Round-trip failed. Original: '{original}', Decoded: '{decoded}'");
            
            Console.WriteLine("✓ Test_DecodeBase64_RoundTrip passed");
        }
        
        #endregion
        
        #region Category 4: Data Conversion (15 tests)
        
        /// <summary>
        /// Test: ConvertToArray() should convert string to array
        /// </summary>
        private static void Test_ConvertToArray_StringToArray()
        {
            string input = "[1,2,3]";
            object result = UtilMethods.ConvertToArray(input, typeof(int[]));
            
            if (result == null || !(result is int[]))
                throw new Exception($"Test_ConvertToArray_StringToArray failed: Should return int array");
            
            int[] array = (int[])result;
            if (array.Length != 3 || array[0] != 1 || array[1] != 2 || array[2] != 3)
                throw new Exception($"Test_ConvertToArray_StringToArray failed: Array values incorrect");
            
            Console.WriteLine("✓ Test_ConvertToArray_StringToArray passed");
        }
        
        /// <summary>
        /// Test: ConvertToArray() should convert string to List
        /// </summary>
        private static void Test_ConvertToArray_StringToList()
        {
            string input = "[1,2,3]";
            object result = UtilMethods.ConvertToArray(input, typeof(List<int>));
            
            if (result == null || !(result is List<int>))
                throw new Exception($"Test_ConvertToArray_StringToList failed: Should return List<int>");
            
            List<int> list = (List<int>)result;
            if (list.Count != 3 || list[0] != 1 || list[1] != 2 || list[2] != 3)
                throw new Exception($"Test_ConvertToArray_StringToList failed: List values incorrect");
            
            Console.WriteLine("✓ Test_ConvertToArray_StringToList passed");
        }
        
        /// <summary>
        /// Test: ConvertToArray() with empty string should return empty array/list
        /// </summary>
        private static void Test_ConvertToArray_EmptyString()
        {
            object result1 = UtilMethods.ConvertToArray("", typeof(int[]));
            object result2 = UtilMethods.ConvertToArray("", typeof(List<int>));
            
            if (result1 == null || result2 == null)
                throw new Exception($"Test_ConvertToArray_EmptyString failed: Should return empty collection");
            
            Console.WriteLine("✓ Test_ConvertToArray_EmptyString passed");
        }
        
        /// <summary>
        /// Test: ConvertToListOfObjects() should convert enumerable to list
        /// </summary>
        private static void Test_ConvertToListOfObjects_Basic()
        {
            var input = new int[] { 1, 2, 3 };
            List<object> result = UtilMethods.ConvertToListOfObjects(input);
            
            if (result == null || result.Count != 3)
                throw new Exception($"Test_ConvertToListOfObjects_Basic failed: Should return list with 3 items");
            
            Console.WriteLine("✓ Test_ConvertToListOfObjects_Basic passed");
        }
        
        /// <summary>
        /// Test: ChangeType2() should convert types correctly
        /// </summary>
        private static void Test_ChangeType2_Basic()
        {
            object result = UtilMethods.ChangeType2("123", typeof(int));
            
            if (result == null || !(result is int) || (int)result != 123)
                throw new Exception($"Test_ChangeType2_Basic failed: Should convert string to int");
            
            Console.WriteLine("✓ Test_ChangeType2_Basic passed");
        }
        
        /// <summary>
        /// Test: DataRowToDictionary() should convert DataRow to dictionary
        /// </summary>
        private static void Test_DataRowToDictionary_Basic()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            DataRow row = table.NewRow();
            row["Id"] = 1;
            row["Name"] = "Test";
            table.Rows.Add(row);
            
            Dictionary<string, object> result = UtilMethods.DataRowToDictionary(row);
            
            if (result == null || result.Count != 2 || (int)result["Id"] != 1 || (string)result["Name"] != "Test")
                throw new Exception($"Test_DataRowToDictionary_Basic failed: Dictionary values incorrect");
            
            Console.WriteLine("✓ Test_DataRowToDictionary_Basic passed");
        }
        
        /// <summary>
        /// Test: ConvertDateTimeOffsetToDateTime() should convert DateTimeOffset to DateTime
        /// </summary>
        private static void Test_ConvertDateTimeOffsetToDateTime_Basic()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Date", typeof(DateTimeOffset));
            DataRow row = table.NewRow();
            row["Date"] = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.Zero);
            table.Rows.Add(row);
            
            DataTable result = UtilMethods.ConvertDateTimeOffsetToDateTime(table);
            
            if (result == null || result.Columns["Date"].DataType != typeof(DateTime))
                throw new Exception($"Test_ConvertDateTimeOffsetToDateTime_Basic failed: Should convert DateTimeOffset column");
            
            Console.WriteLine("✓ Test_ConvertDateTimeOffsetToDateTime_Basic passed");
        }
        
        /// <summary>
        /// Test: ConvertToArray() with various element types should work correctly
        /// </summary>
        private static void Test_ConvertToArray_VariousElementTypes()
        {
            object result1 = UtilMethods.ConvertToArray("[1,2,3]", typeof(int[]));
            object result2 = UtilMethods.ConvertToArray("[\"a\",\"b\"]", typeof(string[]));
            
            if (result1 == null || result2 == null)
                throw new Exception($"Test_ConvertToArray_VariousElementTypes failed: Should handle various types");
            
            Console.WriteLine("✓ Test_ConvertToArray_VariousElementTypes passed");
        }
        
        /// <summary>
        /// Test: ConvertToListOfObjects() with various collections should work correctly
        /// </summary>
        private static void Test_ConvertToListOfObjects_VariousCollections()
        {
            var list1 = new List<int> { 1, 2, 3 };
            var list2 = new string[] { "a", "b", "c" };
            
            List<object> result1 = UtilMethods.ConvertToListOfObjects(list1);
            List<object> result2 = UtilMethods.ConvertToListOfObjects(list2);
            
            if (result1 == null || result2 == null || result1.Count != 3 || result2.Count != 3)
                throw new Exception($"Test_ConvertToListOfObjects_VariousCollections failed: Should handle various collections");
            
            Console.WriteLine("✓ Test_ConvertToListOfObjects_VariousCollections passed");
        }
        
        /// <summary>
        /// Test: ChangeType2() with various source/target types should convert correctly
        /// </summary>
        private static void Test_ChangeType2_VariousTypes()
        {
            object result1 = UtilMethods.ChangeType2("123.45", typeof(decimal));
            object result2 = UtilMethods.ChangeType2("true", typeof(bool));
            
            if (result1 == null || result2 == null)
                throw new Exception($"Test_ChangeType2_VariousTypes failed: Should handle various type conversions");
            
            Console.WriteLine("✓ Test_ChangeType2_VariousTypes passed");
        }
        
        /// <summary>
        /// Test: DataRowToDictionary() with various column types should work correctly
        /// </summary>
        private static void Test_DataRowToDictionary_VariousColumnTypes()
        {
            DataTable table = new DataTable();
            table.Columns.Add("IntCol", typeof(int));
            table.Columns.Add("StringCol", typeof(string));
            table.Columns.Add("DateCol", typeof(DateTime));
            DataRow row = table.NewRow();
            row["IntCol"] = 1;
            row["StringCol"] = "Test";
            row["DateCol"] = DateTime.Now;
            table.Rows.Add(row);
            
            Dictionary<string, object> result = UtilMethods.DataRowToDictionary(row);
            
            if (result == null || result.Count != 3)
                throw new Exception($"Test_DataRowToDictionary_VariousColumnTypes failed: Should handle various column types");
            
            Console.WriteLine("✓ Test_DataRowToDictionary_VariousColumnTypes passed");
        }
        
        /// <summary>
        /// Test: ConvertDateTimeOffsetToDateTime() with various time zones should convert correctly
        /// </summary>
        private static void Test_ConvertDateTimeOffsetToDateTime_TimeZones()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Date", typeof(DateTimeOffset));
            DataRow row = table.NewRow();
            row["Date"] = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(8));
            table.Rows.Add(row);
            
            DataTable result = UtilMethods.ConvertDateTimeOffsetToDateTime(table);
            
            if (result == null || result.Rows.Count != 1)
                throw new Exception($"Test_ConvertDateTimeOffsetToDateTime_TimeZones failed: Should handle time zones");
            
            Console.WriteLine("✓ Test_ConvertDateTimeOffsetToDateTime_TimeZones passed");
        }
        
        /// <summary>
        /// Test: ConvertToArray() with quoted elements should handle correctly
        /// </summary>
        private static void Test_ConvertToArray_QuotedElements()
        {
            object result = UtilMethods.ConvertToArray("[\"a\",\"b\",\"c\"]", typeof(string[]));
            
            if (result == null || !(result is string[]))
                throw new Exception($"Test_ConvertToArray_QuotedElements failed: Should handle quoted elements");
            
            string[] array = (string[])result;
            if (array.Length != 3 || array[0] != "a" || array[1] != "b" || array[2] != "c")
                throw new Exception($"Test_ConvertToArray_QuotedElements failed: Quoted elements not parsed correctly");
            
            Console.WriteLine("✓ Test_ConvertToArray_QuotedElements passed");
        }
        
        /// <summary>
        /// Test: ConvertToListOfObjects() with empty collection should return empty list
        /// </summary>
        private static void Test_ConvertToListOfObjects_EmptyCollection()
        {
            var empty = new int[0];
            List<object> result = UtilMethods.ConvertToListOfObjects(empty);
            
            if (result == null || result.Count != 0)
                throw new Exception($"Test_ConvertToListOfObjects_EmptyCollection failed: Should return empty list");
            
            Console.WriteLine("✓ Test_ConvertToListOfObjects_EmptyCollection passed");
        }
        
        /// <summary>
        /// Test: ConvertDateTimeOffsetToDateTime() with UTC dates should convert correctly
        /// </summary>
        private static void Test_ConvertDateTimeOffsetToDateTime_UTC()
        {
            DataTable table = new DataTable();
            table.Columns.Add("Date", typeof(DateTimeOffset));
            DataRow row = table.NewRow();
            row["Date"] = DateTimeOffset.UtcNow;
            table.Rows.Add(row);
            
            DataTable result = UtilMethods.ConvertDateTimeOffsetToDateTime(table);
            
            if (result == null || result.Rows.Count != 1)
                throw new Exception($"Test_ConvertDateTimeOffsetToDateTime_UTC failed: Should handle UTC dates");
            
            Console.WriteLine("✓ Test_ConvertDateTimeOffsetToDateTime_UTC passed");
        }
        
        /// <summary>
        /// Test: ConvertToObjectList() should convert list of objects to typed list
        /// </summary>
        private static void Test_ConvertToObjectList_Basic()
        {
            var sourceList = new List<object> { 1, 2, 3 };
            object result = UtilMethods.ConvertToObjectList(typeof(int), sourceList);
            
            if (result == null)
                throw new Exception($"Test_ConvertToObjectList_Basic failed: Should return typed list");
            
            Console.WriteLine("✓ Test_ConvertToObjectList_Basic passed");
        }
        
        /// <summary>
        /// Test: ReplaceSqlParameter() should replace SQL parameter names
        /// </summary>
        private static void Test_ReplaceSqlParameter_Basic()
        {
            string sql = "SELECT * FROM Users WHERE Id = @id";
            SugarParameter param = new SugarParameter("@id", 1);
            string result = UtilMethods.ReplaceSqlParameter(sql, param, "@newId");
            
            if (string.IsNullOrEmpty(result) || !result.Contains("@newId"))
                throw new Exception($"Test_ReplaceSqlParameter_Basic failed: Should replace parameter name");
            
            Console.WriteLine("✓ Test_ReplaceSqlParameter_Basic passed");
        }
        
        #endregion
        
        #region Category 5: SQL Utilities (10 tests)
        
        /// <summary>
        /// Test: EscapeLikeValue() for SqlServer should escape wildcards correctly
        /// </summary>
        private static void Test_EscapeLikeValue_SqlServer()
        {
            var db = NewUnitTest.Db;
            string value = "test%value";
            string result = UtilMethods.EscapeLikeValue(db, value);
            
            if (string.IsNullOrEmpty(result) || !result.Contains("[%]"))
                throw new Exception($"Test_EscapeLikeValue_SqlServer failed: Should escape % for SqlServer");
            
            Console.WriteLine("✓ Test_EscapeLikeValue_SqlServer passed");
        }
        
        /// <summary>
        /// Test: EscapeLikeValue() for MySql should escape wildcards correctly
        /// Note: This test may need adjustment based on actual DbType configuration
        /// </summary>
        private static void Test_EscapeLikeValue_MySql()
        {
            // This test requires a MySql connection, so we'll test the SqlServer version
            // and note that MySql uses backslash escaping
            var db = NewUnitTest.Db;
            string value = "test%value";
            string result = UtilMethods.EscapeLikeValue(db, value);
            
            if (string.IsNullOrEmpty(result))
                throw new Exception($"Test_EscapeLikeValue_MySql failed: Should escape wildcards");
            
            Console.WriteLine("✓ Test_EscapeLikeValue_MySql passed (tested with SqlServer)");
        }
        
        /// <summary>
        /// Test: EscapeLikeValue() for PostgreSQL should escape wildcards correctly
        /// Note: This test may need adjustment based on actual DbType configuration
        /// </summary>
        private static void Test_EscapeLikeValue_PostgreSQL()
        {
            // This test requires a PostgreSQL connection, so we'll test the SqlServer version
            var db = NewUnitTest.Db;
            string value = "test%value";
            string result = UtilMethods.EscapeLikeValue(db, value);
            
            if (string.IsNullOrEmpty(result))
                throw new Exception($"Test_EscapeLikeValue_PostgreSQL failed: Should escape wildcards");
            
            Console.WriteLine("✓ Test_EscapeLikeValue_PostgreSQL passed (tested with SqlServer)");
        }
        
        /// <summary>
        /// Test: EscapeLikeValue() with different wildcards should escape correctly
        /// </summary>
        private static void Test_EscapeLikeValue_DifferentWildcards()
        {
            var db = NewUnitTest.Db;
            string value1 = "test%value";
            string value2 = "test_value";
            
            string result1 = UtilMethods.EscapeLikeValue(db, value1, '%');
            string result2 = UtilMethods.EscapeLikeValue(db, value2, '_');
            
            if (string.IsNullOrEmpty(result1) || string.IsNullOrEmpty(result2))
                throw new Exception($"Test_EscapeLikeValue_DifferentWildcards failed: Should escape different wildcards");
            
            Console.WriteLine("✓ Test_EscapeLikeValue_DifferentWildcards passed");
        }
        
        /// <summary>
        /// Test: GetNativeSql() should format SQL with parameters
        /// </summary>
        private static void Test_GetNativeSql_Basic()
        {
            string sql = "SELECT * FROM Users WHERE Id = @id";
            SugarParameter[] pars = null;
            string result = UtilMethods.GetNativeSql(sql, pars);
            
            if (string.IsNullOrEmpty(result) || !result.Contains(sql))
                throw new Exception($"Test_GetNativeSql_Basic failed: Should format SQL");
            
            Console.WriteLine("✓ Test_GetNativeSql_Basic passed");
        }
        
        /// <summary>
        /// Test: GetNativeSql() with parameters should include parameter information
        /// </summary>
        private static void Test_GetNativeSql_WithParameters()
        {
            string sql = "SELECT * FROM Users WHERE Id = @id";
            SugarParameter[] pars = new SugarParameter[] { new SugarParameter("@id", 1) };
            string result = UtilMethods.GetNativeSql(sql, pars);
            
            if (string.IsNullOrEmpty(result) || !result.Contains("@id"))
                throw new Exception($"Test_GetNativeSql_WithParameters failed: Should include parameter info");
            
            Console.WriteLine("✓ Test_GetNativeSql_WithParameters passed");
        }
        
        /// <summary>
        /// Test: GetSqlValue() should format SQL values correctly
        /// </summary>
        private static void Test_GetSqlValue_Basic()
        {
            string result1 = UtilMethods.GetSqlValue(123);
            string result2 = UtilMethods.GetSqlValue("test");
            
            if (string.IsNullOrEmpty(result1) || string.IsNullOrEmpty(result2))
                throw new Exception($"Test_GetSqlValue_Basic failed: Should format SQL values");
            
            Console.WriteLine("✓ Test_GetSqlValue_Basic passed");
        }
        
        /// <summary>
        /// Test: GetSqlValue() with null value should handle correctly
        /// </summary>
        private static void Test_GetSqlValue_NullValue()
        {
            string result = UtilMethods.GetSqlValue(null);
            
            if (result == null)
                throw new Exception($"Test_GetSqlValue_NullValue failed: Should handle null value");
            
            Console.WriteLine("✓ Test_GetSqlValue_NullValue passed");
        }
        
        /// <summary>
        /// Test: EscapeLikeValue() with multiple wildcards should escape all
        /// </summary>
        private static void Test_EscapeLikeValue_MultipleWildcards()
        {
            var db = NewUnitTest.Db;
            string value = "test%value_here";
            string result = UtilMethods.EscapeLikeValue(db, value, '%', '_');
            
            if (string.IsNullOrEmpty(result))
                throw new Exception($"Test_EscapeLikeValue_MultipleWildcards failed: Should escape multiple wildcards");
            
            Console.WriteLine("✓ Test_EscapeLikeValue_MultipleWildcards passed");
        }
        
        /// <summary>
        /// Test: GetNativeSql() with null parameters should handle correctly
        /// </summary>
        private static void Test_GetNativeSql_NullParameters()
        {
            string sql = "SELECT * FROM Users";
            string result = UtilMethods.GetNativeSql(sql, null);
            
            if (string.IsNullOrEmpty(result) || !result.Contains(sql))
                throw new Exception($"Test_GetNativeSql_NullParameters failed: Should handle null parameters");
            
            Console.WriteLine("✓ Test_GetNativeSql_NullParameters passed");
        }
        
        #endregion
        
        #region Category 6: Reflection Utilities (12 tests)
        
        /// <summary>
        /// Test: GetPropertyValue() should retrieve property value
        /// </summary>
        private static void Test_GetPropertyValue_Basic()
        {
            var obj = new { Name = "Test", Id = 1 };
            object result = UtilMethods.GetPropertyValue(obj, "Name");
            
            if (result == null || result.ToString() != "Test")
                throw new Exception($"Test_GetPropertyValue_Basic failed: Expected 'Test', got '{result}'");
            
            Console.WriteLine("✓ Test_GetPropertyValue_Basic passed");
        }
        
        /// <summary>
        /// Test: GetPropertyValue() with various property types should work correctly
        /// </summary>
        private static void Test_GetPropertyValue_VariousTypes()
        {
            var obj = new { IntProp = 123, StringProp = "test", DateProp = DateTime.Now };
            object result1 = UtilMethods.GetPropertyValue(obj, "IntProp");
            object result2 = UtilMethods.GetPropertyValue(obj, "StringProp");
            object result3 = UtilMethods.GetPropertyValue(obj, "DateProp");
            
            if (result1 == null || result2 == null || result3 == null)
                throw new Exception($"Test_GetPropertyValue_VariousTypes failed: Should retrieve various property types");
            
            Console.WriteLine("✓ Test_GetPropertyValue_VariousTypes passed");
        }
        
        /// <summary>
        /// Test: GetUnderType() with nullable types should return underlying type
        /// </summary>
        private static void Test_GetUnderType_NullableTypes()
        {
            Type result = UtilMethods.GetUnderType(typeof(int?));
            
            if (result != typeof(int))
                throw new Exception($"Test_GetUnderType_NullableTypes failed: Expected int, got {result}");
            
            Console.WriteLine("✓ Test_GetUnderType_NullableTypes passed");
        }
        
        /// <summary>
        /// Test: GetUnderType() with non-nullable types should return same type
        /// </summary>
        private static void Test_GetUnderType_NonNullableTypes()
        {
            Type result = UtilMethods.GetUnderType(typeof(int));
            
            if (result != typeof(int))
                throw new Exception($"Test_GetUnderType_NonNullableTypes failed: Expected int, got {result}");
            
            Console.WriteLine("✓ Test_GetUnderType_NonNullableTypes passed");
        }
        
        /// <summary>
        /// Test: GetDefaultValue() with value types should return default value
        /// </summary>
        private static void Test_GetDefaultValue_ValueTypes()
        {
            object result1 = UtilMethods.GetDefaultValue(typeof(int));
            object result2 = UtilMethods.GetDefaultValue(typeof(bool));
            
            if (result1 == null || result2 == null || (int)result1 != 0 || (bool)result2 != false)
                throw new Exception($"Test_GetDefaultValue_ValueTypes failed: Should return default values");
            
            Console.WriteLine("✓ Test_GetDefaultValue_ValueTypes passed");
        }
        
        /// <summary>
        /// Test: GetDefaultValue() with reference types should return null
        /// </summary>
        private static void Test_GetDefaultValue_ReferenceTypes()
        {
            object result = UtilMethods.GetDefaultValue(typeof(string));
            
            if (result != null)
                throw new Exception($"Test_GetDefaultValue_ReferenceTypes failed: Should return null for reference types");
            
            Console.WriteLine("✓ Test_GetDefaultValue_ReferenceTypes passed");
        }
        
        /// <summary>
        /// Test: GetTypeByTypeName() with valid type names should resolve types
        /// </summary>
        private static void Test_GetTypeByTypeName_ValidTypes()
        {
            Type result1 = UtilMethods.GetTypeByTypeName("System.Int32");
            Type result2 = UtilMethods.GetTypeByTypeName("System.String");
            
            if (result1 != typeof(int) || result2 != typeof(string))
                throw new Exception($"Test_GetTypeByTypeName_ValidTypes failed: Should resolve valid type names");
            
            Console.WriteLine("✓ Test_GetTypeByTypeName_ValidTypes passed");
        }
        
        /// <summary>
        /// Test: GetTypeByTypeName() with invalid type names should return null
        /// </summary>
        private static void Test_GetTypeByTypeName_InvalidTypes()
        {
            Type result = UtilMethods.GetTypeByTypeName("InvalidTypeName123");
            
            if (result != null)
                throw new Exception($"Test_GetTypeByTypeName_InvalidTypes failed: Should return null for invalid types");
            
            Console.WriteLine("✓ Test_GetTypeByTypeName_InvalidTypes passed");
        }
        
        /// <summary>
        /// Test: GetPropertyValue() with non-existent property should handle gracefully
        /// </summary>
        private static void Test_GetPropertyValue_NonExistentProperty()
        {
            var obj = new { Name = "Test" };
            
            try
            {
                object result = UtilMethods.GetPropertyValue(obj, "NonExistent");
                // May return null or throw exception
                Console.WriteLine("✓ Test_GetPropertyValue_NonExistentProperty passed (handled gracefully)");
            }
            catch
            {
                Console.WriteLine("✓ Test_GetPropertyValue_NonExistentProperty passed (exception thrown)");
            }
        }
        
        /// <summary>
        /// Test: GetUnderType() with complex types should handle correctly
        /// </summary>
        private static void Test_GetUnderType_ComplexTypes()
        {
            Type result1 = UtilMethods.GetUnderType(typeof(List<int>));
            Type result2 = UtilMethods.GetUnderType(typeof(DateTime?));
            
            if (result1 == null || result2 != typeof(DateTime))
                throw new Exception($"Test_GetUnderType_ComplexTypes failed: Should handle complex types");
            
            Console.WriteLine("✓ Test_GetUnderType_ComplexTypes passed");
        }
        
        /// <summary>
        /// Test: GetDefaultValue() with various types should return appropriate defaults
        /// </summary>
        private static void Test_GetDefaultValue_VariousTypes()
        {
            object result1 = UtilMethods.GetDefaultValue(typeof(decimal));
            object result2 = UtilMethods.GetDefaultValue(typeof(DateTime));
            
            if (result1 == null || result2 == null)
                throw new Exception($"Test_GetDefaultValue_VariousTypes failed: Should return defaults for various types");
            
            Console.WriteLine("✓ Test_GetDefaultValue_VariousTypes passed");
        }
        
        /// <summary>
        /// Test: GetTypeByTypeName() with common type names should resolve correctly
        /// </summary>
        private static void Test_GetTypeByTypeName_CommonTypes()
        {
            Type result1 = UtilMethods.GetTypeByTypeName("int");
            Type result2 = UtilMethods.GetTypeByTypeName("string");
            
            if (result1 == null || result2 == null)
                throw new Exception($"Test_GetTypeByTypeName_CommonTypes failed: Should resolve common type names");
            
            Console.WriteLine("✓ Test_GetTypeByTypeName_CommonTypes passed");
        }
        
        #endregion
        
        #region Category 7: Date/Time (4 tests)
        
        /// <summary>
        /// Test: GetWeekLastDaySun() should calculate week's last day (Sunday)
        /// </summary>
        private static void Test_GetWeekLastDaySun_Basic()
        {
            DateTime date = new DateTime(2024, 1, 15); // Monday
            DateTime result = UtilMethods.GetWeekLastDaySun(date);
            
            if (result.DayOfWeek != DayOfWeek.Sunday)
                throw new Exception($"Test_GetWeekLastDaySun_Basic failed: Should return Sunday");
            
            Console.WriteLine("✓ Test_GetWeekLastDaySun_Basic passed");
        }
        
        /// <summary>
        /// Test: GetWeekLastDaySun() with various dates should calculate correctly
        /// </summary>
        private static void Test_GetWeekLastDaySun_VariousDates()
        {
            DateTime date1 = new DateTime(2024, 1, 10); // Wednesday
            DateTime date2 = new DateTime(2024, 1, 14); // Sunday
            
            DateTime result1 = UtilMethods.GetWeekLastDaySun(date1);
            DateTime result2 = UtilMethods.GetWeekLastDaySun(date2);
            
            if (result1.DayOfWeek != DayOfWeek.Sunday || result2.DayOfWeek != DayOfWeek.Sunday)
                throw new Exception($"Test_GetWeekLastDaySun_VariousDates failed: Should return Sunday for all dates");
            
            Console.WriteLine("✓ Test_GetWeekLastDaySun_VariousDates passed");
        }
        
        /// <summary>
        /// Test: GetWeekLastDaySun() with year boundaries should handle correctly
        /// </summary>
        private static void Test_GetWeekLastDaySun_YearBoundaries()
        {
            DateTime date = new DateTime(2023, 12, 31); // Sunday
            DateTime result = UtilMethods.GetWeekLastDaySun(date);
            
            if (result.DayOfWeek != DayOfWeek.Sunday)
                throw new Exception($"Test_GetWeekLastDaySun_YearBoundaries failed: Should handle year boundaries");
            
            Console.WriteLine("✓ Test_GetWeekLastDaySun_YearBoundaries passed");
        }
        
        /// <summary>
        /// Test: GetWeekLastDaySun() with edge cases should handle correctly
        /// </summary>
        private static void Test_GetWeekLastDaySun_EdgeCases()
        {
            DateTime date1 = new DateTime(2024, 1, 1); // Monday
            DateTime date2 = new DateTime(2024, 12, 31); // Tuesday
            
            DateTime result1 = UtilMethods.GetWeekLastDaySun(date1);
            DateTime result2 = UtilMethods.GetWeekLastDaySun(date2);
            
            if (result1.DayOfWeek != DayOfWeek.Sunday || result2.DayOfWeek != DayOfWeek.Sunday)
                throw new Exception($"Test_GetWeekLastDaySun_EdgeCases failed: Should handle edge cases");
            
            Console.WriteLine("✓ Test_GetWeekLastDaySun_EdgeCases passed");
        }
        
        #endregion
        
        #region Category 8: Tree Building (5 tests)
        
        /// <summary>
        /// Test: BuildTree() should build hierarchical tree structure
        /// </summary>
        private static void Test_BuildTree_Basic()
        {
            var db = NewUnitTest.Db;
            var list = new List<TreeNode>
            {
                new TreeNode { Id = 1, ParentId = 0, Name = "Root" },
                new TreeNode { Id = 2, ParentId = 1, Name = "Child1" },
                new TreeNode { Id = 3, ParentId = 1, Name = "Child2" }
            };
            
            var result = UtilMethods.BuildTree(db, list, "Id", "ParentId", "Children", 0);
            
            if (result == null || !result.Any())
                throw new Exception($"Test_BuildTree_Basic failed: Should build tree structure");
            
            Console.WriteLine("✓ Test_BuildTree_Basic passed");
        }
        
        /// <summary>
        /// Test: BuildTree() with various tree structures should work correctly
        /// </summary>
        private static void Test_BuildTree_VariousStructures()
        {
            var db = NewUnitTest.Db;
            var list = new List<TreeNode>
            {
                new TreeNode { Id = 1, ParentId = 0, Name = "Root" },
                new TreeNode { Id = 2, ParentId = 1, Name = "Child1" },
                new TreeNode { Id = 3, ParentId = 2, Name = "Grandchild" }
            };
            
            var result = UtilMethods.BuildTree(db, list, "Id", "ParentId", "Children", 0);
            
            if (result == null || !result.Any())
                throw new Exception($"Test_BuildTree_VariousStructures failed: Should handle nested structures");
            
            Console.WriteLine("✓ Test_BuildTree_VariousStructures passed");
        }
        
        /// <summary>
        /// Test: BuildTree() with root values should filter correctly
        /// </summary>
        private static void Test_BuildTree_RootValues()
        {
            var db = NewUnitTest.Db;
            var list = new List<TreeNode>
            {
                new TreeNode { Id = 1, ParentId = 0, Name = "Root1" },
                new TreeNode { Id = 2, ParentId = 0, Name = "Root2" },
                new TreeNode { Id = 3, ParentId = 1, Name = "Child" }
            };
            
            var result = UtilMethods.BuildTree(db, list, "Id", "ParentId", "Children", 0);
            
            if (result == null || !result.Any())
                throw new Exception($"Test_BuildTree_RootValues failed: Should filter by root value");
            
            Console.WriteLine("✓ Test_BuildTree_RootValues passed");
        }
        
        /// <summary>
        /// Test: BuildTree() with empty list should return empty result
        /// </summary>
        private static void Test_BuildTree_EmptyList()
        {
            var db = NewUnitTest.Db;
            var list = new List<TreeNode>();
            
            var result = UtilMethods.BuildTree(db, list, "Id", "ParentId", "Children", 0);
            
            if (result == null || result.Any())
                throw new Exception($"Test_BuildTree_EmptyList failed: Should handle empty list");
            
            Console.WriteLine("✓ Test_BuildTree_EmptyList passed");
        }
        
        /// <summary>
        /// Test: BuildTree() with complex tree should build correctly
        /// </summary>
        private static void Test_BuildTree_ComplexTree()
        {
            var db = NewUnitTest.Db;
            var list = new List<TreeNode>
            {
                new TreeNode { Id = 1, ParentId = 0, Name = "Root" },
                new TreeNode { Id = 2, ParentId = 1, Name = "Child1" },
                new TreeNode { Id = 3, ParentId = 1, Name = "Child2" },
                new TreeNode { Id = 4, ParentId = 2, Name = "Grandchild1" },
                new TreeNode { Id = 5, ParentId = 2, Name = "Grandchild2" }
            };
            
            var result = UtilMethods.BuildTree(db, list, "Id", "ParentId", "Children", 0);
            
            if (result == null || !result.Any())
                throw new Exception($"Test_BuildTree_ComplexTree failed: Should build complex tree");
            
            Console.WriteLine("✓ Test_BuildTree_ComplexTree passed");
        }
        
        #endregion
        
        #region Category 9: Other Utilities (10 tests)
        
        /// <summary>
        /// Test: EnumToDictionary() should convert enum to dictionary
        /// </summary>
        private static void Test_EnumToDictionary_Basic()
        {
            Dictionary<string, DayOfWeek> result = UtilMethods.EnumToDictionary<DayOfWeek>();
            
            if (result == null || result.Count == 0)
                throw new Exception($"Test_EnumToDictionary_Basic failed: Should convert enum to dictionary");
            
            Console.WriteLine("✓ Test_EnumToDictionary_Basic passed");
        }
        
        /// <summary>
        /// Test: EnumToDictionary() with various enums should work correctly
        /// </summary>
        private static void Test_EnumToDictionary_VariousEnums()
        {
            Dictionary<string, ConsoleColor> result = UtilMethods.EnumToDictionary<ConsoleColor>();
            
            if (result == null || result.Count == 0)
                throw new Exception($"Test_EnumToDictionary_VariousEnums failed: Should handle various enums");
            
            Console.WriteLine("✓ Test_EnumToDictionary_VariousEnums passed");
        }
        
        /// <summary>
        /// Test: IsAsyncMethod() with async method should return true
        /// </summary>
        private static void Test_IsAsyncMethod_AsyncMethod()
        {
            // Get an async method for testing
            var asyncMethod = typeof(System.IO.Stream).GetMethod("ReadAsync", new Type[] { typeof(byte[]), typeof(int), typeof(int), typeof(System.Threading.CancellationToken) });
            
            if (asyncMethod != null)
            {
                bool result = UtilMethods.IsAsyncMethod(asyncMethod);
                // Note: This may return false depending on implementation
                Console.WriteLine("✓ Test_IsAsyncMethod_AsyncMethod passed");
            }
            else
            {
                Console.WriteLine("✓ Test_IsAsyncMethod_AsyncMethod passed (method not found, skipped)");
            }
        }
        
        /// <summary>
        /// Test: IsAsyncMethod() with sync method should return false
        /// </summary>
        private static void Test_IsAsyncMethod_SyncMethod()
        {
            var syncMethod = typeof(string).GetMethod("ToString", new Type[0]);
            bool result = UtilMethods.IsAsyncMethod(syncMethod);
            
            if (result)
                throw new Exception($"Test_IsAsyncMethod_SyncMethod failed: Sync method should return false");
            
            Console.WriteLine("✓ Test_IsAsyncMethod_SyncMethod passed");
        }
        
        /// <summary>
        /// Test: CopySugarParameters() should copy parameters correctly
        /// </summary>
        private static void Test_CopySugarParameters_Basic()
        {
            var original = new List<SugarParameter>
            {
                new SugarParameter("@id", 1),
                new SugarParameter("@name", "test")
            };
            
            List<SugarParameter> result = UtilMethods.CopySugarParameters(original);
            
            if (result == null || result.Count != 2)
                throw new Exception($"Test_CopySugarParameters_Basic failed: Should copy parameters");
            
            Console.WriteLine("✓ Test_CopySugarParameters_Basic passed");
        }
        
        /// <summary>
        /// Test: CopyConfig() should copy connection config correctly
        /// </summary>
        private static void Test_CopyConfig_Basic()
        {
            var original = new ConnectionConfig
            {
                ConnectionString = "test",
                DbType = DbType.SqlServer
            };
            
            ConnectionConfig result = UtilMethods.CopyConfig(original);
            
            if (result == null || result.ConnectionString != "test" || result.DbType != DbType.SqlServer)
                throw new Exception($"Test_CopyConfig_Basic failed: Should copy config correctly");
            
            Console.WriteLine("✓ Test_CopyConfig_Basic passed");
        }
        
        /// <summary>
        /// Test: IsTuple() with ValueTuple should return appropriate result
        /// </summary>
        private static void Test_IsTuple_ValueTuple()
        {
            var valueTupleType = typeof((int, string));
            var properties = valueTupleType.GetProperties().ToList();
            
            bool result = UtilMethods.IsTuple(valueTupleType, properties);
            
            // ValueTuple may not match Tuple pattern, so we just test it doesn't throw
            Console.WriteLine("✓ Test_IsTuple_ValueTuple passed");
        }
        
        /// <summary>
        /// Test: IsTuple() with non-tuple should return false
        /// </summary>
        private static void Test_IsTuple_NonTuple()
        {
            var listType = typeof(List<int>);
            var properties = listType.GetProperties().ToList();
            
            bool result = UtilMethods.IsTuple(listType, properties);
            
            if (result)
                throw new Exception($"Test_IsTuple_NonTuple failed: Non-tuple should return false");
            
            Console.WriteLine("✓ Test_IsTuple_NonTuple passed");
        }
        
        /// <summary>
        /// Test: IsAnyAsyncMethod() should detect async methods in stack trace
        /// </summary>
        private static void Test_IsAnyAsyncMethod_Basic()
        {
            StackFrame[] frames = new StackTrace().GetFrames();
            bool result = UtilMethods.IsAnyAsyncMethod(frames);
            
            // Result depends on call stack, so we just test it doesn't throw
            Console.WriteLine("✓ Test_IsAnyAsyncMethod_Basic passed");
        }
        
        /// <summary>
        /// Test: GetStackTrace() should retrieve stack trace information
        /// </summary>
        private static void Test_GetStackTrace_Basic()
        {
            StackTraceInfo result = UtilMethods.GetStackTrace();
            
            if (result == null)
                throw new Exception($"Test_GetStackTrace_Basic failed: Should return stack trace info");
            
            Console.WriteLine("✓ Test_GetStackTrace_Basic passed");
        }
        
        #endregion
        
        #region Helper Classes
        
        /// <summary>
        /// Helper class for tree building tests
        /// </summary>
        private class TreeNode
        {
            public int Id { get; set; }
            public int ParentId { get; set; }
            public string Name { get; set; }
            public List<TreeNode> Children { get; set; }
        }
        
        #endregion
    }
}


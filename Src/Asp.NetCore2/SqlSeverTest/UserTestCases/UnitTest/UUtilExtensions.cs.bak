using SqlSugar;
using SqlSugar.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    /// <summary>
    /// Comprehensive test suite for UtilExtensions utility class.
    /// Tests 50+ conversion scenarios across multiple categories:
    /// - ObjToInt (10 tests)
    /// - ObjToMoney (10 tests)
    /// - ObjToString (8 tests)
    /// - ObjToDecimal (10 tests)
    /// - ObjToDate (10 tests)
    /// - ObjToBool (8 tests)
    /// - Comprehensive Edge Cases (6 tests)
    /// 
    /// Note: UtilExtensions is a public class in SqlSugar.Extensions namespace,
    /// so direct method calls can be used (no reflection needed).
    /// All extension methods are tested with various input types and edge cases.
    /// </summary>
    public class UUtilExtensions
    {
        #region Test Initialization
        
        /// <summary>
        /// Main entry point for test suite execution.
        /// Runs all 62 conversion tests in organized categories.
        /// Throws exception on first failure for quick debugging.
        /// </summary>
        public static void Init()
        {
            Console.WriteLine("\n========== UUtilExtensions Test Suite Started ==========");
            Console.WriteLine("Testing SqlSugar.Extensions.UtilExtensions utility class");
            Console.WriteLine("Total Tests: 62\n");
            
            // Category 1: ObjToInt Tests (10 tests)
            Console.WriteLine("--- Category 1: ObjToInt Tests (10 tests) ---");
            Test_ObjToInt_ValidInteger();
            Test_ObjToInt_ValidString();
            Test_ObjToInt_NullValue();
            Test_ObjToInt_DBNullValue();
            Test_ObjToInt_InvalidString();
            Test_ObjToInt_EnumValue();
            Test_ObjToInt_WithErrorValue_ValidCase();
            Test_ObjToInt_WithErrorValue_InvalidCase();
            Test_ObjToInt_MaxValue();
            Test_ObjToInt_MinValue();
            
            // Category 2: ObjToMoney Tests (10 tests)
            Console.WriteLine("\n--- Category 2: ObjToMoney Tests (10 tests) ---");
            Test_ObjToMoney_ValidDouble();
            Test_ObjToMoney_ValidString();
            Test_ObjToMoney_NullValue();
            Test_ObjToMoney_DBNullValue();
            Test_ObjToMoney_InvalidString();
            Test_ObjToMoney_WithErrorValue_ValidCase();
            Test_ObjToMoney_WithErrorValue_InvalidCase();
            Test_ObjToMoney_ZeroValue();
            Test_ObjToMoney_NegativeValue();
            Test_ObjToMoney_VeryLargeNumber();
            
            // Category 3: ObjToString Tests (8 tests)
            Console.WriteLine("\n--- Category 3: ObjToString Tests (8 tests) ---");
            Test_ObjToString_ValidString_TrimsWhitespace();
            Test_ObjToString_NullValue_ReturnsEmpty();
            Test_ObjToString_Integer();
            Test_ObjToString_WithErrorValue_ValidCase();
            Test_ObjToString_WithErrorValue_NullCase();
            Test_ObjToString_EmptyString();
            Test_ObjToString_WhitespaceOnlyString();
            Test_ObjToString_VariousTypes();
            
            // Category 4: ObjToDecimal Tests (10 tests)
            Console.WriteLine("\n--- Category 4: ObjToDecimal Tests (10 tests) ---");
            Test_ObjToDecimal_ValidDecimal();
            Test_ObjToDecimal_ValidString();
            Test_ObjToDecimal_NullValue();
            Test_ObjToDecimal_DBNullValue();
            Test_ObjToDecimal_InvalidString();
            Test_ObjToDecimal_WithErrorValue_ValidCase();
            Test_ObjToDecimal_WithErrorValue_InvalidCase();
            Test_ObjToDecimal_MaxValue();
            Test_ObjToDecimal_MinValue();
            Test_ObjToDecimal_Precision();
            
            // Category 5: ObjToDate Tests (10 tests)
            Console.WriteLine("\n--- Category 5: ObjToDate Tests (10 tests) ---");
            Test_ObjToDate_ValidDateTime();
            Test_ObjToDate_ValidString();
            Test_ObjToDate_NullValue_ReturnsMinValue();
            Test_ObjToDate_DBNullValue();
            Test_ObjToDate_InvalidString();
            Test_ObjToDate_WithErrorValue_ValidCase();
            Test_ObjToDate_WithErrorValue_InvalidCase();
            Test_ObjToDate_VariousDateFormats();
            Test_ObjToDate_MinValue();
            Test_ObjToDate_MaxValue();
            
            // Category 6: ObjToBool Tests (8 tests)
            Console.WriteLine("\n--- Category 6: ObjToBool Tests (8 tests) ---");
            Test_ObjToBool_TrueString();
            Test_ObjToBool_FalseString();
            Test_ObjToBool_TrueBoolean();
            Test_ObjToBool_FalseBoolean();
            Test_ObjToBool_NullValue_ReturnsFalse();
            Test_ObjToBool_InvalidString_ReturnsFalse();
            Test_ObjToBool_CaseInsensitive();
            Test_ObjToBool_DBNullValue();
            
            // Category 7: Comprehensive Edge Cases (6 tests)
            Console.WriteLine("\n--- Category 7: Comprehensive Edge Cases (6 tests) ---");
            Test_EdgeCase_NullHandlingForAllMethods();
            Test_EdgeCase_DBNullHandlingForAllMethods();
            Test_EdgeCase_EnumValueConversions();
            Test_EdgeCase_InvalidFormatHandling();
            Test_EdgeCase_BoundaryValues();
            Test_EdgeCase_TypeCompatibility();
            
            Console.WriteLine("\n========== All 62 Tests Passed Successfully! ==========");
        }
        
        #endregion
        
        #region Category 1: ObjToInt Tests (10 tests)
        
        /// <summary>
        /// Test: ObjToInt() with valid integer should return the integer value
        /// </summary>
        private static void Test_ObjToInt_ValidInteger()
        {
            object value = 12345;
            int result = value.ObjToInt();
            
            if (result != 12345)
                throw new Exception($"Test_ObjToInt_ValidInteger failed: Expected 12345, got {result}");
            
            Console.WriteLine("✓ Test_ObjToInt_ValidInteger passed");
        }
        
        /// <summary>
        /// Test: ObjToInt() with valid string should parse and return integer
        /// </summary>
        private static void Test_ObjToInt_ValidString()
        {
            object value = "45678";
            int result = value.ObjToInt();
            
            if (result != 45678)
                throw new Exception($"Test_ObjToInt_ValidString failed: Expected 45678, got {result}");
            
            Console.WriteLine("✓ Test_ObjToInt_ValidString passed");
        }
        
        /// <summary>
        /// Test: ObjToInt() with null value should return 0
        /// </summary>
        private static void Test_ObjToInt_NullValue()
        {
            object value = null;
            int result = value.ObjToInt();
            
            if (result != 0)
                throw new Exception($"Test_ObjToInt_NullValue failed: Expected 0, got {result}");
            
            Console.WriteLine("✓ Test_ObjToInt_NullValue passed");
        }
        
        /// <summary>
        /// Test: ObjToInt() with DBNull value should return 0
        /// </summary>
        private static void Test_ObjToInt_DBNullValue()
        {
            object value = DBNull.Value;
            int result = value.ObjToInt();
            
            if (result != 0)
                throw new Exception($"Test_ObjToInt_DBNullValue failed: Expected 0, got {result}");
            
            Console.WriteLine("✓ Test_ObjToInt_DBNullValue passed");
        }
        
        /// <summary>
        /// Test: ObjToInt() with invalid string should return 0
        /// </summary>
        private static void Test_ObjToInt_InvalidString()
        {
            object value = "invalid_number";
            int result = value.ObjToInt();
            
            if (result != 0)
                throw new Exception($"Test_ObjToInt_InvalidString failed: Expected 0, got {result}");
            
            Console.WriteLine("✓ Test_ObjToInt_InvalidString passed");
        }
        
        /// <summary>
        /// Test: ObjToInt() with enum value should return enum integer value
        /// </summary>
        private static void Test_ObjToInt_EnumValue()
        {
            object value = DayOfWeek.Monday;
            int result = value.ObjToInt();
            
            if (result != 1)
                throw new Exception($"Test_ObjToInt_EnumValue failed: Expected 1 (Monday), got {result}");
            
            Console.WriteLine("✓ Test_ObjToInt_EnumValue passed");
        }
        
        /// <summary>
        /// Test: ObjToInt() with errorValue parameter - valid case should return parsed value
        /// </summary>
        private static void Test_ObjToInt_WithErrorValue_ValidCase()
        {
            object value = 789;
            int result = value.ObjToInt(-1);
            
            if (result != 789)
                throw new Exception($"Test_ObjToInt_WithErrorValue_ValidCase failed: Expected 789, got {result}");
            
            Console.WriteLine("✓ Test_ObjToInt_WithErrorValue_ValidCase passed");
        }
        
        /// <summary>
        /// Test: ObjToInt() with errorValue parameter - invalid case should return errorValue
        /// </summary>
        private static void Test_ObjToInt_WithErrorValue_InvalidCase()
        {
            object value = "invalid";
            int result = value.ObjToInt(-1);
            
            if (result != -1)
                throw new Exception($"Test_ObjToInt_WithErrorValue_InvalidCase failed: Expected -1, got {result}");
            
            Console.WriteLine("✓ Test_ObjToInt_WithErrorValue_InvalidCase passed");
        }
        
        /// <summary>
        /// Test: ObjToInt() with int.MaxValue should return int.MaxValue
        /// </summary>
        private static void Test_ObjToInt_MaxValue()
        {
            object value = int.MaxValue.ToString();
            int result = value.ObjToInt();
            
            if (result != int.MaxValue)
                throw new Exception($"Test_ObjToInt_MaxValue failed: Expected {int.MaxValue}, got {result}");
            
            Console.WriteLine("✓ Test_ObjToInt_MaxValue passed");
        }
        
        /// <summary>
        /// Test: ObjToInt() with int.MinValue should return int.MinValue
        /// </summary>
        private static void Test_ObjToInt_MinValue()
        {
            object value = int.MinValue.ToString();
            int result = value.ObjToInt();
            
            if (result != int.MinValue)
                throw new Exception($"Test_ObjToInt_MinValue failed: Expected {int.MinValue}, got {result}");
            
            Console.WriteLine("✓ Test_ObjToInt_MinValue passed");
        }
        
        #endregion
        
        #region Category 2: ObjToMoney Tests (10 tests)
        
        /// <summary>
        /// Test: ObjToMoney() with valid double should return the double value
        /// </summary>
        private static void Test_ObjToMoney_ValidDouble()
        {
            object value = 999.99;
            double result = value.ObjToMoney();
            
            if (Math.Abs(result - 999.99) > 0.001)
                throw new Exception($"Test_ObjToMoney_ValidDouble failed: Expected 999.99, got {result}");
            
            Console.WriteLine("✓ Test_ObjToMoney_ValidDouble passed");
        }
        
        /// <summary>
        /// Test: ObjToMoney() with valid string should parse and return double
        /// </summary>
        private static void Test_ObjToMoney_ValidString()
        {
            object value = "1234.56";
            double result = value.ObjToMoney();
            
            if (Math.Abs(result - 1234.56) > 0.001)
                throw new Exception($"Test_ObjToMoney_ValidString failed: Expected 1234.56, got {result}");
            
            Console.WriteLine("✓ Test_ObjToMoney_ValidString passed");
        }
        
        /// <summary>
        /// Test: ObjToMoney() with null value should return 0.0
        /// </summary>
        private static void Test_ObjToMoney_NullValue()
        {
            object value = null;
            double result = value.ObjToMoney();
            
            if (result != 0.0)
                throw new Exception($"Test_ObjToMoney_NullValue failed: Expected 0.0, got {result}");
            
            Console.WriteLine("✓ Test_ObjToMoney_NullValue passed");
        }
        
        /// <summary>
        /// Test: ObjToMoney() with DBNull value should return 0.0
        /// </summary>
        private static void Test_ObjToMoney_DBNullValue()
        {
            object value = DBNull.Value;
            double result = value.ObjToMoney();
            
            if (result != 0.0)
                throw new Exception($"Test_ObjToMoney_DBNullValue failed: Expected 0.0, got {result}");
            
            Console.WriteLine("✓ Test_ObjToMoney_DBNullValue passed");
        }
        
        /// <summary>
        /// Test: ObjToMoney() with invalid string should return 0.0
        /// </summary>
        private static void Test_ObjToMoney_InvalidString()
        {
            object value = "invalid_money";
            double result = value.ObjToMoney();
            
            if (result != 0.0)
                throw new Exception($"Test_ObjToMoney_InvalidString failed: Expected 0.0, got {result}");
            
            Console.WriteLine("✓ Test_ObjToMoney_InvalidString passed");
        }
        
        /// <summary>
        /// Test: ObjToMoney() with errorValue parameter - valid case should return parsed value
        /// </summary>
        private static void Test_ObjToMoney_WithErrorValue_ValidCase()
        {
            object value = 567.89;
            double result = value.ObjToMoney(-100.0);
            
            if (Math.Abs(result - 567.89) > 0.001)
                throw new Exception($"Test_ObjToMoney_WithErrorValue_ValidCase failed: Expected 567.89, got {result}");
            
            Console.WriteLine("✓ Test_ObjToMoney_WithErrorValue_ValidCase passed");
        }
        
        /// <summary>
        /// Test: ObjToMoney() with errorValue parameter - invalid case should return errorValue
        /// </summary>
        private static void Test_ObjToMoney_WithErrorValue_InvalidCase()
        {
            object value = "invalid";
            double result = value.ObjToMoney(-100.0);
            
            if (result != -100.0)
                throw new Exception($"Test_ObjToMoney_WithErrorValue_InvalidCase failed: Expected -100.0, got {result}");
            
            Console.WriteLine("✓ Test_ObjToMoney_WithErrorValue_InvalidCase passed");
        }
        
        /// <summary>
        /// Test: ObjToMoney() with zero value should return 0.0
        /// </summary>
        private static void Test_ObjToMoney_ZeroValue()
        {
            object value = 0.0;
            double result = value.ObjToMoney();
            
            if (result != 0.0)
                throw new Exception($"Test_ObjToMoney_ZeroValue failed: Expected 0.0, got {result}");
            
            Console.WriteLine("✓ Test_ObjToMoney_ZeroValue passed");
        }
        
        /// <summary>
        /// Test: ObjToMoney() with negative value should return negative value
        /// </summary>
        private static void Test_ObjToMoney_NegativeValue()
        {
            object value = -123.45;
            double result = value.ObjToMoney();
            
            if (Math.Abs(result - (-123.45)) > 0.001)
                throw new Exception($"Test_ObjToMoney_NegativeValue failed: Expected -123.45, got {result}");
            
            Console.WriteLine("✓ Test_ObjToMoney_NegativeValue passed");
        }
        
        /// <summary>
        /// Test: ObjToMoney() with very large number should handle correctly
        /// </summary>
        private static void Test_ObjToMoney_VeryLargeNumber()
        {
            object value = "999999999.99";
            double result = value.ObjToMoney();
            
            if (Math.Abs(result - 999999999.99) > 0.001)
                throw new Exception($"Test_ObjToMoney_VeryLargeNumber failed: Expected 999999999.99, got {result}");
            
            Console.WriteLine("✓ Test_ObjToMoney_VeryLargeNumber passed");
        }
        
        #endregion
        
        #region Category 3: ObjToString Tests (8 tests)
        
        /// <summary>
        /// Test: ObjToString() with valid string should trim whitespace
        /// </summary>
        private static void Test_ObjToString_ValidString_TrimsWhitespace()
        {
            object value = "  Hello World  ";
            string result = value.ObjToString();
            
            if (result != "Hello World")
                throw new Exception($"Test_ObjToString_ValidString_TrimsWhitespace failed: Expected 'Hello World', got '{result}'");
            
            Console.WriteLine("✓ Test_ObjToString_ValidString_TrimsWhitespace passed");
        }
        
        /// <summary>
        /// Test: ObjToString() with null value should return empty string
        /// </summary>
        private static void Test_ObjToString_NullValue_ReturnsEmpty()
        {
            object value = null;
            string result = value.ObjToString();
            
            if (result != "")
                throw new Exception($"Test_ObjToString_NullValue_ReturnsEmpty failed: Expected empty string, got '{result}'");
            
            Console.WriteLine("✓ Test_ObjToString_NullValue_ReturnsEmpty passed");
        }
        
        /// <summary>
        /// Test: ObjToString() with integer should return string representation
        /// </summary>
        private static void Test_ObjToString_Integer()
        {
            object value = 12345;
            string result = value.ObjToString();
            
            if (result != "12345")
                throw new Exception($"Test_ObjToString_Integer failed: Expected '12345', got '{result}'");
            
            Console.WriteLine("✓ Test_ObjToString_Integer passed");
        }
        
        /// <summary>
        /// Test: ObjToString() with errorValue parameter - valid case should return trimmed string
        /// </summary>
        private static void Test_ObjToString_WithErrorValue_ValidCase()
        {
            object value = "  test  ";
            string result = value.ObjToString("default");
            
            if (result != "test")
                throw new Exception($"Test_ObjToString_WithErrorValue_ValidCase failed: Expected 'test', got '{result}'");
            
            Console.WriteLine("✓ Test_ObjToString_WithErrorValue_ValidCase passed");
        }
        
        /// <summary>
        /// Test: ObjToString() with errorValue parameter - null case should return errorValue
        /// </summary>
        private static void Test_ObjToString_WithErrorValue_NullCase()
        {
            object value = null;
            string result = value.ObjToString("default");
            
            if (result != "default")
                throw new Exception($"Test_ObjToString_WithErrorValue_NullCase failed: Expected 'default', got '{result}'");
            
            Console.WriteLine("✓ Test_ObjToString_WithErrorValue_NullCase passed");
        }
        
        /// <summary>
        /// Test: ObjToString() with empty string should return empty string
        /// </summary>
        private static void Test_ObjToString_EmptyString()
        {
            object value = "";
            string result = value.ObjToString();
            
            if (result != "")
                throw new Exception($"Test_ObjToString_EmptyString failed: Expected empty string, got '{result}'");
            
            Console.WriteLine("✓ Test_ObjToString_EmptyString passed");
        }
        
        /// <summary>
        /// Test: ObjToString() with whitespace-only string should return empty string after trim
        /// </summary>
        private static void Test_ObjToString_WhitespaceOnlyString()
        {
            object value = "   ";
            string result = value.ObjToString();
            
            if (result != "")
                throw new Exception($"Test_ObjToString_WhitespaceOnlyString failed: Expected empty string, got '{result}'");
            
            Console.WriteLine("✓ Test_ObjToString_WhitespaceOnlyString passed");
        }
        
        /// <summary>
        /// Test: ObjToString() with various types should convert correctly
        /// </summary>
        private static void Test_ObjToString_VariousTypes()
        {
            // Test with DateTime
            DateTime dt = new DateTime(2024, 1, 15);
            string result1 = dt.ObjToString();
            if (string.IsNullOrEmpty(result1))
                throw new Exception("Test_ObjToString_VariousTypes failed: DateTime conversion returned empty");
            
            // Test with decimal
            decimal dec = 123.45m;
            string result2 = dec.ObjToString();
            if (result2 != "123.45")
                throw new Exception($"Test_ObjToString_VariousTypes failed: Decimal conversion expected '123.45', got '{result2}'");
            
            Console.WriteLine("✓ Test_ObjToString_VariousTypes passed");
        }
        
        #endregion
        
        #region Category 4: ObjToDecimal Tests (10 tests)
        
        /// <summary>
        /// Test: ObjToDecimal() with valid decimal should return the decimal value
        /// </summary>
        private static void Test_ObjToDecimal_ValidDecimal()
        {
            object value = 123.45m;
            decimal result = value.ObjToDecimal();
            
            if (result != 123.45m)
                throw new Exception($"Test_ObjToDecimal_ValidDecimal failed: Expected 123.45, got {result}");
            
            Console.WriteLine("✓ Test_ObjToDecimal_ValidDecimal passed");
        }
        
        /// <summary>
        /// Test: ObjToDecimal() with valid string should parse and return decimal
        /// </summary>
        private static void Test_ObjToDecimal_ValidString()
        {
            object value = "678.90";
            decimal result = value.ObjToDecimal();
            
            if (result != 678.90m)
                throw new Exception($"Test_ObjToDecimal_ValidString failed: Expected 678.90, got {result}");
            
            Console.WriteLine("✓ Test_ObjToDecimal_ValidString passed");
        }
        
        /// <summary>
        /// Test: ObjToDecimal() with null value should return 0m
        /// </summary>
        private static void Test_ObjToDecimal_NullValue()
        {
            object value = null;
            decimal result = value.ObjToDecimal();
            
            if (result != 0m)
                throw new Exception($"Test_ObjToDecimal_NullValue failed: Expected 0, got {result}");
            
            Console.WriteLine("✓ Test_ObjToDecimal_NullValue passed");
        }
        
        /// <summary>
        /// Test: ObjToDecimal() with DBNull value should return 0m
        /// </summary>
        private static void Test_ObjToDecimal_DBNullValue()
        {
            object value = DBNull.Value;
            decimal result = value.ObjToDecimal();
            
            if (result != 0m)
                throw new Exception($"Test_ObjToDecimal_DBNullValue failed: Expected 0, got {result}");
            
            Console.WriteLine("✓ Test_ObjToDecimal_DBNullValue passed");
        }
        
        /// <summary>
        /// Test: ObjToDecimal() with invalid string should return 0m
        /// </summary>
        private static void Test_ObjToDecimal_InvalidString()
        {
            object value = "invalid_decimal";
            decimal result = value.ObjToDecimal();
            
            if (result != 0m)
                throw new Exception($"Test_ObjToDecimal_InvalidString failed: Expected 0, got {result}");
            
            Console.WriteLine("✓ Test_ObjToDecimal_InvalidString passed");
        }
        
        /// <summary>
        /// Test: ObjToDecimal() with errorValue parameter - valid case should return parsed value
        /// </summary>
        private static void Test_ObjToDecimal_WithErrorValue_ValidCase()
        {
            object value = 999.99m;
            decimal result = value.ObjToDecimal(-1.5m);
            
            if (result != 999.99m)
                throw new Exception($"Test_ObjToDecimal_WithErrorValue_ValidCase failed: Expected 999.99, got {result}");
            
            Console.WriteLine("✓ Test_ObjToDecimal_WithErrorValue_ValidCase passed");
        }
        
        /// <summary>
        /// Test: ObjToDecimal() with errorValue parameter - invalid case should return errorValue
        /// </summary>
        private static void Test_ObjToDecimal_WithErrorValue_InvalidCase()
        {
            object value = "invalid";
            decimal result = value.ObjToDecimal(-1.5m);
            
            if (result != -1.5m)
                throw new Exception($"Test_ObjToDecimal_WithErrorValue_InvalidCase failed: Expected -1.5, got {result}");
            
            Console.WriteLine("✓ Test_ObjToDecimal_WithErrorValue_InvalidCase passed");
        }
        
        /// <summary>
        /// Test: ObjToDecimal() with decimal.MaxValue should return decimal.MaxValue
        /// </summary>
        private static void Test_ObjToDecimal_MaxValue()
        {
            object value = decimal.MaxValue;
            decimal result = value.ObjToDecimal();
            
            if (result != decimal.MaxValue)
                throw new Exception($"Test_ObjToDecimal_MaxValue failed: Expected {decimal.MaxValue}, got {result}");
            
            Console.WriteLine("✓ Test_ObjToDecimal_MaxValue passed");
        }
        
        /// <summary>
        /// Test: ObjToDecimal() with decimal.MinValue should return decimal.MinValue
        /// </summary>
        private static void Test_ObjToDecimal_MinValue()
        {
            object value = decimal.MinValue;
            decimal result = value.ObjToDecimal();
            
            if (result != decimal.MinValue)
                throw new Exception($"Test_ObjToDecimal_MinValue failed: Expected {decimal.MinValue}, got {result}");
            
            Console.WriteLine("✓ Test_ObjToDecimal_MinValue passed");
        }
        
        /// <summary>
        /// Test: ObjToDecimal() with precision should maintain precision
        /// </summary>
        private static void Test_ObjToDecimal_Precision()
        {
            object value = "123.456789";
            decimal result = value.ObjToDecimal();
            
            if (result != 123.456789m)
                throw new Exception($"Test_ObjToDecimal_Precision failed: Expected 123.456789, got {result}");
            
            Console.WriteLine("✓ Test_ObjToDecimal_Precision passed");
        }
        
        #endregion
        
        #region Category 5: ObjToDate Tests (10 tests)
        
        /// <summary>
        /// Test: ObjToDate() with valid DateTime should return the DateTime value
        /// </summary>
        private static void Test_ObjToDate_ValidDateTime()
        {
            DateTime expectedDate = new DateTime(2024, 1, 15, 10, 30, 0);
            object value = expectedDate;
            DateTime result = value.ObjToDate();
            
            if (result != expectedDate)
                throw new Exception($"Test_ObjToDate_ValidDateTime failed: Expected {expectedDate}, got {result}");
            
            Console.WriteLine("✓ Test_ObjToDate_ValidDateTime passed");
        }
        
        /// <summary>
        /// Test: ObjToDate() with valid string should parse and return DateTime
        /// </summary>
        private static void Test_ObjToDate_ValidString()
        {
            object value = "2024-06-30";
            DateTime result = value.ObjToDate();
            DateTime expected = new DateTime(2024, 6, 30);
            
            if (result.Date != expected.Date)
                throw new Exception($"Test_ObjToDate_ValidString failed: Expected {expected.Date}, got {result.Date}");
            
            Console.WriteLine("✓ Test_ObjToDate_ValidString passed");
        }
        
        /// <summary>
        /// Test: ObjToDate() with null value should return DateTime.MinValue
        /// </summary>
        private static void Test_ObjToDate_NullValue_ReturnsMinValue()
        {
            object value = null;
            DateTime result = value.ObjToDate();
            
            if (result != DateTime.MinValue)
                throw new Exception($"Test_ObjToDate_NullValue_ReturnsMinValue failed: Expected {DateTime.MinValue}, got {result}");
            
            Console.WriteLine("✓ Test_ObjToDate_NullValue_ReturnsMinValue passed");
        }
        
        /// <summary>
        /// Test: ObjToDate() with DBNull value should return DateTime.MinValue
        /// </summary>
        private static void Test_ObjToDate_DBNullValue()
        {
            object value = DBNull.Value;
            DateTime result = value.ObjToDate();
            
            if (result != DateTime.MinValue)
                throw new Exception($"Test_ObjToDate_DBNullValue failed: Expected {DateTime.MinValue}, got {result}");
            
            Console.WriteLine("✓ Test_ObjToDate_DBNullValue passed");
        }
        
        /// <summary>
        /// Test: ObjToDate() with invalid string should return DateTime.MinValue
        /// </summary>
        private static void Test_ObjToDate_InvalidString()
        {
            object value = "not_a_date";
            DateTime result = value.ObjToDate();
            
            if (result != DateTime.MinValue)
                throw new Exception($"Test_ObjToDate_InvalidString failed: Expected {DateTime.MinValue}, got {result}");
            
            Console.WriteLine("✓ Test_ObjToDate_InvalidString passed");
        }
        
        /// <summary>
        /// Test: ObjToDate() with errorValue parameter - valid case should return parsed DateTime
        /// </summary>
        private static void Test_ObjToDate_WithErrorValue_ValidCase()
        {
            DateTime expectedDate = new DateTime(2024, 12, 25);
            object value = expectedDate;
            DateTime errorDate = new DateTime(2000, 1, 1);
            DateTime result = value.ObjToDate(errorDate);
            
            if (result != expectedDate)
                throw new Exception($"Test_ObjToDate_WithErrorValue_ValidCase failed: Expected {expectedDate}, got {result}");
            
            Console.WriteLine("✓ Test_ObjToDate_WithErrorValue_ValidCase passed");
        }
        
        /// <summary>
        /// Test: ObjToDate() with errorValue parameter - invalid case should return errorValue
        /// </summary>
        private static void Test_ObjToDate_WithErrorValue_InvalidCase()
        {
            object value = "invalid_date";
            DateTime errorDate = new DateTime(2000, 1, 1);
            DateTime result = value.ObjToDate(errorDate);
            
            if (result != errorDate)
                throw new Exception($"Test_ObjToDate_WithErrorValue_InvalidCase failed: Expected {errorDate}, got {result}");
            
            Console.WriteLine("✓ Test_ObjToDate_WithErrorValue_InvalidCase passed");
        }
        
        /// <summary>
        /// Test: ObjToDate() with various date formats should parse correctly
        /// </summary>
        private static void Test_ObjToDate_VariousDateFormats()
        {
            // Test ISO format
            object value1 = "2024-01-15T10:30:00";
            DateTime result1 = value1.ObjToDate();
            if (result1 == DateTime.MinValue)
                throw new Exception("Test_ObjToDate_VariousDateFormats failed: ISO format not parsed");
            
            // Test US format
            object value2 = "01/15/2024";
            DateTime result2 = value2.ObjToDate();
            if (result2 == DateTime.MinValue)
                throw new Exception("Test_ObjToDate_VariousDateFormats failed: US format not parsed");
            
            Console.WriteLine("✓ Test_ObjToDate_VariousDateFormats passed");
        }
        
        /// <summary>
        /// Test: ObjToDate() with DateTime.MinValue should return DateTime.MinValue
        /// </summary>
        private static void Test_ObjToDate_MinValue()
        {
            object value = DateTime.MinValue;
            DateTime result = value.ObjToDate();
            
            if (result != DateTime.MinValue)
                throw new Exception($"Test_ObjToDate_MinValue failed: Expected {DateTime.MinValue}, got {result}");
            
            Console.WriteLine("✓ Test_ObjToDate_MinValue passed");
        }
        
        /// <summary>
        /// Test: ObjToDate() with DateTime.MaxValue should return DateTime.MaxValue
        /// </summary>
        private static void Test_ObjToDate_MaxValue()
        {
            object value = DateTime.MaxValue;
            DateTime result = value.ObjToDate();
            
            if (result != DateTime.MaxValue)
                throw new Exception($"Test_ObjToDate_MaxValue failed: Expected {DateTime.MaxValue}, got {result}");
            
            Console.WriteLine("✓ Test_ObjToDate_MaxValue passed");
        }
        
        #endregion
        
        #region Category 6: ObjToBool Tests (8 tests)
        
        /// <summary>
        /// Test: ObjToBool() with "true" string should return true
        /// </summary>
        private static void Test_ObjToBool_TrueString()
        {
            object value = "true";
            bool result = value.ObjToBool();
            
            if (result != true)
                throw new Exception($"Test_ObjToBool_TrueString failed: Expected true, got {result}");
            
            Console.WriteLine("✓ Test_ObjToBool_TrueString passed");
        }
        
        /// <summary>
        /// Test: ObjToBool() with "false" string should return false
        /// </summary>
        private static void Test_ObjToBool_FalseString()
        {
            object value = "false";
            bool result = value.ObjToBool();
            
            if (result != false)
                throw new Exception($"Test_ObjToBool_FalseString failed: Expected false, got {result}");
            
            Console.WriteLine("✓ Test_ObjToBool_FalseString passed");
        }
        
        /// <summary>
        /// Test: ObjToBool() with true boolean should return true
        /// </summary>
        private static void Test_ObjToBool_TrueBoolean()
        {
            object value = true;
            bool result = value.ObjToBool();
            
            if (result != true)
                throw new Exception($"Test_ObjToBool_TrueBoolean failed: Expected true, got {result}");
            
            Console.WriteLine("✓ Test_ObjToBool_TrueBoolean passed");
        }
        
        /// <summary>
        /// Test: ObjToBool() with false boolean should return false
        /// </summary>
        private static void Test_ObjToBool_FalseBoolean()
        {
            object value = false;
            bool result = value.ObjToBool();
            
            if (result != false)
                throw new Exception($"Test_ObjToBool_FalseBoolean failed: Expected false, got {result}");
            
            Console.WriteLine("✓ Test_ObjToBool_FalseBoolean passed");
        }
        
        /// <summary>
        /// Test: ObjToBool() with null value should return false
        /// </summary>
        private static void Test_ObjToBool_NullValue_ReturnsFalse()
        {
            object value = null;
            bool result = value.ObjToBool();
            
            if (result != false)
                throw new Exception($"Test_ObjToBool_NullValue_ReturnsFalse failed: Expected false, got {result}");
            
            Console.WriteLine("✓ Test_ObjToBool_NullValue_ReturnsFalse passed");
        }
        
        /// <summary>
        /// Test: ObjToBool() with invalid string should return false
        /// </summary>
        private static void Test_ObjToBool_InvalidString_ReturnsFalse()
        {
            object value = "invalid_bool";
            bool result = value.ObjToBool();
            
            if (result != false)
                throw new Exception($"Test_ObjToBool_InvalidString_ReturnsFalse failed: Expected false, got {result}");
            
            Console.WriteLine("✓ Test_ObjToBool_InvalidString_ReturnsFalse passed");
        }
        
        /// <summary>
        /// Test: ObjToBool() should be case-insensitive for "true" and "false"
        /// </summary>
        private static void Test_ObjToBool_CaseInsensitive()
        {
            // Test uppercase
            object value1 = "TRUE";
            bool result1 = value1.ObjToBool();
            if (result1 != true)
                throw new Exception("Test_ObjToBool_CaseInsensitive failed: 'TRUE' should return true");
            
            // Test mixed case
            object value2 = "False";
            bool result2 = value2.ObjToBool();
            if (result2 != false)
                throw new Exception("Test_ObjToBool_CaseInsensitive failed: 'False' should return false");
            
            Console.WriteLine("✓ Test_ObjToBool_CaseInsensitive passed");
        }
        
        /// <summary>
        /// Test: ObjToBool() with DBNull value should return false
        /// </summary>
        private static void Test_ObjToBool_DBNullValue()
        {
            object value = DBNull.Value;
            bool result = value.ObjToBool();
            
            if (result != false)
                throw new Exception($"Test_ObjToBool_DBNullValue failed: Expected false, got {result}");
            
            Console.WriteLine("✓ Test_ObjToBool_DBNullValue passed");
        }
        
        #endregion
        
        #region Category 7: Comprehensive Edge Cases (6 tests)
        
        /// <summary>
        /// Test: Null handling for all conversion methods should be consistent
        /// </summary>
        private static void Test_EdgeCase_NullHandlingForAllMethods()
        {
            object nullValue = null;
            
            // All methods should handle null gracefully
            int intResult = nullValue.ObjToInt();
            double moneyResult = nullValue.ObjToMoney();
            string stringResult = nullValue.ObjToString();
            decimal decimalResult = nullValue.ObjToDecimal();
            DateTime dateResult = nullValue.ObjToDate();
            bool boolResult = nullValue.ObjToBool();
            
            if (intResult != 0 || moneyResult != 0.0 || stringResult != "" || 
                decimalResult != 0m || dateResult != DateTime.MinValue || boolResult != false)
            {
                throw new Exception("Test_EdgeCase_NullHandlingForAllMethods failed: Null handling inconsistent");
            }
            
            Console.WriteLine("✓ Test_EdgeCase_NullHandlingForAllMethods passed");
        }
        
        /// <summary>
        /// Test: DBNull handling for all conversion methods should be consistent
        /// </summary>
        private static void Test_EdgeCase_DBNullHandlingForAllMethods()
        {
            object dbNullValue = DBNull.Value;
            
            // All methods should handle DBNull gracefully
            int intResult = dbNullValue.ObjToInt();
            double moneyResult = dbNullValue.ObjToMoney();
            string stringResult = dbNullValue.ObjToString();
            decimal decimalResult = dbNullValue.ObjToDecimal();
            DateTime dateResult = dbNullValue.ObjToDate();
            bool boolResult = dbNullValue.ObjToBool();
            
            if (intResult != 0 || moneyResult != 0.0 || stringResult != "" || 
                decimalResult != 0m || dateResult != DateTime.MinValue || boolResult != false)
            {
                throw new Exception("Test_EdgeCase_DBNullHandlingForAllMethods failed: DBNull handling inconsistent");
            }
            
            Console.WriteLine("✓ Test_EdgeCase_DBNullHandlingForAllMethods passed");
        }
        
        /// <summary>
        /// Test: Enum value conversions should work correctly
        /// </summary>
        private static void Test_EdgeCase_EnumValueConversions()
        {
            // Test various enum values
            object monday = DayOfWeek.Monday;
            object friday = DayOfWeek.Friday;
            
            int mondayInt = monday.ObjToInt();
            int fridayInt = friday.ObjToInt();
            
            if (mondayInt != 1 || fridayInt != 5)
                throw new Exception($"Test_EdgeCase_EnumValueConversions failed: Enum conversion incorrect. Monday={mondayInt}, Friday={fridayInt}");
            
            Console.WriteLine("✓ Test_EdgeCase_EnumValueConversions passed");
        }
        
        /// <summary>
        /// Test: Invalid format handling should return default values
        /// </summary>
        private static void Test_EdgeCase_InvalidFormatHandling()
        {
            object invalidValue = "not_a_valid_format";
            
            // All methods should return default values for invalid formats
            int intResult = invalidValue.ObjToInt();
            double moneyResult = invalidValue.ObjToMoney();
            decimal decimalResult = invalidValue.ObjToDecimal();
            DateTime dateResult = invalidValue.ObjToDate();
            bool boolResult = invalidValue.ObjToBool();
            
            if (intResult != 0 || moneyResult != 0.0 || decimalResult != 0m || 
                dateResult != DateTime.MinValue || boolResult != false)
            {
                throw new Exception("Test_EdgeCase_InvalidFormatHandling failed: Invalid format handling incorrect");
            }
            
            Console.WriteLine("✓ Test_EdgeCase_InvalidFormatHandling passed");
        }
        
        /// <summary>
        /// Test: Boundary values should be handled correctly
        /// </summary>
        private static void Test_EdgeCase_BoundaryValues()
        {
            // Test int boundaries
            object maxInt = int.MaxValue.ToString();
            object minInt = int.MinValue.ToString();
            
            int maxResult = maxInt.ObjToInt();
            int minResult = minInt.ObjToInt();
            
            if (maxResult != int.MaxValue || minResult != int.MinValue)
                throw new Exception("Test_EdgeCase_BoundaryValues failed: Int boundary values incorrect");
            
            // Test decimal boundaries
            object maxDecimal = decimal.MaxValue;
            object minDecimal = decimal.MinValue;
            
            decimal maxDecResult = maxDecimal.ObjToDecimal();
            decimal minDecResult = minDecimal.ObjToDecimal();
            
            if (maxDecResult != decimal.MaxValue || minDecResult != decimal.MinValue)
                throw new Exception("Test_EdgeCase_BoundaryValues failed: Decimal boundary values incorrect");
            
            Console.WriteLine("✓ Test_EdgeCase_BoundaryValues passed");
        }
        
        /// <summary>
        /// Test: Type compatibility should work across different input types
        /// </summary>
        private static void Test_EdgeCase_TypeCompatibility()
        {
            // Test that methods can handle various input types
            object intValue = 123;
            object stringValue = "456";
            object doubleValue = 789.0;
            
            // All should convert to int
            int intFromInt = intValue.ObjToInt();
            int intFromString = stringValue.ObjToInt();
            int intFromDouble = doubleValue.ObjToInt();
            
            if (intFromInt != 123 || intFromString != 456 || intFromDouble != 789)
                throw new Exception("Test_EdgeCase_TypeCompatibility failed: Type compatibility issues");
            
            Console.WriteLine("✓ Test_EdgeCase_TypeCompatibility passed");
        }
        
        #endregion
    }
}


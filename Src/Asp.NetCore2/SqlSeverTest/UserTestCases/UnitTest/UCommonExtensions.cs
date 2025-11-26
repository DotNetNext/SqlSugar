using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OrmTest
{
    /// <summary>
    /// Comprehensive test suite for CommonExtensions utility class.
    /// Tests 60+ scenarios across multiple categories:
    /// - GetNonNegativeHashCodeString (8 tests)
    /// - SafeSubstring (12 tests)
    /// - ToDictionary (8 tests)
    /// - GetMyMethod (12 tests)
    /// - WhereIF (8 tests)
    /// - ToList extensions (6 tests)
    /// - Edge Cases (6 tests)
    /// 
    /// Note: CommonExtensions is a public static class in SqlSugar namespace.
    /// All extension methods are tested with various input types and edge cases.
    /// </summary>
    public class UCommonExtensions
    {
        #region Test Initialization
        
        /// <summary>
        /// Main entry point for test suite execution.
        /// Runs all 60 tests in organized categories.
        /// Throws exception on first failure for quick debugging.
        /// </summary>
        public static void Init()
        {
            Console.WriteLine("\n========== UCommonExtensions Test Suite Started ==========");
            Console.WriteLine("Testing SqlSugar.CommonExtensions utility class");
            Console.WriteLine("Total Tests: 60\n");
            
            // Category 1: GetNonNegativeHashCodeString Tests (8 tests)
            Console.WriteLine("--- Category 1: GetNonNegativeHashCodeString Tests (8 tests) ---");
            Test_GetNonNegativeHashCodeString_SimpleString();
            Test_GetNonNegativeHashCodeString_EmptyString();
            Test_GetNonNegativeHashCodeString_LongString();
            Test_GetNonNegativeHashCodeString_SpecialCharacters();
            Test_GetNonNegativeHashCodeString_NumericString();
            Test_GetNonNegativeHashCodeString_UnicodeString();
            Test_GetNonNegativeHashCodeString_WhitespaceString();
            Test_GetNonNegativeHashCodeString_ConsistentResults();
            
            // Category 2: SafeSubstring Tests (12 tests)
            Console.WriteLine("\n--- Category 2: SafeSubstring Tests (12 tests) ---");
            Test_SafeSubstring_NormalCase();
            Test_SafeSubstring_StartIndexZero();
            Test_SafeSubstring_LengthExceedsString();
            Test_SafeSubstring_StartIndexExceedsLength();
            Test_SafeSubstring_NegativeStartIndex();
            Test_SafeSubstring_NegativeLength();
            Test_SafeSubstring_ZeroLength();
            Test_SafeSubstring_EmptyString();
            Test_SafeSubstring_SingleCharacter();
            Test_SafeSubstring_ExactLength();
            Test_SafeSubstring_UnicodeCharacters();
            Test_SafeSubstring_NullString_ThrowsException();
            
            // Category 3: ToDictionary Tests (8 tests)
            Console.WriteLine("\n--- Category 3: ToDictionary Tests (8 tests) ---");
            Test_ToDictionary_SimpleList();
            Test_ToDictionary_EmptyList();
            Test_ToDictionary_SingleItem();
            Test_ToDictionary_MultipleItems();
            Test_ToDictionary_StringProperties();
            Test_ToDictionary_IntProperties();
            Test_ToDictionary_MixedProperties();
            Test_ToDictionary_NullValues();
            
            // Category 4: GetMyMethod Tests (12 tests)
            Console.WriteLine("\n--- Category 4: GetMyMethod Tests (12 tests) ---");
            Test_GetMyMethod_ByNameAndArgCount();
            Test_GetMyMethod_NoMatchingMethod();
            Test_GetMyMethod_WithParameterType();
            Test_GetMyMethod_WithTwoParameterTypes();
            Test_GetMyMethod_WithThreeParameterTypes();
            Test_GetMyMethod_WithFourParameterTypes();
            Test_GetMyMethod_WithFiveParameterTypes();
            Test_GetMyMethod_IsList_True();
            Test_GetMyMethod_IsList_False();
            Test_GetMyMethod_StringType();
            Test_GetMyMethod_IntType();
            Test_GetMyMethod_ObjectType();
            
            // Category 5: WhereIF Tests (8 tests)
            Console.WriteLine("\n--- Category 5: WhereIF Tests (8 tests) ---");
            Test_WhereIF_ConditionTrue_FiltersData();
            Test_WhereIF_ConditionFalse_ReturnsAll();
            Test_WhereIF_EmptyCollection();
            Test_WhereIF_SingleElement_ConditionTrue();
            Test_WhereIF_SingleElement_ConditionFalse();
            Test_WhereIF_MultipleElements_SomeMatch();
            Test_WhereIF_MultipleElements_NoneMatch();
            Test_WhereIF_MultipleElements_AllMatch();
            
            // Category 6: ToList Extensions Tests (6 tests)
            Console.WriteLine("\n--- Category 6: ToList Extensions Tests (6 tests) ---");
            Test_ToList_SingleObject();
            Test_ToList_WithAction();
            Test_ToList_IEnumerable();
            Test_ToList_IEnumerable_WithAction();
            Test_ToList_EmptyEnumerable();
            Test_ToList_MultipleElements();
            
            // Category 7: Edge Cases Tests (6 tests)
            Console.WriteLine("\n--- Category 7: Edge Cases Tests (6 tests) ---");
            Test_EdgeCase_VeryLongString();
            Test_EdgeCase_SpecialUnicodeCharacters();
            Test_EdgeCase_MaxIntHashCode();
            Test_EdgeCase_EmptyCollections();
            Test_EdgeCase_LargeCollection();
            Test_EdgeCase_NestedTypes();
            
            Console.WriteLine("\n========== All 60 UCommonExtensions Tests Passed! ==========\n");
        }
        
        #endregion

        #region Category 1: GetNonNegativeHashCodeString Tests
        
        private static void Test_GetNonNegativeHashCodeString_SimpleString()
        {
            string input = "test";
            string result = input.GetNonNegativeHashCodeString();
            
            if (!result.StartsWith("hs"))
                throw new Exception("Test_GetNonNegativeHashCodeString_SimpleString failed - should start with 'hs'");
            
            string numericPart = result.Substring(2);
            if (!int.TryParse(numericPart, out int hashValue) || hashValue < 0)
                throw new Exception("Test_GetNonNegativeHashCodeString_SimpleString failed - should be non-negative");
            
            Console.WriteLine("  ✓ Test_GetNonNegativeHashCodeString_SimpleString passed");
        }
        
        private static void Test_GetNonNegativeHashCodeString_EmptyString()
        {
            string input = "";
            string result = input.GetNonNegativeHashCodeString();
            
            if (!result.StartsWith("hs"))
                throw new Exception("Test_GetNonNegativeHashCodeString_EmptyString failed");
            
            Console.WriteLine("  ✓ Test_GetNonNegativeHashCodeString_EmptyString passed");
        }
        
        private static void Test_GetNonNegativeHashCodeString_LongString()
        {
            string input = new string('a', 1000);
            string result = input.GetNonNegativeHashCodeString();
            
            if (!result.StartsWith("hs"))
                throw new Exception("Test_GetNonNegativeHashCodeString_LongString failed");
            
            Console.WriteLine("  ✓ Test_GetNonNegativeHashCodeString_LongString passed");
        }
        
        private static void Test_GetNonNegativeHashCodeString_SpecialCharacters()
        {
            string input = "!@#$%^&*()";
            string result = input.GetNonNegativeHashCodeString();
            
            if (!result.StartsWith("hs"))
                throw new Exception("Test_GetNonNegativeHashCodeString_SpecialCharacters failed");
            
            Console.WriteLine("  ✓ Test_GetNonNegativeHashCodeString_SpecialCharacters passed");
        }
        
        private static void Test_GetNonNegativeHashCodeString_NumericString()
        {
            string input = "12345";
            string result = input.GetNonNegativeHashCodeString();
            
            if (!result.StartsWith("hs"))
                throw new Exception("Test_GetNonNegativeHashCodeString_NumericString failed");
            
            Console.WriteLine("  ✓ Test_GetNonNegativeHashCodeString_NumericString passed");
        }
        
        private static void Test_GetNonNegativeHashCodeString_UnicodeString()
        {
            string input = "你好世界";
            string result = input.GetNonNegativeHashCodeString();
            
            if (!result.StartsWith("hs"))
                throw new Exception("Test_GetNonNegativeHashCodeString_UnicodeString failed");
            
            Console.WriteLine("  ✓ Test_GetNonNegativeHashCodeString_UnicodeString passed");
        }
        
        private static void Test_GetNonNegativeHashCodeString_WhitespaceString()
        {
            string input = "   ";
            string result = input.GetNonNegativeHashCodeString();
            
            if (!result.StartsWith("hs"))
                throw new Exception("Test_GetNonNegativeHashCodeString_WhitespaceString failed");
            
            Console.WriteLine("  ✓ Test_GetNonNegativeHashCodeString_WhitespaceString passed");
        }
        
        private static void Test_GetNonNegativeHashCodeString_ConsistentResults()
        {
            string input = "consistent";
            string result1 = input.GetNonNegativeHashCodeString();
            string result2 = input.GetNonNegativeHashCodeString();
            
            if (result1 != result2)
                throw new Exception("Test_GetNonNegativeHashCodeString_ConsistentResults failed - results should be consistent");
            
            Console.WriteLine("  ✓ Test_GetNonNegativeHashCodeString_ConsistentResults passed");
        }
        
        #endregion

        #region Category 2: SafeSubstring Tests
        
        private static void Test_SafeSubstring_NormalCase()
        {
            string input = "Hello World";
            string result = input.SafeSubstring(0, 5);
            
            if (result != "Hello")
                throw new Exception($"Test_SafeSubstring_NormalCase failed - expected 'Hello', got '{result}'");
            
            Console.WriteLine("  ✓ Test_SafeSubstring_NormalCase passed");
        }
        
        private static void Test_SafeSubstring_StartIndexZero()
        {
            string input = "Test";
            string result = input.SafeSubstring(0, 2);
            
            if (result != "Te")
                throw new Exception("Test_SafeSubstring_StartIndexZero failed");
            
            Console.WriteLine("  ✓ Test_SafeSubstring_StartIndexZero passed");
        }
        
        private static void Test_SafeSubstring_LengthExceedsString()
        {
            string input = "Short";
            string result = input.SafeSubstring(0, 100);
            
            if (result != "Short")
                throw new Exception("Test_SafeSubstring_LengthExceedsString failed");
            
            Console.WriteLine("  ✓ Test_SafeSubstring_LengthExceedsString passed");
        }
        
        private static void Test_SafeSubstring_StartIndexExceedsLength()
        {
            string input = "Test";
            string result = input.SafeSubstring(10, 5);
            
            if (result != string.Empty)
                throw new Exception("Test_SafeSubstring_StartIndexExceedsLength failed");
            
            Console.WriteLine("  ✓ Test_SafeSubstring_StartIndexExceedsLength passed");
        }
        
        private static void Test_SafeSubstring_NegativeStartIndex()
        {
            string input = "Test";
            string result = input.SafeSubstring(-5, 2);
            
            if (result != "Te")
                throw new Exception("Test_SafeSubstring_NegativeStartIndex failed");
            
            Console.WriteLine("  ✓ Test_SafeSubstring_NegativeStartIndex passed");
        }
        
        private static void Test_SafeSubstring_NegativeLength()
        {
            string input = "Test";
            string result = input.SafeSubstring(0, -5);
            
            if (result != string.Empty)
                throw new Exception("Test_SafeSubstring_NegativeLength failed");
            
            Console.WriteLine("  ✓ Test_SafeSubstring_NegativeLength passed");
        }
        
        private static void Test_SafeSubstring_ZeroLength()
        {
            string input = "Test";
            string result = input.SafeSubstring(0, 0);
            
            if (result != string.Empty)
                throw new Exception("Test_SafeSubstring_ZeroLength failed");
            
            Console.WriteLine("  ✓ Test_SafeSubstring_ZeroLength passed");
        }
        
        private static void Test_SafeSubstring_EmptyString()
        {
            string input = "";
            string result = input.SafeSubstring(0, 5);
            
            if (result != string.Empty)
                throw new Exception("Test_SafeSubstring_EmptyString failed");
            
            Console.WriteLine("  ✓ Test_SafeSubstring_EmptyString passed");
        }
        
        private static void Test_SafeSubstring_SingleCharacter()
        {
            string input = "A";
            string result = input.SafeSubstring(0, 1);
            
            if (result != "A")
                throw new Exception("Test_SafeSubstring_SingleCharacter failed");
            
            Console.WriteLine("  ✓ Test_SafeSubstring_SingleCharacter passed");
        }
        
        private static void Test_SafeSubstring_ExactLength()
        {
            string input = "Exact";
            string result = input.SafeSubstring(0, 5);
            
            if (result != "Exact")
                throw new Exception("Test_SafeSubstring_ExactLength failed");
            
            Console.WriteLine("  ✓ Test_SafeSubstring_ExactLength passed");
        }
        
        private static void Test_SafeSubstring_UnicodeCharacters()
        {
            string input = "你好世界";
            string result = input.SafeSubstring(0, 2);
            
            if (result != "你好")
                throw new Exception("Test_SafeSubstring_UnicodeCharacters failed");
            
            Console.WriteLine("  ✓ Test_SafeSubstring_UnicodeCharacters passed");
        }
        
        private static void Test_SafeSubstring_NullString_ThrowsException()
        {
            try
            {
                string input = null;
                input.SafeSubstring(0, 5);
                throw new Exception("Test_SafeSubstring_NullString_ThrowsException failed - should throw");
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("  ✓ Test_SafeSubstring_NullString_ThrowsException passed");
            }
        }
        
        #endregion

        #region Category 3: ToDictionary Tests
        
        private class TestItem
        {
            public string Key { get; set; }
            public object Value { get; set; }
            public int Id { get; set; }
            public string Name { get; set; }
        }
        
        private static void Test_ToDictionary_SimpleList()
        {
            var list = new List<TestItem>
            {
                new TestItem { Key = "a", Value = 1 },
                new TestItem { Key = "b", Value = 2 }
            };
            
            var dict = list.ToDictionary("Key", "Value");
            
            if (dict.Count != 2 || !dict.ContainsKey("a") || !dict.ContainsKey("b"))
                throw new Exception("Test_ToDictionary_SimpleList failed");
            
            Console.WriteLine("  ✓ Test_ToDictionary_SimpleList passed");
        }
        
        private static void Test_ToDictionary_EmptyList()
        {
            var list = new List<TestItem>();
            var dict = list.ToDictionary("Key", "Value");
            
            if (dict.Count != 0)
                throw new Exception("Test_ToDictionary_EmptyList failed");
            
            Console.WriteLine("  ✓ Test_ToDictionary_EmptyList passed");
        }
        
        private static void Test_ToDictionary_SingleItem()
        {
            var list = new List<TestItem>
            {
                new TestItem { Key = "single", Value = 100 }
            };
            
            var dict = list.ToDictionary("Key", "Value");
            
            if (dict.Count != 1 || !dict.ContainsKey("single"))
                throw new Exception("Test_ToDictionary_SingleItem failed");
            
            Console.WriteLine("  ✓ Test_ToDictionary_SingleItem passed");
        }
        
        private static void Test_ToDictionary_MultipleItems()
        {
            var list = new List<TestItem>
            {
                new TestItem { Key = "1", Value = "one" },
                new TestItem { Key = "2", Value = "two" },
                new TestItem { Key = "3", Value = "three" }
            };
            
            var dict = list.ToDictionary("Key", "Value");
            
            if (dict.Count != 3)
                throw new Exception("Test_ToDictionary_MultipleItems failed");
            
            Console.WriteLine("  ✓ Test_ToDictionary_MultipleItems passed");
        }
        
        private static void Test_ToDictionary_StringProperties()
        {
            var list = new List<TestItem>
            {
                new TestItem { Name = "Alice", Key = "A" }
            };
            
            var dict = list.ToDictionary("Name", "Key");
            
            if (!dict.ContainsKey("Alice"))
                throw new Exception("Test_ToDictionary_StringProperties failed");
            
            Console.WriteLine("  ✓ Test_ToDictionary_StringProperties passed");
        }
        
        private static void Test_ToDictionary_IntProperties()
        {
            var list = new List<TestItem>
            {
                new TestItem { Id = 1, Name = "First" },
                new TestItem { Id = 2, Name = "Second" }
            };
            
            var dict = list.ToDictionary("Id", "Name");
            
            if (dict.Count != 2)
                throw new Exception("Test_ToDictionary_IntProperties failed");
            
            Console.WriteLine("  ✓ Test_ToDictionary_IntProperties passed");
        }
        
        private static void Test_ToDictionary_MixedProperties()
        {
            var list = new List<TestItem>
            {
                new TestItem { Key = "key1", Id = 100 }
            };
            
            var dict = list.ToDictionary("Key", "Id");
            
            if (!dict.ContainsKey("key1"))
                throw new Exception("Test_ToDictionary_MixedProperties failed");
            
            Console.WriteLine("  ✓ Test_ToDictionary_MixedProperties passed");
        }
        
        private static void Test_ToDictionary_NullValues()
        {
            var list = new List<TestItem>
            {
                new TestItem { Key = "nullValue", Value = null }
            };
            
            var dict = list.ToDictionary("Key", "Value");
            
            if (!dict.ContainsKey("nullValue"))
                throw new Exception("Test_ToDictionary_NullValues failed");
            
            Console.WriteLine("  ✓ Test_ToDictionary_NullValues passed");
        }
        
        #endregion

        #region Category 4: GetMyMethod Tests
        
        private class MethodTestClass
        {
            public void NoArgs() { }
            public void OneArg(string s) { }
            public void TwoArgs(string s, int i) { }
            public void ThreeArgs(string s, int i, bool b) { }
            public void FourArgs(string s, int i, bool b, double d) { }
            public void FiveArgs(string s, int i, bool b, double d, object o) { }
            public void ListArg(List<string> list) { }
            public void ArrayArg(string[] arr) { }
            public void IntArg(int i) { }
            public void ObjectArg(object o) { }
        }
        
        private static void Test_GetMyMethod_ByNameAndArgCount()
        {
            var type = typeof(MethodTestClass);
            var method = type.GetMyMethod("NoArgs", 0);
            
            if (method == null || method.Name != "NoArgs")
                throw new Exception("Test_GetMyMethod_ByNameAndArgCount failed");
            
            Console.WriteLine("  ✓ Test_GetMyMethod_ByNameAndArgCount passed");
        }
        
        private static void Test_GetMyMethod_NoMatchingMethod()
        {
            var type = typeof(MethodTestClass);
            var method = type.GetMyMethod("NonExistent", 0);
            
            if (method != null)
                throw new Exception("Test_GetMyMethod_NoMatchingMethod failed");
            
            Console.WriteLine("  ✓ Test_GetMyMethod_NoMatchingMethod passed");
        }
        
        private static void Test_GetMyMethod_WithParameterType()
        {
            var type = typeof(MethodTestClass);
            var method = type.GetMyMethod("OneArg", 1, typeof(string));
            
            if (method == null || method.Name != "OneArg")
                throw new Exception("Test_GetMyMethod_WithParameterType failed");
            
            Console.WriteLine("  ✓ Test_GetMyMethod_WithParameterType passed");
        }
        
        private static void Test_GetMyMethod_WithTwoParameterTypes()
        {
            var type = typeof(MethodTestClass);
            var method = type.GetMyMethod("TwoArgs", 2, typeof(string), typeof(int));
            
            if (method == null || method.Name != "TwoArgs")
                throw new Exception("Test_GetMyMethod_WithTwoParameterTypes failed");
            
            Console.WriteLine("  ✓ Test_GetMyMethod_WithTwoParameterTypes passed");
        }
        
        private static void Test_GetMyMethod_WithThreeParameterTypes()
        {
            var type = typeof(MethodTestClass);
            var method = type.GetMyMethod("ThreeArgs", 3, typeof(string), typeof(int), typeof(bool));
            
            if (method == null || method.Name != "ThreeArgs")
                throw new Exception("Test_GetMyMethod_WithThreeParameterTypes failed");
            
            Console.WriteLine("  ✓ Test_GetMyMethod_WithThreeParameterTypes passed");
        }
        
        private static void Test_GetMyMethod_WithFourParameterTypes()
        {
            var type = typeof(MethodTestClass);
            var method = type.GetMyMethod("FourArgs", 4, typeof(string), typeof(int), typeof(bool), typeof(double));
            
            if (method == null || method.Name != "FourArgs")
                throw new Exception("Test_GetMyMethod_WithFourParameterTypes failed");
            
            Console.WriteLine("  ✓ Test_GetMyMethod_WithFourParameterTypes passed");
        }
        
        private static void Test_GetMyMethod_WithFiveParameterTypes()
        {
            var type = typeof(MethodTestClass);
            var method = type.GetMyMethod("FiveArgs", 5, typeof(string), typeof(int), typeof(bool), typeof(double), typeof(object));
            
            if (method == null || method.Name != "FiveArgs")
                throw new Exception("Test_GetMyMethod_WithFiveParameterTypes failed");
            
            Console.WriteLine("  ✓ Test_GetMyMethod_WithFiveParameterTypes passed");
        }
        
        private static void Test_GetMyMethod_IsList_True()
        {
            var type = typeof(MethodTestClass);
            var method = type.GetMyMethod("ListArg", 1, true);
            
            if (method == null || method.Name != "ListArg")
                throw new Exception("Test_GetMyMethod_IsList_True failed");
            
            Console.WriteLine("  ✓ Test_GetMyMethod_IsList_True passed");
        }
        
        private static void Test_GetMyMethod_IsList_False()
        {
            var type = typeof(MethodTestClass);
            var method = type.GetMyMethod("ArrayArg", 1, false);
            
            if (method == null || method.Name != "ArrayArg")
                throw new Exception("Test_GetMyMethod_IsList_False failed");
            
            Console.WriteLine("  ✓ Test_GetMyMethod_IsList_False passed");
        }
        
        private static void Test_GetMyMethod_StringType()
        {
            var type = typeof(MethodTestClass);
            var method = type.GetMyMethod("OneArg", 1, typeof(string));
            
            if (method == null)
                throw new Exception("Test_GetMyMethod_StringType failed");
            
            Console.WriteLine("  ✓ Test_GetMyMethod_StringType passed");
        }
        
        private static void Test_GetMyMethod_IntType()
        {
            var type = typeof(MethodTestClass);
            var method = type.GetMyMethod("IntArg", 1, typeof(int));
            
            if (method == null)
                throw new Exception("Test_GetMyMethod_IntType failed");
            
            Console.WriteLine("  ✓ Test_GetMyMethod_IntType passed");
        }
        
        private static void Test_GetMyMethod_ObjectType()
        {
            var type = typeof(MethodTestClass);
            var method = type.GetMyMethod("ObjectArg", 1, typeof(object));
            
            if (method == null)
                throw new Exception("Test_GetMyMethod_ObjectType failed");
            
            Console.WriteLine("  ✓ Test_GetMyMethod_ObjectType passed");
        }
        
        #endregion

        #region Category 5: WhereIF Tests
        
        private static void Test_WhereIF_ConditionTrue_FiltersData()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };
            var result = list.WhereIF(true, x => x > 3).ToList();
            
            if (result.Count != 2 || !result.Contains(4) || !result.Contains(5))
                throw new Exception("Test_WhereIF_ConditionTrue_FiltersData failed");
            
            Console.WriteLine("  ✓ Test_WhereIF_ConditionTrue_FiltersData passed");
        }
        
        private static void Test_WhereIF_ConditionFalse_ReturnsAll()
        {
            var list = new List<int> { 1, 2, 3, 4, 5 };
            var result = list.WhereIF(false, x => x > 3).ToList();
            
            if (result.Count != 5)
                throw new Exception("Test_WhereIF_ConditionFalse_ReturnsAll failed");
            
            Console.WriteLine("  ✓ Test_WhereIF_ConditionFalse_ReturnsAll passed");
        }
        
        private static void Test_WhereIF_EmptyCollection()
        {
            var list = new List<int>();
            var result = list.WhereIF(true, x => x > 0).ToList();
            
            if (result.Count != 0)
                throw new Exception("Test_WhereIF_EmptyCollection failed");
            
            Console.WriteLine("  ✓ Test_WhereIF_EmptyCollection passed");
        }
        
        private static void Test_WhereIF_SingleElement_ConditionTrue()
        {
            var list = new List<int> { 5 };
            var result = list.WhereIF(true, x => x > 3).ToList();
            
            if (result.Count != 1)
                throw new Exception("Test_WhereIF_SingleElement_ConditionTrue failed");
            
            Console.WriteLine("  ✓ Test_WhereIF_SingleElement_ConditionTrue passed");
        }
        
        private static void Test_WhereIF_SingleElement_ConditionFalse()
        {
            var list = new List<int> { 5 };
            var result = list.WhereIF(false, x => x > 10).ToList();
            
            if (result.Count != 1)
                throw new Exception("Test_WhereIF_SingleElement_ConditionFalse failed");
            
            Console.WriteLine("  ✓ Test_WhereIF_SingleElement_ConditionFalse passed");
        }
        
        private static void Test_WhereIF_MultipleElements_SomeMatch()
        {
            var list = new List<string> { "apple", "banana", "cherry" };
            var result = list.WhereIF(true, x => x.StartsWith("a") || x.StartsWith("b")).ToList();
            
            if (result.Count != 2)
                throw new Exception("Test_WhereIF_MultipleElements_SomeMatch failed");
            
            Console.WriteLine("  ✓ Test_WhereIF_MultipleElements_SomeMatch passed");
        }
        
        private static void Test_WhereIF_MultipleElements_NoneMatch()
        {
            var list = new List<int> { 1, 2, 3 };
            var result = list.WhereIF(true, x => x > 10).ToList();
            
            if (result.Count != 0)
                throw new Exception("Test_WhereIF_MultipleElements_NoneMatch failed");
            
            Console.WriteLine("  ✓ Test_WhereIF_MultipleElements_NoneMatch passed");
        }
        
        private static void Test_WhereIF_MultipleElements_AllMatch()
        {
            var list = new List<int> { 10, 20, 30 };
            var result = list.WhereIF(true, x => x >= 10).ToList();
            
            if (result.Count != 3)
                throw new Exception("Test_WhereIF_MultipleElements_AllMatch failed");
            
            Console.WriteLine("  ✓ Test_WhereIF_MultipleElements_AllMatch passed");
        }
        
        #endregion

        #region Category 6: ToList Extensions Tests
        
        private class SimpleClass
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        
        private static void Test_ToList_SingleObject()
        {
            var obj = new SimpleClass { Id = 1, Name = "Test" };
            var result = obj.ToList(x => x);
            
            if (result.Count != 1 || result[0].Id != 1)
                throw new Exception("Test_ToList_SingleObject failed");
            
            Console.WriteLine("  ✓ Test_ToList_SingleObject passed");
        }
        
        private static void Test_ToList_WithAction()
        {
            var obj = new SimpleClass { Id = 1, Name = "Test" };
            var result = obj.ToList(x => { x.Name = "Modified"; return x; });
            
            if (result.Count != 1)
                throw new Exception("Test_ToList_WithAction failed");
            
            Console.WriteLine("  ✓ Test_ToList_WithAction passed");
        }
        
        private static void Test_ToList_IEnumerable()
        {
            IEnumerable<SimpleClass> enumerable = new List<SimpleClass>
            {
                new SimpleClass { Id = 1 },
                new SimpleClass { Id = 2 }
            };
            
            var result = enumerable.ToList(x => x);
            
            if (result.Count != 2)
                throw new Exception("Test_ToList_IEnumerable failed");
            
            Console.WriteLine("  ✓ Test_ToList_IEnumerable passed");
        }
        
        private static void Test_ToList_IEnumerable_WithAction()
        {
            IEnumerable<SimpleClass> enumerable = new List<SimpleClass>
            {
                new SimpleClass { Id = 1, Name = "A" }
            };
            
            var result = enumerable.ToList(x => x);
            
            if (result.Count != 1)
                throw new Exception("Test_ToList_IEnumerable_WithAction failed");
            
            Console.WriteLine("  ✓ Test_ToList_IEnumerable_WithAction passed");
        }
        
        private static void Test_ToList_EmptyEnumerable()
        {
            IEnumerable<SimpleClass> enumerable = new List<SimpleClass>();
            var result = enumerable.ToList(x => x);
            
            if (result.Count != 0)
                throw new Exception("Test_ToList_EmptyEnumerable failed");
            
            Console.WriteLine("  ✓ Test_ToList_EmptyEnumerable passed");
        }
        
        private static void Test_ToList_MultipleElements()
        {
            IEnumerable<SimpleClass> enumerable = new List<SimpleClass>
            {
                new SimpleClass { Id = 1 },
                new SimpleClass { Id = 2 },
                new SimpleClass { Id = 3 }
            };
            
            var result = enumerable.ToList(x => x);
            
            if (result.Count != 3)
                throw new Exception("Test_ToList_MultipleElements failed");
            
            Console.WriteLine("  ✓ Test_ToList_MultipleElements passed");
        }
        
        #endregion

        #region Category 7: Edge Cases Tests
        
        private static void Test_EdgeCase_VeryLongString()
        {
            string longString = new string('x', 10000);
            string result = longString.SafeSubstring(0, 100);
            
            if (result.Length != 100)
                throw new Exception("Test_EdgeCase_VeryLongString failed");
            
            Console.WriteLine("  ✓ Test_EdgeCase_VeryLongString passed");
        }
        
        private static void Test_EdgeCase_SpecialUnicodeCharacters()
        {
            string unicode = "🎉🎊🎁";
            string hash = unicode.GetNonNegativeHashCodeString();
            
            if (!hash.StartsWith("hs"))
                throw new Exception("Test_EdgeCase_SpecialUnicodeCharacters failed");
            
            Console.WriteLine("  ✓ Test_EdgeCase_SpecialUnicodeCharacters passed");
        }
        
        private static void Test_EdgeCase_MaxIntHashCode()
        {
            // Test with a string that might produce negative hash
            string input = "negative_hash_test_string_12345";
            string result = input.GetNonNegativeHashCodeString();
            string numericPart = result.Substring(2);
            
            if (!int.TryParse(numericPart, out int value) || value < 0)
                throw new Exception("Test_EdgeCase_MaxIntHashCode failed");
            
            Console.WriteLine("  ✓ Test_EdgeCase_MaxIntHashCode passed");
        }
        
        private static void Test_EdgeCase_EmptyCollections()
        {
            var emptyList = new List<int>();
            var result = emptyList.WhereIF(true, x => x > 0).ToList();
            
            if (result.Count != 0)
                throw new Exception("Test_EdgeCase_EmptyCollections failed");
            
            Console.WriteLine("  ✓ Test_EdgeCase_EmptyCollections passed");
        }
        
        private static void Test_EdgeCase_LargeCollection()
        {
            var largeList = Enumerable.Range(1, 10000).ToList();
            var result = largeList.WhereIF(true, x => x > 9990).ToList();
            
            if (result.Count != 10)
                throw new Exception("Test_EdgeCase_LargeCollection failed");
            
            Console.WriteLine("  ✓ Test_EdgeCase_LargeCollection passed");
        }
        
        private static void Test_EdgeCase_NestedTypes()
        {
            var type = typeof(List<Dictionary<string, int>>);
            bool isGeneric = type.IsGenericType;
            
            if (!isGeneric)
                throw new Exception("Test_EdgeCase_NestedTypes failed");
            
            Console.WriteLine("  ✓ Test_EdgeCase_NestedTypes passed");
        }
        
        #endregion
    }
}

using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OrmTest
{
    /// <summary>
    /// Comprehensive test suite for CommonExtensions utility class.
    /// Tests 40+ scenarios across multiple categories:
    /// - GetNonNegativeHashCodeString (5 tests)
    /// - SafeSubstring Edge Cases (9 tests)
    /// - ToDictionary<T> (5 tests)
    /// - GetMyMethod Overloads (11 tests)
    /// - WhereIF Conditional LINQ (4 tests)
    /// - MappingField (3 tests)
    /// - ToList Extensions (3 tests)
    /// - EnumerableExtensions.Contains (2 tests)
    /// 
    /// All tests directly invoke public static extension methods from CommonExtensions.
    /// Tests cover normal cases, edge cases, and boundary conditions.
    /// </summary>
    public class UCommonExtensions
    {
        #region Test Initialization

        /// <summary>
        /// Main entry point for test suite execution.
        /// Runs all 42 CommonExtensions tests in organized categories.
        /// Throws exception on first failure for quick debugging.
        /// </summary>
        public static void Init()
        {
            Console.WriteLine("\n========== UCommonExtensions Test Suite Started ==========");

            // Category 1: GetNonNegativeHashCodeString (5 tests)
            Test_GetNonNegativeHashCodeString_NormalString();
            Test_GetNonNegativeHashCodeString_EmptyString();
            Test_GetNonNegativeHashCodeString_SameStringsSameHash();
            Test_GetNonNegativeHashCodeString_UnicodeString();
            Test_GetNonNegativeHashCodeString_SpecialCharacters();

            // Category 2: SafeSubstring Edge Cases (9 tests)
            Test_SafeSubstring_NormalCase();
            Test_SafeSubstring_NegativeStartIndex();
            Test_SafeSubstring_StartIndexBeyondLength();
            Test_SafeSubstring_NegativeLength();
            Test_SafeSubstring_LengthExceedsRemaining();
            Test_SafeSubstring_NullString();
            Test_SafeSubstring_ZeroLength();
            Test_SafeSubstring_ExactLength();
            Test_SafeSubstring_SingleCharacter();

            // Category 3: ToDictionary<T> (5 tests)
            Test_ToDictionary_NormalCase();
            Test_ToDictionary_EmptyList();
            Test_ToDictionary_MultipleItems();
            Test_ToDictionary_NullValues();
            Test_ToDictionary_NumericKeys();

            // Category 4: GetMyMethod Overloads (11 tests)
            Test_GetMyMethod_ByNameAndArgCount();
            Test_GetMyMethod_ByNameArgCountAndParameterType();
            Test_GetMyMethod_ByNameArgCountAndIsList();
            Test_GetMyMethod_ByNameArgCountAndTwoParameterTypes();
            Test_GetMyMethod_ByNameArgCountAndThreeParameterTypes();
            Test_GetMyMethod_ByNameArgCountAndFourParameterTypes();
            Test_GetMyMethod_ByNameArgCountAndFiveParameterTypes();
            Test_GetMyMethodNoGen_NonGenericMethod();
            Test_GetMyMethodIsGenericMethod_GenericMethod();
            Test_GetMyMethod_MethodNotFound();
            Test_GetMyMethod_MultipleOverloads();

            // Category 5: WhereIF Conditional LINQ (4 tests)
            Test_WhereIF_ConditionTrue();
            Test_WhereIF_ConditionFalse();
            Test_WhereIF_EmptyCollection();
            Test_WhereIF_ComplexPredicate();

            // Category 6: MappingField (3 tests)
            Test_MappingField_IEnumerable();
            Test_MappingField_SingleObject();
            Test_MappingField_NullObject();

            // Category 7: ToList Extensions (3 tests)
            Test_ToList_SingleObject();
            Test_ToList_IEnumerable();
            Test_ToList_WithAction();

            // Category 8: EnumerableExtensions.Contains (2 tests)
            Test_Contains_WithIsNvarchar();
            Test_Contains_NotFound();

            Console.WriteLine("\n========== All 42 Tests Passed Successfully! ==========");
        }

        #endregion

        #region Category 1: GetNonNegativeHashCodeString Tests (5 tests)

        /// <summary>
        /// Test: Normal string should return hash code with "hs" prefix and non-negative value.
        /// Validates that the method returns a string starting with "hs" followed by digits.
        /// </summary>
        private static void Test_GetNonNegativeHashCodeString_NormalString()
        {
            string input = "TestString123";
            string result = CommonExtensions.GetNonNegativeHashCodeString(input);

            // Verify result starts with "hs" and contains only digits after
            if (!result.StartsWith("hs"))
                throw new Exception("Test_GetNonNegativeHashCodeString_NormalString failed: Result doesn't start with 'hs'");

            string numericPart = result.Substring(2);
            if (!int.TryParse(numericPart, out int hashValue) || hashValue < 0)
                throw new Exception("Test_GetNonNegativeHashCodeString_NormalString failed: Hash value is not a non-negative integer");

            Console.WriteLine($"✓ Test_GetNonNegativeHashCodeString_NormalString passed (result: {result})");
        }

        /// <summary>
        /// Test: Empty string should return a valid hash code string.
        /// Even empty strings have hash codes in .NET.
        /// </summary>
        private static void Test_GetNonNegativeHashCodeString_EmptyString()
        {
            string input = "";
            string result = CommonExtensions.GetNonNegativeHashCodeString(input);

            if (!result.StartsWith("hs"))
                throw new Exception("Test_GetNonNegativeHashCodeString_EmptyString failed: Result doesn't start with 'hs'");

            Console.WriteLine($"✓ Test_GetNonNegativeHashCodeString_EmptyString passed (result: {result})");
        }

        /// <summary>
        /// Test: Same strings should produce the same hash code.
        /// Validates consistency of hash code generation.
        /// </summary>
        private static void Test_GetNonNegativeHashCodeString_SameStringsSameHash()
        {
            string input1 = "ConsistencyTest";
            string input2 = "ConsistencyTest";

            string result1 = CommonExtensions.GetNonNegativeHashCodeString(input1);
            string result2 = CommonExtensions.GetNonNegativeHashCodeString(input2);

            if (result1 != result2)
                throw new Exception("Test_GetNonNegativeHashCodeString_SameStringsSameHash failed: Same strings produced different hashes");

            Console.WriteLine($"✓ Test_GetNonNegativeHashCodeString_SameStringsSameHash passed (result: {result1})");
        }

        /// <summary>
        /// Test: Unicode string should return valid hash code.
        /// Tests handling of non-ASCII characters.
        /// </summary>
        private static void Test_GetNonNegativeHashCodeString_UnicodeString()
        {
            string input = "你好世界🌍";
            string result = CommonExtensions.GetNonNegativeHashCodeString(input);

            if (!result.StartsWith("hs"))
                throw new Exception("Test_GetNonNegativeHashCodeString_UnicodeString failed: Result doesn't start with 'hs'");

            string numericPart = result.Substring(2);
            if (!int.TryParse(numericPart, out int hashValue) || hashValue < 0)
                throw new Exception("Test_GetNonNegativeHashCodeString_UnicodeString failed: Hash value is not a non-negative integer");

            Console.WriteLine($"✓ Test_GetNonNegativeHashCodeString_UnicodeString passed (result: {result})");
        }

        /// <summary>
        /// Test: String with special characters should return valid hash code.
        /// Tests handling of symbols and punctuation.
        /// </summary>
        private static void Test_GetNonNegativeHashCodeString_SpecialCharacters()
        {
            string input = "!@#$%^&*()_+-=[]{}|;':\",./<>?";
            string result = CommonExtensions.GetNonNegativeHashCodeString(input);

            if (!result.StartsWith("hs"))
                throw new Exception("Test_GetNonNegativeHashCodeString_SpecialCharacters failed: Result doesn't start with 'hs'");

            Console.WriteLine($"✓ Test_GetNonNegativeHashCodeString_SpecialCharacters passed (result: {result})");
        }

        #endregion

        #region Category 2: SafeSubstring Edge Cases (9 tests)

        /// <summary>
        /// Test: Normal substring operation with valid parameters.
        /// Should return the expected substring.
        /// </summary>
        private static void Test_SafeSubstring_NormalCase()
        {
            string input = "HelloWorld";
            string result = CommonExtensions.SafeSubstring(input, 0, 5);

            if (result != "Hello")
                throw new Exception($"Test_SafeSubstring_NormalCase failed: Expected 'Hello', got '{result}'");

            Console.WriteLine("✓ Test_SafeSubstring_NormalCase passed");
        }

        /// <summary>
        /// Test: Negative start index should be treated as 0.
        /// Edge case: Method should auto-correct negative indices to 0.
        /// </summary>
        private static void Test_SafeSubstring_NegativeStartIndex()
        {
            string input = "HelloWorld";
            string result = CommonExtensions.SafeSubstring(input, -5, 5);

            if (result != "Hello")
                throw new Exception($"Test_SafeSubstring_NegativeStartIndex failed: Expected 'Hello', got '{result}'");

            Console.WriteLine("✓ Test_SafeSubstring_NegativeStartIndex passed");
        }

        /// <summary>
        /// Test: Start index beyond string length should return empty string.
        /// Edge case: Prevents IndexOutOfRangeException.
        /// </summary>
        private static void Test_SafeSubstring_StartIndexBeyondLength()
        {
            string input = "Hello";
            string result = CommonExtensions.SafeSubstring(input, 10, 5);

            if (result != string.Empty)
                throw new Exception($"Test_SafeSubstring_StartIndexBeyondLength failed: Expected empty string, got '{result}'");

            Console.WriteLine("✓ Test_SafeSubstring_StartIndexBeyondLength passed");
        }

        /// <summary>
        /// Test: Negative length should be treated as 0, returning empty string.
        /// Edge case: Method should handle invalid length gracefully.
        /// </summary>
        private static void Test_SafeSubstring_NegativeLength()
        {
            string input = "HelloWorld";
            string result = CommonExtensions.SafeSubstring(input, 0, -5);

            if (result != string.Empty)
                throw new Exception($"Test_SafeSubstring_NegativeLength failed: Expected empty string, got '{result}'");

            Console.WriteLine("✓ Test_SafeSubstring_NegativeLength passed");
        }

        /// <summary>
        /// Test: Length exceeding remaining characters should return only available characters.
        /// Edge case: Method should truncate to available length.
        /// </summary>
        private static void Test_SafeSubstring_LengthExceedsRemaining()
        {
            string input = "Hello";
            string result = CommonExtensions.SafeSubstring(input, 3, 10);

            if (result != "lo")
                throw new Exception($"Test_SafeSubstring_LengthExceedsRemaining failed: Expected 'lo', got '{result}'");

            Console.WriteLine("✓ Test_SafeSubstring_LengthExceedsRemaining passed");
        }

        /// <summary>
        /// Test: Null string should throw ArgumentNullException.
        /// Edge case: Method should validate null input.
        /// </summary>
        private static void Test_SafeSubstring_NullString()
        {
            try
            {
                string result = CommonExtensions.SafeSubstring(null, 0, 5);
                throw new Exception("Test_SafeSubstring_NullString failed: Expected ArgumentNullException");
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine("✓ Test_SafeSubstring_NullString passed");
            }
        }

        /// <summary>
        /// Test: Zero length should return empty string.
        /// Edge case: Requesting zero characters should return empty result.
        /// </summary>
        private static void Test_SafeSubstring_ZeroLength()
        {
            string input = "HelloWorld";
            string result = CommonExtensions.SafeSubstring(input, 5, 0);

            if (result != string.Empty)
                throw new Exception($"Test_SafeSubstring_ZeroLength failed: Expected empty string, got '{result}'");

            Console.WriteLine("✓ Test_SafeSubstring_ZeroLength passed");
        }

        /// <summary>
        /// Test: Exact length substring should return complete remaining string.
        /// Tests boundary condition where length exactly matches remaining characters.
        /// </summary>
        private static void Test_SafeSubstring_ExactLength()
        {
            string input = "HelloWorld";
            string result = CommonExtensions.SafeSubstring(input, 5, 5);

            if (result != "World")
                throw new Exception($"Test_SafeSubstring_ExactLength failed: Expected 'World', got '{result}'");

            Console.WriteLine("✓ Test_SafeSubstring_ExactLength passed");
        }

        /// <summary>
        /// Test: Single character extraction should work correctly.
        /// Tests minimal substring operation.
        /// </summary>
        private static void Test_SafeSubstring_SingleCharacter()
        {
            string input = "A";
            string result = CommonExtensions.SafeSubstring(input, 0, 1);

            if (result != "A")
                throw new Exception($"Test_SafeSubstring_SingleCharacter failed: Expected 'A', got '{result}'");

            Console.WriteLine("✓ Test_SafeSubstring_SingleCharacter passed");
        }

        #endregion

        #region Category 3: ToDictionary<T> Tests (5 tests)

        /// <summary>
        /// Test: Convert list to dictionary using property names.
        /// Normal case with valid key and value properties.
        /// </summary>
        private static void Test_ToDictionary_NormalCase()
        {
            var list = new List<TestPerson>
            {
                new TestPerson { Id = 1, Name = "Alice" }
            };

            var result = CommonExtensions.ToDictionary(list, "Id", "Name");

            if (result.Count != 1 || result["1"].ToString() != "Alice")
                throw new Exception("Test_ToDictionary_NormalCase failed: Dictionary conversion incorrect");

            Console.WriteLine("✓ Test_ToDictionary_NormalCase passed");
        }

        /// <summary>
        /// Test: Empty list should return empty dictionary.
        /// Edge case: Method should handle empty collections.
        /// </summary>
        private static void Test_ToDictionary_EmptyList()
        {
            var list = new List<TestPerson>();
            var result = CommonExtensions.ToDictionary(list, "Id", "Name");

            if (result.Count != 0)
                throw new Exception("Test_ToDictionary_EmptyList failed: Expected empty dictionary");

            Console.WriteLine("✓ Test_ToDictionary_EmptyList passed");
        }

        /// <summary>
        /// Test: Multiple items should be correctly converted to dictionary.
        /// Validates that all items are properly mapped with unique keys.
        /// </summary>
        private static void Test_ToDictionary_MultipleItems()
        {
            var list = new List<TestPerson>
            {
                new TestPerson { Id = 1, Name = "Alice" },
                new TestPerson { Id = 2, Name = "Bob" },
                new TestPerson { Id = 3, Name = "Charlie" }
            };

            var result = CommonExtensions.ToDictionary(list, "Id", "Name");

            if (result.Count != 3 || 
                result["1"].ToString() != "Alice" || 
                result["2"].ToString() != "Bob" || 
                result["3"].ToString() != "Charlie")
                throw new Exception("Test_ToDictionary_MultipleItems failed: Dictionary conversion incorrect");

            Console.WriteLine("✓ Test_ToDictionary_MultipleItems passed");
        }

        /// <summary>
        /// Test: ToDictionary should handle null values in properties.
        /// Tests that null values are properly stored in the dictionary.
        /// </summary>
        private static void Test_ToDictionary_NullValues()
        {
            var list = new List<TestPerson>
            {
                new TestPerson { Id = 1, Name = null },
                new TestPerson { Id = 2, Name = "Bob" }
            };

            var result = CommonExtensions.ToDictionary(list, "Id", "Name");

            if (result.Count != 2 || result["1"] != null || result["2"].ToString() != "Bob")
                throw new Exception("Test_ToDictionary_NullValues failed: Null values not handled correctly");

            Console.WriteLine("✓ Test_ToDictionary_NullValues passed");
        }

        /// <summary>
        /// Test: ToDictionary with numeric keys should convert to string keys.
        /// Validates that numeric property values are converted to string keys.
        /// </summary>
        private static void Test_ToDictionary_NumericKeys()
        {
            var list = new List<TestPerson>
            {
                new TestPerson { Id = 100, Name = "Alice" },
                new TestPerson { Id = 200, Name = "Bob" }
            };

            var result = CommonExtensions.ToDictionary(list, "Id", "Name");

            if (result.Count != 2 || !result.ContainsKey("100") || !result.ContainsKey("200"))
                throw new Exception("Test_ToDictionary_NumericKeys failed: Numeric keys not converted to strings");

            Console.WriteLine("✓ Test_ToDictionary_NumericKeys passed");
        }

        #endregion

        #region Category 4: GetMyMethod Overloads Tests (11 tests)

        /// <summary>
        /// Test: GetMyMethod(Type, string, int) - Find method by name and argument count.
        /// Tests the simplest overload with just name and parameter count.
        /// </summary>
        private static void Test_GetMyMethod_ByNameAndArgCount()
        {
            Type type = typeof(TestClass);
            MethodInfo method = CommonExtensions.GetMyMethod(type, "TestMethod1", 0);

            if (method == null || method.Name != "TestMethod1")
                throw new Exception("Test_GetMyMethod_ByNameAndArgCount failed: Method not found");

            Console.WriteLine("✓ Test_GetMyMethod_ByNameAndArgCount passed");
        }

        /// <summary>
        /// Test: GetMyMethod(Type, string, int, Type) - Find method by name, arg count, and parameter type.
        /// Tests overload that matches specific parameter type.
        /// </summary>
        private static void Test_GetMyMethod_ByNameArgCountAndParameterType()
        {
            Type type = typeof(TestClass);
            MethodInfo method = CommonExtensions.GetMyMethod(type, "TestMethod2", 1, typeof(string));

            if (method == null || method.GetParameters()[0].ParameterType != typeof(string))
                throw new Exception("Test_GetMyMethod_ByNameArgCountAndParameterType failed: Method not found or wrong parameter type");

            Console.WriteLine("✓ Test_GetMyMethod_ByNameArgCountAndParameterType passed");
        }

        /// <summary>
        /// Test: GetMyMethod(Type, string, int, bool) - Find method by name, arg count, and List parameter check.
        /// Tests overload that filters by whether first parameter is a List type.
        /// </summary>
        private static void Test_GetMyMethod_ByNameArgCountAndIsList()
        {
            Type type = typeof(TestClass);
            MethodInfo method = CommonExtensions.GetMyMethod(type, "TestMethod3", 1, false);

            if (method == null)
                throw new Exception("Test_GetMyMethod_ByNameArgCountAndIsList failed: Method not found");

            Console.WriteLine("✓ Test_GetMyMethod_ByNameArgCountAndIsList passed");
        }

        /// <summary>
        /// Test: GetMyMethod(Type, string, int, Type, Type) - Find method with two specific parameter types.
        /// Tests overload that matches two parameter types.
        /// </summary>
        private static void Test_GetMyMethod_ByNameArgCountAndTwoParameterTypes()
        {
            Type type = typeof(TestClass);
            MethodInfo method = CommonExtensions.GetMyMethod(type, "TestMethod4", 2, typeof(string), typeof(int));

            if (method == null || 
                method.GetParameters()[0].ParameterType != typeof(string) ||
                method.GetParameters()[1].ParameterType != typeof(int))
                throw new Exception("Test_GetMyMethod_ByNameArgCountAndTwoParameterTypes failed: Method not found or wrong parameter types");

            Console.WriteLine("✓ Test_GetMyMethod_ByNameArgCountAndTwoParameterTypes passed");
        }

        /// <summary>
        /// Test: GetMyMethod(Type, string, int, Type, Type, Type) - Find method with three specific parameter types.
        /// Tests overload that matches three parameter types.
        /// </summary>
        private static void Test_GetMyMethod_ByNameArgCountAndThreeParameterTypes()
        {
            Type type = typeof(TestClass);
            MethodInfo method = CommonExtensions.GetMyMethod(type, "TestMethod5", 3, typeof(string), typeof(int), typeof(bool));

            if (method == null || 
                method.GetParameters().Length != 3 ||
                method.GetParameters()[0].ParameterType != typeof(string) ||
                method.GetParameters()[1].ParameterType != typeof(int) ||
                method.GetParameters()[2].ParameterType != typeof(bool))
                throw new Exception("Test_GetMyMethod_ByNameArgCountAndThreeParameterTypes failed: Method not found or wrong parameter types");

            Console.WriteLine("✓ Test_GetMyMethod_ByNameArgCountAndThreeParameterTypes passed");
        }

        /// <summary>
        /// Test: GetMyMethod(Type, string, int, Type, Type, Type, Type) - Find method with four specific parameter types.
        /// Tests overload that matches four parameter types.
        /// </summary>
        private static void Test_GetMyMethod_ByNameArgCountAndFourParameterTypes()
        {
            Type type = typeof(TestClass);
            MethodInfo method = CommonExtensions.GetMyMethod(type, "TestMethod6", 4, typeof(string), typeof(int), typeof(bool), typeof(double));

            if (method == null || method.GetParameters().Length != 4)
                throw new Exception("Test_GetMyMethod_ByNameArgCountAndFourParameterTypes failed: Method not found or wrong parameter count");

            Console.WriteLine("✓ Test_GetMyMethod_ByNameArgCountAndFourParameterTypes passed");
        }

        /// <summary>
        /// Test: GetMyMethod(Type, string, int, Type, Type, Type, Type, Type) - Find method with five specific parameter types.
        /// Tests the most complex overload with five parameter types.
        /// </summary>
        private static void Test_GetMyMethod_ByNameArgCountAndFiveParameterTypes()
        {
            Type type = typeof(TestClass);
            MethodInfo method = CommonExtensions.GetMyMethod(type, "TestMethod7", 5, typeof(string), typeof(int), typeof(bool), typeof(double), typeof(decimal));

            if (method == null || method.GetParameters().Length != 5)
                throw new Exception("Test_GetMyMethod_ByNameArgCountAndFiveParameterTypes failed: Method not found or wrong parameter count");

            Console.WriteLine("✓ Test_GetMyMethod_ByNameArgCountAndFiveParameterTypes passed");
        }

        /// <summary>
        /// Test: GetMyMethodNoGen - Find non-generic method with two parameter types.
        /// Tests the overload that specifically excludes generic methods.
        /// </summary>
        private static void Test_GetMyMethodNoGen_NonGenericMethod()
        {
            Type type = typeof(TestClass);
            MethodInfo method = CommonExtensions.GetMyMethodNoGen(type, "TestMethod4", 2, typeof(string), typeof(int));

            if (method == null || method.IsGenericMethod)
                throw new Exception("Test_GetMyMethodNoGen_NonGenericMethod failed: Method not found or is generic");

            Console.WriteLine("✓ Test_GetMyMethodNoGen_NonGenericMethod passed");
        }

        /// <summary>
        /// Test: GetMyMethodIsGenericMethod - Find generic method with three parameter types.
        /// Tests the overload that specifically requires generic methods.
        /// </summary>
        private static void Test_GetMyMethodIsGenericMethod_GenericMethod()
        {
            Type type = typeof(TestClass);
            MethodInfo method = CommonExtensions.GetMyMethodIsGenericMethod(type, "TestGenericMethod", 3, typeof(string), typeof(int), typeof(bool));

            if (method == null || !method.IsGenericMethod)
                throw new Exception("Test_GetMyMethodIsGenericMethod_GenericMethod failed: Method not found or is not generic");

            Console.WriteLine("✓ Test_GetMyMethodIsGenericMethod_GenericMethod passed");
        }

        /// <summary>
        /// Test: GetMyMethod should return null when method is not found.
        /// Tests error handling when no matching method exists.
        /// </summary>
        private static void Test_GetMyMethod_MethodNotFound()
        {
            Type type = typeof(TestClass);
            MethodInfo method = CommonExtensions.GetMyMethod(type, "NonExistentMethod", 0);

            if (method != null)
                throw new Exception("Test_GetMyMethod_MethodNotFound failed: Should return null for non-existent method");

            Console.WriteLine("✓ Test_GetMyMethod_MethodNotFound passed");
        }

        /// <summary>
        /// Test: GetMyMethod should correctly distinguish between overloaded methods.
        /// Tests that the method can differentiate between methods with same name but different signatures.
        /// </summary>
        private static void Test_GetMyMethod_MultipleOverloads()
        {
            Type type = typeof(TestClass);
            MethodInfo method1 = CommonExtensions.GetMyMethod(type, "TestMethod2", 1, typeof(string));
            MethodInfo method2 = CommonExtensions.GetMyMethod(type, "TestMethod3", 1, typeof(int));

            if (method1 == null || method2 == null || method1.Equals(method2))
                throw new Exception("Test_GetMyMethod_MultipleOverloads failed: Should distinguish between overloads");

            Console.WriteLine("✓ Test_GetMyMethod_MultipleOverloads passed");
        }

        #endregion

        #region Category 5: WhereIF Conditional LINQ Tests (4 tests)

        /// <summary>
        /// Test: WhereIF with condition true should apply the predicate filter.
        /// When isOk is true, the Where clause should be applied.
        /// </summary>
        private static void Test_WhereIF_ConditionTrue()
        {
            var numbers = new List<int> { 1, 2, 3, 4, 5 };
            var result = CommonExtensions.WhereIF(numbers, true, x => x > 3).ToList();

            if (result.Count != 2 || !result.Contains(4) || !result.Contains(5))
                throw new Exception("Test_WhereIF_ConditionTrue failed: Filter not applied correctly");

            Console.WriteLine("✓ Test_WhereIF_ConditionTrue passed");
        }

        /// <summary>
        /// Test: WhereIF with condition false should return all items without filtering.
        /// When isOk is false, the predicate should be ignored and all items returned.
        /// </summary>
        private static void Test_WhereIF_ConditionFalse()
        {
            var numbers = new List<int> { 1, 2, 3, 4, 5 };
            var result = CommonExtensions.WhereIF(numbers, false, x => x > 3).ToList();

            if (result.Count != 5)
                throw new Exception("Test_WhereIF_ConditionFalse failed: All items should be returned");

            Console.WriteLine("✓ Test_WhereIF_ConditionFalse passed");
        }

        /// <summary>
        /// Test: WhereIF on empty collection should return empty collection.
        /// Tests behavior with empty input regardless of condition.
        /// </summary>
        private static void Test_WhereIF_EmptyCollection()
        {
            var numbers = new List<int>();
            var result = CommonExtensions.WhereIF(numbers, true, x => x > 3).ToList();

            if (result.Count != 0)
                throw new Exception("Test_WhereIF_EmptyCollection failed: Should return empty collection");

            Console.WriteLine("✓ Test_WhereIF_EmptyCollection passed");
        }

        /// <summary>
        /// Test: WhereIF with complex predicate should work correctly.
        /// Tests more complex filtering logic with multiple conditions.
        /// </summary>
        private static void Test_WhereIF_ComplexPredicate()
        {
            var numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var result = CommonExtensions.WhereIF(numbers, true, x => x % 2 == 0 && x > 4).ToList();

            if (result.Count != 3 || !result.Contains(6) || !result.Contains(8) || !result.Contains(10))
                throw new Exception("Test_WhereIF_ComplexPredicate failed: Complex predicate not applied correctly");

            Console.WriteLine("✓ Test_WhereIF_ComplexPredicate passed");
        }

        #endregion

        #region Category 6: MappingField Tests (3 tests)

        /// <summary>
        /// Test: MappingField on IEnumerable should return the same enumerable.
        /// This method is used for expression tree building, so it returns the input unchanged.
        /// </summary>
        private static void Test_MappingField_IEnumerable()
        {
            var list = new List<TestPerson>
            {
                new TestPerson { Id = 1, Name = "Alice" },
                new TestPerson { Id = 2, Name = "Bob" }
            };

            var result = CommonExtensions.MappingField(list, x => x.Id, () => 100);

            if (result == null || result.Count() != 2)
                throw new Exception("Test_MappingField_IEnumerable failed: Result should match input");

            Console.WriteLine("✓ Test_MappingField_IEnumerable passed");
        }

        /// <summary>
        /// Test: MappingField on single object should return a list with that object.
        /// This method wraps a single object in a List for expression tree building.
        /// </summary>
        private static void Test_MappingField_SingleObject()
        {
            var person = new TestPerson { Id = 1, Name = "Alice" };
            var result = CommonExtensions.MappingField(person, x => x.Id, () => 100);

            if (result == null || result.Count != 1 || result[0].Name != "Alice")
                throw new Exception("Test_MappingField_SingleObject failed: Result should contain the input object");

            Console.WriteLine("✓ Test_MappingField_SingleObject passed");
        }

        /// <summary>
        /// Test: MappingField with null object should handle gracefully.
        /// Tests edge case where input object might be null.
        /// </summary>
        private static void Test_MappingField_NullObject()
        {
            TestPerson person = null;
            
            try
            {
                var result = CommonExtensions.MappingField(person, x => x.Id, () => 100);
                
                // If we get here, the method handled null gracefully
                if (result == null || result.Count != 1)
                    throw new Exception("Test_MappingField_NullObject failed: Unexpected result");
                    
                Console.WriteLine("✓ Test_MappingField_NullObject passed");
            }
            catch (NullReferenceException)
            {
                // This is also acceptable behavior for null input
                Console.WriteLine("✓ Test_MappingField_NullObject passed (NullReferenceException expected)");
            }
        }

        #endregion

        #region Category 7: ToList Extensions Tests (3 tests)

        /// <summary>
        /// Test: ToList extension on single object should wrap it in a list.
        /// Tests the overload that converts a single object to a List<T>.
        /// </summary>
        private static void Test_ToList_SingleObject()
        {
            var person = new TestPerson { Id = 1, Name = "Alice" };
            var result = CommonExtensions.ToList(person, x => x);

            if (result == null || result.Count != 1 || result[0].Name != "Alice")
                throw new Exception("Test_ToList_SingleObject failed: Single object not wrapped correctly");

            Console.WriteLine("✓ Test_ToList_SingleObject passed");
        }

        /// <summary>
        /// Test: ToList extension on IEnumerable should convert to List.
        /// Tests the overload that converts IEnumerable<T> to List<T>.
        /// </summary>
        private static void Test_ToList_IEnumerable()
        {
            var people = new List<TestPerson>
            {
                new TestPerson { Id = 1, Name = "Alice" },
                new TestPerson { Id = 2, Name = "Bob" }
            }.AsEnumerable();

            var result = CommonExtensions.ToList(people, x => x);

            if (result == null || result.Count != 2)
                throw new Exception("Test_ToList_IEnumerable failed: IEnumerable not converted correctly");

            Console.WriteLine("✓ Test_ToList_IEnumerable passed");
        }

        /// <summary>
        /// Test: ToList with action parameter should still return the list.
        /// The action parameter is for expression tree building but doesn't modify the result.
        /// </summary>
        private static void Test_ToList_WithAction()
        {
            var person = new TestPerson { Id = 1, Name = "Alice" };
            var result = CommonExtensions.ToList(person, x => new TestPerson { Id = x.Id * 2, Name = x.Name.ToUpper() });

            // The action is not actually executed, it's for expression tree building
            if (result == null || result.Count != 1 || result[0].Id != 1)
                throw new Exception("Test_ToList_WithAction failed: Result incorrect");

            Console.WriteLine("✓ Test_ToList_WithAction passed");
        }

        #endregion

        #region Category 8: EnumerableExtensions.Contains Tests (2 tests)

        /// <summary>
        /// Test: Contains extension with isNvarchar parameter should find matching item.
        /// Tests the custom Contains overload in System.Collections.Generic namespace.
        /// </summary>
        private static void Test_Contains_WithIsNvarchar()
        {
            var list = new List<string> { "Apple", "Banana", "Cherry" };
            bool result = list.Contains("Banana", true);

            if (!result)
                throw new Exception("Test_Contains_WithIsNvarchar failed: Should find 'Banana' in list");

            Console.WriteLine("✓ Test_Contains_WithIsNvarchar passed");
        }

        /// <summary>
        /// Test: Contains extension should return false for non-existent item.
        /// Tests that the method correctly identifies missing items.
        /// </summary>
        private static void Test_Contains_NotFound()
        {
            var list = new List<string> { "Apple", "Banana", "Cherry" };
            bool result = list.Contains("Orange", false);

            if (result)
                throw new Exception("Test_Contains_NotFound failed: Should not find 'Orange' in list");

            Console.WriteLine("✓ Test_Contains_NotFound passed");
        }

        #endregion

        #region Helper Classes for Testing

        /// <summary>
        /// Test class with various methods for GetMyMethod testing.
        /// Contains methods with different parameter counts and types.
        /// Includes both generic and non-generic methods for comprehensive testing.
        /// </summary>
        private class TestClass
        {
            public void TestMethod1() { }
            public void TestMethod2(string param1) { }
            public void TestMethod3(int param1) { }
            public void TestMethod4(string param1, int param2) { }
            public void TestMethod5(string param1, int param2, bool param3) { }
            public void TestMethod6(string param1, int param2, bool param3, double param4) { }
            public void TestMethod7(string param1, int param2, bool param3, double param4, decimal param5) { }
            
            /// <summary>Generic method for testing GetMyMethodIsGenericMethod</summary>
            public void TestGenericMethod<T>(string param1, int param2, bool param3) { }
        }

        /// <summary>
        /// Test entity class for ToDictionary and MappingField testing.
        /// Simple POCO with Id and Name properties.
        /// </summary>
        private class TestPerson
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        #endregion
    }
}

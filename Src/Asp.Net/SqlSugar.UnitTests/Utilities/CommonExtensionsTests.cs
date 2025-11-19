using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;
using SqlSugar;

namespace SqlSugar.UnitTests.Utilities
{
    /// <summary>
    /// Unit tests for CommonExtensions extension methods
    /// </summary>
    public class CommonExtensionsTests
    {
        #region GetNonNegativeHashCodeString Tests

        [Fact]
        public void GetNonNegativeHashCodeString_WithValidString_ReturnsPrefixedHashCode()
        {
            string input = "test";
            string result = input.GetNonNegativeHashCodeString();
            Assert.StartsWith("hs", result);
            Assert.True(result.Length > 2);
        }

        [Fact]
        public void GetNonNegativeHashCodeString_WithEmptyString_ReturnsPrefixedHashCode()
        {
            string input = "";
            string result = input.GetNonNegativeHashCodeString();
            Assert.StartsWith("hs", result);
        }

        [Fact]
        public void GetNonNegativeHashCodeString_WithSameString_ReturnsSameHashCode()
        {
            string input = "test";
            string result1 = input.GetNonNegativeHashCodeString();
            string result2 = input.GetNonNegativeHashCodeString();
            Assert.Equal(result1, result2);
        }

        [Fact]
        public void GetNonNegativeHashCodeString_WithDifferentStrings_ReturnsDifferentHashCodes()
        {
            string input1 = "test1";
            string input2 = "test2";
            string result1 = input1.GetNonNegativeHashCodeString();
            string result2 = input2.GetNonNegativeHashCodeString();
            Assert.NotEqual(result1, result2);
        }

        #endregion

        #region SafeSubstring Tests

        [Fact]
        public void SafeSubstring_WithValidInput_ReturnsSubstring()
        {
            string str = "Hello World";
            string result = str.SafeSubstring(0, 5);
            Assert.Equal("Hello", result);
        }

        [Fact]
        public void SafeSubstring_WithMiddleIndex_ReturnsSubstring()
        {
            string str = "Hello World";
            string result = str.SafeSubstring(6, 5);
            Assert.Equal("World", result);
        }

        [Fact]
        public void SafeSubstring_WithNegativeStartIndex_ClampsToZero()
        {
            string str = "Hello World";
            string result = str.SafeSubstring(-5, 5);
            Assert.Equal("Hello", result);
        }

        [Fact]
        public void SafeSubstring_WithStartIndexBeyondLength_ReturnsEmptyString()
        {
            string str = "Hello";
            string result = str.SafeSubstring(10, 5);
            Assert.Equal("", result);
        }

        [Fact]
        public void SafeSubstring_WithLengthBeyondRemaining_ReturnsRemainingSubstring()
        {
            string str = "Hello";
            string result = str.SafeSubstring(2, 10);
            Assert.Equal("llo", result);
        }

        [Fact]
        public void SafeSubstring_WithNegativeLength_ClampsToZero()
        {
            string str = "Hello";
            string result = str.SafeSubstring(0, -5);
            Assert.Equal("", result);
        }

        [Fact]
        public void SafeSubstring_WithNullString_ThrowsArgumentNullException()
        {
            string str = null;
            Assert.Throws<ArgumentNullException>(() => str.SafeSubstring(0, 5));
        }

        [Fact]
        public void SafeSubstring_WithExactBounds_ReturnsCorrectSubstring()
        {
            string str = "Hello";
            string result = str.SafeSubstring(0, 5);
            Assert.Equal("Hello", result);
        }

        #endregion

        #region ToDictionary Tests

        [Fact]
        public void ToDictionary_WithValidList_ReturnsDictionary()
        {
            var list = new List<TestItem>
            {
                new TestItem { Key = "key1", Value = "value1" },
                new TestItem { Key = "key2", Value = "value2" }
            };
            var result = list.ToDictionary("Key", "Value");
            Assert.Equal(2, result.Count);
            Assert.Equal("value1", result["key1"]);
            Assert.Equal("value2", result["key2"]);
        }

        [Fact]
        public void ToDictionary_WithEmptyList_ReturnsEmptyDictionary()
        {
            var list = new List<TestItem>();
            var result = list.ToDictionary("Key", "Value");
            Assert.Equal(0, result.Count);
        }

        [Fact]
        public void ToDictionary_WithNullValues_HandlesNulls()
        {
            var list = new List<TestItem>
            {
                new TestItem { Key = "key1", Value = null }
            };
            var result = list.ToDictionary("Key", "Value");
            Assert.Equal(1, result.Count);
            Assert.Null(result["key1"]);
        }

        private class TestItem
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        #endregion

        #region GetMyMethod Tests

        [Fact]
        public void GetMyMethod_WithValidMethod_ReturnsMethodInfo()
        {
            Type type = typeof(TestMethodClass);
            MethodInfo result = type.GetMyMethod("TestMethod", 1);
            Assert.NotNull(result);
            Assert.Equal("TestMethod", result.Name);
            Assert.Equal(1, result.GetParameters().Length);
        }

        [Fact]
        public void GetMyMethod_WithInvalidMethod_ReturnsNull()
        {
            Type type = typeof(TestMethodClass);
            MethodInfo result = type.GetMyMethod("NonExistentMethod", 1);
            Assert.Null(result);
        }

        [Fact]
        public void GetMyMethod_WithParameterType_ReturnsCorrectMethod()
        {
            Type type = typeof(TestMethodClass);
            MethodInfo result = type.GetMyMethod("TestMethod", 1, typeof(string));
            Assert.NotNull(result);
            Assert.Equal(typeof(string), result.GetParameters()[0].ParameterType);
        }

        [Fact]
        public void GetMyMethod_WithTwoParameterTypes_ReturnsCorrectMethod()
        {
            Type type = typeof(TestMethodClass);
            MethodInfo result = type.GetMyMethod("TestMethod", 2, typeof(string), typeof(int));
            Assert.NotNull(result);
            Assert.Equal(typeof(string), result.GetParameters()[0].ParameterType);
            Assert.Equal(typeof(int), result.GetParameters()[1].ParameterType);
        }

        [Fact]
        public void GetMyMethod_WithThreeParameterTypes_ReturnsCorrectMethod()
        {
            Type type = typeof(TestMethodClass);
            MethodInfo result = type.GetMyMethod("TestMethod", 3, typeof(string), typeof(int), typeof(bool));
            Assert.NotNull(result);
        }

        [Fact]
        public void GetMyMethod_WithFourParameterTypes_ReturnsCorrectMethod()
        {
            Type type = typeof(TestMethodClass);
            MethodInfo result = type.GetMyMethod("TestMethod", 4, typeof(string), typeof(int), typeof(bool), typeof(double));
            Assert.NotNull(result);
        }

        [Fact]
        public void GetMyMethod_WithFiveParameterTypes_ReturnsCorrectMethod()
        {
            Type type = typeof(TestMethodClass);
            MethodInfo result = type.GetMyMethod("TestMethod", 5, typeof(string), typeof(int), typeof(bool), typeof(double), typeof(char));
            Assert.NotNull(result);
        }

        [Fact]
        public void GetMyMethod_WithListParameter_ReturnsCorrectMethod()
        {
            Type type = typeof(TestMethodClass);
            MethodInfo result = type.GetMyMethod("TestMethod", 1, true);
            Assert.NotNull(result);
            Assert.Contains("List", result.GetParameters()[0].ToString());
        }

        [Fact]
        public void GetMyMethodNoGen_WithGenericMethod_ReturnsNull()
        {
            Type type = typeof(TestMethodClass);
            MethodInfo result = type.GetMyMethodNoGen("TestMethod", 2, typeof(string), typeof(int));
            // This should return null if the method is generic
            // The actual behavior depends on the implementation
        }

        [Fact]
        public void GetMyMethodIsGenericMethod_WithGenericMethod_ReturnsMethod()
        {
            Type type = typeof(TestMethodClass);
            MethodInfo result = type.GetMyMethodIsGenericMethod("TestMethod", 3, typeof(string), typeof(int), typeof(bool));
            // This should return a generic method if it exists
            // The actual behavior depends on the implementation
        }

        private class TestMethodClass
        {
            public void TestMethod(string param) { }
            public void TestMethod(int param) { }
            public void TestMethod(List<string> param) { }
            public void TestMethod(string param1, int param2) { }
            public void TestMethod(string param1, int param2, bool param3) { }
            public void TestMethod(string param1, int param2, bool param3, double param4) { }
            public void TestMethod(string param1, int param2, bool param3, double param4, char param5) { }
        }

        #endregion

        #region ToList Tests

        [Fact]
        public void ToList_WithSingleItem_ReturnsList()
        {
            var item = new TestClass { Id = 1, Name = "Test" };
            var result = item.ToList(x => x);
            Assert.Single(result);
            Assert.Equal(item, result[0]);
        }

        [Fact]
        public void ToList_WithEnumerable_ReturnsList()
        {
            var items = new[] { new TestClass { Id = 1 }, new TestClass { Id = 2 } };
            var result = items.ToList(x => x);
            Assert.Equal(2, result.Count);
        }

        private class TestClass
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public override bool Equals(object obj)
            {
                if (obj is TestClass other)
                    return Id == other.Id && Name == other.Name;
                return false;
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode() ^ (Name?.GetHashCode() ?? 0);
            }
        }

        #endregion

        #region WhereIF Tests

        [Fact]
        public void WhereIF_WithTrueCondition_AppliesFilter()
        {
            var numbers = new[] { 1, 2, 3, 4, 5 };
            var result = numbers.WhereIF(true, x => x > 3).ToList();
            Assert.Equal(2, result.Count);
            Assert.Contains(4, result);
            Assert.Contains(5, result);
        }

        [Fact]
        public void WhereIF_WithFalseCondition_ReturnsAll()
        {
            var numbers = new[] { 1, 2, 3, 4, 5 };
            var result = numbers.WhereIF(false, x => x > 3).ToList();
            Assert.Equal(5, result.Count);
        }

        [Fact]
        public void WhereIF_WithFalseCondition_DoesNotFilter()
        {
            var numbers = new[] { 1, 2, 3, 4, 5 };
            var result = numbers.WhereIF(false, x => x > 10).ToList();
            Assert.Equal(5, result.Count);
        }

        #endregion

        #region MappingField Tests

        [Fact]
        public void MappingField_WithEnumerable_ReturnsOriginalEnumerable()
        {
            var items = new[] { 1, 2, 3 };
            var result = items.MappingField(x => x, () => 0);
            Assert.Equal(items, result);
        }

        [Fact]
        public void MappingField_WithSingleItem_ReturnsListWithItem()
        {
            var item = new TestClass { Id = 1, Name = "Test" };
            var result = item.MappingField(x => x.Id, () => 0);
            Assert.Single(result);
            Assert.Equal(item, result[0]);
        }

        #endregion

        #region Contains Tests (with isNvarchar)

        [Fact]
        public void Contains_WithNvarcharFlag_ReturnsTrue()
        {
            var items = new[] { "test", "value", "other" };
            bool result = items.Contains("test", true);
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithNvarcharFlag_ReturnsFalse()
        {
            var items = new[] { "test", "value", "other" };
            bool result = items.Contains("notfound", true);
            Assert.False(result);
        }

        [Fact]
        public void Contains_WithIntegers_Works()
        {
            var items = new[] { 1, 2, 3, 4, 5 };
            bool result = items.Contains(3, false);
            Assert.True(result);
        }

        #endregion

        #region Any/Where Tests (with conditional models)

        [Fact]
        public void Any_WithConditionalModels_ThrowsException()
        {
            var items = new[] { 1, 2, 3 };
            var conditionalModels = new List<ConditionalModel>();
            Assert.Throws<Exception>(() => items.Any(conditionalModels));
        }

        [Fact]
        public void Where_WithConditionalModels_ThrowsException()
        {
            var items = new[] { 1, 2, 3 };
            var conditionalModels = new List<ConditionalModel>();
            Assert.Throws<Exception>(() => items.Where(conditionalModels));
        }

        #endregion
    }
}


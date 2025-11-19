using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xunit;
using SqlSugar;

namespace SqlSugar.UnitTests.Utilities
{
    /// <summary>
    /// Unit tests for ValidateExtensions extension methods
    /// </summary>
    public class ValidateExtensionsTests
    {
        #region IsInRange Tests (int)

        [Fact]
        public void IsInRange_Int_WithValueInRange_ReturnsTrue()
        {
            int value = 5;
            bool result = value.IsInRange(1, 10);
            Assert.True(result);
        }

        [Fact]
        public void IsInRange_Int_WithValueAtStart_ReturnsTrue()
        {
            int value = 1;
            bool result = value.IsInRange(1, 10);
            Assert.True(result);
        }

        [Fact]
        public void IsInRange_Int_WithValueAtEnd_ReturnsTrue()
        {
            int value = 10;
            bool result = value.IsInRange(1, 10);
            Assert.True(result);
        }

        [Fact]
        public void IsInRange_Int_WithValueBelowRange_ReturnsFalse()
        {
            int value = 0;
            bool result = value.IsInRange(1, 10);
            Assert.False(result);
        }

        [Fact]
        public void IsInRange_Int_WithValueAboveRange_ReturnsFalse()
        {
            int value = 11;
            bool result = value.IsInRange(1, 10);
            Assert.False(result);
        }

        #endregion

        #region IsInRange Tests (DateTime)

        [Fact]
        public void IsInRange_DateTime_WithValueInRange_ReturnsTrue()
        {
            DateTime value = new DateTime(2024, 6, 15);
            DateTime begin = new DateTime(2024, 1, 1);
            DateTime end = new DateTime(2024, 12, 31);
            bool result = value.IsInRange(begin, end);
            Assert.True(result);
        }

        [Fact]
        public void IsInRange_DateTime_WithValueAtStart_ReturnsTrue()
        {
            DateTime value = new DateTime(2024, 1, 1);
            DateTime begin = new DateTime(2024, 1, 1);
            DateTime end = new DateTime(2024, 12, 31);
            bool result = value.IsInRange(begin, end);
            Assert.True(result);
        }

        [Fact]
        public void IsInRange_DateTime_WithValueAtEnd_ReturnsTrue()
        {
            DateTime value = new DateTime(2024, 12, 31);
            DateTime begin = new DateTime(2024, 1, 1);
            DateTime end = new DateTime(2024, 12, 31);
            bool result = value.IsInRange(begin, end);
            Assert.True(result);
        }

        [Fact]
        public void IsInRange_DateTime_WithValueBeforeRange_ReturnsFalse()
        {
            DateTime value = new DateTime(2023, 12, 31);
            DateTime begin = new DateTime(2024, 1, 1);
            DateTime end = new DateTime(2024, 12, 31);
            bool result = value.IsInRange(begin, end);
            Assert.False(result);
        }

        [Fact]
        public void IsInRange_DateTime_WithValueAfterRange_ReturnsFalse()
        {
            DateTime value = new DateTime(2025, 1, 1);
            DateTime begin = new DateTime(2024, 1, 1);
            DateTime end = new DateTime(2024, 12, 31);
            bool result = value.IsInRange(begin, end);
            Assert.False(result);
        }

        #endregion

        #region IsIn Tests

        [Fact]
        public void IsIn_WithValueInParams_ReturnsTrue()
        {
            int value = 5;
            bool result = value.IsIn(1, 2, 3, 4, 5, 6);
            Assert.True(result);
        }

        [Fact]
        public void IsIn_WithValueNotInParams_ReturnsFalse()
        {
            int value = 10;
            bool result = value.IsIn(1, 2, 3, 4, 5, 6);
            Assert.False(result);
        }

        [Fact]
        public void IsIn_WithStringValue_ReturnsTrue()
        {
            string value = "test";
            bool result = value.IsIn("test", "value", "other");
            Assert.True(result);
        }

        [Fact]
        public void IsIn_WithEmptyParams_ReturnsFalse()
        {
            int value = 5;
            bool result = value.IsIn();
            Assert.False(result);
        }

        #endregion

        #region IsContainsIn Tests

        [Fact]
        public void IsContainsIn_WithMatchingString_ReturnsTrue()
        {
            string value = "Hello World";
            bool result = value.IsContainsIn("World", "Test", "Other");
            Assert.True(result);
        }

        [Fact]
        public void IsContainsIn_WithNoMatch_ReturnsFalse()
        {
            string value = "Hello World";
            bool result = value.IsContainsIn("Test", "Other", "Value");
            Assert.False(result);
        }

        [Fact]
        public void IsContainsIn_WithMultipleMatches_ReturnsTrue()
        {
            string value = "Hello World Test";
            bool result = value.IsContainsIn("World", "Test", "Other");
            Assert.True(result);
        }

        [Fact]
        public void IsContainsIn_WithEmptyParams_ReturnsFalse()
        {
            string value = "Hello World";
            bool result = value.IsContainsIn();
            Assert.False(result);
        }

        #endregion

        #region IsContainsStartWithIn Tests

        [Fact]
        public void IsContainsStartWithIn_WithMatchingPrefix_ReturnsTrue()
        {
            string value = "Hello World";
            bool result = value.IsContainsStartWithIn("Hello", "Test", "Other");
            Assert.True(result);
        }

        [Fact]
        public void IsContainsStartWithIn_WithNoMatch_ReturnsFalse()
        {
            string value = "Hello World";
            bool result = value.IsContainsStartWithIn("World", "Test", "Other");
            Assert.False(result);
        }

        [Fact]
        public void IsContainsStartWithIn_WithMultipleMatches_ReturnsTrue()
        {
            string value = "Hello World";
            bool result = value.IsContainsStartWithIn("Hello", "Hell", "H");
            Assert.True(result);
        }

        #endregion

        #region IsNullOrEmpty Tests (object)

        [Fact]
        public void IsNullOrEmpty_WithNull_ReturnsTrue()
        {
            object value = null;
            bool result = value.IsNullOrEmpty();
            Assert.True(result);
        }

        [Fact]
        public void IsNullOrEmpty_WithDBNull_ReturnsTrue()
        {
            object value = DBNull.Value;
            bool result = value.IsNullOrEmpty();
            Assert.True(result);
        }

        [Fact]
        public void IsNullOrEmpty_WithEmptyString_ReturnsTrue()
        {
            object value = "";
            bool result = value.IsNullOrEmpty();
            Assert.True(result);
        }

        [Fact]
        public void IsNullOrEmpty_WithNonEmptyString_ReturnsFalse()
        {
            object value = "test";
            bool result = value.IsNullOrEmpty();
            Assert.False(result);
        }

        [Fact]
        public void IsNullOrEmpty_WithInteger_ReturnsFalse()
        {
            object value = 42;
            bool result = value.IsNullOrEmpty();
            Assert.False(result);
        }

        #endregion

        #region IsNullOrEmpty Tests (Guid?)

        [Fact]
        public void IsNullOrEmpty_GuidNullable_WithNull_ReturnsTrue()
        {
            Guid? value = null;
            bool result = value.IsNullOrEmpty();
            Assert.True(result);
        }

        [Fact]
        public void IsNullOrEmpty_GuidNullable_WithEmptyGuid_ReturnsTrue()
        {
            Guid? value = Guid.Empty;
            bool result = value.IsNullOrEmpty();
            Assert.True(result);
        }

        [Fact]
        public void IsNullOrEmpty_GuidNullable_WithValidGuid_ReturnsFalse()
        {
            Guid? value = Guid.NewGuid();
            bool result = value.IsNullOrEmpty();
            Assert.False(result);
        }

        #endregion

        #region IsNullOrEmpty Tests (Guid)

        [Fact]
        public void IsNullOrEmpty_Guid_WithEmptyGuid_ReturnsTrue()
        {
            Guid value = Guid.Empty;
            bool result = value.IsNullOrEmpty();
            Assert.True(result);
        }

        [Fact]
        public void IsNullOrEmpty_Guid_WithValidGuid_ReturnsFalse()
        {
            Guid value = Guid.NewGuid();
            bool result = value.IsNullOrEmpty();
            Assert.False(result);
        }

        #endregion

        #region IsNullOrEmpty Tests (IEnumerable)

        [Fact]
        public void IsNullOrEmpty_IEnumerable_WithNull_ReturnsTrue()
        {
            IEnumerable<object> value = null;
            bool result = value.IsNullOrEmpty();
            Assert.True(result);
        }

        [Fact]
        public void IsNullOrEmpty_IEnumerable_WithEmptyList_ReturnsTrue()
        {
            IEnumerable<object> value = new List<object>();
            bool result = value.IsNullOrEmpty();
            Assert.True(result);
        }

        [Fact]
        public void IsNullOrEmpty_IEnumerable_WithItems_ReturnsFalse()
        {
            IEnumerable<object> value = new List<object> { 1, 2, 3 };
            bool result = value.IsNullOrEmpty();
            Assert.False(result);
        }

        #endregion

        #region HasValue Tests (object)

        [Fact]
        public void HasValue_WithNull_ReturnsFalse()
        {
            object value = null;
            bool result = value.HasValue();
            Assert.False(result);
        }

        [Fact]
        public void HasValue_WithDBNull_ReturnsFalse()
        {
            object value = DBNull.Value;
            bool result = value.HasValue();
            Assert.False(result);
        }

        [Fact]
        public void HasValue_WithEmptyString_ReturnsFalse()
        {
            object value = "";
            bool result = value.HasValue();
            Assert.False(result);
        }

        [Fact]
        public void HasValue_WithNonEmptyString_ReturnsTrue()
        {
            object value = "test";
            bool result = value.HasValue();
            Assert.True(result);
        }

        [Fact]
        public void HasValue_WithInteger_ReturnsTrue()
        {
            object value = 42;
            bool result = value.HasValue();
            Assert.True(result);
        }

        #endregion

        #region HasValue Tests (IEnumerable)

        [Fact]
        public void HasValue_IEnumerable_WithNull_ReturnsFalse()
        {
            IEnumerable<object> value = null;
            bool result = value.HasValue();
            Assert.False(result);
        }

        [Fact]
        public void HasValue_IEnumerable_WithEmptyList_ReturnsFalse()
        {
            IEnumerable<object> value = new List<object>();
            bool result = value.HasValue();
            Assert.False(result);
        }

        [Fact]
        public void HasValue_IEnumerable_WithItems_ReturnsTrue()
        {
            IEnumerable<object> value = new List<object> { 1, 2, 3 };
            bool result = value.HasValue();
            Assert.True(result);
        }

        #endregion

        #region IsValuable Tests

        [Fact]
        public void IsValuable_WithNull_ReturnsFalse()
        {
            IEnumerable<KeyValuePair<string, string>> value = null;
            bool result = value.IsValuable();
            Assert.False(result);
        }

        [Fact]
        public void IsValuable_WithEmptyDictionary_ReturnsFalse()
        {
            IEnumerable<KeyValuePair<string, string>> value = new Dictionary<string, string>();
            bool result = value.IsValuable();
            Assert.False(result);
        }

        [Fact]
        public void IsValuable_WithItems_ReturnsTrue()
        {
            IEnumerable<KeyValuePair<string, string>> value = new Dictionary<string, string>
            {
                { "key1", "value1" },
                { "key2", "value2" }
            };
            bool result = value.IsValuable();
            Assert.True(result);
        }

        #endregion

        #region IsZero Tests

        [Fact]
        public void IsZero_WithNull_ReturnsTrue()
        {
            object value = null;
            bool result = value.IsZero();
            Assert.True(result);
        }

        [Fact]
        public void IsZero_WithZeroString_ReturnsTrue()
        {
            object value = "0";
            bool result = value.IsZero();
            Assert.True(result);
        }

        [Fact]
        public void IsZero_WithNonZeroString_ReturnsFalse()
        {
            object value = "42";
            bool result = value.IsZero();
            Assert.False(result);
        }

        [Fact]
        public void IsZero_WithZeroInteger_ReturnsTrue()
        {
            object value = 0;
            bool result = value.IsZero();
            Assert.True(result);
        }

        #endregion

        #region IsInt Tests

        [Fact]
        public void IsInt_WithNull_ReturnsFalse()
        {
            object value = null;
            bool result = value.IsInt();
            Assert.False(result);
        }

        [Fact]
        public void IsInt_WithValidIntegerString_ReturnsTrue()
        {
            object value = "123";
            bool result = value.IsInt();
            Assert.True(result);
        }

        [Fact]
        public void IsInt_WithNegativeIntegerString_ReturnsTrue()
        {
            object value = "-123";
            bool result = value.IsInt();
            Assert.False(result); // Regex @"^\d+$" only matches positive integers
        }

        [Fact]
        public void IsInt_WithInvalidString_ReturnsFalse()
        {
            object value = "123abc";
            bool result = value.IsInt();
            Assert.False(result);
        }

        [Fact]
        public void IsInt_WithDecimalString_ReturnsFalse()
        {
            object value = "123.45";
            bool result = value.IsInt();
            Assert.False(result);
        }

        #endregion

        #region IsNoInt Tests

        [Fact]
        public void IsNoInt_WithNull_ReturnsTrue()
        {
            object value = null;
            bool result = value.IsNoInt();
            Assert.True(result);
        }

        [Fact]
        public void IsNoInt_WithValidIntegerString_ReturnsFalse()
        {
            object value = "123";
            bool result = value.IsNoInt();
            Assert.False(result);
        }

        [Fact]
        public void IsNoInt_WithInvalidString_ReturnsTrue()
        {
            object value = "abc";
            bool result = value.IsNoInt();
            Assert.True(result);
        }

        #endregion

        #region IsMoney Tests

        [Fact]
        public void IsMoney_WithNull_ReturnsFalse()
        {
            object value = null;
            bool result = value.IsMoney();
            Assert.False(result);
        }

        [Fact]
        public void IsMoney_WithValidDoubleString_ReturnsTrue()
        {
            object value = "123.45";
            bool result = value.IsMoney();
            Assert.True(result);
        }

        [Fact]
        public void IsMoney_WithIntegerString_ReturnsTrue()
        {
            object value = "123";
            bool result = value.IsMoney();
            Assert.True(result);
        }

        [Fact]
        public void IsMoney_WithInvalidString_ReturnsFalse()
        {
            object value = "abc";
            bool result = value.IsMoney();
            Assert.False(result);
        }

        [Fact]
        public void IsMoney_WithNegativeDoubleString_ReturnsTrue()
        {
            object value = "-123.45";
            bool result = value.IsMoney();
            Assert.True(result);
        }

        #endregion

        #region IsGuid Tests

        [Fact]
        public void IsGuid_WithNull_ReturnsFalse()
        {
            object value = null;
            bool result = value.IsGuid();
            Assert.False(result);
        }

        [Fact]
        public void IsGuid_WithValidGuidString_ReturnsTrue()
        {
            Guid guid = Guid.NewGuid();
            object value = guid.ToString();
            bool result = value.IsGuid();
            Assert.True(result);
        }

        [Fact]
        public void IsGuid_WithInvalidString_ReturnsFalse()
        {
            object value = "not-a-guid";
            bool result = value.IsGuid();
            Assert.False(result);
        }

        [Fact]
        public void IsGuid_WithEmptyGuidString_ReturnsTrue()
        {
            object value = Guid.Empty.ToString();
            bool result = value.IsGuid();
            Assert.True(result);
        }

        #endregion

        #region IsDate Tests

        [Fact]
        public void IsDate_WithNull_ReturnsFalse()
        {
            object value = null;
            bool result = value.IsDate();
            Assert.False(result);
        }

        [Fact]
        public void IsDate_WithValidDateString_ReturnsTrue()
        {
            object value = "2024-01-15";
            bool result = value.IsDate();
            Assert.True(result);
        }

        [Fact]
        public void IsDate_WithInvalidString_ReturnsFalse()
        {
            object value = "not-a-date";
            bool result = value.IsDate();
            Assert.False(result);
        }

        [Fact]
        public void IsDate_WithDateTimeString_ReturnsTrue()
        {
            object value = "2024-01-15 10:30:00";
            bool result = value.IsDate();
            Assert.True(result);
        }

        #endregion

        #region IsEamil Tests

        [Fact]
        public void IsEamil_WithNull_ReturnsFalse()
        {
            object value = null;
            bool result = value.IsEamil();
            Assert.False(result);
        }

        [Fact]
        public void IsEamil_WithValidEmail_ReturnsTrue()
        {
            object value = "test@example.com";
            bool result = value.IsEamil();
            Assert.True(result);
        }

        [Fact]
        public void IsEamil_WithInvalidEmail_ReturnsFalse()
        {
            object value = "not-an-email";
            bool result = value.IsEamil();
            Assert.False(result);
        }

        [Fact]
        public void IsEamil_WithComplexEmail_ReturnsTrue()
        {
            object value = "test.user+tag@example.co.uk";
            bool result = value.IsEamil();
            Assert.True(result);
        }

        #endregion

        #region IsMobile Tests

        [Fact]
        public void IsMobile_WithNull_ReturnsFalse()
        {
            object value = null;
            bool result = value.IsMobile();
            Assert.False(result);
        }

        [Fact]
        public void IsMobile_WithValidMobile_ReturnsTrue()
        {
            object value = "13800138000";
            bool result = value.IsMobile();
            Assert.True(result);
        }

        [Fact]
        public void IsMobile_WithInvalidLength_ReturnsFalse()
        {
            object value = "1234567890";
            bool result = value.IsMobile();
            Assert.False(result);
        }

        [Fact]
        public void IsMobile_WithNonNumeric_ReturnsFalse()
        {
            object value = "1380013800a";
            bool result = value.IsMobile();
            Assert.False(result);
        }

        #endregion

        #region IsTelephone Tests

        [Fact]
        public void IsTelephone_WithNull_ReturnsFalse()
        {
            object value = null;
            bool result = value.IsTelephone();
            Assert.False(result);
        }

        [Fact]
        public void IsTelephone_WithValidTelephone_ReturnsTrue()
        {
            object value = "01012345678";
            bool result = value.IsTelephone();
            Assert.True(result);
        }

        [Fact]
        public void IsTelephone_WithFormattedTelephone_ReturnsTrue()
        {
            object value = "(010)12345678";
            bool result = value.IsTelephone();
            Assert.True(result);
        }

        [Fact]
        public void IsTelephone_WithInvalidFormat_ReturnsFalse()
        {
            object value = "123";
            bool result = value.IsTelephone();
            Assert.False(result);
        }

        #endregion

        #region IsIDcard Tests

        [Fact]
        public void IsIDcard_WithNull_ReturnsFalse()
        {
            object value = null;
            bool result = value.IsIDcard();
            Assert.False(result);
        }

        [Fact]
        public void IsIDcard_WithValid15DigitID_ReturnsTrue()
        {
            object value = "123456789012345";
            bool result = value.IsIDcard();
            Assert.True(result);
        }

        [Fact]
        public void IsIDcard_WithValid18DigitID_ReturnsTrue()
        {
            object value = "123456789012345678";
            bool result = value.IsIDcard();
            Assert.True(result);
        }

        [Fact]
        public void IsIDcard_WithInvalidID_ReturnsFalse()
        {
            object value = "1234567890";
            bool result = value.IsIDcard();
            Assert.False(result);
        }

        #endregion

        #region IsFax Tests

        [Fact]
        public void IsFax_WithNull_ReturnsFalse()
        {
            object value = null;
            bool result = value.IsFax();
            Assert.False(result);
        }

        [Fact]
        public void IsFax_WithValidFax_ReturnsTrue()
        {
            object value = "+86 10 12345678";
            bool result = value.IsFax();
            Assert.True(result);
        }

        [Fact]
        public void IsFax_WithSimpleFax_ReturnsTrue()
        {
            object value = "12345678";
            bool result = value.IsFax();
            Assert.True(result);
        }

        #endregion

        #region IsMatch Tests

        [Fact]
        public void IsMatch_WithNull_ReturnsFalse()
        {
            object value = null;
            bool result = value.IsMatch(@"^\d+$");
            Assert.False(result);
        }

        [Fact]
        public void IsMatch_WithMatchingPattern_ReturnsTrue()
        {
            object value = "123";
            bool result = value.IsMatch(@"^\d+$");
            Assert.True(result);
        }

        [Fact]
        public void IsMatch_WithNonMatchingPattern_ReturnsFalse()
        {
            object value = "abc";
            bool result = value.IsMatch(@"^\d+$");
            Assert.False(result);
        }

        #endregion

        #region IsAnonymousType Tests

        [Fact]
        public void IsAnonymousType_WithAnonymousType_ReturnsTrue()
        {
            var anonymous = new { Name = "Test", Value = 42 };
            Type type = anonymous.GetType();
            bool result = type.IsAnonymousType();
            Assert.True(result);
        }

        [Fact]
        public void IsAnonymousType_WithRegularClass_ReturnsFalse()
        {
            Type type = typeof(string);
            bool result = type.IsAnonymousType();
            Assert.False(result);
        }

        #endregion

        #region IsCollectionsList Tests

        [Fact]
        public void IsCollectionsList_WithListFullName_ReturnsTrue()
        {
            string fullName = "System.Collections.Generic.List`1";
            bool result = fullName.IsCollectionsList();
            Assert.True(result);
        }

        [Fact]
        public void IsCollectionsList_WithIEnumerableFullName_ReturnsTrue()
        {
            string fullName = "System.Collections.Generic.IEnumerable`1";
            bool result = fullName.IsCollectionsList();
            Assert.True(result);
        }

        [Fact]
        public void IsCollectionsList_WithInvalidString_ReturnsFalse()
        {
            string fullName = "System.String";
            bool result = fullName.IsCollectionsList();
            Assert.False(result);
        }

        #endregion

        #region IsIterator Tests

        [Fact]
        public void IsIterator_WithIteratorType_ReturnsTrue()
        {
            var list = Enumerable.Range(1, 10).Where(x => x > 5);
            Type type = list.GetType();
            bool result = type.IsIterator();
            Assert.True(result);
        }

        [Fact]
        public void IsIterator_WithListType_ReturnsFalse()
        {
            Type type = typeof(List<int>);
            bool result = type.IsIterator();
            Assert.False(result);
        }

        #endregion

        #region IsStringArray Tests

        [Fact]
        public void IsStringArray_WithStringArrayFullName_ReturnsTrue()
        {
            string fullName = "System.String[]";
            bool result = fullName.IsStringArray();
            Assert.True(result);
        }

        [Fact]
        public void IsStringArray_WithIntArrayFullName_ReturnsTrue()
        {
            string fullName = "System.Int32[]";
            bool result = fullName.IsStringArray();
            Assert.True(result);
        }

        [Fact]
        public void IsStringArray_WithInvalidString_ReturnsFalse()
        {
            string fullName = "System.String";
            bool result = fullName.IsStringArray();
            Assert.False(result);
        }

        #endregion

        #region IsEnumerable Tests

        [Fact]
        public void IsEnumerable_WithEnumerableFullName_ReturnsTrue()
        {
            string fullName = "System.Linq.Enumerable";
            bool result = fullName.IsEnumerable();
            Assert.True(result);
        }

        [Fact]
        public void IsEnumerable_WithInvalidString_ReturnsFalse()
        {
            string fullName = "System.Collections.Generic.List";
            bool result = fullName.IsEnumerable();
            Assert.False(result);
        }

        #endregion

        #region IsClass Tests

        [Fact]
        public void IsClass_WithStringType_ReturnsFalse()
        {
            Type type = typeof(string);
            bool result = type.IsClass();
            Assert.False(result);
        }

        [Fact]
        public void IsClass_WithByteArrayType_ReturnsFalse()
        {
            Type type = typeof(byte[]);
            bool result = type.IsClass();
            Assert.False(result);
        }

        [Fact]
        public void IsClass_WithRegularClass_ReturnsTrue()
        {
            Type type = typeof(List<int>);
            bool result = type.IsClass();
            Assert.True(result);
        }

        #endregion
    }
}


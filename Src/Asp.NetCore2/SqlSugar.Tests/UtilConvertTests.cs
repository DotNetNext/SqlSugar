using System;
using Xunit;

namespace SqlSugar.Tests
{
    /// <summary>
    /// Unit tests for UtilConvert utility class
    /// UtilConvert 实用工具类的单元测试
    /// </summary>
    public class UtilConvertTests
    {
        #region ObjToInt Tests / ObjToInt 测试

        [Fact]
        public void ObjToInt_ValidInteger_ReturnsCorrectValue()
        {
            // Arrange
            object value = 123;

            // Act
            var result = value.ObjToInt();

            // Assert
            Assert.Equal(123, result);
        }

        [Fact]
        public void ObjToInt_ValidString_ReturnsCorrectValue()
        {
            // Arrange
            object value = "456";

            // Act
            var result = value.ObjToInt();

            // Assert
            Assert.Equal(456, result);
        }

        [Fact]
        public void ObjToInt_NullValue_ReturnsZero()
        {
            // Arrange
            object value = null;

            // Act
            var result = value.ObjToInt();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void ObjToInt_DBNullValue_ReturnsZero()
        {
            // Arrange
            object value = DBNull.Value;

            // Act
            var result = value.ObjToInt();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void ObjToInt_InvalidString_ReturnsZero()
        {
            // Arrange
            object value = "invalid";

            // Act
            var result = value.ObjToInt();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void ObjToInt_EnumValue_ReturnsIntegerValue()
        {
            // Arrange
            object value = DayOfWeek.Monday;

            // Act
            var result = value.ObjToInt();

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public void ObjToInt_WithErrorValue_ValidInteger_ReturnsCorrectValue()
        {
            // Arrange
            object value = 789;

            // Act
            var result = value.ObjToInt(-1);

            // Assert
            Assert.Equal(789, result);
        }

        [Fact]
        public void ObjToInt_WithErrorValue_InvalidValue_ReturnsErrorValue()
        {
            // Arrange
            object value = "invalid";

            // Act
            var result = value.ObjToInt(-1);

            // Assert
            Assert.Equal(-1, result);
        }

        #endregion

        #region ObjToLong Tests / ObjToLong 测试

        [Fact]
        public void ObjToLong_ValidLong_ReturnsCorrectValue()
        {
            // Arrange
            object value = 9876543210L;

            // Act
            var result = value.ObjToLong();

            // Assert
            Assert.Equal(9876543210L, result);
        }

        [Fact]
        public void ObjToLong_ValidString_ReturnsCorrectValue()
        {
            // Arrange
            object value = "1234567890";

            // Act
            var result = value.ObjToLong();

            // Assert
            Assert.Equal(1234567890L, result);
        }

        [Fact]
        public void ObjToLong_NullValue_ReturnsZero()
        {
            // Arrange
            object value = null;

            // Act
            var result = value.ObjToLong();

            // Assert
            Assert.Equal(0L, result);
        }

        [Fact]
        public void ObjToLong_EnumValue_ReturnsLongValue()
        {
            // Arrange
            object value = DayOfWeek.Friday;

            // Act
            var result = value.ObjToLong();

            // Assert
            Assert.Equal(5L, result);
        }

        #endregion

        #region ObjToString Tests / ObjToString 测试

        [Fact]
        public void ObjToString_ValidString_ReturnsString()
        {
            // Arrange
            object value = "  Hello World  ";

            // Act
            var result = value.ObjToString();

            // Assert
            Assert.Equal("Hello World", result);
        }

        [Fact]
        public void ObjToString_NullValue_ReturnsEmptyString()
        {
            // Arrange
            object value = null;

            // Act
            var result = value.ObjToString();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ObjToString_Integer_ReturnsStringRepresentation()
        {
            // Arrange
            object value = 12345;

            // Act
            var result = value.ObjToString();

            // Assert
            Assert.Equal("12345", result);
        }

        [Fact]
        public void ObjToString_WithErrorValue_ValidValue_ReturnsString()
        {
            // Arrange
            object value = "test";

            // Act
            var result = value.ObjToString("default");

            // Assert
            Assert.Equal("test", result);
        }

        [Fact]
        public void ObjToString_WithErrorValue_NullValue_ReturnsErrorValue()
        {
            // Arrange
            object value = null;

            // Act
            var result = value.ObjToString("default");

            // Assert
            Assert.Equal("default", result);
        }

        [Fact]
        public void ObjToStringNoTrim_WithWhitespace_PreservesWhitespace()
        {
            // Arrange
            object value = "  Hello  ";

            // Act
            var result = value.ObjToStringNoTrim();

            // Assert
            Assert.Equal("  Hello  ", result);
        }

        #endregion

        #region ObjToDecimal Tests / ObjToDecimal 测试

        [Fact]
        public void ObjToDecimal_ValidDecimal_ReturnsCorrectValue()
        {
            // Arrange
            object value = 123.45m;

            // Act
            var result = value.ObjToDecimal();

            // Assert
            Assert.Equal(123.45m, result);
        }

        [Fact]
        public void ObjToDecimal_ValidString_ReturnsCorrectValue()
        {
            // Arrange
            object value = "678.90";

            // Act
            var result = value.ObjToDecimal();

            // Assert
            Assert.Equal(678.90m, result);
        }

        [Fact]
        public void ObjToDecimal_NullValue_ReturnsZero()
        {
            // Arrange
            object value = null;

            // Act
            var result = value.ObjToDecimal();

            // Assert
            Assert.Equal(0m, result);
        }

        [Fact]
        public void ObjToDecimal_DBNullValue_ReturnsZero()
        {
            // Arrange
            object value = DBNull.Value;

            // Act
            var result = value.ObjToDecimal();

            // Assert
            Assert.Equal(0m, result);
        }

        [Fact]
        public void ObjToDecimal_WithErrorValue_InvalidValue_ReturnsErrorValue()
        {
            // Arrange
            object value = "invalid";

            // Act
            var result = value.ObjToDecimal(-1.5m);

            // Assert
            Assert.Equal(-1.5m, result);
        }

        #endregion

        #region ObjToMoney Tests / ObjToMoney 测试

        [Fact]
        public void ObjToMoney_ValidDouble_ReturnsCorrectValue()
        {
            // Arrange
            object value = 999.99;

            // Act
            var result = value.ObjToMoney();

            // Assert
            Assert.Equal(999.99, result);
        }

        [Fact]
        public void ObjToMoney_ValidString_ReturnsCorrectValue()
        {
            // Arrange
            object value = "1234.56";

            // Act
            var result = value.ObjToMoney();

            // Assert
            Assert.Equal(1234.56, result);
        }

        [Fact]
        public void ObjToMoney_NullValue_ReturnsZero()
        {
            // Arrange
            object value = null;

            // Act
            var result = value.ObjToMoney();

            // Assert
            Assert.Equal(0.0, result);
        }

        [Fact]
        public void ObjToMoney_WithErrorValue_InvalidValue_ReturnsErrorValue()
        {
            // Arrange
            object value = "invalid";

            // Act
            var result = value.ObjToMoney(-100.0);

            // Assert
            Assert.Equal(-100.0, result);
        }

        #endregion

        #region ObjToDate Tests / ObjToDate 测试

        [Fact]
        public void ObjToDate_ValidDateTime_ReturnsCorrectValue()
        {
            // Arrange
            var expectedDate = new DateTime(2024, 1, 15);
            object value = expectedDate;

            // Act
            var result = value.ObjToDate();

            // Assert
            Assert.Equal(expectedDate, result);
        }

        [Fact]
        public void ObjToDate_ValidString_ReturnsCorrectValue()
        {
            // Arrange
            object value = "2024-06-30";

            // Act
            var result = value.ObjToDate();

            // Assert
            Assert.Equal(new DateTime(2024, 6, 30), result);
        }

        [Fact]
        public void ObjToDate_NullValue_ReturnsMinValue()
        {
            // Arrange
            object value = null;

            // Act
            var result = value.ObjToDate();

            // Assert
            Assert.Equal(DateTime.MinValue, result);
        }

        [Fact]
        public void ObjToDate_DBNullValue_ReturnsMinValue()
        {
            // Arrange
            object value = DBNull.Value;

            // Act
            var result = value.ObjToDate();

            // Assert
            Assert.Equal(DateTime.MinValue, result);
        }

        [Fact]
        public void ObjToDate_WithErrorValue_InvalidValue_ReturnsErrorValue()
        {
            // Arrange
            object value = "invalid";
            var errorDate = new DateTime(2000, 1, 1);

            // Act
            var result = value.ObjToDate(errorDate);

            // Assert
            Assert.Equal(errorDate, result);
        }

        #endregion

        #region ObjToBool Tests / ObjToBool 测试

        [Fact]
        public void ObjToBool_TrueString_ReturnsTrue()
        {
            // Arrange
            object value = "true";

            // Act
            var result = value.ObjToBool();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ObjToBool_FalseString_ReturnsFalse()
        {
            // Arrange
            object value = "false";

            // Act
            var result = value.ObjToBool();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ObjToBool_TrueBoolean_ReturnsTrue()
        {
            // Arrange
            object value = true;

            // Act
            var result = value.ObjToBool();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ObjToBool_FalseBoolean_ReturnsFalse()
        {
            // Arrange
            object value = false;

            // Act
            var result = value.ObjToBool();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ObjToBool_NullValue_ReturnsFalse()
        {
            // Arrange
            object value = null;

            // Act
            var result = value.ObjToBool();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ObjToBool_InvalidString_ReturnsFalse()
        {
            // Arrange
            object value = "invalid";

            // Act
            var result = value.ObjToBool();

            // Assert
            Assert.False(result);
        }

        #endregion

        #region EqualCase Tests / EqualCase 测试

        [Fact]
        public void EqualCase_SameCase_ReturnsTrue()
        {
            // Arrange
            string value1 = "Hello";
            string value2 = "Hello";

            // Act
            var result = value1.EqualCase(value2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EqualCase_DifferentCase_ReturnsTrue()
        {
            // Arrange
            string value1 = "Hello";
            string value2 = "hello";

            // Act
            var result = value1.EqualCase(value2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EqualCase_MixedCase_ReturnsTrue()
        {
            // Arrange
            string value1 = "HeLLo WoRLd";
            string value2 = "hello world";

            // Act
            var result = value1.EqualCase(value2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EqualCase_DifferentStrings_ReturnsFalse()
        {
            // Arrange
            string value1 = "Hello";
            string value2 = "World";

            // Act
            var result = value1.EqualCase(value2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void EqualCase_BothNull_ReturnsTrue()
        {
            // Arrange
            string value1 = null;
            string value2 = null;

            // Act
            var result = value1.EqualCase(value2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EqualCase_OneNull_ReturnsFalse()
        {
            // Arrange
            string value1 = "Hello";
            string value2 = null;

            // Act
            var result = value1.EqualCase(value2);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Edge Cases / 边界情况测试

        [Fact]
        public void ObjToInt_MaxValue_ReturnsCorrectValue()
        {
            // Arrange
            object value = int.MaxValue.ToString();

            // Act
            var result = value.ObjToInt();

            // Assert
            Assert.Equal(int.MaxValue, result);
        }

        [Fact]
        public void ObjToInt_MinValue_ReturnsCorrectValue()
        {
            // Arrange
            object value = int.MinValue.ToString();

            // Act
            var result = value.ObjToInt();

            // Assert
            Assert.Equal(int.MinValue, result);
        }

        [Fact]
        public void ObjToLong_MaxValue_ReturnsCorrectValue()
        {
            // Arrange
            object value = long.MaxValue.ToString();

            // Act
            var result = value.ObjToLong();

            // Assert
            Assert.Equal(long.MaxValue, result);
        }

        [Fact]
        public void ObjToDecimal_MaxValue_ReturnsCorrectValue()
        {
            // Arrange
            object value = decimal.MaxValue;

            // Act
            var result = value.ObjToDecimal();

            // Assert
            Assert.Equal(decimal.MaxValue, result);
        }

        [Fact]
        public void ObjToString_EmptyString_ReturnsEmptyString()
        {
            // Arrange
            object value = string.Empty;

            // Act
            var result = value.ObjToString();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void ObjToDecimal_NegativeValue_ReturnsCorrectValue()
        {
            // Arrange
            object value = -123.45m;

            // Act
            var result = value.ObjToDecimal();

            // Assert
            Assert.Equal(-123.45m, result);
        }

        [Fact]
        public void ObjToMoney_ZeroValue_ReturnsZero()
        {
            // Arrange
            object value = 0.0;

            // Act
            var result = value.ObjToMoney();

            // Assert
            Assert.Equal(0.0, result);
        }

        #endregion
    }
}

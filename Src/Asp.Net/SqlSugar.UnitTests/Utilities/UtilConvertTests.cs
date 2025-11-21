using System;
using System.Data;
using System.Linq.Expressions;
using Xunit;
using SqlSugar;

namespace SqlSugar.UnitTests.Utilities
{
    /// <summary>
    /// Unit tests for UtilConvert extension methods
    /// </summary>
    public class UtilConvertTests
    {
        #region ObjToInt Tests

        [Fact]
        public void ObjToInt_WithNull_ReturnsZero()
        {
            object value = null;
            int result = value.ObjToInt();
            Assert.Equal(0, result);
        }

        [Fact]
        public void ObjToInt_WithValidInteger_ReturnsInteger()
        {
            object value = 42;
            int result = value.ObjToInt();
            Assert.Equal(42, result);
        }

        [Fact]
        public void ObjToInt_WithStringInteger_ReturnsInteger()
        {
            object value = "123";
            int result = value.ObjToInt();
            Assert.Equal(123, result);
        }

        [Fact]
        public void ObjToInt_WithInvalidString_ReturnsZero()
        {
            object value = "invalid";
            int result = value.ObjToInt();
            Assert.Equal(0, result);
        }

        [Fact]
        public void ObjToInt_WithDBNull_ReturnsZero()
        {
            object value = DBNull.Value;
            int result = value.ObjToInt();
            Assert.Equal(0, result);
        }

        [Fact]
        public void ObjToInt_WithEnum_ReturnsEnumValue()
        {
            object value = DayOfWeek.Monday;
            int result = value.ObjToInt();
            Assert.Equal((int)DayOfWeek.Monday, result);
        }

        [Fact]
        public void ObjToInt_WithNegativeInteger_ReturnsNegativeInteger()
        {
            object value = -42;
            int result = value.ObjToInt();
            Assert.Equal(-42, result);
        }

        [Fact]
        public void ObjToInt_WithErrorValue_WithValidInteger_ReturnsInteger()
        {
            object value = 42;
            int result = value.ObjToInt(-1);
            Assert.Equal(42, result);
        }

        [Fact]
        public void ObjToInt_WithErrorValue_WithInvalidString_ReturnsErrorValue()
        {
            object value = "invalid";
            int result = value.ObjToInt(-1);
            Assert.Equal(-1, result);
        }

        [Fact]
        public void ObjToInt_WithErrorValue_WithEnum_ReturnsEnumValue()
        {
            object value = DayOfWeek.Friday;
            int result = value.ObjToInt(-1);
            Assert.Equal((int)DayOfWeek.Friday, result);
        }

        #endregion

        #region ObjToLong Tests

        [Fact]
        public void ObjToLong_WithNull_ReturnsZero()
        {
            object value = null;
            long result = value.ObjToLong();
            Assert.Equal(0L, result);
        }

        [Fact]
        public void ObjToLong_WithValidLong_ReturnsLong()
        {
            object value = 12345678901234L;
            long result = value.ObjToLong();
            Assert.Equal(12345678901234L, result);
        }

        [Fact]
        public void ObjToLong_WithStringLong_ReturnsLong()
        {
            object value = "98765432109876";
            long result = value.ObjToLong();
            Assert.Equal(98765432109876L, result);
        }

        [Fact]
        public void ObjToLong_WithInvalidString_ReturnsZero()
        {
            object value = "invalid";
            long result = value.ObjToLong();
            Assert.Equal(0L, result);
        }

        [Fact]
        public void ObjToLong_WithEnum_ReturnsEnumValue()
        {
            object value = DayOfWeek.Wednesday;
            long result = value.ObjToLong();
            Assert.Equal((long)(int)DayOfWeek.Wednesday, result);
        }

        [Fact]
        public void ObjToLong_WithDBNull_ReturnsZero()
        {
            object value = DBNull.Value;
            long result = value.ObjToLong();
            Assert.Equal(0L, result);
        }

        #endregion

        #region ObjToMoney Tests

        [Fact]
        public void ObjToMoney_WithNull_ReturnsZero()
        {
            object value = null;
            double result = value.ObjToMoney();
            Assert.Equal(0.0, result);
        }

        [Fact]
        public void ObjToMoney_WithValidDouble_ReturnsDouble()
        {
            object value = 123.45;
            double result = value.ObjToMoney();
            Assert.Equal(123.45, result);
        }

        [Fact]
        public void ObjToMoney_WithStringDouble_ReturnsDouble()
        {
            object value = "456.78";
            double result = value.ObjToMoney();
            Assert.Equal(456.78, result);
        }

        [Fact]
        public void ObjToMoney_WithInvalidString_ReturnsZero()
        {
            object value = "invalid";
            double result = value.ObjToMoney();
            Assert.Equal(0.0, result);
        }

        [Fact]
        public void ObjToMoney_WithDBNull_ReturnsZero()
        {
            object value = DBNull.Value;
            double result = value.ObjToMoney();
            Assert.Equal(0.0, result);
        }

        [Fact]
        public void ObjToMoney_WithErrorValue_WithValidDouble_ReturnsDouble()
        {
            object value = 789.12;
            double result = value.ObjToMoney(-1.0);
            Assert.Equal(789.12, result);
        }

        [Fact]
        public void ObjToMoney_WithErrorValue_WithInvalidString_ReturnsErrorValue()
        {
            object value = "invalid";
            double result = value.ObjToMoney(-1.0);
            Assert.Equal(-1.0, result);
        }

        #endregion

        #region EqualCase Tests

        [Fact]
        public void EqualCase_WithMatchingStrings_ReturnsTrue()
        {
            string value = "Hello";
            bool result = value.EqualCase("Hello");
            Assert.True(result);
        }

        [Fact]
        public void EqualCase_WithDifferentCase_ReturnsTrue()
        {
            string value = "Hello";
            bool result = value.EqualCase("HELLO");
            Assert.True(result);
        }

        [Fact]
        public void EqualCase_WithDifferentStrings_ReturnsFalse()
        {
            string value = "Hello";
            bool result = value.EqualCase("World");
            Assert.False(result);
        }

        [Fact]
        public void EqualCase_WithNullAndNull_ReturnsTrue()
        {
            string value = null;
            bool result = value.EqualCase(null);
            Assert.True(result);
        }

        [Fact]
        public void EqualCase_WithNullAndNotNull_ReturnsFalse()
        {
            string value = null;
            bool result = value.EqualCase("Hello");
            Assert.False(result);
        }

        [Fact]
        public void EqualCase_WithNotNullAndNull_ReturnsFalse()
        {
            string value = "Hello";
            bool result = value.EqualCase(null);
            Assert.False(result);
        }

        [Fact]
        public void EqualCase_WithMixedCase_ReturnsTrue()
        {
            string value = "HeLLo WoRLd";
            bool result = value.EqualCase("hello world");
            Assert.True(result);
        }

        #endregion

        #region ObjToString Tests

        [Fact]
        public void ObjToString_WithNull_ReturnsEmptyString()
        {
            object value = null;
            string result = value.ObjToString();
            Assert.Equal("", result);
        }

        [Fact]
        public void ObjToString_WithString_ReturnsTrimmedString()
        {
            object value = "  Hello World  ";
            string result = value.ObjToString();
            Assert.Equal("Hello World", result);
        }

        [Fact]
        public void ObjToString_WithInteger_ReturnsString()
        {
            object value = 42;
            string result = value.ObjToString();
            Assert.Equal("42", result);
        }

        [Fact]
        public void ObjToString_WithDateTime_ReturnsFormattedString()
        {
            DateTime dt = new DateTime(2024, 1, 15, 10, 30, 0);
            string result = dt.ObjToString(d => d.ToString("yyyy-MM-dd"));
            Assert.Equal("2024-01-15", result);
        }

        [Fact]
        public void ObjToString_WithDateTimeAndNullFormatter_ReturnsString()
        {
            DateTime dt = new DateTime(2024, 1, 15, 10, 30, 0);
            string result = dt.ObjToString(null);
            Assert.Equal(dt.ToString(), result.Trim());
        }

        [Fact]
        public void ObjToString_WithNonDateTimeAndFormatter_ReturnsString()
        {
            object value = 42;
            string result = value.ObjToString(d => d.ToString("yyyy-MM-dd"));
            Assert.Equal("42", result);
        }

        [Fact]
        public void ObjToString_WithErrorValue_WithNull_ReturnsErrorValue()
        {
            object value = null;
            string result = value.ObjToString("default");
            Assert.Equal("default", result);
        }

        [Fact]
        public void ObjToString_WithErrorValue_WithString_ReturnsTrimmedString()
        {
            object value = "  Test  ";
            string result = value.ObjToString("default");
            Assert.Equal("Test", result);
        }

        [Fact]
        public void ObjToStringNoTrim_WithString_ReturnsUntrimmedString()
        {
            object value = "  Hello World  ";
            string result = value.ObjToStringNoTrim();
            Assert.Equal("  Hello World  ", result);
        }

        [Fact]
        public void ObjToStringNoTrim_WithNull_ReturnsEmptyString()
        {
            object value = null;
            string result = value.ObjToStringNoTrim();
            Assert.Equal("", result);
        }

        [Fact]
        public void ObjToStringNew_WithByteArray_ReturnsJoinedString()
        {
            byte[] bytes = new byte[] { 1, 2, 3, 4, 5 };
            object value = bytes;
            string result = value.ObjToStringNew();
            Assert.Equal("1|2|3|4|5", result);
        }

        [Fact]
        public void ObjToStringNew_WithString_ReturnsTrimmedString()
        {
            object value = "  Hello  ";
            string result = value.ObjToStringNew();
            Assert.Equal("Hello", result);
        }

        [Fact]
        public void ObjToStringNew_WithNull_ReturnsEmptyString()
        {
            object value = null;
            string result = value.ObjToStringNew();
            Assert.Equal("", result);
        }

        #endregion

        #region ObjToDecimal Tests

        [Fact]
        public void ObjToDecimal_WithNull_ReturnsZero()
        {
            object value = null;
            decimal result = value.ObjToDecimal();
            Assert.Equal(0m, result);
        }

        [Fact]
        public void ObjToDecimal_WithValidDecimal_ReturnsDecimal()
        {
            object value = 123.45m;
            decimal result = value.ObjToDecimal();
            Assert.Equal(123.45m, result);
        }

        [Fact]
        public void ObjToDecimal_WithStringDecimal_ReturnsDecimal()
        {
            object value = "456.78";
            decimal result = value.ObjToDecimal();
            Assert.Equal(456.78m, result);
        }

        [Fact]
        public void ObjToDecimal_WithInvalidString_ReturnsZero()
        {
            object value = "invalid";
            decimal result = value.ObjToDecimal();
            Assert.Equal(0m, result);
        }

        [Fact]
        public void ObjToDecimal_WithDBNull_ReturnsZero()
        {
            object value = DBNull.Value;
            decimal result = value.ObjToDecimal();
            Assert.Equal(0m, result);
        }

        [Fact]
        public void ObjToDecimal_WithErrorValue_WithValidDecimal_ReturnsDecimal()
        {
            object value = 789.12m;
            decimal result = value.ObjToDecimal(-1m);
            Assert.Equal(789.12m, result);
        }

        [Fact]
        public void ObjToDecimal_WithErrorValue_WithInvalidString_ReturnsErrorValue()
        {
            object value = "invalid";
            decimal result = value.ObjToDecimal(-1m);
            Assert.Equal(-1m, result);
        }

        #endregion

        #region ObjToDate Tests

        [Fact]
        public void ObjToDate_WithNull_ReturnsMinValue()
        {
            object value = null;
            DateTime result = value.ObjToDate();
            Assert.Equal(DateTime.MinValue, result);
        }

        [Fact]
        public void ObjToDate_WithDateTime_ReturnsDateTime()
        {
            DateTime expected = new DateTime(2024, 1, 15, 10, 30, 0);
            object value = expected;
            DateTime result = value.ObjToDate();
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ObjToDate_WithStringDate_ReturnsDateTime()
        {
            object value = "2024-01-15";
            DateTime result = value.ObjToDate();
            Assert.Equal(new DateTime(2024, 1, 15), result.Date);
        }

        [Fact]
        public void ObjToDate_WithInvalidString_ReturnsMinValue()
        {
            object value = "invalid";
            DateTime result = value.ObjToDate();
            Assert.Equal(DateTime.MinValue, result);
        }

        [Fact]
        public void ObjToDate_WithDBNull_ReturnsMinValue()
        {
            object value = DBNull.Value;
            DateTime result = value.ObjToDate();
            Assert.Equal(DateTime.MinValue, result);
        }

        [Fact]
        public void ObjToDate_WithErrorValue_WithValidDateTime_ReturnsDateTime()
        {
            DateTime expected = new DateTime(2024, 1, 15);
            object value = expected;
            DateTime result = value.ObjToDate(new DateTime(2000, 1, 1));
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ObjToDate_WithErrorValue_WithInvalidString_ReturnsErrorValue()
        {
            object value = "invalid";
            DateTime errorValue = new DateTime(2000, 1, 1);
            DateTime result = value.ObjToDate(errorValue);
            Assert.Equal(errorValue, result);
        }

        #endregion

        #region ObjToBool Tests

        [Fact]
        public void ObjToBool_WithNull_ReturnsFalse()
        {
            object value = null;
            bool result = value.ObjToBool();
            Assert.False(result);
        }

        [Fact]
        public void ObjToBool_WithTrue_ReturnsTrue()
        {
            object value = true;
            bool result = value.ObjToBool();
            Assert.True(result);
        }

        [Fact]
        public void ObjToBool_WithFalse_ReturnsFalse()
        {
            object value = false;
            bool result = value.ObjToBool();
            Assert.False(result);
        }

        [Fact]
        public void ObjToBool_WithStringTrue_ReturnsTrue()
        {
            object value = "true";
            bool result = value.ObjToBool();
            Assert.True(result);
        }

        [Fact]
        public void ObjToBool_WithStringFalse_ReturnsFalse()
        {
            object value = "false";
            bool result = value.ObjToBool();
            Assert.False(result);
        }

        [Fact]
        public void ObjToBool_WithInvalidString_ReturnsFalse()
        {
            object value = "invalid";
            bool result = value.ObjToBool();
            Assert.False(result);
        }

        [Fact]
        public void ObjToBool_WithDBNull_ReturnsFalse()
        {
            object value = DBNull.Value;
            bool result = value.ObjToBool();
            Assert.False(result);
        }

        #endregion

        #region ToMemberExpression Tests

        [Fact]
        public void ToMemberExpression_WithMemberExpression_ReturnsMemberExpression()
        {
            Expression<Func<TestClass, int>> expr = x => x.Id;
            var memberExpr = expr.Body as MemberExpression;
            var result = UtilConvert.ToMemberExpression(expr.Body);
            Assert.NotNull(result);
            Assert.Equal("Id", result.Member.Name);
        }

        [Fact]
        public void ToMemberExpression_WithUnaryExpression_ReturnsMemberExpression()
        {
            Expression<Func<TestClass, int>> expr = x => x.Id;
            var unaryExpr = Expression.Convert(expr.Body, typeof(object));
            var result = UtilConvert.ToMemberExpression(unaryExpr);
            Assert.NotNull(result);
            Assert.Equal("Id", result.Member.Name);
        }

        private class TestClass
        {
            public int Id { get; set; }
        }

        #endregion
    }
}


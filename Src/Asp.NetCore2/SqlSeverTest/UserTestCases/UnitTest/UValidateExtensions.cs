using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OrmTest
{
    /// <summary>
    /// Comprehensive test suite for ValidateExtensions utility class.
    /// Tests 60 validation scenarios across multiple categories:
    /// - IsInRange/IsIn (9 tests)
    /// - IsNullOrEmpty/HasValue (8 tests)
    /// - Type Validation (6 tests)
    /// - Regex Validation (10 tests)
    /// - String Type Detection (6 tests)
    /// - Comprehensive Edge Cases (21 tests)
    /// 
    /// Note: ValidateExtensions is internal, so reflection is used to access methods.
    /// All tests use helper methods to invoke extension methods dynamically.
    /// </summary>
    public class UValidateExtensions
    {
        #region Fields and Constructor
        
        /// <summary>Cached Type reference for ValidateExtensions class</summary>
        private static Type _validateExtensionsType;
        
        /// <summary>
        /// Static constructor - initializes reflection type cache.
        /// Loads the internal ValidateExtensions type from SqlSugar assembly.
        /// </summary>
        static UValidateExtensions()
        {
            var sqlSugarAssembly = typeof(SqlSugarClient).Assembly;
            _validateExtensionsType = sqlSugarAssembly.GetType("SqlSugar.ValidateExtensions");
            
            if (_validateExtensionsType == null)
            {
                throw new Exception("Could not find SqlSugar.ValidateExtensions type");
            }
        }
        
        #endregion
        
        #region Reflection Helper Methods
        
        /// <summary>
        /// Invokes non-generic extension methods via reflection.
        /// Handles method overloads, params arrays, and type matching.
        /// </summary>
        /// <param name="methodName">Name of the extension method to invoke</param>
        /// <param name="instance">The 'this' parameter (first parameter)</param>
        /// <param name="parameters">Additional method parameters</param>
        /// <returns>Boolean result from the validation method</returns>
        private static bool InvokeExtensionMethod(string methodName, object instance, params object[] parameters)
        {
            // Get all non-generic methods with matching name, prefer specific types over object
            var methods = _validateExtensionsType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == methodName && !m.IsGenericMethod)
                .OrderBy(m => {
                    var firstParam = m.GetParameters()[0];
                    return firstParam.ParameterType == typeof(object) ? 1 : 0;
                });
            
            foreach (var method in methods)
            {
                try
                {
                    var methodParams = method.GetParameters();
                    
                    // Verify first parameter type is compatible
                    var firstParamType = methodParams[0].ParameterType;
                    if (instance != null && !firstParamType.IsAssignableFrom(instance.GetType()))
                    {
                        continue;
                    }
                    
                    // Handle params array (e.g., IsIn, IsContainsIn)
                    if (methodParams.Length >= 2 && methodParams[methodParams.Length - 1].IsDefined(typeof(ParamArrayAttribute), false))
                    {
                        var paramsArrayType = methodParams[methodParams.Length - 1].ParameterType.GetElementType();
                        var paramsArray = Array.CreateInstance(paramsArrayType, parameters.Length);
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            paramsArray.SetValue(parameters[i], i);
                        }
                        return (bool)method.Invoke(null, new object[] { instance, paramsArray });
                    }
                    else
                    {
                        // Handle regular parameters
                        var allParams = new object[parameters.Length + 1];
                        allParams[0] = instance;
                        Array.Copy(parameters, 0, allParams, 1, parameters.Length);
                        
                        if (methodParams.Length == allParams.Length)
                        {
                            return (bool)method.Invoke(null, allParams);
                        }
                    }
                }
                catch { continue; }
            }
            
            throw new Exception($"Method {methodName} with matching parameters not found in ValidateExtensions");
        }
        
        /// <summary>
        /// Invokes generic extension methods via reflection.
        /// Handles generic type parameters and params arrays.
        /// </summary>
        /// <typeparam name="T">Generic type parameter</typeparam>
        /// <param name="methodName">Name of the generic extension method</param>
        /// <param name="instance">The 'this' parameter (first parameter)</param>
        /// <param name="parameters">Additional method parameters</param>
        /// <returns>Boolean result from the validation method</returns>
        private static bool InvokeGenericExtensionMethod<T>(string methodName, T instance, params object[] parameters)
        {
            var methods = _validateExtensionsType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == methodName && m.IsGenericMethod);
            
            foreach (var method in methods)
            {
                try
                {
                    var genericMethod = method.MakeGenericMethod(typeof(T));
                    var methodParams = genericMethod.GetParameters();
                    
                    // Handle params array for generic methods (e.g., IsIn<T>)
                    if (methodParams.Length == 2 && methodParams[1].IsDefined(typeof(ParamArrayAttribute), false))
                    {
                        var paramsArray = Array.CreateInstance(typeof(T), parameters.Length);
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            paramsArray.SetValue(parameters[i], i);
                        }
                        return (bool)genericMethod.Invoke(null, new object[] { instance, paramsArray });
                    }
                    else
                    {
                        // Handle regular generic parameters
                        var allParams = new object[parameters.Length + 1];
                        allParams[0] = instance;
                        Array.Copy(parameters, 0, allParams, 1, parameters.Length);
                        return (bool)genericMethod.Invoke(null, allParams);
                    }
                }
                catch { continue; }
            }
            
            throw new Exception($"Generic method {methodName} not found or could not be invoked");
        }
        
        #endregion
        
        #region Test Initialization
        
        /// <summary>
        /// Main entry point for test suite execution.
        /// Runs all 60 validation tests in organized categories.
        /// Throws exception on first failure for quick debugging.
        /// </summary>
        public static void Init()
        {
            Console.WriteLine("\n========== UValidateExtensions Test Suite Started ==========");
            
            // Category 1: Range and Collection Membership (9 tests)
            Test_IsInRange_Int_WithinRange();
            Test_IsInRange_Int_BelowRange();
            Test_IsInRange_Int_AboveRange();
            Test_IsInRange_DateTime_WithinRange();
            Test_IsInRange_DateTime_OutsideRange();
            Test_IsIn_String_Found();
            Test_IsIn_String_NotFound();
            Test_IsIn_Int_Found();
            Test_IsIn_Int_NotFound();
            
            // Category 2: Null/Empty Checks (8 tests)
            Test_IsNullOrEmpty_Object_Null();
            Test_IsNullOrEmpty_Object_Empty();
            Test_IsNullOrEmpty_Object_WithValue();
            Test_IsNullOrEmpty_Guid_Empty();
            Test_IsNullOrEmpty_Collection_Empty();
            Test_HasValue_Object_Null();
            Test_HasValue_Object_WithValue();
            Test_HasValue_Collection_WithItems();
            
            // Category 3: Type Format Validation (6 tests)
            Test_IsInt_ValidInteger();
            Test_IsInt_InvalidInteger();
            Test_IsMoney_ValidDecimal();
            Test_IsGuid_ValidGuid();
            Test_IsDate_ValidDate();
            Test_IsZero_ZeroValue();
            
            // Category 4: Regex Pattern Matching (10 tests)
            Test_IsEamil_ValidEmail();
            Test_IsEamil_InvalidEmail();
            Test_IsMobile_ValidMobile();
            Test_IsMobile_InvalidMobile();
            Test_IsTelephone_ValidTelephone();
            Test_IsTelephone_InvalidTelephone();
            Test_IsIDcard_Valid15Digit();
            Test_IsIDcard_Valid18Digit();
            Test_IsFax_ValidFax();
            Test_IsMatch_CustomPattern();
            
            // Category 5: Type Name Detection (6 tests)
            Test_IsCollectionsList_ValidList();
            Test_IsCollectionsList_InvalidType();
            Test_IsStringArray_ValidArray();
            Test_IsEnumerable_ValidEnumerable();
            Test_IsContainsIn_Found();
            Test_IsContainsStartWithIn_Found();
            
            // Category 6: Edge Cases and Boundaries (21 tests)
            Test_IsInRange_Int_BoundaryMin();
            Test_IsInRange_Int_BoundaryMax();
            Test_IsInRange_DateTime_BoundaryMin();
            Test_IsInRange_DateTime_BoundaryMax();
            Test_IsNullOrEmpty_DBNull();
            Test_IsNullOrEmpty_NullableGuid_Null();
            Test_IsNullOrEmpty_NullableGuid_Empty();
            Test_HasValue_DBNull();
            Test_IsZero_StringZero();
            Test_IsZero_NumericZero();
            Test_IsInt_NegativeNumber();
            Test_IsInt_Null();
            Test_IsNoInt_ValidInteger();
            Test_IsNoInt_InvalidInteger();
            Test_IsMoney_NegativeValue();
            Test_IsGuid_InvalidGuid();
            Test_IsDate_InvalidDate();
            Test_IsEamil_Null();
            Test_IsMobile_TooShort();
            Test_IsIDcard_Invalid();
            Test_IsMatch_NoMatch();
            
            Console.WriteLine("\n========== All 60 Tests Passed Successfully! ==========");
        }
        
        #endregion
        
        #region Category 1: IsInRange/IsIn Tests (9 tests)
        
        /// <summary>Test: Integer value within specified range should return true</summary>
        private static void Test_IsInRange_Int_WithinRange()
        {
            int value = 50;
            bool result = InvokeExtensionMethod("IsInRange", value, 1, 100);
            if (!result) throw new Exception("Test_IsInRange_Int_WithinRange failed");
            Console.WriteLine("✓ Test_IsInRange_Int_WithinRange passed");
        }
        
        /// <summary>Test: Integer value below range should return false</summary>
        private static void Test_IsInRange_Int_BelowRange()
        {
            int value = 0;
            bool result = InvokeExtensionMethod("IsInRange", value, 1, 100);
            if (result) throw new Exception("Test_IsInRange_Int_BelowRange failed");
            Console.WriteLine("✓ Test_IsInRange_Int_BelowRange passed");
        }
        
        /// <summary>Test: Integer value above range should return false</summary>
        private static void Test_IsInRange_Int_AboveRange()
        {
            int value = 101;
            bool result = InvokeExtensionMethod("IsInRange", value, 1, 100);
            if (result) throw new Exception("Test_IsInRange_Int_AboveRange failed");
            Console.WriteLine("✓ Test_IsInRange_Int_AboveRange passed");
        }
        
        /// <summary>Test: DateTime value within range should return true</summary>
        private static void Test_IsInRange_DateTime_WithinRange()
        {
            DateTime value = new DateTime(2023, 6, 15);
            DateTime begin = new DateTime(2023, 1, 1);
            DateTime end = new DateTime(2023, 12, 31);
            bool result = InvokeExtensionMethod("IsInRange", value, begin, end);
            if (!result) throw new Exception("Test_IsInRange_DateTime_WithinRange failed");
            Console.WriteLine("✓ Test_IsInRange_DateTime_WithinRange passed");
        }
        
        /// <summary>Test: DateTime value outside range should return false</summary>
        private static void Test_IsInRange_DateTime_OutsideRange()
        {
            DateTime value = new DateTime(2024, 1, 1);
            DateTime begin = new DateTime(2023, 1, 1);
            DateTime end = new DateTime(2023, 12, 31);
            bool result = InvokeExtensionMethod("IsInRange", value, begin, end);
            if (result) throw new Exception("Test_IsInRange_DateTime_OutsideRange failed");
            Console.WriteLine("✓ Test_IsInRange_DateTime_OutsideRange passed");
        }
        
        /// <summary>Test: String value in array should return true</summary>
        private static void Test_IsIn_String_Found()
        {
            string value = "apple";
            bool result = InvokeGenericExtensionMethod("IsIn", value, "apple", "banana", "orange");
            if (!result) throw new Exception("Test_IsIn_String_Found failed");
            Console.WriteLine("✓ Test_IsIn_String_Found passed");
        }
        
        /// <summary>Test: String value not in array should return false</summary>
        private static void Test_IsIn_String_NotFound()
        {
            string value = "grape";
            bool result = InvokeGenericExtensionMethod("IsIn", value, "apple", "banana", "orange");
            if (result) throw new Exception("Test_IsIn_String_NotFound failed");
            Console.WriteLine("✓ Test_IsIn_String_NotFound passed");
        }
        
        /// <summary>Test: Integer value in array should return true</summary>
        private static void Test_IsIn_Int_Found()
        {
            int value = 5;
            bool result = InvokeGenericExtensionMethod("IsIn", value, 1, 3, 5, 7, 9);
            if (!result) throw new Exception("Test_IsIn_Int_Found failed");
            Console.WriteLine("✓ Test_IsIn_Int_Found passed");
        }
        
        /// <summary>Test: Integer value not in array should return false</summary>
        private static void Test_IsIn_Int_NotFound()
        {
            int value = 4;
            bool result = InvokeGenericExtensionMethod("IsIn", value, 1, 3, 5, 7, 9);
            if (result) throw new Exception("Test_IsIn_Int_NotFound failed");
            Console.WriteLine("✓ Test_IsIn_Int_NotFound passed");
        }
        
        #endregion
        
        #region Category 2: IsNullOrEmpty/HasValue Tests (8 tests)
        
        /// <summary>Test: Null object should return true for IsNullOrEmpty</summary>
        private static void Test_IsNullOrEmpty_Object_Null()
        {
            object value = null;
            bool result = InvokeExtensionMethod("IsNullOrEmpty", value);
            if (!result) throw new Exception("Test_IsNullOrEmpty_Object_Null failed");
            Console.WriteLine("✓ Test_IsNullOrEmpty_Object_Null passed");
        }
        
        /// <summary>Test: Empty string should return true for IsNullOrEmpty</summary>
        private static void Test_IsNullOrEmpty_Object_Empty()
        {
            object value = "";
            bool result = InvokeExtensionMethod("IsNullOrEmpty", value);
            if (!result) throw new Exception("Test_IsNullOrEmpty_Object_Empty failed");
            Console.WriteLine("✓ Test_IsNullOrEmpty_Object_Empty passed");
        }
        
        /// <summary>Test: Object with value should return false for IsNullOrEmpty</summary>
        private static void Test_IsNullOrEmpty_Object_WithValue()
        {
            object value = "test";
            bool result = InvokeExtensionMethod("IsNullOrEmpty", value);
            if (result) throw new Exception("Test_IsNullOrEmpty_Object_WithValue failed");
            Console.WriteLine("✓ Test_IsNullOrEmpty_Object_WithValue passed");
        }
        
        /// <summary>Test: Empty Guid should return true for IsNullOrEmpty</summary>
        private static void Test_IsNullOrEmpty_Guid_Empty()
        {
            Guid value = Guid.Empty;
            bool result = InvokeExtensionMethod("IsNullOrEmpty", value);
            if (!result) throw new Exception("Test_IsNullOrEmpty_Guid_Empty failed");
            Console.WriteLine("✓ Test_IsNullOrEmpty_Guid_Empty passed");
        }
        
        /// <summary>Test: Empty collection should return true for IsNullOrEmpty</summary>
        private static void Test_IsNullOrEmpty_Collection_Empty()
        {
            IEnumerable<object> value = new List<object>();
            bool result = InvokeExtensionMethod("IsNullOrEmpty", value);
            if (!result) throw new Exception("Test_IsNullOrEmpty_Collection_Empty failed");
            Console.WriteLine("✓ Test_IsNullOrEmpty_Collection_Empty passed");
        }
        
        /// <summary>Test: Null object should return false for HasValue</summary>
        private static void Test_HasValue_Object_Null()
        {
            object value = null;
            bool result = InvokeExtensionMethod("HasValue", value);
            if (result) throw new Exception("Test_HasValue_Object_Null failed");
            Console.WriteLine("✓ Test_HasValue_Object_Null passed");
        }
        
        /// <summary>Test: Object with value should return true for HasValue</summary>
        private static void Test_HasValue_Object_WithValue()
        {
            object value = "test";
            bool result = InvokeExtensionMethod("HasValue", value);
            if (!result) throw new Exception("Test_HasValue_Object_WithValue failed");
            Console.WriteLine("✓ Test_HasValue_Object_WithValue passed");
        }
        
        /// <summary>Test: Collection with items should return true for HasValue</summary>
        private static void Test_HasValue_Collection_WithItems()
        {
            IEnumerable<object> value = new List<object> { "item1", "item2" };
            bool result = InvokeExtensionMethod("HasValue", value);
            if (!result) throw new Exception("Test_HasValue_Collection_WithItems failed");
            Console.WriteLine("✓ Test_HasValue_Collection_WithItems passed");
        }
        
        #endregion
        
        #region Category 3: Type Validation Tests (6 tests)
        
        /// <summary>Test: Valid integer string should return true for IsInt</summary>
        private static void Test_IsInt_ValidInteger()
        {
            object value = "12345";
            bool result = InvokeExtensionMethod("IsInt", value);
            if (!result) throw new Exception("Test_IsInt_ValidInteger failed");
            Console.WriteLine("✓ Test_IsInt_ValidInteger passed");
        }
        
        /// <summary>Test: Invalid integer string should return false for IsInt</summary>
        private static void Test_IsInt_InvalidInteger()
        {
            object value = "123.45";
            bool result = InvokeExtensionMethod("IsInt", value);
            if (result) throw new Exception("Test_IsInt_InvalidInteger failed");
            Console.WriteLine("✓ Test_IsInt_InvalidInteger passed");
        }
        
        /// <summary>Test: Valid decimal string should return true for IsMoney</summary>
        private static void Test_IsMoney_ValidDecimal()
        {
            object value = "123.45";
            bool result = InvokeExtensionMethod("IsMoney", value);
            if (!result) throw new Exception("Test_IsMoney_ValidDecimal failed");
            Console.WriteLine("✓ Test_IsMoney_ValidDecimal passed");
        }
        
        /// <summary>Test: Valid Guid string should return true for IsGuid</summary>
        private static void Test_IsGuid_ValidGuid()
        {
            object value = Guid.NewGuid().ToString();
            bool result = InvokeExtensionMethod("IsGuid", value);
            if (!result) throw new Exception("Test_IsGuid_ValidGuid failed");
            Console.WriteLine("✓ Test_IsGuid_ValidGuid passed");
        }
        
        /// <summary>Test: Valid date string should return true for IsDate</summary>
        private static void Test_IsDate_ValidDate()
        {
            object value = "2023-12-25";
            bool result = InvokeExtensionMethod("IsDate", value);
            if (!result) throw new Exception("Test_IsDate_ValidDate failed");
            Console.WriteLine("✓ Test_IsDate_ValidDate passed");
        }
        
        /// <summary>Test: Zero value should return true for IsZero</summary>
        private static void Test_IsZero_ZeroValue()
        {
            object value = 0;
            bool result = InvokeExtensionMethod("IsZero", value);
            if (!result) throw new Exception("Test_IsZero_ZeroValue failed");
            Console.WriteLine("✓ Test_IsZero_ZeroValue passed");
        }
        
        #endregion
        
        #region Category 4: Regex Validation Tests (10 tests)
        
        /// <summary>Test: Valid email format should return true</summary>
        private static void Test_IsEamil_ValidEmail()
        {
            object value = "test@example.com";
            bool result = InvokeExtensionMethod("IsEamil", value);
            if (!result) throw new Exception("Test_IsEamil_ValidEmail failed");
            Console.WriteLine("✓ Test_IsEamil_ValidEmail passed");
        }
        
        /// <summary>Test: Invalid email format should return false</summary>
        private static void Test_IsEamil_InvalidEmail()
        {
            object value = "invalid-email";
            bool result = InvokeExtensionMethod("IsEamil", value);
            if (result) throw new Exception("Test_IsEamil_InvalidEmail failed");
            Console.WriteLine("✓ Test_IsEamil_InvalidEmail passed");
        }
        
        /// <summary>Test: Valid 11-digit mobile number should return true</summary>
        private static void Test_IsMobile_ValidMobile()
        {
            object value = "13800138000";
            bool result = InvokeExtensionMethod("IsMobile", value);
            if (!result) throw new Exception("Test_IsMobile_ValidMobile failed");
            Console.WriteLine("✓ Test_IsMobile_ValidMobile passed");
        }
        
        /// <summary>Test: Invalid mobile number should return false</summary>
        private static void Test_IsMobile_InvalidMobile()
        {
            object value = "123456";
            bool result = InvokeExtensionMethod("IsMobile", value);
            if (result) throw new Exception("Test_IsMobile_InvalidMobile failed");
            Console.WriteLine("✓ Test_IsMobile_InvalidMobile passed");
        }
        
        /// <summary>Test: Valid telephone number should return true</summary>
        private static void Test_IsTelephone_ValidTelephone()
        {
            object value = "021-12345678";
            bool result = InvokeExtensionMethod("IsTelephone", value);
            if (!result) throw new Exception("Test_IsTelephone_ValidTelephone failed");
            Console.WriteLine("✓ Test_IsTelephone_ValidTelephone passed");
        }
        
        /// <summary>Test: Invalid telephone number should return false</summary>
        private static void Test_IsTelephone_InvalidTelephone()
        {
            object value = "123";
            bool result = InvokeExtensionMethod("IsTelephone", value);
            if (result) throw new Exception("Test_IsTelephone_InvalidTelephone failed");
            Console.WriteLine("✓ Test_IsTelephone_InvalidTelephone passed");
        }
        
        /// <summary>Test: Valid 15-digit ID card should return true</summary>
        private static void Test_IsIDcard_Valid15Digit()
        {
            object value = "110101900101001";
            bool result = InvokeExtensionMethod("IsIDcard", value);
            if (!result) throw new Exception("Test_IsIDcard_Valid15Digit failed");
            Console.WriteLine("✓ Test_IsIDcard_Valid15Digit passed");
        }
        
        /// <summary>Test: Valid 18-digit ID card should return true</summary>
        private static void Test_IsIDcard_Valid18Digit()
        {
            object value = "110101199001010011";
            bool result = InvokeExtensionMethod("IsIDcard", value);
            if (!result) throw new Exception("Test_IsIDcard_Valid18Digit failed");
            Console.WriteLine("✓ Test_IsIDcard_Valid18Digit passed");
        }
        
        /// <summary>Test: Valid fax number should return true</summary>
        private static void Test_IsFax_ValidFax()
        {
            object value = "+86 021-12345678";
            bool result = InvokeExtensionMethod("IsFax", value);
            if (!result) throw new Exception("Test_IsFax_ValidFax failed");
            Console.WriteLine("✓ Test_IsFax_ValidFax passed");
        }
        
        /// <summary>Test: Custom regex pattern matching</summary>
        private static void Test_IsMatch_CustomPattern()
        {
            object value = "ABC123";
            bool result = InvokeExtensionMethod("IsMatch", value, @"^[A-Z]{3}\d{3}$");
            if (!result) throw new Exception("Test_IsMatch_CustomPattern failed");
            Console.WriteLine("✓ Test_IsMatch_CustomPattern passed");
        }
        
        #endregion
        
        #region Category 5: String Type Detection Tests (6 tests)
        
        /// <summary>Test: List type string should return true for IsCollectionsList</summary>
        private static void Test_IsCollectionsList_ValidList()
        {
            string value = "System.Collections.Generic.List`1[System.String]";
            bool result = InvokeExtensionMethod("IsCollectionsList", value);
            if (!result) throw new Exception("Test_IsCollectionsList_ValidList failed");
            Console.WriteLine("✓ Test_IsCollectionsList_ValidList passed");
        }
        
        /// <summary>Test: Non-list type string should return false for IsCollectionsList</summary>
        private static void Test_IsCollectionsList_InvalidType()
        {
            string value = "System.String";
            bool result = InvokeExtensionMethod("IsCollectionsList", value);
            if (result) throw new Exception("Test_IsCollectionsList_InvalidType failed");
            Console.WriteLine("✓ Test_IsCollectionsList_InvalidType passed");
        }
        
        /// <summary>Test: Array type string should return true for IsStringArray</summary>
        private static void Test_IsStringArray_ValidArray()
        {
            string value = "System.String[]";
            bool result = InvokeExtensionMethod("IsStringArray", value);
            if (!result) throw new Exception("Test_IsStringArray_ValidArray failed");
            Console.WriteLine("✓ Test_IsStringArray_ValidArray passed");
        }
        
        /// <summary>Test: Enumerable type string should return true for IsEnumerable</summary>
        private static void Test_IsEnumerable_ValidEnumerable()
        {
            string value = "System.Linq.Enumerable+Iterator";
            bool result = InvokeExtensionMethod("IsEnumerable", value);
            if (!result) throw new Exception("Test_IsEnumerable_ValidEnumerable failed");
            Console.WriteLine("✓ Test_IsEnumerable_ValidEnumerable passed");
        }
        
        /// <summary>Test: String containing any of the specified values should return true</summary>
        private static void Test_IsContainsIn_Found()
        {
            string value = "Hello World";
            bool result = InvokeExtensionMethod("IsContainsIn", value, "World", "Test", "Example");
            if (!result) throw new Exception("Test_IsContainsIn_Found failed");
            Console.WriteLine("✓ Test_IsContainsIn_Found passed");
        }
        
        /// <summary>Test: String starting with any of the specified values should return true</summary>
        private static void Test_IsContainsStartWithIn_Found()
        {
            string value = "HelloWorld";
            bool result = InvokeExtensionMethod("IsContainsStartWithIn", value, "Hello", "Test", "Example");
            if (!result) throw new Exception("Test_IsContainsStartWithIn_Found failed");
            Console.WriteLine("✓ Test_IsContainsStartWithIn_Found passed");
        }
        
        #endregion
        
        #region Category 6: Comprehensive Edge Cases (21 tests)
        
        /// <summary>Test: Integer at minimum boundary of range should return true</summary>
        private static void Test_IsInRange_Int_BoundaryMin()
        {
            int value = 1;
            bool result = InvokeExtensionMethod("IsInRange", value, 1, 100);
            if (!result) throw new Exception("Test_IsInRange_Int_BoundaryMin failed");
            Console.WriteLine("✓ Test_IsInRange_Int_BoundaryMin passed");
        }
        
        /// <summary>Test: Integer at maximum boundary of range should return true</summary>
        private static void Test_IsInRange_Int_BoundaryMax()
        {
            int value = 100;
            bool result = InvokeExtensionMethod("IsInRange", value, 1, 100);
            if (!result) throw new Exception("Test_IsInRange_Int_BoundaryMax failed");
            Console.WriteLine("✓ Test_IsInRange_Int_BoundaryMax passed");
        }
        
        /// <summary>Test: DateTime at minimum boundary of range should return true</summary>
        private static void Test_IsInRange_DateTime_BoundaryMin()
        {
            DateTime value = new DateTime(2023, 1, 1);
            DateTime begin = new DateTime(2023, 1, 1);
            DateTime end = new DateTime(2023, 12, 31);
            bool result = InvokeExtensionMethod("IsInRange", value, begin, end);
            if (!result) throw new Exception("Test_IsInRange_DateTime_BoundaryMin failed");
            Console.WriteLine("✓ Test_IsInRange_DateTime_BoundaryMin passed");
        }
        
        /// <summary>Test: DateTime at maximum boundary of range should return true</summary>
        private static void Test_IsInRange_DateTime_BoundaryMax()
        {
            DateTime value = new DateTime(2023, 12, 31);
            DateTime begin = new DateTime(2023, 1, 1);
            DateTime end = new DateTime(2023, 12, 31);
            bool result = InvokeExtensionMethod("IsInRange", value, begin, end);
            if (!result) throw new Exception("Test_IsInRange_DateTime_BoundaryMax failed");
            Console.WriteLine("✓ Test_IsInRange_DateTime_BoundaryMax passed");
        }
        
        /// <summary>Test: DBNull value should return true for IsNullOrEmpty</summary>
        private static void Test_IsNullOrEmpty_DBNull()
        {
            object value = DBNull.Value;
            bool result = InvokeExtensionMethod("IsNullOrEmpty", value);
            if (!result) throw new Exception("Test_IsNullOrEmpty_DBNull failed");
            Console.WriteLine("✓ Test_IsNullOrEmpty_DBNull passed");
        }
        
        /// <summary>Test: Nullable Guid with null value should return true for IsNullOrEmpty</summary>
        private static void Test_IsNullOrEmpty_NullableGuid_Null()
        {
            Guid? value = null;
            bool result = InvokeExtensionMethod("IsNullOrEmpty", value);
            if (!result) throw new Exception("Test_IsNullOrEmpty_NullableGuid_Null failed");
            Console.WriteLine("✓ Test_IsNullOrEmpty_NullableGuid_Null passed");
        }
        
        /// <summary>Test: Nullable Guid with empty value should return true for IsNullOrEmpty</summary>
        private static void Test_IsNullOrEmpty_NullableGuid_Empty()
        {
            Guid? value = Guid.Empty;
            bool result = InvokeExtensionMethod("IsNullOrEmpty", value);
            if (!result) throw new Exception("Test_IsNullOrEmpty_NullableGuid_Empty failed");
            Console.WriteLine("✓ Test_IsNullOrEmpty_NullableGuid_Empty passed");
        }
        
        /// <summary>Test: DBNull value should return false for HasValue</summary>
        private static void Test_HasValue_DBNull()
        {
            object value = DBNull.Value;
            bool result = InvokeExtensionMethod("HasValue", value);
            if (result) throw new Exception("Test_HasValue_DBNull failed");
            Console.WriteLine("✓ Test_HasValue_DBNull passed");
        }
        
        /// <summary>Test: String "0" should return true for IsZero</summary>
        private static void Test_IsZero_StringZero()
        {
            object value = "0";
            bool result = InvokeExtensionMethod("IsZero", value);
            if (!result) throw new Exception("Test_IsZero_StringZero failed");
            Console.WriteLine("✓ Test_IsZero_StringZero passed");
        }
        
        /// <summary>Test: Numeric 0 should return true for IsZero</summary>
        private static void Test_IsZero_NumericZero()
        {
            object value = 0;
            bool result = InvokeExtensionMethod("IsZero", value);
            if (!result) throw new Exception("Test_IsZero_NumericZero failed");
            Console.WriteLine("✓ Test_IsZero_NumericZero passed");
        }
        
        /// <summary>Test: Negative number string should return false for IsInt (only positive integers)</summary>
        private static void Test_IsInt_NegativeNumber()
        {
            object value = "-123";
            bool result = InvokeExtensionMethod("IsInt", value);
            if (result) throw new Exception("Test_IsInt_NegativeNumber failed");
            Console.WriteLine("✓ Test_IsInt_NegativeNumber passed");
        }
        
        /// <summary>Test: Null value should return false for IsInt</summary>
        private static void Test_IsInt_Null()
        {
            object value = null;
            bool result = InvokeExtensionMethod("IsInt", value);
            if (result) throw new Exception("Test_IsInt_Null failed");
            Console.WriteLine("✓ Test_IsInt_Null passed");
        }
        
        /// <summary>Test: Valid integer should return false for IsNoInt</summary>
        private static void Test_IsNoInt_ValidInteger()
        {
            object value = "12345";
            bool result = InvokeExtensionMethod("IsNoInt", value);
            if (result) throw new Exception("Test_IsNoInt_ValidInteger failed");
            Console.WriteLine("✓ Test_IsNoInt_ValidInteger passed");
        }
        
        /// <summary>Test: Invalid integer should return true for IsNoInt</summary>
        private static void Test_IsNoInt_InvalidInteger()
        {
            object value = "abc";
            bool result = InvokeExtensionMethod("IsNoInt", value);
            if (!result) throw new Exception("Test_IsNoInt_InvalidInteger failed");
            Console.WriteLine("✓ Test_IsNoInt_InvalidInteger passed");
        }
        
        /// <summary>Test: Negative value should return true for IsMoney</summary>
        private static void Test_IsMoney_NegativeValue()
        {
            object value = "-123.45";
            bool result = InvokeExtensionMethod("IsMoney", value);
            if (!result) throw new Exception("Test_IsMoney_NegativeValue failed");
            Console.WriteLine("✓ Test_IsMoney_NegativeValue passed");
        }
        
        /// <summary>Test: Invalid Guid string should return false for IsGuid</summary>
        private static void Test_IsGuid_InvalidGuid()
        {
            object value = "not-a-guid";
            bool result = InvokeExtensionMethod("IsGuid", value);
            if (result) throw new Exception("Test_IsGuid_InvalidGuid failed");
            Console.WriteLine("✓ Test_IsGuid_InvalidGuid passed");
        }
        
        /// <summary>Test: Invalid date string should return false for IsDate</summary>
        private static void Test_IsDate_InvalidDate()
        {
            object value = "not-a-date";
            bool result = InvokeExtensionMethod("IsDate", value);
            if (result) throw new Exception("Test_IsDate_InvalidDate failed");
            Console.WriteLine("✓ Test_IsDate_InvalidDate passed");
        }
        
        /// <summary>Test: Null value should return false for IsEamil</summary>
        private static void Test_IsEamil_Null()
        {
            object value = null;
            bool result = InvokeExtensionMethod("IsEamil", value);
            if (result) throw new Exception("Test_IsEamil_Null failed");
            Console.WriteLine("✓ Test_IsEamil_Null passed");
        }
        
        /// <summary>Test: Too short mobile number should return false for IsMobile</summary>
        private static void Test_IsMobile_TooShort()
        {
            object value = "12345";
            bool result = InvokeExtensionMethod("IsMobile", value);
            if (result) throw new Exception("Test_IsMobile_TooShort failed");
            Console.WriteLine("✓ Test_IsMobile_TooShort passed");
        }
        
        /// <summary>Test: Invalid ID card should return false for IsIDcard</summary>
        private static void Test_IsIDcard_Invalid()
        {
            object value = "12345";
            bool result = InvokeExtensionMethod("IsIDcard", value);
            if (result) throw new Exception("Test_IsIDcard_Invalid failed");
            Console.WriteLine("✓ Test_IsIDcard_Invalid passed");
        }
        
        /// <summary>Test: Pattern that doesn't match should return false for IsMatch</summary>
        private static void Test_IsMatch_NoMatch()
        {
            object value = "ABC";
            bool result = InvokeExtensionMethod("IsMatch", value, @"^[A-Z]{3}\d{3}$");
            if (result) throw new Exception("Test_IsMatch_NoMatch failed");
            Console.WriteLine("✓ Test_IsMatch_NoMatch passed");
        }
        
        #endregion
    }
}

using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public class USugarRetry
    {
        public static void Init()
        {
            Console.WriteLine("\n========== USugarRetry Test Suite ==========");
            Console.WriteLine("Testing SqlSugar.SugarRetry utility class\n");
            
            // Execute Action Tests
            Console.WriteLine("--- Execute Action Tests ---");
            Test_Execute_Action_Success();
            Test_Execute_Action_RetryOnce();
            Test_Execute_Action_RetryMultiple();
            Test_Execute_Action_AllFail();
            
            // Execute Action with Args Tests
            Console.WriteLine("\n--- Execute Action with Args Tests ---");
            Test_Execute_Action_OneArg();
            Test_Execute_Action_TwoArgs();
            Test_Execute_Action_ThreeArgs();
            Test_Execute_Action_FourArgs();
            
            // Execute Func Tests
            Console.WriteLine("\n--- Execute Func Tests ---");
            Test_Execute_Func_Success();
            Test_Execute_Func_RetryOnce();
            Test_Execute_Func_RetryMultiple();
            Test_Execute_Func_AllFail();
            Test_Execute_Func_ReturnsValue();
            
            // Execute Func with Args Tests
            Console.WriteLine("\n--- Execute Func with Args Tests ---");
            Test_Execute_Func_OneArg();
            Test_Execute_Func_TwoArgs();
            Test_Execute_Func_ThreeArgs();
            Test_Execute_Func_FourArgs();
            
            // Retry Interval Tests
            Console.WriteLine("\n--- Retry Interval Tests ---");
            Test_RetryInterval_Zero();
            Test_RetryInterval_Short();
            Test_RetryInterval_Custom();
            
            // Retry Count Tests
            Console.WriteLine("\n--- Retry Count Tests ---");
            Test_RetryCount_One();
            Test_RetryCount_Five();
            Test_RetryCount_Default();
            
            // Exception Tests
            Console.WriteLine("\n--- Exception Tests ---");
            Test_AggregateException_ContainsAll();
            Test_AggregateException_Count();
            Test_DifferentExceptionTypes();
            
            // Edge Cases
            Console.WriteLine("\n--- Edge Cases ---");
            Test_SuccessOnLastRetry();
            Test_NullReturnValue();
            Test_ComplexReturnType();
            
            Console.WriteLine("\n========== All USugarRetry Tests Passed! ==========\n");
        }

        #region Execute Action Tests
        
        private static void Test_Execute_Action_Success()
        {
            bool executed = false;
            SugarRetry.Execute(() => { executed = true; }, TimeSpan.FromMilliseconds(1), 3);
            
            if (!executed)
                throw new Exception("Test_Execute_Action_Success failed");
            Console.WriteLine("  ✓ Test_Execute_Action_Success passed");
        }
        
        private static void Test_Execute_Action_RetryOnce()
        {
            int attempts = 0;
            SugarRetry.Execute(() => 
            {
                attempts++;
                if (attempts < 2) throw new Exception("Retry");
            }, TimeSpan.FromMilliseconds(1), 3);
            
            if (attempts != 2)
                throw new Exception("Test_Execute_Action_RetryOnce failed");
            Console.WriteLine("  ✓ Test_Execute_Action_RetryOnce passed");
        }
        
        private static void Test_Execute_Action_RetryMultiple()
        {
            int attempts = 0;
            SugarRetry.Execute(() => 
            {
                attempts++;
                if (attempts < 3) throw new Exception("Retry");
            }, TimeSpan.FromMilliseconds(1), 5);
            
            if (attempts != 3)
                throw new Exception("Test_Execute_Action_RetryMultiple failed");
            Console.WriteLine("  ✓ Test_Execute_Action_RetryMultiple passed");
        }
        
        private static void Test_Execute_Action_AllFail()
        {
            int attempts = 0;
            try
            {
                SugarRetry.Execute(() => 
                {
                    attempts++;
                    throw new Exception("Always fail");
                }, TimeSpan.FromMilliseconds(1), 3);
                throw new Exception("Should have thrown");
            }
            catch (AggregateException ex)
            {
                if (attempts != 3 || ex.InnerExceptions.Count != 3)
                    throw new Exception("Test_Execute_Action_AllFail failed");
            }
            Console.WriteLine("  ✓ Test_Execute_Action_AllFail passed");
        }
        
        #endregion

        #region Execute Action with Args Tests
        
        private static void Test_Execute_Action_OneArg()
        {
            string result = null;
            SugarRetry.Execute<string>((arg) => { result = arg; }, "test", TimeSpan.FromMilliseconds(1), 3);
            
            if (result != "test")
                throw new Exception("Test_Execute_Action_OneArg failed");
            Console.WriteLine("  ✓ Test_Execute_Action_OneArg passed");
        }
        
        private static void Test_Execute_Action_TwoArgs()
        {
            int sum = 0;
            SugarRetry.Execute<int, int>((a, b) => { sum = a + b; }, 5, 10, TimeSpan.FromMilliseconds(1), 3);
            
            if (sum != 15)
                throw new Exception("Test_Execute_Action_TwoArgs failed");
            Console.WriteLine("  ✓ Test_Execute_Action_TwoArgs passed");
        }
        
        private static void Test_Execute_Action_ThreeArgs()
        {
            string concat = "";
            SugarRetry.Execute<string, string, string>((a, b, c) => { concat = a + b + c; }, 
                "a", "b", "c", TimeSpan.FromMilliseconds(1), 3);
            
            if (concat != "abc")
                throw new Exception("Test_Execute_Action_ThreeArgs failed");
            Console.WriteLine("  ✓ Test_Execute_Action_ThreeArgs passed");
        }
        
        private static void Test_Execute_Action_FourArgs()
        {
            int product = 0;
            SugarRetry.Execute<int, int, int, int>((a, b, c, d) => { product = a * b * c * d; }, 
                2, 3, 4, 5, TimeSpan.FromMilliseconds(1), 3);
            
            if (product != 120)
                throw new Exception("Test_Execute_Action_FourArgs failed");
            Console.WriteLine("  ✓ Test_Execute_Action_FourArgs passed");
        }
        
        #endregion

        #region Execute Func Tests
        
        private static void Test_Execute_Func_Success()
        {
            var result = SugarRetry.Execute(() => 42, TimeSpan.FromMilliseconds(1), 3);
            
            if (result != 42)
                throw new Exception("Test_Execute_Func_Success failed");
            Console.WriteLine("  ✓ Test_Execute_Func_Success passed");
        }
        
        private static void Test_Execute_Func_RetryOnce()
        {
            int attempts = 0;
            var result = SugarRetry.Execute(() => 
            {
                attempts++;
                if (attempts < 2) throw new Exception("Retry");
                return "success";
            }, TimeSpan.FromMilliseconds(1), 3);
            
            if (result != "success" || attempts != 2)
                throw new Exception("Test_Execute_Func_RetryOnce failed");
            Console.WriteLine("  ✓ Test_Execute_Func_RetryOnce passed");
        }
        
        private static void Test_Execute_Func_RetryMultiple()
        {
            int attempts = 0;
            var result = SugarRetry.Execute(() => 
            {
                attempts++;
                if (attempts < 4) throw new Exception("Retry");
                return 100;
            }, TimeSpan.FromMilliseconds(1), 5);
            
            if (result != 100 || attempts != 4)
                throw new Exception("Test_Execute_Func_RetryMultiple failed");
            Console.WriteLine("  ✓ Test_Execute_Func_RetryMultiple passed");
        }
        
        private static void Test_Execute_Func_AllFail()
        {
            try
            {
                SugarRetry.Execute<int>(() => throw new Exception("Fail"), TimeSpan.FromMilliseconds(1), 3);
                throw new Exception("Should have thrown");
            }
            catch (AggregateException)
            {
                // Expected
            }
            Console.WriteLine("  ✓ Test_Execute_Func_AllFail passed");
        }
        
        private static void Test_Execute_Func_ReturnsValue()
        {
            var list = SugarRetry.Execute(() => new List<int> { 1, 2, 3 }, TimeSpan.FromMilliseconds(1), 3);
            
            if (list.Count != 3)
                throw new Exception("Test_Execute_Func_ReturnsValue failed");
            Console.WriteLine("  ✓ Test_Execute_Func_ReturnsValue passed");
        }
        
        #endregion

        #region Execute Func with Args Tests
        
        private static void Test_Execute_Func_OneArg()
        {
            var result = SugarRetry.Execute<int, int>((x) => x * 2, 5, TimeSpan.FromMilliseconds(1), 3);
            
            if (result != 10)
                throw new Exception("Test_Execute_Func_OneArg failed");
            Console.WriteLine("  ✓ Test_Execute_Func_OneArg passed");
        }
        
        private static void Test_Execute_Func_TwoArgs()
        {
            var result = SugarRetry.Execute<int, int, int>((a, b) => a + b, 10, 20, TimeSpan.FromMilliseconds(1), 3);
            
            if (result != 30)
                throw new Exception("Test_Execute_Func_TwoArgs failed");
            Console.WriteLine("  ✓ Test_Execute_Func_TwoArgs passed");
        }
        
        private static void Test_Execute_Func_ThreeArgs()
        {
            var result = SugarRetry.Execute<string, string, string, string>(
                (a, b, c) => $"{a}-{b}-{c}", "x", "y", "z", TimeSpan.FromMilliseconds(1), 3);
            
            if (result != "x-y-z")
                throw new Exception("Test_Execute_Func_ThreeArgs failed");
            Console.WriteLine("  ✓ Test_Execute_Func_ThreeArgs passed");
        }
        
        private static void Test_Execute_Func_FourArgs()
        {
            var result = SugarRetry.Execute<int, int, int, int, int>(
                (a, b, c, d) => a + b + c + d, 1, 2, 3, 4, TimeSpan.FromMilliseconds(1), 3);
            
            if (result != 10)
                throw new Exception("Test_Execute_Func_FourArgs failed");
            Console.WriteLine("  ✓ Test_Execute_Func_FourArgs passed");
        }
        
        #endregion

        #region Retry Interval Tests
        
        private static void Test_RetryInterval_Zero()
        {
            int attempts = 0;
            SugarRetry.Execute(() => 
            {
                attempts++;
                if (attempts < 2) throw new Exception("Retry");
            }, TimeSpan.Zero, 3);
            
            if (attempts != 2)
                throw new Exception("Test_RetryInterval_Zero failed");
            Console.WriteLine("  ✓ Test_RetryInterval_Zero passed");
        }
        
        private static void Test_RetryInterval_Short()
        {
            int attempts = 0;
            var start = DateTime.Now;
            SugarRetry.Execute(() => 
            {
                attempts++;
                if (attempts < 3) throw new Exception("Retry");
            }, TimeSpan.FromMilliseconds(10), 3);
            var elapsed = DateTime.Now - start;
            
            if (attempts != 3)
                throw new Exception("Test_RetryInterval_Short failed");
            Console.WriteLine("  ✓ Test_RetryInterval_Short passed");
        }
        
        private static void Test_RetryInterval_Custom()
        {
            int attempts = 0;
            SugarRetry.Execute(() => 
            {
                attempts++;
                if (attempts < 2) throw new Exception("Retry");
            }, TimeSpan.FromMilliseconds(5), 3);
            
            if (attempts != 2)
                throw new Exception("Test_RetryInterval_Custom failed");
            Console.WriteLine("  ✓ Test_RetryInterval_Custom passed");
        }
        
        #endregion

        #region Retry Count Tests
        
        private static void Test_RetryCount_One()
        {
            int attempts = 0;
            try
            {
                SugarRetry.Execute(() => 
                {
                    attempts++;
                    throw new Exception("Fail");
                }, TimeSpan.FromMilliseconds(1), 1);
            }
            catch (AggregateException)
            {
                if (attempts != 1)
                    throw new Exception("Test_RetryCount_One failed");
            }
            Console.WriteLine("  ✓ Test_RetryCount_One passed");
        }
        
        private static void Test_RetryCount_Five()
        {
            int attempts = 0;
            try
            {
                SugarRetry.Execute(() => 
                {
                    attempts++;
                    throw new Exception("Fail");
                }, TimeSpan.FromMilliseconds(1), 5);
            }
            catch (AggregateException)
            {
                if (attempts != 5)
                    throw new Exception("Test_RetryCount_Five failed");
            }
            Console.WriteLine("  ✓ Test_RetryCount_Five passed");
        }
        
        private static void Test_RetryCount_Default()
        {
            int attempts = 0;
            try
            {
                SugarRetry.Execute(() => 
                {
                    attempts++;
                    throw new Exception("Fail");
                }, TimeSpan.FromMilliseconds(1));
            }
            catch (AggregateException)
            {
                if (attempts != 3) // default is 3
                    throw new Exception("Test_RetryCount_Default failed");
            }
            Console.WriteLine("  ✓ Test_RetryCount_Default passed");
        }
        
        #endregion

        #region Exception Tests
        
        private static void Test_AggregateException_ContainsAll()
        {
            try
            {
                int i = 0;
                SugarRetry.Execute(() => 
                {
                    i++;
                    throw new Exception($"Error {i}");
                }, TimeSpan.FromMilliseconds(1), 3);
            }
            catch (AggregateException ex)
            {
                if (!ex.InnerExceptions.Any(e => e.Message == "Error 1") ||
                    !ex.InnerExceptions.Any(e => e.Message == "Error 2") ||
                    !ex.InnerExceptions.Any(e => e.Message == "Error 3"))
                    throw new Exception("Test_AggregateException_ContainsAll failed");
            }
            Console.WriteLine("  ✓ Test_AggregateException_ContainsAll passed");
        }
        
        private static void Test_AggregateException_Count()
        {
            try
            {
                SugarRetry.Execute(() => throw new Exception("Fail"), TimeSpan.FromMilliseconds(1), 4);
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Count != 4)
                    throw new Exception("Test_AggregateException_Count failed");
            }
            Console.WriteLine("  ✓ Test_AggregateException_Count passed");
        }
        
        private static void Test_DifferentExceptionTypes()
        {
            int i = 0;
            try
            {
                SugarRetry.Execute(() => 
                {
                    i++;
                    if (i == 1) throw new InvalidOperationException("Op");
                    if (i == 2) throw new ArgumentException("Arg");
                    throw new Exception("Gen");
                }, TimeSpan.FromMilliseconds(1), 3);
            }
            catch (AggregateException ex)
            {
                if (!(ex.InnerExceptions[0] is InvalidOperationException) ||
                    !(ex.InnerExceptions[1] is ArgumentException))
                    throw new Exception("Test_DifferentExceptionTypes failed");
            }
            Console.WriteLine("  ✓ Test_DifferentExceptionTypes passed");
        }
        
        #endregion

        #region Edge Cases
        
        private static void Test_SuccessOnLastRetry()
        {
            int attempts = 0;
            var result = SugarRetry.Execute(() => 
            {
                attempts++;
                if (attempts < 3) throw new Exception("Retry");
                return "final";
            }, TimeSpan.FromMilliseconds(1), 3);
            
            if (result != "final" || attempts != 3)
                throw new Exception("Test_SuccessOnLastRetry failed");
            Console.WriteLine("  ✓ Test_SuccessOnLastRetry passed");
        }
        
        private static void Test_NullReturnValue()
        {
            var result = SugarRetry.Execute<string>(() => null, TimeSpan.FromMilliseconds(1), 3);
            
            if (result != null)
                throw new Exception("Test_NullReturnValue failed");
            Console.WriteLine("  ✓ Test_NullReturnValue passed");
        }
        
        private static void Test_ComplexReturnType()
        {
            var result = SugarRetry.Execute(() => new Dictionary<string, List<int>>
            {
                { "key", new List<int> { 1, 2, 3 } }
            }, TimeSpan.FromMilliseconds(1), 3);
            
            if (result["key"].Count != 3)
                throw new Exception("Test_ComplexReturnType failed");
            Console.WriteLine("  ✓ Test_ComplexReturnType passed");
        }
        
        #endregion
    }
}

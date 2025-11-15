using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    /// <summary>
    /// Security Parameter Handling Test Suite
    /// Tests SQL injection prevention, parameter escaping, and special character handling
    /// Ensures that SqlSugar properly sanitizes and parameterizes user input to prevent SQL injection attacks
    /// </summary>
    public partial class NewUnitTest
    {
        /// <summary>
        /// Main entry point for security parameter handling tests
        /// Executes all 4 test functions covering 20+ attack vectors
        /// </summary>
        public static void SecurityParameterHandling()
        {
            Console.WriteLine("=== Starting Security Parameter Handling Tests ===");
            
            // Test 1: Basic SQL injection attack vectors
            TestBasicSQLInjectionVectors();
            
            // Test 2: Advanced SQL injection techniques
            TestAdvancedSQLInjectionTechniques();
            
            // Test 3: Special character escaping and handling
            TestSpecialCharacterEscaping();
            
            // Test 4: Parameter validation and edge cases
            TestParameterValidationEdgeCases();
            
            Console.WriteLine("=== All Security Parameter Handling Tests Passed ===");
        }

        /// <summary>
        /// Test Function 1: Basic SQL Injection Attack Vectors
        /// Tests common SQL injection patterns including:
        /// - Single quote injection
        /// - Comment-based injection
        /// - UNION-based injection
        /// - Boolean-based blind injection
        /// - Tautology-based injection
        /// </summary>
        private static void TestBasicSQLInjectionVectors()
        {
            Console.WriteLine("\n--- Test 1: Basic SQL Injection Vectors ---");
            var db = Db;
            
            // Initialize test table
            db.CodeFirst.InitTables<SecurityTestEntity>();
            db.DbMaintenance.TruncateTable<SecurityTestEntity>();
            
            // Insert test data
            var testData = new List<SecurityTestEntity>
            {
                new SecurityTestEntity { Id = 1, Username = "admin", Password = "password123", Email = "admin@test.com" },
                new SecurityTestEntity { Id = 2, Username = "user1", Password = "pass456", Email = "user1@test.com" },
                new SecurityTestEntity { Id = 3, Username = "user2", Password = "pass789", Email = "user2@test.com" }
            };
            db.Insertable(testData).ExecuteCommand();

            // Attack Vector 1: Single quote injection attempt
            // Malicious input: ' OR '1'='1
            string maliciousUsername1 = "' OR '1'='1";
            var result1 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == maliciousUsername1)
                .ToList();
            if (result1.Count != 0)
            {
                throw new Exception("Security Test Failed: Single quote injection not properly handled");
            }
            Console.WriteLine("✓ Attack Vector 1 Blocked: Single quote injection");

            // Attack Vector 2: Comment-based injection
            // Malicious input: admin'--
            string maliciousUsername2 = "admin'--";
            var result2 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == maliciousUsername2)
                .ToList();
            if (result2.Count != 0)
            {
                throw new Exception("Security Test Failed: Comment-based injection not properly handled");
            }
            Console.WriteLine("✓ Attack Vector 2 Blocked: Comment-based injection");

            // Attack Vector 3: UNION-based injection
            // Malicious input: ' UNION SELECT NULL, NULL, NULL--
            string maliciousUsername3 = "' UNION SELECT NULL, NULL, NULL--";
            var result3 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == maliciousUsername3)
                .ToList();
            if (result3.Count != 0)
            {
                throw new Exception("Security Test Failed: UNION-based injection not properly handled");
            }
            Console.WriteLine("✓ Attack Vector 3 Blocked: UNION-based injection");

            // Attack Vector 4: Boolean-based blind injection
            // Malicious input: ' OR 1=1--
            string maliciousUsername4 = "' OR 1=1--";
            var result4 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == maliciousUsername4)
                .ToList();
            if (result4.Count != 0)
            {
                throw new Exception("Security Test Failed: Boolean-based injection not properly handled");
            }
            Console.WriteLine("✓ Attack Vector 4 Blocked: Boolean-based blind injection");

            // Attack Vector 5: Tautology-based injection
            // Malicious input: admin' OR 'a'='a
            string maliciousUsername5 = "admin' OR 'a'='a";
            var result5 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == maliciousUsername5)
                .ToList();
            if (result5.Count != 0)
            {
                throw new Exception("Security Test Failed: Tautology-based injection not properly handled");
            }
            Console.WriteLine("✓ Attack Vector 5 Blocked: Tautology-based injection");

            // Attack Vector 6: Stacked queries injection
            // Malicious input: '; DROP TABLE SecurityTestEntity--
            string maliciousUsername6 = "'; DROP TABLE SecurityTestEntity--";
            var result6 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == maliciousUsername6)
                .ToList();
            if (result6.Count != 0)
            {
                throw new Exception("Security Test Failed: Stacked queries injection not properly handled");
            }
            // Verify table still exists
            var tableExists = db.DbMaintenance.IsAnyTable("SecurityTestEntity", false);
            if (!tableExists)
            {
                throw new Exception("Security Test Failed: Table was dropped by injection attack");
            }
            Console.WriteLine("✓ Attack Vector 6 Blocked: Stacked queries injection");

            db.DbMaintenance.TruncateTable<SecurityTestEntity>();
            Console.WriteLine("Test 1 Completed Successfully\n");
        }

        /// <summary>
        /// Test Function 2: Advanced SQL Injection Techniques
        /// Tests sophisticated attack patterns including:
        /// - Time-based blind injection
        /// - Error-based injection
        /// - Second-order injection
        /// - Encoded injection attempts
        /// - Hex encoding attacks
        /// </summary>
        private static void TestAdvancedSQLInjectionTechniques()
        {
            Console.WriteLine("\n--- Test 2: Advanced SQL Injection Techniques ---");
            var db = Db;
            
            db.CodeFirst.InitTables<SecurityTestEntity>();
            db.DbMaintenance.TruncateTable<SecurityTestEntity>();
            
            var testData = new List<SecurityTestEntity>
            {
                new SecurityTestEntity { Id = 1, Username = "testuser", Password = "testpass", Email = "test@example.com" }
            };
            db.Insertable(testData).ExecuteCommand();

            // Attack Vector 7: Time-based blind injection
            // Malicious input: '; WAITFOR DELAY '00:00:05'--
            string maliciousInput7 = "'; WAITFOR DELAY '00:00:05'--";
            var startTime = DateTime.Now;
            var result7 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == maliciousInput7)
                .ToList();
            var elapsed = (DateTime.Now - startTime).TotalSeconds;
            if (elapsed > 3 || result7.Count != 0)
            {
                throw new Exception("Security Test Failed: Time-based injection not properly handled");
            }
            Console.WriteLine("✓ Attack Vector 7 Blocked: Time-based blind injection");

            // Attack Vector 8: Error-based injection
            // Malicious input: ' AND 1=CONVERT(int, (SELECT @@version))--
            string maliciousInput8 = "' AND 1=CONVERT(int, (SELECT @@version))--";
            var result8 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == maliciousInput8)
                .ToList();
            if (result8.Count != 0)
            {
                throw new Exception("Security Test Failed: Error-based injection not properly handled");
            }
            Console.WriteLine("✓ Attack Vector 8 Blocked: Error-based injection");

            // Attack Vector 9: Second-order injection (stored malicious data)
            // Insert malicious data and then query it
            string maliciousData9 = "'; DELETE FROM SecurityTestEntity--";
            db.Insertable(new SecurityTestEntity 
            { 
                Id = 2, 
                Username = maliciousData9, 
                Password = "pass", 
                Email = "mal@test.com" 
            }).ExecuteCommand();
            
            var storedData = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == maliciousData9)
                .First();
            if (storedData == null || storedData.Username != maliciousData9)
            {
                throw new Exception("Security Test Failed: Second-order injection data not stored correctly");
            }
            Console.WriteLine("✓ Attack Vector 9 Blocked: Second-order injection");

            // Attack Vector 10: URL encoded injection
            // Malicious input: %27%20OR%20%271%27%3D%271 (decoded: ' OR '1'='1)
            string maliciousInput10 = "%27%20OR%20%271%27%3D%271";
            var result10 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == maliciousInput10)
                .ToList();
            if (result10.Count != 0)
            {
                throw new Exception("Security Test Failed: URL encoded injection not properly handled");
            }
            Console.WriteLine("✓ Attack Vector 10 Blocked: URL encoded injection");

            // Attack Vector 11: Hex encoding attack
            // Malicious input: 0x27204F52202731273D2731 (hex for ' OR '1'='1)
            string maliciousInput11 = "0x27204F52202731273D2731";
            var result11 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == maliciousInput11)
                .ToList();
            if (result11.Count != 0)
            {
                throw new Exception("Security Test Failed: Hex encoding attack not properly handled");
            }
            Console.WriteLine("✓ Attack Vector 11 Blocked: Hex encoding attack");

            // Attack Vector 12: Batch query injection
            // Malicious input: '; SELECT * FROM SecurityTestEntity; --
            string maliciousInput12 = "'; SELECT * FROM SecurityTestEntity; --";
            var result12 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == maliciousInput12)
                .ToList();
            if (result12.Count != 0)
            {
                throw new Exception("Security Test Failed: Batch query injection not properly handled");
            }
            Console.WriteLine("✓ Attack Vector 12 Blocked: Batch query injection");

            db.DbMaintenance.TruncateTable<SecurityTestEntity>();
            Console.WriteLine("Test 2 Completed Successfully\n");
        }

        /// <summary>
        /// Test Function 3: Special Character Escaping and Handling
        /// Tests proper handling of special characters including:
        /// - Quotes (single and double)
        /// - Backslashes
        /// - Null bytes
        /// - Unicode characters
        /// - Control characters
        /// - Wildcards
        /// </summary>
        private static void TestSpecialCharacterEscaping()
        {
            Console.WriteLine("\n--- Test 3: Special Character Escaping ---");
            var db = Db;
            
            db.CodeFirst.InitTables<SecurityTestEntity>();
            db.DbMaintenance.TruncateTable<SecurityTestEntity>();

            // Attack Vector 13: Multiple single quotes
            string specialChar13 = "test''user''name";
            db.Insertable(new SecurityTestEntity 
            { 
                Id = 1, 
                Username = specialChar13, 
                Password = "pass", 
                Email = "test@test.com" 
            }).ExecuteCommand();
            var result13 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == specialChar13)
                .First();
            if (result13 == null || result13.Username != specialChar13)
            {
                throw new Exception("Security Test Failed: Multiple single quotes not handled correctly");
            }
            Console.WriteLine("✓ Attack Vector 13 Handled: Multiple single quotes");

            // Attack Vector 14: Backslashes
            string specialChar14 = "test\\user\\name";
            db.Insertable(new SecurityTestEntity 
            { 
                Id = 2, 
                Username = specialChar14, 
                Password = "pass", 
                Email = "test@test.com" 
            }).ExecuteCommand();
            var result14 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == specialChar14)
                .First();
            if (result14 == null || result14.Username != specialChar14)
            {
                throw new Exception("Security Test Failed: Backslashes not handled correctly");
            }
            Console.WriteLine("✓ Attack Vector 14 Handled: Backslashes");

            // Attack Vector 15: Double quotes
            string specialChar15 = "test\"user\"name";
            db.Insertable(new SecurityTestEntity 
            { 
                Id = 3, 
                Username = specialChar15, 
                Password = "pass", 
                Email = "test@test.com" 
            }).ExecuteCommand();
            var result15 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == specialChar15)
                .First();
            if (result15 == null || result15.Username != specialChar15)
            {
                throw new Exception("Security Test Failed: Double quotes not handled correctly");
            }
            Console.WriteLine("✓ Attack Vector 15 Handled: Double quotes");

            // Attack Vector 16: Percent sign (LIKE wildcard)
            string specialChar16 = "test%user%name";
            db.Insertable(new SecurityTestEntity 
            { 
                Id = 4, 
                Username = specialChar16, 
                Password = "pass", 
                Email = "test@test.com" 
            }).ExecuteCommand();
            var result16 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == specialChar16)
                .First();
            if (result16 == null || result16.Username != specialChar16)
            {
                throw new Exception("Security Test Failed: Percent signs not handled correctly");
            }
            Console.WriteLine("✓ Attack Vector 16 Handled: Percent signs (wildcards)");

            // Attack Vector 17: Underscore (LIKE wildcard)
            string specialChar17 = "test_user_name";
            db.Insertable(new SecurityTestEntity 
            { 
                Id = 5, 
                Username = specialChar17, 
                Password = "pass", 
                Email = "test@test.com" 
            }).ExecuteCommand();
            var result17 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == specialChar17)
                .First();
            if (result17 == null || result17.Username != specialChar17)
            {
                throw new Exception("Security Test Failed: Underscores not handled correctly");
            }
            Console.WriteLine("✓ Attack Vector 17 Handled: Underscores (wildcards)");

            // Attack Vector 18: Unicode characters
            string specialChar18 = "测试用户名";
            db.Insertable(new SecurityTestEntity 
            { 
                Id = 6, 
                Username = specialChar18, 
                Password = "pass", 
                Email = "test@test.com" 
            }).ExecuteCommand();
            var result18 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == specialChar18)
                .First();
            if (result18 == null || result18.Username != specialChar18)
            {
                throw new Exception("Security Test Failed: Unicode characters not handled correctly");
            }
            Console.WriteLine("✓ Attack Vector 18 Handled: Unicode characters");

            // Attack Vector 19: Semicolons
            string specialChar19 = "test;user;name";
            db.Insertable(new SecurityTestEntity 
            { 
                Id = 7, 
                Username = specialChar19, 
                Password = "pass", 
                Email = "test@test.com" 
            }).ExecuteCommand();
            var result19 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == specialChar19)
                .First();
            if (result19 == null || result19.Username != specialChar19)
            {
                throw new Exception("Security Test Failed: Semicolons not handled correctly");
            }
            Console.WriteLine("✓ Attack Vector 19 Handled: Semicolons");

            db.DbMaintenance.TruncateTable<SecurityTestEntity>();
            Console.WriteLine("Test 3 Completed Successfully\n");
        }

        /// <summary>
        /// Test Function 4: Parameter Validation and Edge Cases
        /// Tests edge cases and boundary conditions including:
        /// - Null values
        /// - Empty strings
        /// - Very long strings
        /// - Mixed attack vectors
        /// - Case sensitivity
        /// </summary>
        private static void TestParameterValidationEdgeCases()
        {
            Console.WriteLine("\n--- Test 4: Parameter Validation Edge Cases ---");
            var db = Db;
            
            db.CodeFirst.InitTables<SecurityTestEntity>();
            db.DbMaintenance.TruncateTable<SecurityTestEntity>();

            // Attack Vector 20: Empty string
            string emptyString = "";
            db.Insertable(new SecurityTestEntity 
            { 
                Id = 1, 
                Username = emptyString, 
                Password = "pass", 
                Email = "test@test.com" 
            }).ExecuteCommand();
            var result20 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == emptyString)
                .First();
            if (result20 == null || result20.Username != emptyString)
            {
                throw new Exception("Security Test Failed: Empty string not handled correctly");
            }
            Console.WriteLine("✓ Attack Vector 20 Handled: Empty string");

            // Attack Vector 21: Very long string with injection attempt
            string longString = new string('a', 1000) + "' OR '1'='1";
            db.Insertable(new SecurityTestEntity 
            { 
                Id = 2, 
                Username = longString, 
                Password = "pass", 
                Email = "test@test.com" 
            }).ExecuteCommand();
            var result21 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == longString)
                .First();
            if (result21 == null || result21.Username != longString)
            {
                throw new Exception("Security Test Failed: Very long string not handled correctly");
            }
            Console.WriteLine("✓ Attack Vector 21 Handled: Very long string with injection");

            // Attack Vector 22: Mixed case SQL keywords
            string mixedCase = "' Or '1'='1";
            var result22 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == mixedCase)
                .ToList();
            if (result22.Count != 0)
            {
                throw new Exception("Security Test Failed: Mixed case SQL keywords not handled correctly");
            }
            Console.WriteLine("✓ Attack Vector 22 Blocked: Mixed case SQL keywords");

            // Attack Vector 23: Multiple attack vectors combined
            string combined = "'; DROP TABLE SecurityTestEntity; SELECT * FROM SecurityTestEntity WHERE '1'='1";
            var result23 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == combined)
                .ToList();
            if (result23.Count != 0)
            {
                throw new Exception("Security Test Failed: Combined attack vectors not handled correctly");
            }
            var tableStillExists = db.DbMaintenance.IsAnyTable("SecurityTestEntity", false);
            if (!tableStillExists)
            {
                throw new Exception("Security Test Failed: Table was affected by combined attack");
            }
            Console.WriteLine("✓ Attack Vector 23 Blocked: Multiple combined attack vectors");

            // Attack Vector 24: Whitespace variations
            string whitespace = "   ' OR '1'='1   ";
            var result24 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == whitespace)
                .ToList();
            if (result24.Count != 0)
            {
                throw new Exception("Security Test Failed: Whitespace variations not handled correctly");
            }
            Console.WriteLine("✓ Attack Vector 24 Blocked: Whitespace variations");

            // Attack Vector 25: Line breaks and tabs
            string lineBreaks = "test\r\n\t' OR '1'='1";
            db.Insertable(new SecurityTestEntity 
            { 
                Id = 3, 
                Username = lineBreaks, 
                Password = "pass", 
                Email = "test@test.com" 
            }).ExecuteCommand();
            var result25 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username == lineBreaks)
                .First();
            if (result25 == null || result25.Username != lineBreaks)
            {
                throw new Exception("Security Test Failed: Line breaks and tabs not handled correctly");
            }
            Console.WriteLine("✓ Attack Vector 25 Handled: Line breaks and tabs");

            // Attack Vector 26: Parameter with LIKE pattern injection
            string likePattern = "%' OR '1'='1' OR username LIKE '%";
            var result26 = db.Queryable<SecurityTestEntity>()
                .Where(x => x.Username.Contains(likePattern))
                .ToList();
            // Should not return all records
            if (result26.Count > 1)
            {
                throw new Exception("Security Test Failed: LIKE pattern injection not handled correctly");
            }
            Console.WriteLine("✓ Attack Vector 26 Blocked: LIKE pattern injection");

            db.DbMaintenance.TruncateTable<SecurityTestEntity>();
            Console.WriteLine("Test 4 Completed Successfully\n");
        }

        /// <summary>
        /// Test entity class for security testing
        /// Represents a typical user table with common fields that are often targeted in SQL injection attacks
        /// </summary>
        [SugarTable("SecurityTestEntity")]
        public class SecurityTestEntity
        {
            /// <summary>
            /// Primary key identifier
            /// </summary>
            [SugarColumn(IsPrimaryKey = true)]
            public int Id { get; set; }

            /// <summary>
            /// Username field - commonly targeted for SQL injection
            /// </summary>
            [SugarColumn(Length = 2000)]
            public string Username { get; set; }

            /// <summary>
            /// Password field - sensitive data that must be protected
            /// </summary>
            [SugarColumn(Length = 200)]
            public string Password { get; set; }

            /// <summary>
            /// Email field - another common injection target
            /// </summary>
            [SugarColumn(Length = 200)]
            public string Email { get; set; }
        }
    }
}

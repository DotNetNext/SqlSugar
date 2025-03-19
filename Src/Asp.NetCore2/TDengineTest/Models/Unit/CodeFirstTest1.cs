using SqlSugar.DbConvert;
using SqlSugar.TDengine;
using System;
using System.Collections.Generic;
using System.Text;

namespace TDengineTest
{
  
    public class CodeFirst0311 : STable
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public DateTime Ts { get; set; }
        public bool Boolean { get; set; } 
        public byte Byte { get; set; } 
        public sbyte SByte { get; set; }
        public char Char { get; set; }
        public decimal Decimal { get; set; } 
        public decimal Decimal2 { get; set; } 
        public double Double { get; set; } 
        public float Float { get; set; }
        public int Int32 { get; set; } 
        public uint UInt32 { get; set; }
        public long Int64 { get; set; } 
        public ulong UInt64 { get; set; }
        public short Int16 { get; set; } 
        public ushort UInt16 { get; set; }
        public string String { get; set; }
        [SqlSugar.SugarColumn(Length = 100)]
        public string String2 { get; set; }
    }

    public class CodeFirst01:STable
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public DateTime Ts { get; set; }
        public bool Boolean { get; set; }
    }
    [STableAttribute( STableName = "CodeFirstStable", Tags="[{ Name:\"Tag1\",Value:\"1\"}]")]
    public class CodeFirstTags44 
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public DateTime Ts { get; set; }
        public bool Boolean { get; set; }
        [SqlSugar.SugarColumn(IsIgnore =true)]
        public string Tag1 { get; set; }
    }
    [STableAttribute(STableName = "CodeFirstStable", Tags = "[{ Name:\"Tag1\",Value:\"2\"}]")]
    public class CodeFirstTags33
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public DateTime Ts { get; set; }
        public bool Boolean { get; set; }
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string Tag1 { get; set; }
    }
    public class CodeFirst04  
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public DateTime Ts { get; set; }
        public bool Boolean { get; set; }
    }
    public class CodeFirst05
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public DateTime Ts { get; set; }
        public bool Boolean { get; set; }
    }
}
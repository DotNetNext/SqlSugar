using System;
using System.Collections.Generic;
using System.Text;

namespace TDengineTest
{
    public class AllCSharpTypes
    {
        public bool Boolean { get; set; }
        public byte Byte { get; set; }
        public sbyte SByte { get; set; }
        public char Char { get; set; }
        public decimal Decimal { get; set; }
        [SqlSugar.SugarColumn(Length =18,DecimalDigits =2)]
        public decimal Decimal2 { get; set; }
        [SqlSugar.SugarColumn(Length = 18, DecimalDigits = 2)]
        public double Double { get; set; }
        public float Float { get; set; }
        public int Int32 { get; set; }
        public uint UInt32 { get; set; }
        public long Int64 { get; set; }
        public ulong UInt64 { get; set; }
        public short Int16 { get; set; }
        public ushort UInt16 { get; set; } 
        public string String { get; set; }
        [SqlSugar.SugarColumn(Length =100)]
        public string String2 { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public enum CSharpDataType
    {
        @int,
        @uint,
        @bool,
        @string,
        @DateTime,
        @decimal,
        @double,
        @Guid,
        @byte,
        @sbyte,
        @enum,
        @short,
        @ushort,
        @long,
        @ulong,
        @object,
        @other,
        @byteArray,
        @float,
        @time,
        @DateTimeOffset,
        @Single,
	    @TimeSpan,
        @char,
        @NpgsqlBox,
        @NpgsqlCircle,
        @NpgsqlLine,
        @NpgsqlLseg,
        @NpgsqlPath,
        @NpgsqlPoint,
        @NpgsqlPolygon,
    }
}

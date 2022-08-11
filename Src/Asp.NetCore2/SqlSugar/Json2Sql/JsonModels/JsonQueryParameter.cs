using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar
{
    internal class JsonQueryParameter
    {
        public bool IsSelect { get; set; }
        public bool IsJoin { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; } = 20;
        public bool JoinNoSelect { get { return IsJoin && !IsSelect; } }

        public bool IsPage { get { return PageIndex != null; } }
    }
}

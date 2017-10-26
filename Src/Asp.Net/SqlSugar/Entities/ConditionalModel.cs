using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class ConditionalModel
    {
        public ConditionalModel()
        {
            this.ConditionalType = ConditionalType.Equal;
        }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public ConditionalType ConditionalType { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public interface IConditionalModel {

    }
    public class ConditionalCollections : IConditionalModel
    {
         public List<KeyValuePair<WhereType, ConditionalModel>> ConditionalList { get; set; }
    }
    public class ConditionalTree : IConditionalModel
    {
        public List<KeyValuePair<WhereType, IConditionalModel>> ConditionalList { get; set; }
    }
    public class ConditionalModel: IConditionalModel
    {
        public ConditionalModel()
        {
            this.ConditionalType = ConditionalType.Equal;
        }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public string CSharpTypeName { get; set; }
        

        public ICustomConditionalFunc CustomConditionalFunc { get; set; }
        public string CustomP1 { get; set; }
        public string CustomP2 { get; set; }
        public string CustomP3 { get; set; }
        public string CustomP4 { get; set; }

        public ConditionalType ConditionalType { get; set; }
        [Newtonsoft.Json.JsonIgnoreAttribute]
        public Func<string,object> FieldValueConvertFunc { get; set; }
    }
}

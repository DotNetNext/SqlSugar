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
 
    public class ConditionalModel: IConditionalModel
    {
        public ConditionalModel()
        {
            this.ConditionalType = ConditionalType.Equal;
        }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public ConditionalType ConditionalType { get; set; }
        [System.Text.Json.Serialization.JsonIgnoreAttribute]
        [Newtonsoft.Json.JsonIgnoreAttribute]
        public Func<string,object> FieldValueConvertFunc { get; set; }
    }
}

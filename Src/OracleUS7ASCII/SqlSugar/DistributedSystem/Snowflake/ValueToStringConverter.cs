using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
	public class ValueToStringConverter : JsonConverter
	{
		public override bool CanRead => false;

		public override bool CanConvert(Type objectType) => objectType.IsValueType;

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			=> throw new NotSupportedException();

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var str = value?.ToString();
			writer.WriteValue(str);
		}
	}
}

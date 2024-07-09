using Newtonsoft.Json;

namespace SqlSugar.Serialization
{
	internal class JsonNetSerializer : ISerializer
	{
		private readonly JsonSerializerSettings _jsonSerializerSettings;
		public JsonNetSerializer(JsonSerializerSettings jsonSerializerSettings = null)
		{
			_jsonSerializerSettings = jsonSerializerSettings;
		}

		public T Deserialize<T>(string data)
		{
			return JsonConvert.DeserializeObject<T>(data, _jsonSerializerSettings);
		}

		public string Serialize(object obj)
		{
			return JsonConvert.SerializeObject(obj, _jsonSerializerSettings);
		}
	}
}

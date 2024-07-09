using SqlSugar.Serialization;

namespace SqlSugar.Entities
{
	internal class SerializeConfig
	{
		ISerializer JsonSerializer { get; set; }
	}
}

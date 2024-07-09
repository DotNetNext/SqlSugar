using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar.Serialization
{
	public interface ISerializer
	{
		string Serialize(object obj);
		T Deserialize<T>(string data);
	}
}

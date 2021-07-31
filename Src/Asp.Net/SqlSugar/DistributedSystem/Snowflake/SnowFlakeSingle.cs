using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.DistributedSystem.Snowflake
{
	public sealed class SnowFlakeSingle 
	{
		public static readonly SnowFlakeSingle instance = new SnowFlakeSingle();
		private SnowFlakeSingle()
		{
			worker = new Snowflake.IdWorker(1, 1);
		}
		static SnowFlakeSingle() { }
		public static SnowFlakeSingle Instance
		{
			get { return instance; }
		}
		private Snowflake.IdWorker worker;
		public long getID()
		{
			return worker.NextId();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
	public sealed class SnowFlakeSingle 
	{
		public static readonly SnowFlakeSingle instance = new SnowFlakeSingle();
		public static int WorkId = 1;
		public static int DatacenterId = 1;
		private SnowFlakeSingle()
		{
			worker = new DistributedSystem.Snowflake.IdWorker(WorkId, DatacenterId);
		}
		static SnowFlakeSingle() { }
		public static SnowFlakeSingle Instance
		{
			get { return instance; }
		}
		private DistributedSystem.Snowflake.IdWorker worker;
		public long getID()
		{
			return worker.NextId();
		}
	}
}

/** Copyright 2010-2012 Twitter, Inc.*/

/**
 * An object that generates IDs.
 * This is broken into a separate class in case
 * we ever want to support multiple worker threads
 * per process
 */

using System;

namespace SqlSugar.DistributedSystem.Snowflake
{
    public class IdWorker
    {
        public const long Twepoch = 1288834974657L;

        const int WorkerIdBits = 5;
        const int DatacenterIdBits = 5;
        const int SequenceBits = 12;
        const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
        const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits);

        private const int WorkerIdShift = SequenceBits;
        private const int DatacenterIdShift = SequenceBits + WorkerIdBits;
        public const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits;
        private const long SequenceMask = -1L ^ (-1L << SequenceBits);

        private long _sequence = 0L;
        private long _lastTimestamp = -1L;
	
	
        public IdWorker(long workerId, long datacenterId, long sequence = 0L) 
        {
            WorkerId = workerId;
            DatacenterId = datacenterId;
            _sequence = sequence;
		
            // sanity check for workerId
            if (workerId > MaxWorkerId || workerId < 0) 
            {
                throw new ArgumentException( String.Format("worker Id can't be greater than {0} or less than 0", MaxWorkerId) );
            }

            if (datacenterId > MaxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentException( String.Format("datacenter Id can't be greater than {0} or less than 0", MaxDatacenterId));
            }

            //log.info(
            //    String.Format("worker starting. timestamp left shift {0}, datacenter id bits {1}, worker id bits {2}, sequence bits {3}, workerid {4}",
            //                  TimestampLeftShift, DatacenterIdBits, WorkerIdBits, SequenceBits, workerId)
            //    );	
        }
	
        public long WorkerId {get; protected set;}
        public long DatacenterId {get; protected set;}

        public long Sequence
        {
            get { return _sequence; }
            internal set { _sequence = value; }
        }

        // def get_timestamp() = System.currentTimeMillis

        readonly object _lock = new Object();
        public long getID()
        {
            return NextId();
        }
        public virtual long NextId() 
        {
            lock(_lock) 
            {
                var timestamp = TimeGen();

                if (timestamp < _lastTimestamp) 
                {
                    //exceptionCounter.incr(1);
                    //log.Error("clock is moving backwards.  Rejecting requests until %d.", _lastTimestamp);
                    throw new InvalidSystemClock(String.Format(
                        "Clock moved backwards.  Refusing to generate id for {0} milliseconds", _lastTimestamp - timestamp));
                }

                if (_lastTimestamp == timestamp) 
                {
                    _sequence = (_sequence + 1) & SequenceMask;
                    if (_sequence == 0) 
                    {
                        timestamp = TilNextMillis(_lastTimestamp);
                    }
                } else {
                    _sequence = 0;
                }

                _lastTimestamp = timestamp;
                var id = ((timestamp - Twepoch) << TimestampLeftShift) |
                         (DatacenterId << DatacenterIdShift) |
                         (WorkerId << WorkerIdShift) | _sequence;
					
                return id;
            }
        }

        protected virtual long TilNextMillis(long lastTimestamp)
        {
            var timestamp = TimeGen();
            while (timestamp <= lastTimestamp) 
            {
                timestamp = TimeGen();
            }
            return timestamp;
        }

        protected virtual long TimeGen()
        {
            return System.CurrentTimeMillis();
        }      
    }
}
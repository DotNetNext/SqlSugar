using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public static class SugarRetry
    {

        public static void Execute(Action action, TimeSpan retryInterval, int retryCount = 3)
        {
            Execute<object>(() =>
            {
                action();
                return null;
            }, retryInterval, retryCount);
        }

 
        public static void Execute<T1>(Action<T1> action, T1 arg1, TimeSpan retryInterval, int retryCount = 3)
        {
            Execute<T1, object>((x1) =>
            {
                action(arg1);
                return null;
            }, arg1, retryInterval, retryCount);
        }

      
        public static void Execute<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2, TimeSpan retryInterval, int retryCount = 3)
        {
            Execute<T1, T2, object>((x1, x2) =>
            {
                action(arg1, arg2);
                return null;
            }, arg1, arg2, retryInterval, retryCount);
        }

   
        public static void Execute<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, TimeSpan retryInterval, int retryCount = 3)
        {
            Execute<T1, T2, T3, object>((x1, x2, x3) =>
            {
                action(arg1, arg2, arg3);
                return null;
            }, arg1, arg2, arg3, retryInterval, retryCount);
        }

   
        public static void Execute<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, TimeSpan retryInterval, int retryCount = 3)
        {
            Execute<T1, T2, T3, T4, object>((x1, x2, x3, x4) =>
            {
                action(arg1, arg2, arg3, arg4);
                return null;
            }, arg1, arg2, arg3, arg4, retryInterval, retryCount);
        }

    
        public static T Execute<T>(Func<T> func, TimeSpan retryInterval, int retryCount = 3)
        {
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < retryCount; retry++)
            {
                try
                {
                    return func();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    Thread.Sleep(retryInterval);
                }
            }

            throw new AggregateException(exceptions);
        }

     
        public static T Execute<T1, T>(Func<T1, T> func, T1 arg1, TimeSpan retryInterval, int retryCount = 3)
        {
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < retryCount; retry++)
            {
                try
                {
                    return func(arg1);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    Thread.Sleep(retryInterval);
                }
            }

            throw new AggregateException(exceptions);
        }
 
        public static T Execute<T1, T2, T>(Func<T1, T2, T> func, T1 arg1, T2 arg2, TimeSpan retryInterval, int retryCount = 3)
        {
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < retryCount; retry++)
            {
                try
                {
                    return func(arg1, arg2);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    Thread.Sleep(retryInterval);
                }
            }

            throw new AggregateException(exceptions);
        }

 
        public static T Execute<T1, T2, T3, T>(Func<T1, T2, T3, T> func, T1 arg1, T2 arg2, T3 arg3, TimeSpan retryInterval, int retryCount = 3)
        {
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < retryCount; retry++)
            {
                try
                {
                    return func(arg1, arg2, arg3);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    Thread.Sleep(retryInterval);
                }
            }

            throw new AggregateException(exceptions);
        }

 
        public static T Execute<T1, T2, T3, T4, T>(Func<T1, T2, T3, T4, T> func, T1 arg1, T2 arg2, T3 arg3, T4 arg4, TimeSpan retryInterval, int retryCount = 3)
        {
            var exceptions = new List<Exception>();

            for (int retry = 0; retry < retryCount; retry++)
            {
                try
                {
                    return func(arg1, arg2, arg3, arg4);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    Thread.Sleep(retryInterval);
                }
            }

            throw new AggregateException(exceptions);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
namespace SqlSugar
{
    /// <summary>
    /// Taskable扩展类
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// 获取count
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static int Count(this Taskable<int> thisValue)
        {
            return thisValue.Tasks.Select(it => it.Result.Value).Sum();
        }

        /// <summary>
        ///是否存在这条记录
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static bool Any(this Taskable<int> thisValue)
        {
            return thisValue.Tasks.Select(it => it.Result.Value).Count() > 0;
        }

        /// <summary>
        /// 获取最大值 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static T Max<T>(this Taskable<T> thisValue)
        {
            var isClass = typeof(T).IsClass;
            if (isClass)
            {
                Check.Exception(isClass, "TaskExtensions.Max.thisValue T只能是为string和值类型。");
            }
            return thisValue.Tasks.Select(it => it.Result.Value).Max();
        }

        /// <summary>
        /// 获取平均值
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static int Avg(this TaskableWithCount<int> thisValue)
        {
            var count = thisValue.Tasks.Select(it => it.Result.Count).Sum();
            if (count == 0) return 0;
            var reval = thisValue.Tasks.Select(it => it.Result.Value * it.Result.Count).Sum() / count;
            return reval;
        }
        /// <summary>
        /// 获取平均值
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static decimal Avg(this TaskableWithCount<decimal> thisValue)
        {
            var count = thisValue.Tasks.Select(it => it.Result.Count).Sum();
            if (count == 0) return 0;
            var reval = thisValue.Tasks.Select(it => it.Result.Value * it.Result.Count).Sum() / count;
            return reval;
        }
        /// <summary>
        /// 获取平均值
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static double Avg(this TaskableWithCount<double> thisValue)
        {
            var count = thisValue.Tasks.Select(it => it.Result.Count).Sum();
            if (count == 0) return 0;
            var reval = thisValue.Tasks.Select(it => it.Result.Value * it.Result.Count).Sum() / count;
            return reval;
        }


        /// <summary>
        /// 获取最小值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static T Min<T>(this Taskable<T> thisValue)
        {
            var isClass = typeof(T).IsClass;
            if (isClass)
            {
                Check.Exception(isClass, "TaskExtensions.Min.thisValue T只能是为string和值类型。");
            }
            return thisValue.Tasks.Select(it => it.Result.Value).Min();
        }


        /// <summary>
        /// 将Task中的结果集合并成List集成
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this Taskable<T> thisValue) where T : class
        {
            var isClass = typeof(T).IsClass;
            if (!isClass)
            {
                Check.Exception(isClass, "TaskExtensions.ToList.thisValue T只能为class。");
            }
            return thisValue.MergeEntities<T>();
        }


        /// <summary>
        ///  返回序列中的唯一元素；如果该序列为空，此方法将引发异常；如果该序列包含多个元素，此方法将引发异常。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static T ToSingle<T>(this Taskable<T> thisValue) where T : class
        {
            var isClass = typeof(T).IsClass;
            if (!isClass)
            {
                Check.Exception(isClass, "TaskExtensions.ToSingle.thisValue T只能为class。");
            }
            return thisValue.MergeEntities<T>().Single();
        }

        /// <summary>
        ///  返回序列中的唯一元素；如果该序列为空，则返回默认值；如果该序列包含多个元素，此方法将引发异常。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static T ToSingleOrDefault<T>(this Taskable<T> thisValue) where T : class
        {
            var isClass = typeof(T).IsClass;
            if (!isClass)
            {
                Check.Exception(isClass, "TaskExtensions.ToSingle.thisValue T只能为class。");
            }
            return thisValue.MergeEntities<T>().SingleOrDefault();
        }


        /// <summary>
        /// 获取第一行数据,，如果序列中不包含任何元素,则会抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static T ToFirst<T>(this Taskable<T> thisValue) where T : class
        {
            var isClass = typeof(T).IsClass;
            if (!isClass)
            {
                Check.Exception(isClass, "TaskExtensions.ToSingle.thisValue T只能为class。");
            }
            return thisValue.MergeEntities<T>().First();
        }

        /// <summary>
        /// 如果序列中不包含任何元素，则返回默认值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static T ToFirstOrDefault<T>(this Taskable<T> thisValue) where T : class
        {
            var isClass = typeof(T).IsClass;
            if (!isClass)
            {
                Check.Exception(isClass, "TaskExtensions.ToSingle.thisValue T只能为class。");
            }
            return thisValue.MergeEntities<T>().FirstOrDefault();
        }


     
       /// <summary>
        /// 将结果集合并到一个集合
       /// </summary>
       /// <typeparam name="DataTable"></typeparam>
       /// <param name="thisValue"></param>
       /// <returns></returns>
        public static IEnumerable<DataRow> MergeTable<DataTable>(this Taskable<DataTable> thisValue)
        {

            var isDataTable = typeof(System.Data.DataTable) == typeof(DataTable);
            if (!isDataTable)
            {
                Check.Exception(isDataTable, "TaskExtensions.MergeTable.thisValue T只能为DataTable。");
            }
            var reval = thisValue.Tasks.SelectMany(it => it.Result.DataTable.AsEnumerable()).ToList();
            return reval;
        }

        /// <summary>
        /// 将结果集合并到一个集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static List<T> MergeEntities<T>(this Taskable<T> thisValue) where T : class
        {

            var reval = thisValue.Tasks.SelectMany(it => it.Result.Entities).ToList();
            return reval;
        }


        /// <summary>
        /// 将结果集合并到一个集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static List<T> MergeValue<T>(this Taskable<T> thisValue)
        {

            var reval = thisValue.Tasks.Select(it => it.Result.Value).ToList();
            return reval;
        }
    }

}

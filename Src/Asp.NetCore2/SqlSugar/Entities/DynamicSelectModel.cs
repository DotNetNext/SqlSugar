using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class DynamicCoreSelectModel
    {
        public object Value { get; set; }

        public object ToList()
        {
            if (Value is null)
            {
                throw new InvalidOperationException("Value cannot be null.");
            }

            var method = Value.GetType().GetMyMethod("ToList", 0);
            if (method == null)
            {
                throw new InvalidOperationException("The Value object does not have a ToList method with no parameters.");
            }

            return method.Invoke(Value, null);
        }

        public async Task<object> ToListAsync()
        {
            if (Value is null)
            {
                throw new InvalidOperationException("Value cannot be null.");
            }

            var method = Value.GetType().GetMyMethod("ToListAsync", 0);
            if (method == null)
            {
                throw new InvalidOperationException("The Value object does not have a ToListAsync method with no parameters.");
            }

            var task = (Task)method.Invoke(Value, null);
            return await GetTask(task).ConfigureAwait(false);
        }

        public object ToPageList(int pageNumber, int pageSize)
        {
            if (Value is null)
            {
                throw new InvalidOperationException("Value cannot be null.");
            }

            var method = Value.GetType().GetMyMethod("ToPageList", 2);
            if (method == null)
            {
                throw new InvalidOperationException("The Value object does not have a ToPageList method with two parameters.");
            }

            return method.Invoke(Value, new object[] { pageNumber, pageSize });
        }

        public async Task<object> ToPageListAsync(int pageNumber, int pageSize)
        {
            if (Value is null)
            {
                throw new InvalidOperationException("Value cannot be null.");
            }

            var method = Value.GetType().GetMyMethod("ToPageListAsync", 2);
            if (method == null)
            {
                throw new InvalidOperationException("The Value object does not have a ToPageListAsync method with two parameters.");
            }

            var task = (Task)method.Invoke(Value, new object[] { pageNumber, pageSize });
            return await GetTask(task).ConfigureAwait(false);
        }

        public object ToPageList(int pageNumber, int pageSize, ref int totalNumber)
        {
            if (Value is null)
            {
                throw new InvalidOperationException("Value cannot be null.");
            }

            var method = Value.GetType().GetMyMethod("ToPageList", 3);
            if (method == null)
            {
                throw new InvalidOperationException("The Value object does not have a ToPageList method with three parameters.");
            }

            var parameters = new object[] { pageNumber, pageSize, totalNumber };
            var result = method.Invoke(Value, parameters);
            totalNumber = (int)parameters[2];
            return result;
        }

        public async Task<object> ToPageListAsync(int pageNumber, int pageSize, int totalNumber)
        {
            if (Value is null)
            {
                throw new InvalidOperationException("Value cannot be null.");
            }

            var method = Value.GetType().GetMyMethod("ToPageListAsync", 3);
            if (method == null)
            {
                throw new InvalidOperationException("The Value object does not have a ToPageListAsync method with three parameters.");
            }

            var parameters = new object[] { pageNumber, pageSize, totalNumber };
            var task = (Task)method.Invoke(Value, parameters);
            var result = await GetTask(task).ConfigureAwait(false);
            totalNumber = (int)parameters[2];
            return result;
        }

        private static async Task<object> GetTask(Task task)
        {
            await task.ConfigureAwait(false); // 等待任务完成
            var resultProperty = task.GetType().GetProperty("Result");
            var value = resultProperty.GetValue(task); 
            return value;
        }
    }
}

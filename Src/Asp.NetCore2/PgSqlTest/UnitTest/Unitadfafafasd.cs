using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitadfafafasd
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<event_handle_task>();
            event_handle_task model = new event_handle_task();
            db.CodeFirst.InitTables<event_handle_task>();
            var p = model.data_source_type;
       
            var xxx2 = db.Updateable<event_handle_task>().SetColumns(u => new event_handle_task
            {
                data_source_type = p ?? 20,
            })
             .Where(c => c.id == 454206551670915072)
             .ExecuteCommand();
        }
        /// <summary>
        ///event_handle_task
        /// </summary>		
        [Serializable]
        public partial class event_handle_task
        {
            #region Model
            private long _id;
            private int _event_type;
            private int? _data_source_type;
            private string _data_source_id;
            private string _data_source_content;
            private int? _opt_count = 0;
            private int _serial_flag = 0;
            private DateTime? _create_date = DateTime.Now;
            private DateTime? _update_date = DateTime.Now;

            /// <summary>
            /// 
            /// </summary>
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public long id
            {
                set { _id = value; }
                get { return _id; }
            }
            /// <summary>
            /// 
            /// </summary>

            public int event_type
            {
                set { _event_type = value; }
                get { return _event_type; }
            }
            /// <summary>
            /// 
            /// </summary>

            public int? data_source_type
            {
                set { _data_source_type = value; }
                get { return _data_source_type; }
            }
            /// <summary>
            /// 
            /// </summary>

            public string data_source_id
            {
                set { _data_source_id = value; }
                get { return _data_source_id; }
            }
            /// <summary>
            /// 
            /// </summary>

            public string data_source_content
            {
                set { _data_source_content = value; }
                get { return _data_source_content; }
            }
            /// <summary>
            /// 
            /// </summary>

            public int? opt_count
            {
                set { _opt_count = value; }
                get { return _opt_count; }
            }
            /// <summary>
            /// 
            /// </summary>

            public int serial_flag
            {
                set { _serial_flag = value; }
                get { return _serial_flag; }
            }

            /// <summary>
            /// 
            /// </summary>

            public DateTime? create_date
            {
                set { _create_date = value; }
                get { return _create_date; }
            }
            /// <summary>
            /// 
            /// </summary>

            public DateTime? update_date
            {
                set { _update_date = value; }
                get { return _update_date; }
            }

            #endregion Model


        }
    }
}

using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace OrmTest
{
    public class Unit2121 
    {
        public static void Init() 
        {
            UpdateTerminalAdjustInfo();
            var dt = DateTime.Now;
            var db = NewUnitTest.Db;
            var dt01 = db.Queryable<Order>().Select(it => dt.ToString("yyyy-MM-dd HH:mm:ss")).First();
            if (dt.ToString("yyyy-MM-dd HH:mm:ss") != dt01)
            {
                throw new Exception("unit error");
            }
            dt01 = db.Queryable<Order>().Select(it => dt.ToString("HH:mm:ss")).First();
            if (dt.ToString("HH:mm:ss") != dt01)
            {
                throw new Exception("unit error");
            }
            dt01 = db.Queryable<Order>().Select(it => dt.ToString("yyyy-MM-dd HH24:mi:ss")).First();
            if (dt.ToString("yyyy-MM-dd HH:mm:ss") != dt01)
            {
                throw new Exception("unit error");
            }
        }
        public static void UpdateTerminalAdjustInfo(string terminalCode="", decimal sendNum=0, decimal drugNum=0, decimal averageNum=0)
        {
            SqlSugarClient service = OrmTest.NewUnitTest.Db;
            NewUnitTest.Db.CodeFirst.InitTables<DRUG_STO_TERMINAL>();
        
            var db = NewUnitTest.Db;
            db.Aop.OnLogExecuting = (s, p) => Console.WriteLine(s);
            var sql = db.Updateable<DRUG_STO_TERMINAL>()
                .SetColumns(s => new DRUG_STO_TERMINAL
                {
                    OPER_DATE = SqlFunc.GetDate(),
                    SEND_QTY = s.SEND_QTY??0 + sendNum ,
                    DRUG_QTY = s.DRUG_QTY??0+drugNum,
                    AVERAGE_NUM = SqlFunc.IF((SqlFunc.IsNull(s.AVERAGE_NUM, 0) + averageNum) > 0).Return(SqlFunc.IsNull(s.AVERAGE_NUM, 0) + averageNum).End(0)
                })
                .Where(s => s.T_CODE == terminalCode)
                .ExecuteCommand();
 
        }
    }
    public class DRUG_STO_TERMINAL
    {
        private string _t_code;
        public string T_CODE
        {
            get { return _t_code; }
            set
            {
                _t_code = value;
            }
        }
        private string _t_name;
        public string T_NAME
        {
            get { return _t_name; }
            set
            {
                _t_name = value;
            }
        }
        private string _dept_code;
        public string DEPT_CODE
        {
            get { return _dept_code; }
            set
            {
                _dept_code = value;
            }
        }
        private string _t_type;
        public string T_TYPE
        {
            get { return _t_type; }
            set
            {
                _t_type = value;
            }
        }
        private string _replace_code = "";
        public string REPLACE_CODE
        {
            get { return _replace_code; }
            set
            {
                _replace_code = value;
            }
        }
        private string _close_flag;
        public string CLOSE_FLAG
        {
            get { return _close_flag; }
            set
            {
                _close_flag = value;
            }
        }
        private string _autoprint_flag;
        public string AUTOPRINT_FLAG
        {
            get { return _autoprint_flag; }
            set
            {
                _autoprint_flag = value;
            }
        }
        private int _refresh_interval1;
        public int REFRESH_INTERVAL1
        {
            get { return _refresh_interval1; }
            set
            {
                _refresh_interval1 = value;
            }
        }
        private int _refresh_interval2;
        public int REFRESH_INTERVAL2
        {
            get { return _refresh_interval2; }
            set
            {
                _refresh_interval2 = value;
            }
        }
        private string _property = "";
        public string PROPERTY
        {
            get { return _property; }
            set
            {
                _property = value;
            }
        }
        private int _alert_num;
        public int ALERT_NUM
        {
            get { return _alert_num; }
            set
            {
                _alert_num = value;
            }
        }
        private int _show_num;
        public int SHOW_NUM
        {
            get { return _show_num; }
            set
            {
                _show_num = value;
            }
        }
        private string _send_window;
        public string SEND_WINDOW
        {
            get { return _send_window; }
            set
            {
                _send_window = value;
            }
        }
        private string _oper_code = "";
        public string OPER_CODE
        {
            get { return _oper_code; }
            set
            {
                _oper_code = value;
            }
        }
        private DateTime? _oper_date;
        public DateTime? OPER_DATE
        {
            get { return _oper_date; }
            set
            {
                _oper_date = value;
            }
        }
        private string _mark = "";
        public string MARK
        {
            get { return _mark; }
            set
            {
                _mark = value;
            }
        }
        private decimal? _send_qty;
        public decimal? SEND_QTY
        {
            get { return _send_qty; }
            set
            {
                _send_qty = value;
            }
        }
        private decimal? _drug_qty;
        public decimal? DRUG_QTY
        {
            get { return _drug_qty; }
            set
            {
                _drug_qty = value;
            }
        }
        private int? _average_num;
        public int? AVERAGE_NUM
        {
            get { return _average_num; }
            set
            {
                _average_num = value;
            }
        }
        private string _print_type = "";
        public string PRINT_TYPE
        {
            get { return _print_type; }
            set
            {
                _print_type = value;
            }
        }

      
    }
}

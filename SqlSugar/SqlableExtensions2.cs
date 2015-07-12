using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public static partial class SqlableExtensions
    {


        public static Sqlable MappingTable<T1, T2>(this Sqlable sqlable, string mappingFeild1)

            where T1 : new()

            where T2 : new()
        {

            T1 t1 = new T1();

            T2 t2 = new T2();
            ;
            sqlable.MappingCurrentState = MappingCurrentState.MappingTable;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" FROM {1} t1 {0} \n   \n {2} JOIN {3} t2 {0} ON {4} ", sqlable.IsNoLock.IsNoLock(), t1.GetType().Name, mappingFeild1.IsLeft(), t2.GetType().Name, mappingFeild1.Remove());
            sqlable.Sql = sb;
            return sqlable;
        }

        public static Sqlable MappingTable<T1, T2, T3>(this Sqlable sqlable, string mappingFeild1, string mappingFeild2)

            where T1 : new()

            where T2 : new()

            where T3 : new()
        {

            T1 t1 = new T1();

            T2 t2 = new T2();

            T3 t3 = new T3();
            ;
            sqlable.MappingCurrentState = MappingCurrentState.MappingTable;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" FROM {1} t1 {0} \n   \n {2} JOIN {3} t2 {0} ON {4} \n {5} JOIN {6} t3 {0} ON {7} ", sqlable.IsNoLock.IsNoLock(), t1.GetType().Name, mappingFeild1.IsLeft(), t2.GetType().Name, mappingFeild1.Remove(), mappingFeild2.IsLeft(), t3.GetType().Name, mappingFeild2.Remove());
            sqlable.Sql = sb;
            return sqlable;
        }

        public static Sqlable MappingTable<T1, T2, T3, T4>(this Sqlable sqlable, string mappingFeild1, string mappingFeild2, string mappingFeild3)

            where T1 : new()

            where T2 : new()

            where T3 : new()

            where T4 : new()
        {

            T1 t1 = new T1();

            T2 t2 = new T2();

            T3 t3 = new T3();

            T4 t4 = new T4();
            ;
            sqlable.MappingCurrentState = MappingCurrentState.MappingTable;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" FROM {1} t1 {0} \n   \n {2} JOIN {3} t2 {0} ON {4} \n {5} JOIN {6} t3 {0} ON {7} \n {8} JOIN {9} t4 {0} ON {10} ", sqlable.IsNoLock.IsNoLock(), t1.GetType().Name, mappingFeild1.IsLeft(), t2.GetType().Name, mappingFeild1.Remove(), mappingFeild2.IsLeft(), t3.GetType().Name, mappingFeild2.Remove(), mappingFeild3.IsLeft(), t4.GetType().Name, mappingFeild3.Remove());
            sqlable.Sql = sb;
            return sqlable;
        }

        public static Sqlable MappingTable<T1, T2, T3, T4, T5>(this Sqlable sqlable, string mappingFeild1, string mappingFeild2, string mappingFeild3, string mappingFeild4)

            where T1 : new()

            where T2 : new()

            where T3 : new()

            where T4 : new()

            where T5 : new()
        {

            T1 t1 = new T1();

            T2 t2 = new T2();

            T3 t3 = new T3();

            T4 t4 = new T4();

            T5 t5 = new T5();
            ;
            sqlable.MappingCurrentState = MappingCurrentState.MappingTable;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" FROM {1} t1 {0} \n   \n {2} JOIN {3} t2 {0} ON {4} \n {5} JOIN {6} t3 {0} ON {7} \n {8} JOIN {9} t4 {0} ON {10} \n {11} JOIN {12} t5 {0} ON {13} ", sqlable.IsNoLock.IsNoLock(), t1.GetType().Name, mappingFeild1.IsLeft(), t2.GetType().Name, mappingFeild1.Remove(), mappingFeild2.IsLeft(), t3.GetType().Name, mappingFeild2.Remove(), mappingFeild3.IsLeft(), t4.GetType().Name, mappingFeild3.Remove(), mappingFeild4.IsLeft(), t5.GetType().Name, mappingFeild4.Remove());
            sqlable.Sql = sb;
            return sqlable;
        }

        public static Sqlable MappingTable<T1, T2, T3, T4, T5, T6>(this Sqlable sqlable, string mappingFeild1, string mappingFeild2, string mappingFeild3, string mappingFeild4, string mappingFeild5)

            where T1 : new()

            where T2 : new()

            where T3 : new()

            where T4 : new()

            where T5 : new()

            where T6 : new()
        {

            T1 t1 = new T1();

            T2 t2 = new T2();

            T3 t3 = new T3();

            T4 t4 = new T4();

            T5 t5 = new T5();

            T6 t6 = new T6();
            ;
            sqlable.MappingCurrentState = MappingCurrentState.MappingTable;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" FROM {1} t1 {0} \n   \n {2} JOIN {3} t2 {0} ON {4} \n {5} JOIN {6} t3 {0} ON {7} \n {8} JOIN {9} t4 {0} ON {10} \n {11} JOIN {12} t5 {0} ON {13} \n {14} JOIN {15} t6 {0} ON {16} ", sqlable.IsNoLock.IsNoLock(), t1.GetType().Name, mappingFeild1.IsLeft(), t2.GetType().Name, mappingFeild1.Remove(), mappingFeild2.IsLeft(), t3.GetType().Name, mappingFeild2.Remove(), mappingFeild3.IsLeft(), t4.GetType().Name, mappingFeild3.Remove(), mappingFeild4.IsLeft(), t5.GetType().Name, mappingFeild4.Remove(), mappingFeild5.IsLeft(), t6.GetType().Name, mappingFeild5.Remove());
            sqlable.Sql = sb;
            return sqlable;
        }

        public static Sqlable MappingTable<T1, T2, T3, T4, T5, T6, T7>(this Sqlable sqlable, string mappingFeild1, string mappingFeild2, string mappingFeild3, string mappingFeild4, string mappingFeild5, string mappingFeild6)

            where T1 : new()

            where T2 : new()

            where T3 : new()

            where T4 : new()

            where T5 : new()

            where T6 : new()

            where T7 : new()
        {

            T1 t1 = new T1();

            T2 t2 = new T2();

            T3 t3 = new T3();

            T4 t4 = new T4();

            T5 t5 = new T5();

            T6 t6 = new T6();

            T7 t7 = new T7();
            ;
            sqlable.MappingCurrentState = MappingCurrentState.MappingTable;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" FROM {1} t1 {0} \n   \n {2} JOIN {3} t2 {0} ON {4} \n {5} JOIN {6} t3 {0} ON {7} \n {8} JOIN {9} t4 {0} ON {10} \n {11} JOIN {12} t5 {0} ON {13} \n {14} JOIN {15} t6 {0} ON {16} \n {17} JOIN {18} t7 {0} ON {19} ", sqlable.IsNoLock.IsNoLock(), t1.GetType().Name, mappingFeild1.IsLeft(), t2.GetType().Name, mappingFeild1.Remove(), mappingFeild2.IsLeft(), t3.GetType().Name, mappingFeild2.Remove(), mappingFeild3.IsLeft(), t4.GetType().Name, mappingFeild3.Remove(), mappingFeild4.IsLeft(), t5.GetType().Name, mappingFeild4.Remove(), mappingFeild5.IsLeft(), t6.GetType().Name, mappingFeild5.Remove(), mappingFeild6.IsLeft(), t7.GetType().Name, mappingFeild6.Remove());
            sqlable.Sql = sb;
            return sqlable;
        }

        public static Sqlable MappingTable<T1, T2, T3, T4, T5, T6, T7, T8>(this Sqlable sqlable, string mappingFeild1, string mappingFeild2, string mappingFeild3, string mappingFeild4, string mappingFeild5, string mappingFeild6, string mappingFeild7)

            where T1 : new()

            where T2 : new()

            where T3 : new()

            where T4 : new()

            where T5 : new()

            where T6 : new()

            where T7 : new()

            where T8 : new()
        {

            T1 t1 = new T1();

            T2 t2 = new T2();

            T3 t3 = new T3();

            T4 t4 = new T4();

            T5 t5 = new T5();

            T6 t6 = new T6();

            T7 t7 = new T7();

            T8 t8 = new T8();
            ;
            sqlable.MappingCurrentState = MappingCurrentState.MappingTable;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" FROM {1} t1 {0} \n   \n {2} JOIN {3} t2 {0} ON {4} \n {5} JOIN {6} t3 {0} ON {7} \n {8} JOIN {9} t4 {0} ON {10} \n {11} JOIN {12} t5 {0} ON {13} \n {14} JOIN {15} t6 {0} ON {16} \n {17} JOIN {18} t7 {0} ON {19} \n {20} JOIN {21} t8 {0} ON {22} ", sqlable.IsNoLock.IsNoLock(), t1.GetType().Name, mappingFeild1.IsLeft(), t2.GetType().Name, mappingFeild1.Remove(), mappingFeild2.IsLeft(), t3.GetType().Name, mappingFeild2.Remove(), mappingFeild3.IsLeft(), t4.GetType().Name, mappingFeild3.Remove(), mappingFeild4.IsLeft(), t5.GetType().Name, mappingFeild4.Remove(), mappingFeild5.IsLeft(), t6.GetType().Name, mappingFeild5.Remove(), mappingFeild6.IsLeft(), t7.GetType().Name, mappingFeild6.Remove(), mappingFeild7.IsLeft(), t8.GetType().Name, mappingFeild7.Remove());
            sqlable.Sql = sb;
            return sqlable;
        }

        public static Sqlable MappingTable<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Sqlable sqlable, string mappingFeild1, string mappingFeild2, string mappingFeild3, string mappingFeild4, string mappingFeild5, string mappingFeild6, string mappingFeild7, string mappingFeild8)

            where T1 : new()

            where T2 : new()

            where T3 : new()

            where T4 : new()

            where T5 : new()

            where T6 : new()

            where T7 : new()

            where T8 : new()

            where T9 : new()
        {

            T1 t1 = new T1();

            T2 t2 = new T2();

            T3 t3 = new T3();

            T4 t4 = new T4();

            T5 t5 = new T5();

            T6 t6 = new T6();

            T7 t7 = new T7();

            T8 t8 = new T8();

            T9 t9 = new T9();
            ;
            sqlable.MappingCurrentState = MappingCurrentState.MappingTable;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" FROM {1} t1 {0} \n   \n {2} JOIN {3} t2 {0} ON {4} \n {5} JOIN {6} t3 {0} ON {7} \n {8} JOIN {9} t4 {0} ON {10} \n {11} JOIN {12} t5 {0} ON {13} \n {14} JOIN {15} t6 {0} ON {16} \n {17} JOIN {18} t7 {0} ON {19} \n {20} JOIN {21} t8 {0} ON {22} \n {23} JOIN {24} t9 {0} ON {25} ", sqlable.IsNoLock.IsNoLock(), t1.GetType().Name, mappingFeild1.IsLeft(), t2.GetType().Name, mappingFeild1.Remove(), mappingFeild2.IsLeft(), t3.GetType().Name, mappingFeild2.Remove(), mappingFeild3.IsLeft(), t4.GetType().Name, mappingFeild3.Remove(), mappingFeild4.IsLeft(), t5.GetType().Name, mappingFeild4.Remove(), mappingFeild5.IsLeft(), t6.GetType().Name, mappingFeild5.Remove(), mappingFeild6.IsLeft(), t7.GetType().Name, mappingFeild6.Remove(), mappingFeild7.IsLeft(), t8.GetType().Name, mappingFeild7.Remove(), mappingFeild8.IsLeft(), t9.GetType().Name, mappingFeild8.Remove());
            sqlable.Sql = sb;
            return sqlable;
        }

        public static Sqlable MappingTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Sqlable sqlable, string mappingFeild1, string mappingFeild2, string mappingFeild3, string mappingFeild4, string mappingFeild5, string mappingFeild6, string mappingFeild7, string mappingFeild8, string mappingFeild9)

            where T1 : new()

            where T2 : new()

            where T3 : new()

            where T4 : new()

            where T5 : new()

            where T6 : new()

            where T7 : new()

            where T8 : new()

            where T9 : new()

            where T10 : new()
        {

            T1 t1 = new T1();

            T2 t2 = new T2();

            T3 t3 = new T3();

            T4 t4 = new T4();

            T5 t5 = new T5();

            T6 t6 = new T6();

            T7 t7 = new T7();

            T8 t8 = new T8();

            T9 t9 = new T9();

            T10 t10 = new T10();
            ;
            sqlable.MappingCurrentState = MappingCurrentState.MappingTable;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" FROM {1} t1 {0} \n   \n {2} JOIN {3} t2 {0} ON {4} \n {5} JOIN {6} t3 {0} ON {7} \n {8} JOIN {9} t4 {0} ON {10} \n {11} JOIN {12} t5 {0} ON {13} \n {14} JOIN {15} t6 {0} ON {16} \n {17} JOIN {18} t7 {0} ON {19} \n {20} JOIN {21} t8 {0} ON {22} \n {23} JOIN {24} t9 {0} ON {25} \n {26} JOIN {27} t10 {0} ON {28} ", sqlable.IsNoLock.IsNoLock(), t1.GetType().Name, mappingFeild1.IsLeft(), t2.GetType().Name, mappingFeild1.Remove(), mappingFeild2.IsLeft(), t3.GetType().Name, mappingFeild2.Remove(), mappingFeild3.IsLeft(), t4.GetType().Name, mappingFeild3.Remove(), mappingFeild4.IsLeft(), t5.GetType().Name, mappingFeild4.Remove(), mappingFeild5.IsLeft(), t6.GetType().Name, mappingFeild5.Remove(), mappingFeild6.IsLeft(), t7.GetType().Name, mappingFeild6.Remove(), mappingFeild7.IsLeft(), t8.GetType().Name, mappingFeild7.Remove(), mappingFeild8.IsLeft(), t9.GetType().Name, mappingFeild8.Remove(), mappingFeild9.IsLeft(), t10.GetType().Name, mappingFeild9.Remove());
            sqlable.Sql = sb;
            return sqlable;
        }

        public static Sqlable MappingTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this Sqlable sqlable, string mappingFeild1, string mappingFeild2, string mappingFeild3, string mappingFeild4, string mappingFeild5, string mappingFeild6, string mappingFeild7, string mappingFeild8, string mappingFeild9, string mappingFeild10)

            where T1 : new()

            where T2 : new()

            where T3 : new()

            where T4 : new()

            where T5 : new()

            where T6 : new()

            where T7 : new()

            where T8 : new()

            where T9 : new()

            where T10 : new()

            where T11 : new()
        {

            T1 t1 = new T1();

            T2 t2 = new T2();

            T3 t3 = new T3();

            T4 t4 = new T4();

            T5 t5 = new T5();

            T6 t6 = new T6();

            T7 t7 = new T7();

            T8 t8 = new T8();

            T9 t9 = new T9();

            T10 t10 = new T10();

            T11 t11 = new T11();
            ;
            sqlable.MappingCurrentState = MappingCurrentState.MappingTable;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" FROM {1} t1 {0} \n   \n {2} JOIN {3} t2 {0} ON {4} \n {5} JOIN {6} t3 {0} ON {7} \n {8} JOIN {9} t4 {0} ON {10} \n {11} JOIN {12} t5 {0} ON {13} \n {14} JOIN {15} t6 {0} ON {16} \n {17} JOIN {18} t7 {0} ON {19} \n {20} JOIN {21} t8 {0} ON {22} \n {23} JOIN {24} t9 {0} ON {25} \n {26} JOIN {27} t10 {0} ON {28} \n {29} JOIN {30} t11 {0} ON {31} ", sqlable.IsNoLock.IsNoLock(), t1.GetType().Name, mappingFeild1.IsLeft(), t2.GetType().Name, mappingFeild1.Remove(), mappingFeild2.IsLeft(), t3.GetType().Name, mappingFeild2.Remove(), mappingFeild3.IsLeft(), t4.GetType().Name, mappingFeild3.Remove(), mappingFeild4.IsLeft(), t5.GetType().Name, mappingFeild4.Remove(), mappingFeild5.IsLeft(), t6.GetType().Name, mappingFeild5.Remove(), mappingFeild6.IsLeft(), t7.GetType().Name, mappingFeild6.Remove(), mappingFeild7.IsLeft(), t8.GetType().Name, mappingFeild7.Remove(), mappingFeild8.IsLeft(), t9.GetType().Name, mappingFeild8.Remove(), mappingFeild9.IsLeft(), t10.GetType().Name, mappingFeild9.Remove(), mappingFeild10.IsLeft(), t11.GetType().Name, mappingFeild10.Remove());
            sqlable.Sql = sb;
            return sqlable;
        }

        public static Sqlable MappingTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this Sqlable sqlable, string mappingFeild1, string mappingFeild2, string mappingFeild3, string mappingFeild4, string mappingFeild5, string mappingFeild6, string mappingFeild7, string mappingFeild8, string mappingFeild9, string mappingFeild10, string mappingFeild11)

            where T1 : new()

            where T2 : new()

            where T3 : new()

            where T4 : new()

            where T5 : new()

            where T6 : new()

            where T7 : new()

            where T8 : new()

            where T9 : new()

            where T10 : new()

            where T11 : new()

            where T12 : new()
        {

            T1 t1 = new T1();

            T2 t2 = new T2();

            T3 t3 = new T3();

            T4 t4 = new T4();

            T5 t5 = new T5();

            T6 t6 = new T6();

            T7 t7 = new T7();

            T8 t8 = new T8();

            T9 t9 = new T9();

            T10 t10 = new T10();

            T11 t11 = new T11();

            T12 t12 = new T12();
            ;
            sqlable.MappingCurrentState = MappingCurrentState.MappingTable;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" FROM {1} t1 {0} \n   \n {2} JOIN {3} t2 {0} ON {4} \n {5} JOIN {6} t3 {0} ON {7} \n {8} JOIN {9} t4 {0} ON {10} \n {11} JOIN {12} t5 {0} ON {13} \n {14} JOIN {15} t6 {0} ON {16} \n {17} JOIN {18} t7 {0} ON {19} \n {20} JOIN {21} t8 {0} ON {22} \n {23} JOIN {24} t9 {0} ON {25} \n {26} JOIN {27} t10 {0} ON {28} \n {29} JOIN {30} t11 {0} ON {31} \n {32} JOIN {33} t12 {0} ON {34} ", sqlable.IsNoLock.IsNoLock(), t1.GetType().Name, mappingFeild1.IsLeft(), t2.GetType().Name, mappingFeild1.Remove(), mappingFeild2.IsLeft(), t3.GetType().Name, mappingFeild2.Remove(), mappingFeild3.IsLeft(), t4.GetType().Name, mappingFeild3.Remove(), mappingFeild4.IsLeft(), t5.GetType().Name, mappingFeild4.Remove(), mappingFeild5.IsLeft(), t6.GetType().Name, mappingFeild5.Remove(), mappingFeild6.IsLeft(), t7.GetType().Name, mappingFeild6.Remove(), mappingFeild7.IsLeft(), t8.GetType().Name, mappingFeild7.Remove(), mappingFeild8.IsLeft(), t9.GetType().Name, mappingFeild8.Remove(), mappingFeild9.IsLeft(), t10.GetType().Name, mappingFeild9.Remove(), mappingFeild10.IsLeft(), t11.GetType().Name, mappingFeild10.Remove(), mappingFeild11.IsLeft(), t12.GetType().Name, mappingFeild11.Remove());
            sqlable.Sql = sb;
            return sqlable;
        }

        public static Sqlable MappingTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this Sqlable sqlable, string mappingFeild1, string mappingFeild2, string mappingFeild3, string mappingFeild4, string mappingFeild5, string mappingFeild6, string mappingFeild7, string mappingFeild8, string mappingFeild9, string mappingFeild10, string mappingFeild11, string mappingFeild12)

            where T1 : new()

            where T2 : new()

            where T3 : new()

            where T4 : new()

            where T5 : new()

            where T6 : new()

            where T7 : new()

            where T8 : new()

            where T9 : new()

            where T10 : new()

            where T11 : new()

            where T12 : new()

            where T13 : new()
        {

            T1 t1 = new T1();

            T2 t2 = new T2();

            T3 t3 = new T3();

            T4 t4 = new T4();

            T5 t5 = new T5();

            T6 t6 = new T6();

            T7 t7 = new T7();

            T8 t8 = new T8();

            T9 t9 = new T9();

            T10 t10 = new T10();

            T11 t11 = new T11();

            T12 t12 = new T12();

            T13 t13 = new T13();
            ;
            sqlable.MappingCurrentState = MappingCurrentState.MappingTable;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" FROM {1} t1 {0} \n   \n {2} JOIN {3} t2 {0} ON {4} \n {5} JOIN {6} t3 {0} ON {7} \n {8} JOIN {9} t4 {0} ON {10} \n {11} JOIN {12} t5 {0} ON {13} \n {14} JOIN {15} t6 {0} ON {16} \n {17} JOIN {18} t7 {0} ON {19} \n {20} JOIN {21} t8 {0} ON {22} \n {23} JOIN {24} t9 {0} ON {25} \n {26} JOIN {27} t10 {0} ON {28} \n {29} JOIN {30} t11 {0} ON {31} \n {32} JOIN {33} t12 {0} ON {34} \n {35} JOIN {36} t13 {0} ON {37} ", sqlable.IsNoLock.IsNoLock(), t1.GetType().Name, mappingFeild1.IsLeft(), t2.GetType().Name, mappingFeild1.Remove(), mappingFeild2.IsLeft(), t3.GetType().Name, mappingFeild2.Remove(), mappingFeild3.IsLeft(), t4.GetType().Name, mappingFeild3.Remove(), mappingFeild4.IsLeft(), t5.GetType().Name, mappingFeild4.Remove(), mappingFeild5.IsLeft(), t6.GetType().Name, mappingFeild5.Remove(), mappingFeild6.IsLeft(), t7.GetType().Name, mappingFeild6.Remove(), mappingFeild7.IsLeft(), t8.GetType().Name, mappingFeild7.Remove(), mappingFeild8.IsLeft(), t9.GetType().Name, mappingFeild8.Remove(), mappingFeild9.IsLeft(), t10.GetType().Name, mappingFeild9.Remove(), mappingFeild10.IsLeft(), t11.GetType().Name, mappingFeild10.Remove(), mappingFeild11.IsLeft(), t12.GetType().Name, mappingFeild11.Remove(), mappingFeild12.IsLeft(), t13.GetType().Name, mappingFeild12.Remove());
            sqlable.Sql = sb;
            return sqlable;
        }

        public static Sqlable MappingTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this Sqlable sqlable, string mappingFeild1, string mappingFeild2, string mappingFeild3, string mappingFeild4, string mappingFeild5, string mappingFeild6, string mappingFeild7, string mappingFeild8, string mappingFeild9, string mappingFeild10, string mappingFeild11, string mappingFeild12, string mappingFeild13)

            where T1 : new()

            where T2 : new()

            where T3 : new()

            where T4 : new()

            where T5 : new()

            where T6 : new()

            where T7 : new()

            where T8 : new()

            where T9 : new()

            where T10 : new()

            where T11 : new()

            where T12 : new()

            where T13 : new()

            where T14 : new()
        {

            T1 t1 = new T1();

            T2 t2 = new T2();

            T3 t3 = new T3();

            T4 t4 = new T4();

            T5 t5 = new T5();

            T6 t6 = new T6();

            T7 t7 = new T7();

            T8 t8 = new T8();

            T9 t9 = new T9();

            T10 t10 = new T10();

            T11 t11 = new T11();

            T12 t12 = new T12();

            T13 t13 = new T13();

            T14 t14 = new T14();
            ;
            sqlable.MappingCurrentState = MappingCurrentState.MappingTable;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" FROM {1} t1 {0} \n   \n {2} JOIN {3} t2 {0} ON {4} \n {5} JOIN {6} t3 {0} ON {7} \n {8} JOIN {9} t4 {0} ON {10} \n {11} JOIN {12} t5 {0} ON {13} \n {14} JOIN {15} t6 {0} ON {16} \n {17} JOIN {18} t7 {0} ON {19} \n {20} JOIN {21} t8 {0} ON {22} \n {23} JOIN {24} t9 {0} ON {25} \n {26} JOIN {27} t10 {0} ON {28} \n {29} JOIN {30} t11 {0} ON {31} \n {32} JOIN {33} t12 {0} ON {34} \n {35} JOIN {36} t13 {0} ON {37} \n {38} JOIN {39} t14 {0} ON {40} ", sqlable.IsNoLock.IsNoLock(), t1.GetType().Name, mappingFeild1.IsLeft(), t2.GetType().Name, mappingFeild1.Remove(), mappingFeild2.IsLeft(), t3.GetType().Name, mappingFeild2.Remove(), mappingFeild3.IsLeft(), t4.GetType().Name, mappingFeild3.Remove(), mappingFeild4.IsLeft(), t5.GetType().Name, mappingFeild4.Remove(), mappingFeild5.IsLeft(), t6.GetType().Name, mappingFeild5.Remove(), mappingFeild6.IsLeft(), t7.GetType().Name, mappingFeild6.Remove(), mappingFeild7.IsLeft(), t8.GetType().Name, mappingFeild7.Remove(), mappingFeild8.IsLeft(), t9.GetType().Name, mappingFeild8.Remove(), mappingFeild9.IsLeft(), t10.GetType().Name, mappingFeild9.Remove(), mappingFeild10.IsLeft(), t11.GetType().Name, mappingFeild10.Remove(), mappingFeild11.IsLeft(), t12.GetType().Name, mappingFeild11.Remove(), mappingFeild12.IsLeft(), t13.GetType().Name, mappingFeild12.Remove(), mappingFeild13.IsLeft(), t14.GetType().Name, mappingFeild13.Remove());
            sqlable.Sql = sb;
            return sqlable;
        }

        public static Sqlable MappingTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this Sqlable sqlable, string mappingFeild1, string mappingFeild2, string mappingFeild3, string mappingFeild4, string mappingFeild5, string mappingFeild6, string mappingFeild7, string mappingFeild8, string mappingFeild9, string mappingFeild10, string mappingFeild11, string mappingFeild12, string mappingFeild13, string mappingFeild14)

            where T1 : new()

            where T2 : new()

            where T3 : new()

            where T4 : new()

            where T5 : new()

            where T6 : new()

            where T7 : new()

            where T8 : new()

            where T9 : new()

            where T10 : new()

            where T11 : new()

            where T12 : new()

            where T13 : new()

            where T14 : new()

            where T15 : new()
        {

            T1 t1 = new T1();

            T2 t2 = new T2();

            T3 t3 = new T3();

            T4 t4 = new T4();

            T5 t5 = new T5();

            T6 t6 = new T6();

            T7 t7 = new T7();

            T8 t8 = new T8();

            T9 t9 = new T9();

            T10 t10 = new T10();

            T11 t11 = new T11();

            T12 t12 = new T12();

            T13 t13 = new T13();

            T14 t14 = new T14();

            T15 t15 = new T15();
            ;
            sqlable.MappingCurrentState = MappingCurrentState.MappingTable;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" FROM {1} t1 {0} \n   \n {2} JOIN {3} t2 {0} ON {4} \n {5} JOIN {6} t3 {0} ON {7} \n {8} JOIN {9} t4 {0} ON {10} \n {11} JOIN {12} t5 {0} ON {13} \n {14} JOIN {15} t6 {0} ON {16} \n {17} JOIN {18} t7 {0} ON {19} \n {20} JOIN {21} t8 {0} ON {22} \n {23} JOIN {24} t9 {0} ON {25} \n {26} JOIN {27} t10 {0} ON {28} \n {29} JOIN {30} t11 {0} ON {31} \n {32} JOIN {33} t12 {0} ON {34} \n {35} JOIN {36} t13 {0} ON {37} \n {38} JOIN {39} t14 {0} ON {40} \n {41} JOIN {42} t15 {0} ON {43} ", sqlable.IsNoLock.IsNoLock(), t1.GetType().Name, mappingFeild1.IsLeft(), t2.GetType().Name, mappingFeild1.Remove(), mappingFeild2.IsLeft(), t3.GetType().Name, mappingFeild2.Remove(), mappingFeild3.IsLeft(), t4.GetType().Name, mappingFeild3.Remove(), mappingFeild4.IsLeft(), t5.GetType().Name, mappingFeild4.Remove(), mappingFeild5.IsLeft(), t6.GetType().Name, mappingFeild5.Remove(), mappingFeild6.IsLeft(), t7.GetType().Name, mappingFeild6.Remove(), mappingFeild7.IsLeft(), t8.GetType().Name, mappingFeild7.Remove(), mappingFeild8.IsLeft(), t9.GetType().Name, mappingFeild8.Remove(), mappingFeild9.IsLeft(), t10.GetType().Name, mappingFeild9.Remove(), mappingFeild10.IsLeft(), t11.GetType().Name, mappingFeild10.Remove(), mappingFeild11.IsLeft(), t12.GetType().Name, mappingFeild11.Remove(), mappingFeild12.IsLeft(), t13.GetType().Name, mappingFeild12.Remove(), mappingFeild13.IsLeft(), t14.GetType().Name, mappingFeild13.Remove(), mappingFeild14.IsLeft(), t15.GetType().Name, mappingFeild14.Remove());
            sqlable.Sql = sb;
            return sqlable;
        }

        public static Sqlable MappingTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this Sqlable sqlable, string mappingFeild1, string mappingFeild2, string mappingFeild3, string mappingFeild4, string mappingFeild5, string mappingFeild6, string mappingFeild7, string mappingFeild8, string mappingFeild9, string mappingFeild10, string mappingFeild11, string mappingFeild12, string mappingFeild13, string mappingFeild14, string mappingFeild15)

            where T1 : new()

            where T2 : new()

            where T3 : new()

            where T4 : new()

            where T5 : new()

            where T6 : new()

            where T7 : new()

            where T8 : new()

            where T9 : new()

            where T10 : new()

            where T11 : new()

            where T12 : new()

            where T13 : new()

            where T14 : new()

            where T15 : new()

            where T16 : new()
        {

            T1 t1 = new T1();

            T2 t2 = new T2();

            T3 t3 = new T3();

            T4 t4 = new T4();

            T5 t5 = new T5();

            T6 t6 = new T6();

            T7 t7 = new T7();

            T8 t8 = new T8();

            T9 t9 = new T9();

            T10 t10 = new T10();

            T11 t11 = new T11();

            T12 t12 = new T12();

            T13 t13 = new T13();

            T14 t14 = new T14();

            T15 t15 = new T15();

            T16 t16 = new T16();
            ;
            sqlable.MappingCurrentState = MappingCurrentState.MappingTable;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" FROM {1} t1 {0} \n   \n {2} JOIN {3} t2 {0} ON {4} \n {5} JOIN {6} t3 {0} ON {7} \n {8} JOIN {9} t4 {0} ON {10} \n {11} JOIN {12} t5 {0} ON {13} \n {14} JOIN {15} t6 {0} ON {16} \n {17} JOIN {18} t7 {0} ON {19} \n {20} JOIN {21} t8 {0} ON {22} \n {23} JOIN {24} t9 {0} ON {25} \n {26} JOIN {27} t10 {0} ON {28} \n {29} JOIN {30} t11 {0} ON {31} \n {32} JOIN {33} t12 {0} ON {34} \n {35} JOIN {36} t13 {0} ON {37} \n {38} JOIN {39} t14 {0} ON {40} \n {41} JOIN {42} t15 {0} ON {43} \n {44} JOIN {45} t16 {0} ON {46} ", sqlable.IsNoLock.IsNoLock(), t1.GetType().Name, mappingFeild1.IsLeft(), t2.GetType().Name, mappingFeild1.Remove(), mappingFeild2.IsLeft(), t3.GetType().Name, mappingFeild2.Remove(), mappingFeild3.IsLeft(), t4.GetType().Name, mappingFeild3.Remove(), mappingFeild4.IsLeft(), t5.GetType().Name, mappingFeild4.Remove(), mappingFeild5.IsLeft(), t6.GetType().Name, mappingFeild5.Remove(), mappingFeild6.IsLeft(), t7.GetType().Name, mappingFeild6.Remove(), mappingFeild7.IsLeft(), t8.GetType().Name, mappingFeild7.Remove(), mappingFeild8.IsLeft(), t9.GetType().Name, mappingFeild8.Remove(), mappingFeild9.IsLeft(), t10.GetType().Name, mappingFeild9.Remove(), mappingFeild10.IsLeft(), t11.GetType().Name, mappingFeild10.Remove(), mappingFeild11.IsLeft(), t12.GetType().Name, mappingFeild11.Remove(), mappingFeild12.IsLeft(), t13.GetType().Name, mappingFeild12.Remove(), mappingFeild13.IsLeft(), t14.GetType().Name, mappingFeild13.Remove(), mappingFeild14.IsLeft(), t15.GetType().Name, mappingFeild14.Remove(), mappingFeild15.IsLeft(), t16.GetType().Name, mappingFeild15.Remove());
            sqlable.Sql = sb;
            return sqlable;
        }

        public static Sqlable MappingTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(this Sqlable sqlable, string mappingFeild1, string mappingFeild2, string mappingFeild3, string mappingFeild4, string mappingFeild5, string mappingFeild6, string mappingFeild7, string mappingFeild8, string mappingFeild9, string mappingFeild10, string mappingFeild11, string mappingFeild12, string mappingFeild13, string mappingFeild14, string mappingFeild15, string mappingFeild16)

            where T1 : new()

            where T2 : new()

            where T3 : new()

            where T4 : new()

            where T5 : new()

            where T6 : new()

            where T7 : new()

            where T8 : new()

            where T9 : new()

            where T10 : new()

            where T11 : new()

            where T12 : new()

            where T13 : new()

            where T14 : new()

            where T15 : new()

            where T16 : new()

            where T17 : new()
        {

            T1 t1 = new T1();

            T2 t2 = new T2();

            T3 t3 = new T3();

            T4 t4 = new T4();

            T5 t5 = new T5();

            T6 t6 = new T6();

            T7 t7 = new T7();

            T8 t8 = new T8();

            T9 t9 = new T9();

            T10 t10 = new T10();

            T11 t11 = new T11();

            T12 t12 = new T12();

            T13 t13 = new T13();

            T14 t14 = new T14();

            T15 t15 = new T15();

            T16 t16 = new T16();

            T17 t17 = new T17();
            ;
            sqlable.MappingCurrentState = MappingCurrentState.MappingTable;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" FROM {1} t1 {0} \n   \n {2} JOIN {3} t2 {0} ON {4} \n {5} JOIN {6} t3 {0} ON {7} \n {8} JOIN {9} t4 {0} ON {10} \n {11} JOIN {12} t5 {0} ON {13} \n {14} JOIN {15} t6 {0} ON {16} \n {17} JOIN {18} t7 {0} ON {19} \n {20} JOIN {21} t8 {0} ON {22} \n {23} JOIN {24} t9 {0} ON {25} \n {26} JOIN {27} t10 {0} ON {28} \n {29} JOIN {30} t11 {0} ON {31} \n {32} JOIN {33} t12 {0} ON {34} \n {35} JOIN {36} t13 {0} ON {37} \n {38} JOIN {39} t14 {0} ON {40} \n {41} JOIN {42} t15 {0} ON {43} \n {44} JOIN {45} t16 {0} ON {46} \n {47} JOIN {48} t17 {0} ON {49} ", sqlable.IsNoLock.IsNoLock(), t1.GetType().Name, mappingFeild1.IsLeft(), t2.GetType().Name, mappingFeild1.Remove(), mappingFeild2.IsLeft(), t3.GetType().Name, mappingFeild2.Remove(), mappingFeild3.IsLeft(), t4.GetType().Name, mappingFeild3.Remove(), mappingFeild4.IsLeft(), t5.GetType().Name, mappingFeild4.Remove(), mappingFeild5.IsLeft(), t6.GetType().Name, mappingFeild5.Remove(), mappingFeild6.IsLeft(), t7.GetType().Name, mappingFeild6.Remove(), mappingFeild7.IsLeft(), t8.GetType().Name, mappingFeild7.Remove(), mappingFeild8.IsLeft(), t9.GetType().Name, mappingFeild8.Remove(), mappingFeild9.IsLeft(), t10.GetType().Name, mappingFeild9.Remove(), mappingFeild10.IsLeft(), t11.GetType().Name, mappingFeild10.Remove(), mappingFeild11.IsLeft(), t12.GetType().Name, mappingFeild11.Remove(), mappingFeild12.IsLeft(), t13.GetType().Name, mappingFeild12.Remove(), mappingFeild13.IsLeft(), t14.GetType().Name, mappingFeild13.Remove(), mappingFeild14.IsLeft(), t15.GetType().Name, mappingFeild14.Remove(), mappingFeild15.IsLeft(), t16.GetType().Name, mappingFeild15.Remove(), mappingFeild16.IsLeft(), t17.GetType().Name, mappingFeild16.Remove());
            sqlable.Sql = sb;
            return sqlable;
        }

        public static Sqlable MappingTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(this Sqlable sqlable, string mappingFeild1, string mappingFeild2, string mappingFeild3, string mappingFeild4, string mappingFeild5, string mappingFeild6, string mappingFeild7, string mappingFeild8, string mappingFeild9, string mappingFeild10, string mappingFeild11, string mappingFeild12, string mappingFeild13, string mappingFeild14, string mappingFeild15, string mappingFeild16, string mappingFeild17)

            where T1 : new()

            where T2 : new()

            where T3 : new()

            where T4 : new()

            where T5 : new()

            where T6 : new()

            where T7 : new()

            where T8 : new()

            where T9 : new()

            where T10 : new()

            where T11 : new()

            where T12 : new()

            where T13 : new()

            where T14 : new()

            where T15 : new()

            where T16 : new()

            where T17 : new()

            where T18 : new()
        {

            T1 t1 = new T1();

            T2 t2 = new T2();

            T3 t3 = new T3();

            T4 t4 = new T4();

            T5 t5 = new T5();

            T6 t6 = new T6();

            T7 t7 = new T7();

            T8 t8 = new T8();

            T9 t9 = new T9();

            T10 t10 = new T10();

            T11 t11 = new T11();

            T12 t12 = new T12();

            T13 t13 = new T13();

            T14 t14 = new T14();

            T15 t15 = new T15();

            T16 t16 = new T16();

            T17 t17 = new T17();

            T18 t18 = new T18();
            ;
            sqlable.MappingCurrentState = MappingCurrentState.MappingTable;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" FROM {1} t1 {0} \n   \n {2} JOIN {3} t2 {0} ON {4} \n {5} JOIN {6} t3 {0} ON {7} \n {8} JOIN {9} t4 {0} ON {10} \n {11} JOIN {12} t5 {0} ON {13} \n {14} JOIN {15} t6 {0} ON {16} \n {17} JOIN {18} t7 {0} ON {19} \n {20} JOIN {21} t8 {0} ON {22} \n {23} JOIN {24} t9 {0} ON {25} \n {26} JOIN {27} t10 {0} ON {28} \n {29} JOIN {30} t11 {0} ON {31} \n {32} JOIN {33} t12 {0} ON {34} \n {35} JOIN {36} t13 {0} ON {37} \n {38} JOIN {39} t14 {0} ON {40} \n {41} JOIN {42} t15 {0} ON {43} \n {44} JOIN {45} t16 {0} ON {46} \n {47} JOIN {48} t17 {0} ON {49} \n {50} JOIN {51} t18 {0} ON {52} ", sqlable.IsNoLock.IsNoLock(), t1.GetType().Name, mappingFeild1.IsLeft(), t2.GetType().Name, mappingFeild1.Remove(), mappingFeild2.IsLeft(), t3.GetType().Name, mappingFeild2.Remove(), mappingFeild3.IsLeft(), t4.GetType().Name, mappingFeild3.Remove(), mappingFeild4.IsLeft(), t5.GetType().Name, mappingFeild4.Remove(), mappingFeild5.IsLeft(), t6.GetType().Name, mappingFeild5.Remove(), mappingFeild6.IsLeft(), t7.GetType().Name, mappingFeild6.Remove(), mappingFeild7.IsLeft(), t8.GetType().Name, mappingFeild7.Remove(), mappingFeild8.IsLeft(), t9.GetType().Name, mappingFeild8.Remove(), mappingFeild9.IsLeft(), t10.GetType().Name, mappingFeild9.Remove(), mappingFeild10.IsLeft(), t11.GetType().Name, mappingFeild10.Remove(), mappingFeild11.IsLeft(), t12.GetType().Name, mappingFeild11.Remove(), mappingFeild12.IsLeft(), t13.GetType().Name, mappingFeild12.Remove(), mappingFeild13.IsLeft(), t14.GetType().Name, mappingFeild13.Remove(), mappingFeild14.IsLeft(), t15.GetType().Name, mappingFeild14.Remove(), mappingFeild15.IsLeft(), t16.GetType().Name, mappingFeild15.Remove(), mappingFeild16.IsLeft(), t17.GetType().Name, mappingFeild16.Remove(), mappingFeild17.IsLeft(), t18.GetType().Name, mappingFeild17.Remove());
            sqlable.Sql = sb;
            return sqlable;
        }

        public static Sqlable MappingTable<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(this Sqlable sqlable, string mappingFeild1, string mappingFeild2, string mappingFeild3, string mappingFeild4, string mappingFeild5, string mappingFeild6, string mappingFeild7, string mappingFeild8, string mappingFeild9, string mappingFeild10, string mappingFeild11, string mappingFeild12, string mappingFeild13, string mappingFeild14, string mappingFeild15, string mappingFeild16, string mappingFeild17, string mappingFeild18)

            where T1 : new()

            where T2 : new()

            where T3 : new()

            where T4 : new()

            where T5 : new()

            where T6 : new()

            where T7 : new()

            where T8 : new()

            where T9 : new()

            where T10 : new()

            where T11 : new()

            where T12 : new()

            where T13 : new()

            where T14 : new()

            where T15 : new()

            where T16 : new()

            where T17 : new()

            where T18 : new()

            where T19 : new()
        {

            T1 t1 = new T1();

            T2 t2 = new T2();

            T3 t3 = new T3();

            T4 t4 = new T4();

            T5 t5 = new T5();

            T6 t6 = new T6();

            T7 t7 = new T7();

            T8 t8 = new T8();

            T9 t9 = new T9();

            T10 t10 = new T10();

            T11 t11 = new T11();

            T12 t12 = new T12();

            T13 t13 = new T13();

            T14 t14 = new T14();

            T15 t15 = new T15();

            T16 t16 = new T16();

            T17 t17 = new T17();

            T18 t18 = new T18();

            T19 t19 = new T19();
            ;
            sqlable.MappingCurrentState = MappingCurrentState.MappingTable;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" FROM {1} t1 {0} \n   \n {2} JOIN {3} t2 {0} ON {4} \n {5} JOIN {6} t3 {0} ON {7} \n {8} JOIN {9} t4 {0} ON {10} \n {11} JOIN {12} t5 {0} ON {13} \n {14} JOIN {15} t6 {0} ON {16} \n {17} JOIN {18} t7 {0} ON {19} \n {20} JOIN {21} t8 {0} ON {22} \n {23} JOIN {24} t9 {0} ON {25} \n {26} JOIN {27} t10 {0} ON {28} \n {29} JOIN {30} t11 {0} ON {31} \n {32} JOIN {33} t12 {0} ON {34} \n {35} JOIN {36} t13 {0} ON {37} \n {38} JOIN {39} t14 {0} ON {40} \n {41} JOIN {42} t15 {0} ON {43} \n {44} JOIN {45} t16 {0} ON {46} \n {47} JOIN {48} t17 {0} ON {49} \n {50} JOIN {51} t18 {0} ON {52} \n {53} JOIN {54} t19 {0} ON {55} ", sqlable.IsNoLock.IsNoLock(), t1.GetType().Name, mappingFeild1.IsLeft(), t2.GetType().Name, mappingFeild1.Remove(), mappingFeild2.IsLeft(), t3.GetType().Name, mappingFeild2.Remove(), mappingFeild3.IsLeft(), t4.GetType().Name, mappingFeild3.Remove(), mappingFeild4.IsLeft(), t5.GetType().Name, mappingFeild4.Remove(), mappingFeild5.IsLeft(), t6.GetType().Name, mappingFeild5.Remove(), mappingFeild6.IsLeft(), t7.GetType().Name, mappingFeild6.Remove(), mappingFeild7.IsLeft(), t8.GetType().Name, mappingFeild7.Remove(), mappingFeild8.IsLeft(), t9.GetType().Name, mappingFeild8.Remove(), mappingFeild9.IsLeft(), t10.GetType().Name, mappingFeild9.Remove(), mappingFeild10.IsLeft(), t11.GetType().Name, mappingFeild10.Remove(), mappingFeild11.IsLeft(), t12.GetType().Name, mappingFeild11.Remove(), mappingFeild12.IsLeft(), t13.GetType().Name, mappingFeild12.Remove(), mappingFeild13.IsLeft(), t14.GetType().Name, mappingFeild13.Remove(), mappingFeild14.IsLeft(), t15.GetType().Name, mappingFeild14.Remove(), mappingFeild15.IsLeft(), t16.GetType().Name, mappingFeild15.Remove(), mappingFeild16.IsLeft(), t17.GetType().Name, mappingFeild16.Remove(), mappingFeild17.IsLeft(), t18.GetType().Name, mappingFeild17.Remove(), mappingFeild18.IsLeft(), t19.GetType().Name, mappingFeild18.Remove());
            sqlable.Sql = sb;
            return sqlable;
        }
            
            
    }
}

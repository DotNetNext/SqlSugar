using SqlSugar.TDengineAdo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TDengineTest 
{
    internal class AdoDemo
    {
        public  static void Init()
        {

            ExecuteNonQuery();//库不存在建库
            ExecuteNonQuery2();//表不存在建表
            DataTable();
            DataReader();
            ExecuteScalar();
            ExecuteScalar2();
            Console.ReadKey();
        }
        private static TDengineCommand ExecuteNonQuery()
        {
            TDengineConnection conn =
                            new TDengineConnection("Host=localhost;Port=6030;Username=root;Password=taosdata;Database=power");
            conn.Open();

            var comm = ((TDengineCommand)conn.CreateCommand());
            comm.Connection = conn;
            comm.CommandText = "CREATE DATABASE IF NOT EXISTS power WAL_RETENTION_PERIOD 3600";
            var dr = comm.ExecuteNonQuery();

            conn.Close();
            return comm;
        }
        private static TDengineCommand ExecuteNonQuery2()
        {
            TDengineConnection conn =
                            new TDengineConnection("Host=localhost;Port=6030;Username=root;Password=taosdata;Database=power");
            conn.Open();

            var comm = ((TDengineCommand)conn.CreateCommand());
            comm.Connection = conn;
            string createTable = "CREATE STABLE IF NOT EXISTS test.meters (ts timestamp, current float, voltage int, phase float) TAGS (location binary(64), groupId int);";
            comm.CommandText = createTable;
            var dr = comm.ExecuteNonQuery();

            conn.Close();
            return comm;
        }
        private static TDengineCommand DataTable()
        {
            TDengineConnection conn =
                            new TDengineConnection("Host=localhost;Port=6030;Username=root;Password=taosdata;Database=power");
            conn.Open();

            var comm = ((TDengineCommand)conn.CreateCommand());
            comm.Connection = conn;
            comm.CommandText = "select * from power.meters ";
            TDengineDataAdapter ds = new TDengineDataAdapter(comm);
            var dt = new DataTable();
            ds.Fill(dt);
            conn.Close();
            return comm;
        }
        private static TDengineCommand ExecuteScalar()
        {
            using TDengineConnection conn =
                            new TDengineConnection("Host=localhost;Port=6030;Username=root;Password=taosdata;Database=power");
            conn.Open();

            var comm = ((TDengineCommand)conn.CreateCommand());
            comm.Connection = conn;
            comm.CommandText = "select count(*) from `power`.`meters` ";
            var dr = comm.ExecuteScalar();
            conn.Close();
            return comm;
        }
        private static TDengineCommand ExecuteScalar2()
        {
            using TDengineConnection conn =
                            new TDengineConnection("Host=localhost;Port=6030;Username=root;Password=taosdata;Database=power");
            conn.Open();

            var comm = ((TDengineCommand)conn.CreateCommand());
            comm.Connection = conn;
            comm.CommandText = "select count(*) from power.meters where ts=@ts";
            comm.Parameters.Add(new TDengineParameter("ts", Convert.ToDateTime("2018-10-03 14:38:05.000")));
            var dr = comm.ExecuteScalar();
            conn.Close();
            return comm;
        }
        private static TDengineCommand DataReader()
        {
            using TDengineConnection conn =
                            new TDengineConnection("Host=localhost;Port=6030;Username=root;Password=taosdata;Database=power");
            conn.Open();
            var comm = ((TDengineCommand)conn.CreateCommand());
            comm.Connection = conn;
            comm.CommandText = "select * from power.meters ";
            var dr = comm.ExecuteReader();
            dr.Read();
            var xx = dr.GetInt32(2);
            conn.Close();
            return comm;
        }

    }
}

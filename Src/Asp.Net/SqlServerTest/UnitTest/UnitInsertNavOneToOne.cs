using Admin.NET.Application;
using SqlSugar;
using System;
using System.Data;
 public class UnitInsertNavOneToOne
{
    public static void Init()
    {
        var db = new SqlSugarScope(new SqlSugar.ConnectionConfig()
        {
            ConnectionString = "server=.;uid=sa;pwd=sasa;database=Question",
            DbType = SqlSugar.DbType.SqlServer,
            IsAutoCloseConnection = true
        });
        db.DbMaintenance.CreateDatabase();
        //建表 
        if (!db.DbMaintenance.IsAnyTable("SysField", false) &&
            !db.DbMaintenance.IsAnyTable("SysFormField", false) &&
            !db.DbMaintenance.IsAnyTable("SysFormUpload", false))
        {
            db.CodeFirst.InitTables<SysField>();
            db.CodeFirst.InitTables<SysFormField>();
            db.CodeFirst.InitTables<SysFormUpload>();
        }


        //用例代码 

        SysFormField addSysFormFieldInput = new SysFormField()
        {
            FieldId = 0,
            Order = 0,
            DefaultValue = "",
            FieldName = "",
       
            FieldTypeName = "",
            FieldAlias = "",
          
            UpdateUserId=1,
            CreateUserId=1,
            IsDelete=false,
            FormUpload=new SysFormUpload() { CreateUserId=1,  CreateTime =DateTime.Now,UpdateTime=DateTime.Now},
            SysField=new SysField() { Order=1, CreateUserId=2, FieldTypeName ="a", FieldName="a", UpdateTime=DateTime.Now, CreateTime=DateTime.Now},
             CreateTime=DateTime.Now,
            UpdateTime=DateTime.Now 

        };

        var entity = addSysFormFieldInput;
       
        db.InsertNav(entity)
      .Include(x => x.SysField)
      .Include(x => x.FormUpload)
      .ExecuteCommand();

        Console.WriteLine("用例跑完");
       // Console.ReadKey();
    }


}

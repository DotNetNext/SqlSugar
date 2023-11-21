using OrmTest;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo
{
    /// <summary>
    /// 数据库帮助类
    /// </summary>
    public class UnitSelectN
    { 

        public static void Init()
        {
            var Db = NewUnitTest.Db;
            Db.CodeFirst.InitTables(typeof(UpFile), typeof(SpShangPin));

            int total = 0;
            Test0(Db, total);
            total = Test1(Db, total);
        }
        private static int Test0(SqlSugarClient Db, int total)
        {
            var list = Db.Queryable<SpShangPin>() 
                            
                            .Select(s => new ShangPinView()
                            {
                                
                                Image = new UploadFile() { Id = s.FileId, Url = s.Image.FilePath } 
                               
                            }).ToList();
            return total;
        }
        private static int Test1(SqlSugarClient Db, int total)
        {
            var list = Db.Queryable<SpShangPin>()
                            .Includes(s => s.Image)
                            .Select(s => new ShangPinView()
                            {
                                Id = s.Id,
                                Name = s.Name,
                                Price = s.Price,

                                Image = new UploadFile() { Id = s.FileId, Url = s.Image.FilePath },
                                FileName = s.Image.FilePath,
                                File = s.Image,
                                //Image = s.Image == null ? null : new UploadFile() { Id = s.FileId, Url = s.Image == null ? "" : s.Image.FilePath }
                            }).ToPageList(1, 2, ref total);
            return total;
        }
    }

    /// <summary>
    /// 商品表
    /// </summary>
    public class SpShangPin : Base
    {
        public string Name { get; set; }
        public int Price { get; set; }

        public Guid FileId { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(FileId), nameof(UpFile.Id))]
        public UpFile Image { get; set; }
    }

    /// <summary>
    /// 上传文件表
    /// </summary>
    public class UpFile : Base
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }

    /// <summary>
    /// 表公共字段
    /// </summary>
    public class Base
    {
        [SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; }
        public DateTime AddTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public bool IsDel { get; set; }
    }

    public class ShangPinView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public UploadFile Image { get; set; }
        public UpFile File { get; set; }
        public string FileName { get; set; }
    }

    public class UploadFile
    {
        [SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; }
        public string Url { get; set; }
    }
}

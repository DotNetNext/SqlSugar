using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlugarDemo
{
    /// <summary>
    /// 关联ERP系统的数据库
    /// </summary>
    public abstract class ERPEntityBaseId
    {
        /// <summary>
        /// ID 
        ///</summary>
        [SugarColumn(ColumnName = "SerialID", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
    }
    public abstract class ERPEntityBase : ERPEntityBaseId
    {
        /// <summary>
        /// RecordID
        /// </summary>
        [SugarColumn(ColumnName = "RecordID")]
        public string RecordID { get; set; }
    }

    public abstract class ERPEntityParentBase : ERPEntityBase
    {
    }

    public abstract class ERPEntitySubBase: ERPEntityBase
    {        

        /// <summary>
        /// ParentID
        /// </summary>
        [SugarColumn(ColumnName = "ParentID")]
        public string ParentID { get; set; }
    }
}

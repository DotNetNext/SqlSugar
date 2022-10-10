using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using SqlSugar;

namespace Admin.NET.Application
{
    /// <summary>
    /// 表单字段输入参数
    /// </summary>
    public class SysFormFieldInput 
    {
        /// <summary>
        /// 模型表单
        /// </summary>
        public virtual long FormId { get; set; }
        
        /// <summary>
        /// 基础字段
        /// </summary>
        public virtual long FieldId { get; set; }
        
        /// <summary>
        /// 功能模型
        /// </summary>
        public virtual long ModelId { get; set; }
        
        /// <summary>
        /// 数据库名称
        /// </summary>
        public virtual string FieldName { get; set; }
        
        /// <summary>
        /// 字段类型长度
        /// </summary>
        public virtual int TypeLength { get; set; }
        
        /// <summary>
        /// 类型名称
        /// </summary>
        public virtual string FieldTypeName { get; set; }
        
        /// <summary>
        /// 字段名称
        /// </summary>
        public virtual string FieldAlias { get; set; }
        
        /// <summary>
        /// 字段类型
        /// </summary>
        public virtual int FieldType { get; set; }
        
        /// <summary>
        /// 是否允许为空
        /// </summary>
        public virtual bool IsAllowNull { get; set; }
        
        /// <summary>
        /// 是否唯一
        /// </summary>
        public virtual bool IsOnly { get; set; }
        
        /// <summary>
        /// 是否只读
        /// </summary>
        public virtual bool IsReadOnly { get; set; }
        
        /// <summary>
        /// 默认值
        /// </summary>
        public virtual string DefaultValue { get; set; }
        
        /// <summary>
        /// 最大长度
        /// </summary>
        public virtual int MaxLength { get; set; }
        
        /// <summary>
        /// 是否显示在列表
        /// </summary>
        public virtual bool IsListShow { get; set; }
        
        /// <summary>
        /// 是否加入搜索
        /// </summary>
        public virtual bool IsSearchShow { get; set; }
        
        /// <summary>
        /// 提示文字
        /// </summary>
        public virtual string TipText { get; set; }
        
        /// <summary>
        /// 验证错误提示文字
        /// </summary>
        public virtual string ValidateText { get; set; }
        
        /// <summary>
        /// 验证类型
        /// </summary>
        public virtual string ValidateType { get; set; }
        
        /// <summary>
        /// 验证正则
        /// </summary>
        public virtual string ValidateExpression { get; set; }
        
        /// <summary>
        /// 排序
        /// </summary>
        public virtual long Order { get; set; }
        
    }

    public class AddSysFormFieldInput : SysFormFieldInput
    {
        /// <summary>
        /// 模型表单
        /// </summary>
        [Required(ErrorMessage = "模型表单不能为空")]
        public override long FormId { get; set; }
        
        /// <summary>
        /// 功能模型
        /// </summary>
        [Required(ErrorMessage = "功能模型不能为空")]
        public override long ModelId { get; set; }
        /// <summary>
        /// 上传模式
        /// </summary>
        public int UploadMode { get; set; }
        /// <summary>
        /// 是否水印
        /// </summary>
        public bool IsWaterMark { get; set; }

        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string FileExts { get; set; } = "jpg|png|gif|bmp";
        /// <summary>
        /// 1省/2市/3区
        /// </summary>
        public int ShowLevel { get; set; } = 1;
        /// <summary>
        /// 创建时间
        /// </summary>
        [SugarColumn(ColumnDescription = "创建时间")]
        public  DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [SugarColumn(ColumnDescription = "更新时间")]
        public  DateTime? UpdateTime { get; set; }



        /// <summary>
        /// 创建者Id
        /// </summary>
        [SugarColumn(ColumnDescription = "创建者Id")]
        public  long? CreateUserId { get; set; }

        /// <summary>
        /// 修改者Id
        /// </summary>
        [SugarColumn(ColumnDescription = "修改者Id")]
        public  long? UpdateUserId { get; set; }

        /// <summary>
        /// 软删除
        /// </summary>
        [SugarColumn(ColumnDescription = "软删除")]
        public  bool IsDelete { get; set; } = false;



    }

    public class DeleteSysFormFieldInput 
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required(ErrorMessage = "Id不能为空")]
        public long Id { get; set; }
        
    }

    public class UpdateSysFormFieldInput : SysFormFieldInput
    {
        /// <summary>
        /// Id
        /// </summary>
        [Required(ErrorMessage = "Id不能为空")]
        public long Id { get; set; }
        
    }

    public class QueryeSysFormFieldInput : DeleteSysFormFieldInput
    {

    }
}

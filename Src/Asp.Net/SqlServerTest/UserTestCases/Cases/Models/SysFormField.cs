

using SqlSugar;

/// <summary>
/// 模型实用字段
/// </summary>
[SugarTable("WebX_Dev_Form_Field", "表单字段")]
public class SysFormField : BaseModel
{
    /// <summary>
    /// 模型表单
    /// </summary>
    public long FormId { get; set; }
    /// <summary>
    /// 基础字段
    /// </summary>
    public long FieldId { get; set; }
    /// <summary>
    /// 功能模型Id
    /// </summary>
    public long ModelId { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    public string FieldName { get; set; }

    /// <summary>
    /// 字段类型长度
    /// </summary>
    public int TypeLength { get; set; }
    /// <summary>
    /// 类型名称
    /// </summary>
    public string FieldTypeName { get; set; }
    /// <summary>
    /// 字段名称
    /// </summary>
    public string FieldAlias { get; set; }
    /// <summary>
    /// 字段类型
    /// </summary>
    public int FieldType { get; set; }
    /// <summary>
    /// 是否允许为空
    /// </summary>
    public bool IsAllowNull { get; set; }
    /// <summary>
    /// 是否唯一
    /// </summary>
    public bool IsOnly { get; set; }
    /// <summary>
    /// 是否只读
    /// </summary>
    public bool IsReadOnly { get; set; }
    /// <summary>
    /// 默认值
    /// </summary>
    public string DefaultValue { get; set; }
    /// <summary>
    /// 最大长度
    /// </summary>
    public int MaxLength { get; set; }
    /// <summary>
    /// 是否显示在列表
    /// </summary>
    public bool IsListShow { get; set; }
    /// <summary>
    /// 是否加入搜索
    /// </summary>
    public bool IsSearchShow { get; set; }

    /// <summary>
    /// 功能字段
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(FieldId))]
    public SysField SysField { get; set; }

    /// <summary>
    /// 文件
    /// </summary>
    [Navigate(NavigateType.OneToOne, nameof(Id), nameof(SysFormUpload.Form_Field_Id))]
    public SysFormUpload FormUpload { get; set; }
}

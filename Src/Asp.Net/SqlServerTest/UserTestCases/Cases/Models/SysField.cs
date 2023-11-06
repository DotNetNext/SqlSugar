using SqlSugar;

/// <summary>
/// 模型实用字段
/// </summary>
[SugarTable("WebX_Dev_Field", "功能模型字段")]
public class SysField:BaseModel
{
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
    public string FieldAlias { get; set; } = "a";




    


}

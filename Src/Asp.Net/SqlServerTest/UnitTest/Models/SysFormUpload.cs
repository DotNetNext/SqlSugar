
using SqlSugar;


/// <summary>
/// 文件字段
/// </summary>
[SugarTable("WebX_Dev_Form_Upload", "文件字段")]
public class SysFormUpload : BaseModel
{
    public long FormId { get; set; }
    public long Form_Field_Id { get; set; }
    public long UploadMode { get; set; }
    public bool IsWaterMark { get; set; }
    public string FileExts { get; set; } = "";
}

using System;
namespace T4Test.Entities.Tables
{
    public class Flow_Step{
                        
     /// <summary>
     /// 说明:- 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string Id {get;set;}

     /// <summary>
     /// 说明:步骤名称 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string Name {get;set;}

     /// <summary>
     /// 说明:步骤说明 
     /// 默认:- 
     /// 可空:True 
     /// </summary>
    public string Remark {get;set;}

     /// <summary>
     /// 说明:排序 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public int Sort {get;set;}

     /// <summary>
     /// 说明:所属表单 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string FormId {get;set;}

     /// <summary>
     /// 说明:流转规则 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public string FlowRule {get;set;}

     /// <summary>
     /// 说明:该流程的 发起人/创建者 是否可以 自行选择 该步骤的审批者 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public Boolean IsCustom {get;set;}

     /// <summary>
     /// 说明:当规则或者角色被选择为多人时候，是否启用多人审核才通过 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public Boolean IsAllCheck {get;set;}

     /// <summary>
     /// 说明:执行者与规则对应 
     /// 默认:- 
     /// 可空:True 
     /// </summary>
    public string Execution {get;set;}

     /// <summary>
     /// 说明:是否可以强制完成整个流程 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public Boolean CompulsoryOver {get;set;}

     /// <summary>
     /// 说明:审核者是否可以编辑发起者的附件 
     /// 默认:- 
     /// 可空:False 
     /// </summary>
    public Boolean IsEditAttr {get;set;}

   }
            
}
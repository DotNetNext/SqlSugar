using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace ICrewApi.Entity
{
    ///<summary>
    ///机场信息
    ///</summary>
    [SugarTable("T_BAS_AIRPORT")]
    public partial class T_BAS_AIRPORT
    {
           public T_BAS_AIRPORT(){


           }
           /// <summary>
           /// Desc:机场四码
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public string AIRPORT_4CODE {get;set;}

           /// <summary>
           /// Desc:机场三字码
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public string AIRPORT_3CODE {get;set;}

           /// <summary>
           /// Desc:机场中文名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CHINESE_NAME {get;set;}

           /// <summary>
           /// Desc:机场英文名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ENGLISH_NAME {get;set;}

           /// <summary>
           /// Desc:机场中文简称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CHINESE_ABBR {get;set;}

           /// <summary>
           /// Desc:所在城市三字码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CITY_3CODE {get;set;}

           /// <summary>
           /// Desc:所在城市中文名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CITY_CH_NAME {get;set;}

           /// <summary>
           /// Desc:国际/国内(D国内I国际R地区)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string D_OR_I {get;set;}

           /// <summary>
           /// Desc:国家(字典)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string NATIVE {get;set;}

           /// <summary>
           /// Desc:时差
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? ZONE_TIME {get;set;}

           /// <summary>
           /// Desc:飞行标准报道时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? IN_TIME {get;set;}

           /// <summary>
           /// Desc:飞行标准离场时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? OUT_TIME {get;set;}

           /// <summary>
           /// Desc:报务区域(字典)X
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string RADIO_AREA {get;set;}

           /// <summary>
           /// Desc:是否外籍可飞(Y/N)X
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string FOREIGN_YN {get;set;}

           /// <summary>
           /// Desc:是否有休息场所(Y/N)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string REST_YN {get;set;}

           /// <summary>
           /// Desc:ICAO(3,4,5,6,7,8级)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? A_ICAO {get;set;}

           /// <summary>
           /// Desc:机场类型(1类2类3类=特殊、高原、高高原)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string A_CLS {get;set;}

           /// <summary>
           /// Desc:机场有效月数
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? A_CLS_CYCLE {get;set;}

           /// <summary>
           /// Desc:机场仪表等级(CAT-I CAT-II RVR)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string A_CATS {get;set;}

           /// <summary>
           /// Desc:程序方式-传统,RNP-AR
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string A_RNPAR {get;set;}

           /// <summary>
           /// Desc:长跑道/L	短跑道/S
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string A_RAILWAY {get;set;}

           /// <summary>
           /// Desc:操作人
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string OPER {get;set;}

           /// <summary>
           /// Desc:操作时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? OP_TIME {get;set;}

           /// <summary>
           /// Desc:备注
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string REMARKS {get;set;}

           /// <summary>
           /// Desc:IP地址
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string OPER_IP {get;set;}

           /// <summary>
           /// Desc:主机名
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string OPER_HOST {get;set;}

           /// <summary>
           /// Desc:特殊机场放飞
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string SPECIAL_YN {get;set;}

           /// <summary>
           /// Desc:报务机场放飞X
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string RADIO_YN {get;set;}

           /// <summary>
           /// Desc:外站值班人
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DUTY_PERSON {get;set;}

           /// <summary>
           /// Desc:外站值班电话
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string DUTY_TEL {get;set;}

           /// <summary>
           /// Desc:长航线Y/N
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string A_DISTANCE {get;set;}

           /// <summary>
           /// Desc:机长单飞区域(字典)X
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CAPT_AREA {get;set;}

           /// <summary>
           /// Desc:是否需要提前报备
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string CERT_YN {get;set;}

           /// <summary>
           /// Desc:开车时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? CAR_TIME {get;set;}

           /// <summary>
           /// Desc:特殊机场适用分公司串
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string SPECIAL_FILIALES {get;set;}

           /// <summary>
           /// Desc:休息时间(分钟)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? RESTTIME {get;set;}

           /// <summary>
           /// Desc:特殊机场放飞所属分公司
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string SPECIAL_OWNER {get;set;}

           /// <summary>
           /// Desc:领取武器包
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string WEAPONS_YN {get;set;}

           /// <summary>
           /// Desc:是否空警特殊机场
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string SPECIAL_POLIC {get;set;}

           /// <summary>
           /// Desc:机场类别 1:一类高原 2：二类高原 3：三类高原 ,来自t7501
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string AIRPORT_TYPE {get;set;}

           /// <summary>
           /// Desc:机场类别：4：港澳地区  5：台湾地区  6：日韩地区 7：东南亚地区  8：其他国际地区,来自t7501
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string AIRPORT_I {get;set;}

           /// <summary>
           /// Desc:是否双机长航线
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string TWIN_CAPTAIN_YN {get;set;}

           /// <summary>
           /// Desc:是否支持任务书打印
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string FTBPRINT {get;set;}

           /// <summary>
           /// Desc:机场可飞机型(飞管使用)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string AC_TYPES {get;set;}

           /// <summary>
           /// Desc:有值代表为带飞对标机场,如些机场带队,则其他相同类型的机场的带飞标准设置为此字段值
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? FNUMBER {get;set;}

           /// <summary>
           /// Desc:飞行标准进场时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? REPORT_TIME {get;set;}

           /// <summary>
           /// Desc:飞行标准前车程时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? PREVIOUS_RIDE_TIME_F {get;set;}

           /// <summary>
           /// Desc:飞行标准后车程时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? REAR_RIDE_TIME_F {get;set;}

           /// <summary>
           /// Desc:乘务标准报道时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? IN_TIME_C {get;set;}

           /// <summary>
           /// Desc:乘务标准进场时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? REPORT_TIME_C {get;set;}

           /// <summary>
           /// Desc:乘务标准离场时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? OUT_TIME_C {get;set;}

           /// <summary>
           /// Desc:乘务标准前车程时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? PREVIOUS_RIDE_TIME_C {get;set;}

           /// <summary>
           /// Desc:乘务标准后车程时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? REAR_RIDE_TIME_C {get;set;}

           /// <summary>
           /// Desc:空警标准报道时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? IN_TIME_A {get;set;}

           /// <summary>
           /// Desc:空警标准进场时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? REPORT_TIME_A {get;set;}

           /// <summary>
           /// Desc:空警标准离场时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? OUT_TIME_A {get;set;}

           /// <summary>
           /// Desc:空警标准前车程时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? PREVIOUS_RIDE_TIME_A {get;set;}

           /// <summary>
           /// Desc:空警标准后车程时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? REAR_RIDE_TIME_A {get;set;}

           /// <summary>
           /// Desc:飞行标准前车程时间2
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? PREVIOUS_RIDE_TIME_F2 {get;set;}

           /// <summary>
           /// Desc:飞行标准后车程时间2
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? REAR_RIDE_TIME_F2 {get;set;}

           /// <summary>
           /// Desc:货机标准报道时间
           /// Default:
           /// Nullable:False
           /// </summary>           
           public short IN_TIME_H {get;set;}

           /// <summary>
           /// Desc:货机标准进场时间
           /// Default:
           /// Nullable:False
           /// </summary>           
           public short REPORT_TIME_H {get;set;}

           /// <summary>
           /// Desc:货机标准离场时间
           /// Default:
           /// Nullable:False
           /// </summary>           
           public short OUT_TIME_H {get;set;}

           /// <summary>
           /// Desc:货机标准前车程时间
           /// Default:
           /// Nullable:False
           /// </summary>           
           public short PREVIOUS_RIDE_TIME_H {get;set;}

           /// <summary>
           /// Desc:货机标准后车程时间
           /// Default:
           /// Nullable:False
           /// </summary>           
           public short REAR_RIDE_TIME_H {get;set;}

           /// <summary>
           /// Desc:货机标准前车程时间2
           /// Default:
           /// Nullable:False
           /// </summary>           
           public short PREVIOUS_RIDE_TIME_H2 {get;set;}

           /// <summary>
           /// Desc:货机标准后车程时间2
           /// Default:
           /// Nullable:False
           /// </summary>           
           public short REAR_RIDE_TIME_H2 {get;set;}

           /// <summary>
           /// Desc:乘务标准国际报道时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? IN_TIME_C_I {get;set;}

           /// <summary>
           /// Desc:空保标准国际报道时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? IN_TIME_A_I {get;set;}

           /// <summary>
           /// Desc:日出裕度
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? UP_MARGIN {get;set;}

           /// <summary>
           /// Desc:日落裕度
           /// Default:
           /// Nullable:True
           /// </summary>           
           public short? DOWN_MARGIN {get;set;}

           /// <summary>
           /// Desc:是否一般高原机场
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string IS_YBGY_YN {get;set;}

           /// <summary>
           /// Desc:是否不校验夜航规则
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string IS_NO_CHECK_NIGHTAIR {get;set;}

           /// <summary>
           /// Desc:是否一般高原机场备注
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string IS_YBGY_YN_REMARK {get;set;}

           /// <summary>
           /// Desc:是否不校验夜航规则备注
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string IS_NO_NIGHTAIR_REMARK {get;set;}

           /// <summary>
           /// Desc:标识特殊机场(Y-是,N-否)-飞管用
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string SPECIAL_FLAG {get;set;}

    }
}

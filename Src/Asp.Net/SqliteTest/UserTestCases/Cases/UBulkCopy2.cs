using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public partial class NewUnitTest
    {
        public static string bulkData1 = @"{""data"":[{""id"":0,""robot_wx_id"":""wxid_0p3bww1ouyvi12"",""robot_serial_no"":""3927EE296470127DD1F1DF6274467A7A"",""device_user_serial_no"":""F27C2578D8004E9F8644EBC023C0A582"",""wx_account"":""lvqiujingliu1986"",""wx_password"":""8836MjQ2MWFhYWEyMzQ5"",""wx_alias"":""lvqiujingliu1986"",""nick_name"":""闾丘睛六"",""base64_nick_name"":""6Ze+5LiY552b5YWt"",""headimg_url"":""https://wx.qlogo.cn/mmhead/ver_1/VlhejJkdGe5YhcXdIVHYibxKbntVE85ZiayxtN2NnSibSViapXLEPY2mt1kHrZbqXnfgenBnNrp8Cx95YN1Uxic2E7Gzdfk2DSnYeUuR2QVpQJ28/132"",""email"":"""",""sex"":0,""enabled"":true,""type"":10,""status"":10,""seal_status"":10,""protocol_type"":2,""city_no"":"""",""province_no"":"""",""separation"":false,""parent_id"":0,""uin"":""1541723256"",""update_time"":""2022-01-12T09:57:13.377"",""create_time"":""2021-12-24T16:08:20.84"",""remark"":""机器人信息·数据初始化""},{""id"":0,""robot_wx_id"":""wxid_ln20qwymng8422"",""robot_serial_no"":""39289F28EAD96B4474A608919011FDEB"",""device_user_serial_no"":""20200917101004889010110000391"",""wx_account"":""ik50u73"",""wx_password"":""3uvhM3V2aDN1dmg2Z3Fl"",""wx_alias"":""ik50u73"",""nick_name"":""缪茹茹"",""base64_nick_name"":""57yq6Iy56Iy5"",""headimg_url"":""https://wx.qlogo.cn/mmhead/ver_1/Q7gjPhRR5ahEIP3DWyzIVvj1dPkGRTXia4hBTEY4VBCjq45jBkXc2A4ZwjWGhgKZ8AWDWt6LZpL3qic5dN2t5H5Nz1BI3iaqWlqBpZYticftusE/132"",""email"":"""",""sex"":2,""enabled"":true,""type"":10,""status"":10,""seal_status"":10,""protocol_type"":2,""city_no"":"""",""province_no"":"""",""separation"":false,""parent_id"":0,""uin"":""2342964524"",""update_time"":""2022-01-11T19:26:38.86"",""create_time"":""2020-07-08T14:26:57.907"",""remark"":""机器人信息·数据初始化""},{""id"":0,""robot_wx_id"":""wxid_0prnyg6n7aat22"",""robot_serial_no"":""39281E97D2F28A1BA848EED509D1D914"",""device_user_serial_no"":""20220105175222407210210002286"",""wx_account"":"""",""wx_password"":"""",""wx_alias"":"""",""nick_name"":""烈文"",""base64_nick_name"":""54OI5paH"",""headimg_url"":""https://wx.qlogo.cn/mmhead/ver_1/3S5EfQYud7JQ9bibib2sUHSE18HskHNF4yE6DrgvjRbBWawfreZlR6KR0ZhE5MrFWWv6b4JLHQdUUlWicqnWWvhgtdLpAlicURuHeD0k6qvX7dE/132"",""email"":"""",""sex"":2,""enabled"":true,""type"":30,""status"":10,""seal_status"":10,""protocol_type"":1,""city_no"":"""",""province_no"":"""",""separation"":false,""parent_id"":0,""uin"":""2483638563"",""update_time"":""2022-01-11T17:55:05.147"",""create_time"":""2021-10-15T13:30:50.637"",""remark"":""机器人信息·数据初始化""},{""id"":0,""robot_wx_id"":""wxid_rbhtxkd819ug21"",""robot_serial_no"":""3928E3EF41E241987190AAD5CD3896FA"",""device_user_serial_no"":""20211129100847022010210001354"",""wx_account"":"""",""wx_password"":"""",""wx_alias"":""jammy__C"",""nick_name"":""婕"",""base64_nick_name"":""5amV"",""headimg_url"":""https://wx.qlogo.cn/mmhead/ver_1/CB1XmYSqgFJ9no4HR6icKgnf8uJKGX8BQUqhsR9CoyFpqh3QxJCWMmGkslicXyVjiaCZyBjC9DL7eXBicOKnbe9kun5yW8cBlqSDhR0vIjGaOuQ/132"",""email"":"""",""sex"":2,""enabled"":true,""type"":30,""status"":10,""seal_status"":10,""protocol_type"":1,""city_no"":"""",""province_no"":"""",""separation"":false,""parent_id"":0,""uin"":""1083130244"",""update_time"":""2022-01-10T13:06:27.823"",""create_time"":""2021-09-27T11:33:16.327"",""remark"":""机器人信息·数据初始化""},{""id"":0,""robot_wx_id"":""wxid_opxbbhmzlakx12"",""robot_serial_no"":""392734726B89AFD98BD54F87A6F7A323"",""device_user_serial_no"":""C64EA13FFEDE4AC587E3E189CC32ED2B"",""wx_account"":""lugao1106"",""wx_password"":""9591Y2UxYnd3d3cxNTQ5"",""wx_alias"":""lugao1106"",""nick_name"":""逯缟"",""base64_nick_name"":""6YCv57yf"",""headimg_url"":""https://wx.qlogo.cn/mmhead/ver_1/6O6v8T7HbFiag6MJTQErfNdoNhG08wQx4rXApdQfjdcnxEZFcjlOmB1mFJ4OfBYhkvhibUzdCegsY0YiaYtwpEtpg/132"",""email"":"""",""sex"":0,""enabled"":true,""type"":10,""status"":10,""seal_status"":10,""protocol_type"":2,""city_no"":"""",""province_no"":"""",""separation"":false,""parent_id"":0,""uin"":""202535294"",""update_time"":""2022-01-11T11:18:08.897"",""create_time"":""2021-11-19T16:29:19.907"",""remark"":""机器人信息·数据初始化""},{""id"":0,""robot_wx_id"":""wxid_fvmabxeptk9n12"",""robot_serial_no"":""392857A86E6E48690FC4648588F22E24"",""device_user_serial_no"":""20220108112325107110110008239"",""wx_account"":"""",""wx_password"":"""",""wx_alias"":""jiangwi0603"",""nick_name"":""jason"",""base64_nick_name"":""amFzb24="",""headimg_url"":""https://wx.qlogo.cn/mmhead/ver_1/MEbn4icxcP6IDTiblzFfhfqwp0LaBAetP4Nobvk5xSTcd0jJtYljIN6BYh5nXEsCJyCGa0jSZrTczYcD0w1SaPb2eyYoKM0tLicx95Nz1q4XBQ/132"",""email"":""jiangwi0603@126.com"",""sex"":1,""enabled"":true,""type"":30,""status"":10,""seal_status"":10,""protocol_type"":1,""city_no"":"""",""province_no"":"""",""separation"":false,""parent_id"":0,""uin"":""3063392316"",""update_time"":""2022-01-11T11:17:28.987"",""create_time"":""2022-01-11T11:16:49.84"",""remark"":""机器人信息·数据初始化""},{""id"":0,""robot_wx_id"":""wxid_00bzjyic7zdu22"",""robot_serial_no"":""39294970652FF9BD5E3412FC24F49368"",""device_user_serial_no"":""20200825110115450710110000807"",""wx_account"":""e45r41wi"",""wx_password"":""oc77b2M3N29jNzc3bzZ1ZXg0bHo="",""wx_alias"":""e45r41wi"",""nick_name"":"" 小川小"",""base64_nick_name"":""IOWwj+W3neWwjw=="",""headimg_url"":""https://wx.qlogo.cn/mmhead/ver_1/FQCiayUo0amVG3s9nulZph8Jeicd1oxklDpL67Zj4dE3ntmSuiaOA54v1D0sxGXnhfeicwDHIAA9ZrbkWoYaGeWoco1rRPxyIU0Dn0cxfyY21T4/132"",""email"":"""",""sex"":1,""enabled"":true,""type"":10,""status"":10,""seal_status"":10,""protocol_type"":2,""city_no"":"""",""province_no"":"""",""separation"":false,""parent_id"":0,""uin"":""1811494957"",""update_time"":""2022-01-12T01:24:19.26"",""create_time"":""2020-04-03T00:18:51.277"",""remark"":""机器人信息·数据初始化""},{""id"":0,""robot_wx_id"":""wxid_11aabdewsz3421"",""robot_serial_no"":""39277ABBFE59C0C318D9271AE004BC8D"",""device_user_serial_no"":""20211217203325799310210000217"",""wx_account"":"""",""wx_password"":"""",""wx_alias"":"""",""nick_name"":""\""碎花洋裙__"",""base64_nick_name"":""IueijuiKsea0i+ijmV9f"",""headimg_url"":""https://wx.qlogo.cn/mmhead/ver_1/Cz96bFGqPJOeHuQLzM7M05QdN0IE6sichTg5SFCXVIqZCOXaLg1GNvkIWYVpwnfRJcTXcwia7eDkShRuPogulqXOVicgZ8pQWmAFjGxefqIYdE/132"",""email"":"""",""sex"":1,""enabled"":true,""type"":30,""status"":10,""seal_status"":10,""protocol_type"":1,""city_no"":"""",""province_no"":"""",""separation"":false,""parent_id"":0,""uin"":""1947658881"",""update_time"":""2022-01-11T02:25:21.407"",""create_time"":""2021-10-22T15:18:50.34"",""remark"":""机器人信息·数据初始化""},{""id"":0,""robot_wx_id"":""wxid_c6iha3x4cxp622"",""robot_serial_no"":""39288C70A34DFF820A150FF29C5E99EE"",""device_user_serial_no"":""A02A33A4DDB54CD1834A21BB55F886FC"",""wx_account"":""15525425235"",""wx_password"":""9FD7OWIyOHh4eHg1MjM1"",""wx_alias"":""a15525425235"",""nick_name"":""华秀莲"",""base64_nick_name"":""5Y2O56eA6I6y"",""headimg_url"":""https://wx.qlogo.cn/mmhead/ver_1/zupZQgKfg5Y6IyZfrYkzNjeIGYEJmQKjOTOXD78UnvibUqunpl04aldGqSYibj44opN53QMZDxa9T3cNib4hM4vqA/0"",""email"":"""",""sex"":0,""enabled"":true,""type"":10,""status"":10,""seal_status"":10,""protocol_type"":4,""city_no"":"""",""province_no"":"""",""separation"":false,""parent_id"":0,""uin"":""925715359"",""update_time"":""2022-01-12T10:17:48.453"",""create_time"":""2021-11-23T23:14:18.97"",""remark"":""机器人信息·数据初始化""},{""id"":0,""robot_wx_id"":""wxid_4l1ivs17ftb222"",""robot_serial_no"":""39269C2B524054811AFCAED61A7C9157"",""device_user_serial_no"":""20210520150600703910110004389"",""wx_account"":""stluohuiofgin"",""wx_password"":""627qNjI3cTYyN3F0d3dnMQ=="",""wx_alias"":""stluohuiofgin"",""nick_name"":""卜爽荷"",""base64_nick_name"":""5Y2c54i96I23"",""headimg_url"":""https://wx.qlogo.cn/mmhead/ver_1/jUwh2gwmQMpBshR4gSkRlIDKdU76eRHKsibebr0abKl8AnevI0T8GaYeHicicEiaia12efM4pBaPKxD5FLaRSZDdZRveYMZibyKQlewxck9wSia2ho/132"",""email"":"""",""sex"":0,""enabled"":true,""type"":10,""status"":10,""seal_status"":10,""protocol_type"":2,""city_no"":"""",""province_no"":"""",""separation"":false,""parent_id"":0,""uin"":""2973613556"",""update_time"":""2022-01-11T05:36:24.143"",""create_time"":""2021-05-20T15:05:46.317"",""remark"":""机器人信息·数据初始化""},{""id"":0,""robot_wx_id"":""wxid_hdjl4c1rvhzm22"",""robot_serial_no"":""39283929293888060537D621C0C07670"",""device_user_serial_no"":""20211211080701699410210000733"",""wx_account"":"""",""wx_password"":"""",""wx_alias"":"""",""nick_name"":""开心果2"",""base64_nick_name"":""5byA5b+D5p6cMg=="",""headimg_url"":""https://wx.qlogo.cn/mmhead/ver_1/CdiatnVopdM5oI6oQYoVaB7XObU6afg81hwVfJm1LvmP2dkjqT9stOJB4QGC6RSjdskuXjshDjLlt9nZCyMOiaLOOyCBmuIOV9RjEic3iaYyibDU/132"",""email"":"""",""sex"":2,""enabled"":true,""type"":30,""status"":10,""seal_status"":10,""protocol_type"":1,""city_no"":"""",""province_no"":"""",""separation"":false,""parent_id"":0,""uin"":""2709509370"",""update_time"":""2022-01-11T02:24:59.55"",""create_time"":""2021-09-11T06:29:24.603"",""remark"":""机器人信息·数据初始化""},{""id"":0,""robot_wx_id"":""wxid_ii0eb7q59cea22"",""robot_serial_no"":""392929B4976E9C3877F875BCDB68D7DF"",""device_user_serial_no"":""20210328130254263510110001471"",""wx_account"":""yyvliansuobh"",""wx_password"":""kd81a2Q4MWtkODFydXlwZzB0eQ=="",""wx_alias"":""yyvliansuobh"",""nick_name"":""赵苑"",""base64_nick_name"":""6LW16IuR"",""headimg_url"":""https://wx.qlogo.cn/mmhead/ver_1/aqvibe2kL44hQrUZBd0SzpvVkwm5ORCUcSdmF2TeS29zu6ZXR5ECJt4WS3FHathburCYQGWYFaS9OfNOcjYddLTibbZM7J4zRlDFVxsqZWicMc/132"",""email"":"""",""sex"":0,""enabled"":true,""type"":10,""status"":10,""seal_status"":10,""protocol_type"":2,""city_no"":"""",""province_no"":"""",""separation"":false,""parent_id"":0,""uin"":""2463150744"",""update_time"":""2022-01-12T01:07:40.733"",""create_time"":""2021-03-28T13:02:39.89"",""remark"":""机器人信息·数据初始化""},{""id"":0,""robot_wx_id"":""wxid_4urf1eo9lh6922"",""robot_serial_no"":""392768D516AD31B5DD63FF7FE1F96234"",""device_user_serial_no"":""20200803170607244010110002026"",""wx_account"":""re41lii8zn"",""wx_password"":""o4d1bzRkMW80ZDF5ZDJrM3N4OTI="",""wx_alias"":""re41lii8zn"",""nick_name"":""小娥骄"",""base64_nick_name"":""5bCP5ail6aqE"",""headimg_url"":""https://wx.qlogo.cn/mmhead/ver_1/wjAGzPlkibvAXIxFbHgobEbgqZNQOg0Ef8XEnvyX5ibciaPzdW8icS3ndjtIDicnnGZMg7IcplZGCeWdywqgkGichg6s7r1sJb3jLRcxkIQm3KA2Q/132"",""email"":"""",""sex"":2,""enabled"":true,""type"":10,""status"":10,""seal_status"":10,""protocol_type"":2,""city_no"":"""",""province_no"":"""",""separation"":false,""parent_id"":0,""uin"":""2873679361"",""update_time"":""2022-01-11T21:12:34.92"",""create_time"":""2020-03-28T23:34:47.013"",""remark"":""机器人信息·数据初始化""},{""id"":0,""robot_wx_id"":""wxid_swjhsaa2gljn22"",""robot_serial_no"":""39285985503DA5A84FC9028D72D45FFA"",""device_user_serial_no"":""20211207033654739920310255216"",""wx_account"":"""",""wx_password"":"""",""wx_alias"":""LVOE1628"",""nick_name"":""静心"",""base64_nick_name"":""6Z2Z5b+D"",""headimg_url"":""https://wx.qlogo.cn/mmhead/ver_1/hR5Gqfefr4pkPMv1E4DLyRBVryRXxe0ajpuR9JHMsiabBQXtdoic5rM9LIeEOABt6npy9eI88VJyfPjmYpuILAXA/132"",""email"":"""",""sex"":0,""enabled"":true,""type"":30,""status"":10,""seal_status"":10,""protocol_type"":1,""city_no"":"""",""province_no"":"""",""separation"":false,""parent_id"":0,""uin"":""373620837"",""update_time"":""2022-01-11T04:07:13.367"",""create_time"":""2021-12-06T21:48:28.217"",""remark"":""机器人信息·数据初始化""},{""id"":0,""robot_wx_id"":""wxid_t5rkwg6psqta12"",""robot_serial_no"":""392955F2CE8D0D85AC40DB5636F5ADFC"",""device_user_serial_no"":""20210302105529014210110000115"",""wx_account"":""Rjisngzhu109588"",""wx_password"":""frapZnJhcGZyYXBwOHkxOWlp"",""wx_alias"":""Rjisngzhu109588"",""nick_name"":""陈小凤"",""base64_nick_name"":""6ZmI5bCP5Yek"",""headimg_url"":""http://wx.qlogo.cn/mmhead/ver_1/LDkfKDR0sO5SAWjHP8c93Vx3FRK4u49mZ6HRkmSt9cOZwNndopH6Gz5x4K8UFC8Iv0u2cYROuqqCvH8v3DQnLqJqanKiaM4eT05ZMbg9XUgA/132"",""email"":"""",""sex"":0,""enabled"":true,""type"":10,""status"":10,""seal_status"":10,""protocol_type"":2,""city_no"":"""",""province_no"":"""",""separation"":false,""parent_id"":0,""uin"":""3414889675"",""update_time"":""2022-01-11T04:25:09.963"",""create_time"":""2021-03-02T10:55:19.837"",""remark"":""机器人信息·数据初始化""},{""id"":0,""robot_wx_id"":""wxid_1du1f7hsnj8422"",""robot_serial_no"":""39280F6BA859EE89842FA77ABB0DC039"",""device_user_serial_no"":""D572F85AA4B641D49C21FE5B14508F29"",""wx_account"":""15534029941"",""wx_password"":""96FBMjViN3p6eno5OTQx"",""wx_alias"":""ydh15534029941"",""nick_name"":""祁云亭"",""base64_nick_name"":""56WB5LqR5Lqt"",""headimg_url"":""https://wx.qlogo.cn/mmhead/ver_1/nDCibWBVKQI3aMNpuiblF6YFHvFiaDN99ZINVZAShsgyx5cGmxu2H1hw4cQtKBD1r3ESYHMUVQnIwPp6BecK3TlEFWRYYayYH8sr3ibUPKbUc9U/0"",""email"":"""",""sex"":0,""enabled"":true,""type"":10,""status"":10,""seal_status"":10,""protocol_type"":4,""city_no"":"""",""province_no"":"""",""separation"":false,""parent_id"":0,""uin"":""18446744072072878797"",""update_time"":""2022-01-11T06:28:40.86"",""create_time"":""2022-01-09T17:03:50.27"",""remark"":""机器人信息·数据初始化""},{""id"":0,""robot_wx_id"":""wxid_btoq5obkvia312"",""robot_serial_no"":""3928C99084242D85837E58D49DD12F42"",""device_user_serial_no"":""B59DD35FEB764162990BFA8559F7408F"",""wx_account"":""19103155341"",""wx_password"":""cal8Y2FsOGNhbDg3eG5jMXc="",""wx_alias"":""Ks15904265536"",""nick_name"":""司寇恋云"",""base64_nick_name"":""5Y+45a+H5oGL5LqR"",""headimg_url"":""https://wx.qlogo.cn/mmhead/ver_1/YdJvibiaA1X7KxedvZQ0z7gibD35QYCrItx9AvgO3CnbCZNELLloFbic2xecGHZbEv6IegTUHCQzHpxDTXRwtSbqiaib603SPw2fwdyO1ux5E6oZ0/132"",""email"":"""",""sex"":2,""enabled"":true,""type"":10,""status"":10,""seal_status"":10,""protocol_type"":2,""city_no"":"""",""province_no"":"""",""separation"":false,""parent_id"":0,""uin"":""1018112863"",""update_time"":""2022-01-12T04:56:09.393"",""create_time"":""2020-12-16T15:01:19.217"",""remark"":""机器人信息·数据初始化""},{""id"":0,""robot_wx_id"":""wxid_b7qz88olje4x21"",""robot_serial_no"":""39269A93540340FFB5BB6080E180FB48"",""device_user_serial_no"":""20211119182956000410210024676"",""wx_account"":"""",""wx_password"":"""",""wx_alias"":""guanmengsi_"",""nick_name"":""??梦·思小厨娘"",""base64_nick_name"":""8J+RhOaipsK35oCd5bCP5Y6o5aiY"",""headimg_url"":""https://wx.qlogo.cn/mmhead/ver_1/OzeZ5ia3bn36BTjgLAZYAUmNNmSLgV5E1wcEcmDNNh5G4ZLjdLPw3GASUZ6iaMzDVtWCRMWibx9tIseedicicibAVVJKjT8BkaI6WrVjjNwuZUQQs/132"",""email"":"""",""sex"":2,""enabled"":true,""type"":30,""status"":10,""seal_status"":10,""protocol_type"":1,""city_no"":"""",""province_no"":"""",""separation"":false,""parent_id"":0,""uin"":""1753890521"",""update_time"":""2022-01-11T17:28:39.473"",""create_time"":""2021-10-12T16:41:17.513"",""remark"":""机器人信息·数据初始化""},{""id"":0,""robot_wx_id"":""wxid_pu59oaivtqs422"",""robot_serial_no"":""392838309D46DDE1A10788FD4A170D07"",""device_user_serial_no"":""20210705141209419610110001562"",""wx_account"":""dijiaohuan1958"",""wx_password"":""i2dyaTJkeWkyZHlhenVmeWx4Yg=="",""wx_alias"":""dijiaohuan1958"",""nick_name"":""经燕琴"",""base64_nick_name"":""57uP54eV55C0"",""headimg_url"":""https://wx.qlogo.cn/mmhead/ver_1/IMGo9t3plEjOd4KA3iaaQk5NFfma3VrXkBCDia6q1R2lsxASZ3lsSahUYdvtBRyUPqteWd5tiaEmYdC6cGXeHue3IQ1f7OjYpvSSQwh7nZpbqI/132"",""email"":"""",""sex"":0,""enabled"":true,""type"":10,""status"":10,""seal_status"":10,""protocol_type"":2,""city_no"":"""",""province_no"":"""",""separation"":false,""parent_id"":0,""uin"":""38429044"",""update_time"":""2022-01-11T09:46:16.657"",""create_time"":""2021-07-05T14:11:39.643"",""remark"":""机器人信息·数据初始化""},{""id"":0,""robot_wx_id"":""wxid_0aax7lsdiql912"",""robot_serial_no"":""3928E79621B2BE780382762047ECE703"",""device_user_serial_no"":""986D325458CB4E69841A31720C2439C4"",""wx_account"":""18852589379"",""wx_password"":""pfolcGZvbHBmb2xhcHhuM2ZxNzE="",""wx_alias"":""XDMx3sngFNF"",""nick_name"":""柏白枫"",""base64_nick_name"":""5p+P55m95p6r"",""headimg_url"":""https://wx.qlogo.cn/mmhead/ver_1/FQ5pJrib0L2kluAGfbV5HZXH96v0rnGqwMsRNTVI8mwUwQuAYkAB3zHV3p5MGpQibCp6g9eVkCrsviaiaJ6aZzibGzibCRO9I98wWtaCu7revWkd8/132"",""email"":"""",""sex"":0,""enabled"":true,""type"":10,""status"":10,""seal_status"":10,""protocol_type"":2,""city_no"":"""",""province_no"":"""",""separation"":false,""parent_id"":0,""uin"":""160768034"",""update_time"":""2022-01-12T09:37:08.833"",""create_time"":""2020-12-04T16:30:41.407"",""remark"":""机器人信息·数据初始化""}]}";

        public static void Bulk()
        {
            if (Db.DbMaintenance.IsAnyTable("UnitIdentity1",false)) 
            {
                Db.DbMaintenance.DropTable("UnitIdentity1");
            }
            Db.CodeFirst.InitTables<UnitIdentity1>();
            var data = new UnitIdentity1()
            {
                Name = "jack"
            };
            Db.Fastest<UnitIdentity1>().BulkCopy(new List<UnitIdentity1>() {
              data
            });
            var list = Db.Queryable<UnitIdentity1>().ToList();
            if (list.Count != 1 || data.Name != list.First().Name)
            {
                throw new Exception("unit Bulk");
            }
            data.Name = "2";
            Db.Fastest<UnitIdentity1>().BulkCopy(new List<UnitIdentity1>() {
              data,
              data
            });
            list = Db.Queryable<UnitIdentity1>().ToList();
            if (list.Count != 3 || !list.Any(it => it.Name == "2"))
            {
                throw new Exception("unit Bulk");
            }
            Db.Fastest<UnitIdentity1>().BulkUpdate(new List<UnitIdentity1>() {
               new UnitIdentity1(){
                Id=1,
                 Name="222"
               },
                 new UnitIdentity1(){
                Id=2,
                 Name="111"
               }
            });
            list = Db.Queryable<UnitIdentity1>().ToList();
            if (list.First(it => it.Id == 1).Name != "222")
            {
                throw new Exception("unit Bulk");
            }
            if (list.First(it => it.Id == 2).Name != "111")
            {
                throw new Exception("unit Bulk");
            }
            if (list.First(it => it.Id == 3).Name != "2")
            {
                throw new Exception("unit Bulk");
            }
            Db.CodeFirst.InitTables<UnitIdentity111>();
            Db.DbMaintenance.TruncateTable<UnitIdentity111>();
            var count = Db.Fastest<UnitIdentity111111111>().AS("UnitIdentity111").BulkCopy(new List<UnitIdentity111111111> {
              new UnitIdentity111111111(){ Id=1, Name="jack" }
            });
            if (count == 0)
            {
                throw new Exception("unit Bulk");
            }
            count = Db.Fastest<UnitIdentity111111111>().AS("UnitIdentity111").BulkUpdate(new List<UnitIdentity111111111> {
              new UnitIdentity111111111(){ Id=1, Name="jack" }
            });
            if (count == 0)
            {
                throw new Exception("unit Bulk");
            }
            Db.CodeFirst.InitTables<UnitTable001>();
            Db.Fastest<UnitTable001>().BulkUpdate(new List<UnitTable001> {
              new UnitTable001(){   Id=1, table="a" }
            });

            Db.CodeFirst.InitTables<UnitBulk23131>();
            Db.DbMaintenance.TruncateTable<UnitBulk23131>();
            Db.Fastest<UnitBulk23131>().BulkCopy(new List<UnitBulk23131> {
            new UnitBulk23131()
            {
                Id = 1,
                table = false
            }
            });
            var list1 = Db.Queryable<UnitBulk23131>().ToList();
            SqlSugar.Check.Exception(list1.First().table == true, "unit error");
            Db.Fastest<UnitBulk23131>().BulkUpdate(new List<UnitBulk23131> {
            new UnitBulk23131()
            {
                Id = 1,
                table = true
            }
            });
            var list2 = Db.Queryable<UnitBulk23131>().ToList();
            SqlSugar.Check.Exception(list2.First().table == false, "unit error");

            Db.DbMaintenance.TruncateTable<UnitBulk23131>();
            Db.Fastest<UnitBulk23131>().BulkCopy(new List<UnitBulk23131> {
            new UnitBulk23131()
            {
                Id = 1,
                table = true
            }
            });
            var list3 = Db.Queryable<UnitBulk23131>().ToList();
            SqlSugar.Check.Exception(list3.First().table == false, "unit error");
            Db.Fastest<UnitBulk23131>().BulkUpdate(new List<UnitBulk23131> {
            new UnitBulk23131()
            {
                Id = 1,
                table = false
            }
            });
            list3 = Db.Queryable<UnitBulk23131>().ToList();
            SqlSugar.Check.Exception(list3.First().table == true, "unit error");

            Db.DbMaintenance.TruncateTable<UnitBulk23131>();
            Db.Fastest<UnitBulk23131>().BulkCopy(new List<UnitBulk23131> {
            new UnitBulk23131()
            {
                Id = 1,
                table = null
            }
            });
            var list4 = Db.Queryable<UnitBulk23131>().ToList();
            SqlSugar.Check.Exception(list4.First().table == true, "unit error");

            Db.CodeFirst.InitTables<UnitBulkCopy1231>();
            Db.CodeFirst.InitTables<UnitDatum>();
            var rootData=Db.Utilities.DeserializeObject<Rootobject>(bulkData1);
            var inserData = rootData.data.ToList();
            var num= Db.Fastest<UnitDatum>().BulkCopy(inserData);
            SqlSugar.Check.Exception(num != 20, "unit error");
        }
    }

    public class Rootobject
    {
        public UnitDatum[] data { get; set; }
    }

    public class UnitDatum
    {
        public int id { get; set; }
        public string robot_wx_id { get; set; }
        public string robot_serial_no { get; set; }
        public string device_user_serial_no { get; set; }
        public string wx_account { get; set; }
        public string wx_password { get; set; }
        public string wx_alias { get; set; }
        public string nick_name { get; set; }
        public string base64_nick_name { get; set; }
        public string headimg_url { get; set; }
        public string email { get; set; }
        public int sex { get; set; }
        public bool enabled { get; set; }
        public int type { get; set; }
        public int status { get; set; }
        public int seal_status { get; set; }
        public int protocol_type { get; set; }
        public string city_no { get; set; }
        public string province_no { get; set; }
        public bool separation { get; set; }
        public int parent_id { get; set; }
        public string uin { get; set; }
        public DateTime update_time { get; set; }
        public DateTime create_time { get; set; }
        public string remark { get; set; }
    }

    public class UnitBulkCopy1231
    {
        public string Id { get; set; }
    }
    public class UnitBulk23131
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        [SqlSugar.SugarColumn(ColumnDataType = "tinyint", Length = 1, IsNullable = true)]
        public bool? table { get; set; }
    }
    public class UnitTable001
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        public string table { get; set; }
    }

    public class UnitIdentity111
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class UnitIdentity111111111
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class UnitIdentity1
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

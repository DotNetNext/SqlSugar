# SonnetDB 验证

依赖公开 SonnetDB 5.0.0，使用 .NET 10。本次验证使用 NuGet 正式包，包内程序集来源为 SonnetDB 5.0.0 公共发布审计记录的 `84433f9edcceb84605520ebecc4b2879e3c4aa79`；上游 PR 合并和后续 SqlSugar 发行分别验收。运行：

```powershell
dotnet run --project Src/Asp.NetCore2/SonnetDBTest/SonnetDBTest.csproj
```

在独立临时数据库执行功能回归并自动清理，包含一万行 Insertable/Updateable/Fastest 插入和集合更新。通过 OnExecutingChangeSql 统计实际命令，确认每批一条集合 SQL，覆盖自增键、部分列、批次选项、decimal 小数、TIME、窗口、日期、JSON、复合键及事务回滚。

完整性检查：

```powershell
dotnet run --project Src/Asp.NetCore2/SonnetDBTest/SonnetDBTest.csproj -- --check-capabilities
```

完整检查包含16组功能和8组能力，要求全部通过并返回0。额外能力为标准 CodeFirst 的事务 DDL、多结果集、原生临时表、TranLock、高有效位 decimal、显式 DATETIME2 的100ns参数、旧 DATETIME 的非静默拒绝和 ALTER DECIMAL 精度元数据。

5.0 的 DATETIME 保留毫秒合同；输入含亚毫秒时明确拒绝。需要100ns时，列声明为 DATETIME2，`SugarParameter.DbType` 显式设为 `System.Data.DbType.DateTime2`。默认 DateTime 映射继续使用 DATETIME，不在客户端偷偷截断 ticks。TIME 的运行绑定使用 SqlSugar 已有 TimeSpan 类型路径，DbFirst 生成 TimeOnly 属性；两种实体和可空读回都有实际断言。

元数据建表通过已有 DbMaintenance 接口准备功能用例；基类 CodeFirst 单独接受完整性检查。QueryChecks 保留窗口、字符串位置、日期时间回读、JSON 存在/缺失、关联更新及差异日志的语义回归。MetadataChecks 实际编译 DbFirst 生成模型，在 `de-DE` 文化下检查默认值，并验证可空 TimeOnly/TimeSpan 实体读回。

设置 `SQLSUGAR_SONNETDB_CONNECTION` 可让同一完整套件连接独占的真实 REST 或 frame-http2 数据库。三种模式必须分别使用新数据库；程序集身份随输出记录。Frame 模式还须核对 Server 实际 HTTP/2/Frame 请求日志，不能只看连接字符串。

候选提供程序组合原生 TEMP CTAS、读取器成功关闭后回写冻结输出参数、实际主键约束名和自增下一值等适配。Fastest 沿用已有集合操作、调用方事务与失败清理；没有逐条更新回退。切换既有列的 identity 声明仍明确拒绝，不通过删除重建列模拟；SQL 数据库备份语句也不受支持，应使用产品备份 API。

详见 [兼容说明](../../../docs/SonnetDB-compatibility.md) 和 [数据库 issue](../../../docs/SonnetDB-issues.md)。候选包、正式公开包、上游合并、生产负载与物理断电证据保持独立。

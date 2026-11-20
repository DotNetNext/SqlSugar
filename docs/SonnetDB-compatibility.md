# SonnetDB 与 SqlSugar 适配

验证日期为 2026-11-21。适配依赖公开 NuGet `SonnetDB 5.0.0`，目标框架为 `net10.0`。本次使用公开包重新执行 `SonnetDBTest --check-capabilities`，并核对 2026-11-04 的 SonnetDB 5.0.0 发布审计。候选包、公开包、上游合入和真实部署仍分别记录。

## 适配原则

SonnetDB 是 SqlSugar 的一个数据库提供程序。适配层实现连接、参数、SQL 方言、批量写入和元数据方言，通用查询、表达式解析、实体映射、CodeFirst 及 Fastest 流程继续复用 SqlSugar。

数据库已经提供的能力通过现有 SqlSugar 公开入口直接使用。适配层不添加能力白名单，不把已支持的入口提前拒绝，不复制数据库解析器，不改调用方事务边界，也不把逐行执行当作批量实现。

## SqlSugar 公共代码接缝

相对上游 `DotNetNext/SqlSugar:master`，本 PR 只保留 SonnetDB 所需的兼容接缝。

| 文件 | 兼容用途 |
| --- | --- |
| `Enum/DbType.cs` | 注册 `DbType.SonnetDB`。 |
| `Abstract/SugarProvider/SqlSugarAccessory.cs` | 按现有外置 provider 机制加载 `SqlSugar.SonnetDBCore`。 |
| `Abstract/CodeFirstProvider/CodeFirstProvider.cs` | 提供受保护的主键约束名钩子，由 SonnetDB 使用真实约束目录名。 |
| `Abstract/DbFirstProvider/DbFirstProvider.cs` | 提供受保护的默认值类型转换钩子，由 SonnetDB 处理原生类型。 |
| `Abstract/FastestProvider/Private.cs` | 仅向 SonnetDB FastBuilder 转发已有的 `IsOffIdentity`、`BatchSize` 和 `IsIgnoreInsertError`。 |
| `Abstract/InsertableProvider/InsertableHelper.cs` | 仅对 PostgreSQL 与 SonnetDB 使用已有的主键类型转换，保留其他 provider 的原有条件。 |
| `Utilities/UtilMethods.cs` | 补齐 `TimeOnly` 与 `TimeSpan` 的既有读回转换路径。 |

这些改动没有新增 SqlSugar 公共接口，也没有修改其他 provider 的 SQL 生成、事务、分页、表达式或执行流程。

## 批量写入

| 入口 | 执行方式 | 一万行结果 |
| --- | --- | --- |
| `Insertable(list).PageSize(1000)` | 每批一条参数化多值 `INSERT` | 10 批，影响行数和读回数据一致 |
| `Updateable(list).PageSize(1000)` | 每批一条参数化 `UPDATE ... CASE` | 10 批，主键和复合键均通过 |
| `Fastest.BulkCopy` | 参数化多值 `INSERT` | 10 批，实际写入一万行 |
| `Fastest.BulkUpdate` | 工作表批量导入后单条 `UPDATE ... FROM` | 实际更新一万行 |
| `Fastest.BulkMerge` | 集合 `UPDATE` 后 `INSERT ... SELECT ... WHERE NOT EXISTS` | 插入和更新结果一致 |

命令计数、影响行数、读回值、部分列、自增键、复合键、`OffIdentity`、`BatchSize`、重复键忽略和事务回滚均由 `SonnetDBTest` 覆盖。批内循环只用于构造 SQL 和参数，每批只执行一次数据库命令。

## 完整性检查

公开 `SonnetDB 5.0.0` 包上的 `--check-capabilities` 通过以下合同。

- 基类 CodeFirst 的事务 DDL
- ADO 多语句与 `NextResult`
- 原生会话临时表与 CTAS
- 事务行锁与 NOWAIT
- 高有效位 decimal 参数
- 显式 `DATETIME2` 的 100ns 参数
- 旧 `DATETIME` 对亚毫秒参数的明确拒绝
- `ALTER DECIMAL(p,s)` 精度元数据

同一套公开包回归还通过维护接口、同步和异步自增键、分页、窗口、JSON、日期、字符串、复合键、关联更新、删除、DbFirst 模型编译和集合 DML 回滚。

## 三协议消费者证据

SonnetDB 5.0.0 发布审计记录了 provider source `18f9df7ede7b575b6caba5f46bcc070a4da95784` 的三协议结果。本次后续提交只收窄通用接缝的适用范围，未改变 SonnetDB provider 的 SQL 或执行路径。

| 协议 | 结果 |
| --- | --- |
| embedded | 93/93 PASS |
| REST | 93/93 PASS |
| HTTP/2 Frame | 93/93 PASS |

审计文件为 [release-5.0-20261104-public-acceptance.json](https://github.com/IoTSharp/SonnetDB/blob/main/docs/audits/release-5.0-20261104-public-acceptance.json)。公开包的 `SonnetDB.Core`、`SonnetDB.Data` 和 `SonnetDB.Protocol` 均来自 SonnetDB 5.0.0 正式发布物。

## 交付边界

本 PR 已完成 SonnetDB 公开入口所需的适配和回归。SqlSugar 上游维护者仍需按其仓库流程将 PR 转为 Ready for review 并合入。该结论不扩展为所有 SqlSugar provider 的完整性承诺，也不替代上游合入后的发行版本验收。

运行验证项目的命令如下。

```powershell
dotnet build Src/Asp.NetCore2/SonnetDBTest/SonnetDBTest.csproj -c Release
dotnet run --project Src/Asp.NetCore2/SonnetDBTest/SonnetDBTest.csproj -c Release -- --check-capabilities
```

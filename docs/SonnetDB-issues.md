# SonnetDB 所需数据库能力

本页按 SqlSugar PR #1457 的公开入口重新核对，验证基线为公开 `SonnetDB 5.0.0`。SonnetDB 侧能力已经完成或明确标记产品边界，适配层只接入已有 SqlSugar 接口。

## 状态矩阵

| SonnetDB issue | 所需合同 | 当前状态 | SqlSugar 影响 |
| --- | --- | --- | --- |
| [#223](https://github.com/IoTSharp/SonnetDB/issues/223) | 关系表字符串聚合 | 已完成并关闭 | `SelectStringJoin` 可使用原生聚合。 |
| [#226](https://github.com/IoTSharp/SonnetDB/issues/226) | 有数据表的主键结构演进 | 已完成并关闭 | CodeFirst 存量主键迁移有真实回归。 |
| [#227](https://github.com/IoTSharp/SonnetDB/issues/227) | 关系 DDL 事务提交与回滚 | 已完成并关闭 | 基类 CodeFirst 事务 DDL 检查通过。 |
| [#229](https://github.com/IoTSharp/SonnetDB/issues/229) | 多 SELECT 与 `NextResult` | 已完成并关闭 | ADO 多结果集检查通过。 |
| [#233](https://github.com/IoTSharp/SonnetDB/issues/233) | 普通事务锁定读 | 已完成并关闭 | `TranLock(Wait)` 入口有真实回归。 |
| [#235](https://github.com/IoTSharp/SonnetDB/issues/235) | decimal 参数有效位保真 | 已完成并关闭 | 高有效位参数检查通过。 |
| [#236](https://github.com/IoTSharp/SonnetDB/issues/236) | DATETIME 参数亚毫秒精度 | 已完成并关闭 | `DATETIME2` 100ns 与旧 `DATETIME` 拒绝合同通过。 |
| [#237](https://github.com/IoTSharp/SonnetDB/issues/237) | 持久化表列备注与反向元数据 | 已完成并关闭 | CodeFirst、DbMaintenance 和 DbFirst 路径有回归。 |
| [#239](https://github.com/IoTSharp/SonnetDB/issues/239) | XCode 专属备注与反向元数据 | 不计划实现 | 该 XCode 专属范围不属于 SqlSugar 适配合同，SqlSugar 所需备注合同由 #237 覆盖。 |
| [#241](https://github.com/IoTSharp/SonnetDB/issues/241) | `ADD/ALTER DECIMAL(p,s)` 精度与元数据 | 已完成并关闭 | ALTER 精度元数据检查通过。 |
| [#245](https://github.com/IoTSharp/SonnetDB/issues/245) | 原生临时关系表与 CTAS | 已完成并关闭 | Fastest 工作表和调用方事务回归通过。 |
| [#246](https://github.com/IoTSharp/SonnetDB/issues/246) | 批量关联更新派生来源与事务连续写入 | 已完成并关闭 | `UPDATE ... FROM` 派生来源回归通过。 |
| [#247](https://github.com/IoTSharp/SonnetDB/issues/247) | 标准存储过程命令与输出参数 | 已完成并关闭 | 过程、多结果和输出参数合同通过。 |
| [#248](https://github.com/IoTSharp/SonnetDB/issues/248) | NOWAIT 锁定读 | 已完成并关闭 | 锁等待与立即失败选项有真实回归。 |
| [#249](https://github.com/IoTSharp/SonnetDB/issues/249) | 动态 JSON path 与对象数组谓词 | 已完成并关闭 | JSON path、数组和对象谓词回归通过。 |
| [#250](https://github.com/IoTSharp/SonnetDB/issues/250) | 约束目录、索引选项和自增起点 | 已完成并关闭 | 维护接口、索引元数据和自增起点回归通过。 |

## 证据入口

- [SonnetDB 与 SqlSugar 适配说明](SonnetDB-compatibility.md)
- [SonnetDB 5.0.0 公共发布审计](https://github.com/IoTSharp/SonnetDB/blob/main/docs/audits/release-5.0-20261104-public-acceptance.json)
- [Fastest 消费者回归](https://github.com/IoTSharp/SonnetDB/blob/main/docs/audits/issue245-fastest-consumer-20261019.md)
- [存储过程消费者回归](https://github.com/IoTSharp/SonnetDB/blob/main/docs/audits/issue247-consumer-20261019.md)
- [锁定读实现回归](https://github.com/IoTSharp/SonnetDB/blob/main/docs/audits/issue-248-implementation-20261016.md)

关闭 issue 证明 SonnetDB 侧合同已经收口。公开包和 SqlSugar 三协议消费者证据另行记录，上游合入和后续 SqlSugar 发行仍由上游维护流程确认。

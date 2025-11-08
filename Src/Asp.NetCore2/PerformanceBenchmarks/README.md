# SqlSugar Performance Benchmark Suite

## 📋 概述 / Overview

这是 SqlSugar ORM 的综合性能基准测试套件，用于比较 SqlSugar 与其他流行 ORM（如 Dapper、Entity Framework Core）的性能表现。

This is a comprehensive performance benchmark suite for SqlSugar ORM, designed to compare SqlSugar's performance with other popular ORMs like Dapper and Entity Framework Core.

### 🎯 Quick Start / 快速开始

```powershell
# 1. Configure database connection in BenchmarkConfig.cs
#    在 BenchmarkConfig.cs 中配置数据库连接

# 2. Run validation tests / 运行验证测试
.\RunTests.ps1

# 3. Run full benchmarks / 运行完整基准测试
dotnet run -c Release
```

## 功能特性 / Features

- ✅ **查询基准测试** / Query Benchmarks
  - 简单查询 / Simple queries
  - 分页查询 / Pagination
  - 复杂条件查询 / Complex conditional queries
  - 动态查询 / Dynamic queries

- ✅ **插入基准测试** / Insert Benchmarks
  - 单条插入 / Single insert
  - 批量插入 / Batch insert
  - 返回主键 / Return identity

- ✅ **更新基准测试** / Update Benchmarks
  - 单条更新 / Single update
  - 批量更新 / Batch update
  - 条件更新 / Conditional update
  - 指定列更新 / Update specific columns

- ✅ **删除基准测试** / Delete Benchmarks
  - 按实体删除 / Delete by entity
  - 按 ID 删除 / Delete by ID
  - 条件删除 / Conditional delete
  - 批量删除 / Batch delete

- ✅ **批量操作基准测试** / Bulk Operation Benchmarks
  - BulkCopy (1000+ 记录)
  - BulkUpdate
  - BulkMerge
  - Storageable (Insert or Update)

- ✅ **联接查询基准测试** / Join Query Benchmarks
  - Inner Join
  - Left Join
  - 多表联接 / Multiple joins
  - 带条件的联接 / Join with conditions
  - 带分组的联接 / Join with GROUP BY

- ✅ **复杂查询基准测试** / Complex Query Benchmarks
  - 子查询 / Subqueries
  - Union 查询 / Union queries
  - 导航属性 / Navigation properties
  - 聚合函数 / Aggregate functions
  - Distinct 查询 / Distinct queries

## 🧪 Testing / 测试

### Validation Tests / 验证测试

Before running benchmarks, validate your setup:

在运行基准测试之前，验证您的设置：

```powershell
# Using PowerShell script (Recommended) / 使用 PowerShell 脚本（推荐）
.\RunTests.ps1

# Or using dotnet CLI / 或使用 dotnet CLI
dotnet run -c Release -- --test
```

**Tests Include / 测试包括**:
- ✅ Database Connection Test / 数据库连接测试
- ✅ Entity Creation Test / 实体创建测试
- ✅ CRUD Operations Test / CRUD 操作测试
- ✅ Bulk Operations Test / 批量操作测试
- ✅ Join Queries Test / 联接查询测试
- ✅ Navigation Properties Test / 导航属性测试

### Expected Output / 预期输出

```
Test 1: Database Connection... ✓ PASSED
Test 2: Entity Creation... ✓ PASSED
Test 3: Basic CRUD Operations... ✓ PASSED
Test 4: Bulk Operations... ✓ PASSED
Test 5: Join Queries... ✓ PASSED
Test 6: Navigation Properties... ✓ PASSED

✓ All validation tests PASSED!
```

---

## 环境要求 / Requirements

- .NET 6.0 或更高版本 / .NET 6.0 or higher
- SQL Server (默认) / SQL Server (default)
- BenchmarkDotNet 0.13.12+

## 配置 / Configuration

在运行基准测试之前，请修改 `BenchmarkConfig.cs` 中的数据库连接字符串：

Before running benchmarks, modify the database connection strings in `BenchmarkConfig.cs`:

```csharp
public const string SqlServerConnection = "server=.;uid=sa;pwd=YOUR_PASSWORD;database=SqlSugarBenchmark;Encrypt=True;TrustServerCertificate=True";
```

## 使用方法 / Usage

### 运行完整基准测试 / Run Full Benchmarks

```bash
cd d:\dev\bit\74\SqlSugar\Src\Asp.NetCore2\PerformanceBenchmarks
dotnet run -c Release
```

### 运行快速验证测试 / Run Quick Validation Tests

```bash
dotnet run -c Release -- --test
```

### 运行特定基准测试 / Run Specific Benchmark

```bash
dotnet run -c Release -- --filter *QueryBenchmarks*
```

## 基准测试类别 / Benchmark Categories

### 1. QueryBenchmarks (查询基准测试)

比较 SqlSugar 和 Dapper 在各种查询场景下的性能：

Compares SqlSugar and Dapper performance across various query scenarios:

- `SqlSugar_SimpleQuery` - 简单查询
- `SqlSugar_PaginationQuery` - 分页查询
- `SqlSugar_ComplexQuery` - 复杂条件查询
- `SqlSugar_QueryById` - 按 ID 查询
- `SqlSugar_DynamicQuery` - 动态条件查询
- `SqlSugar_SelectColumns` - 查询指定列

### 2. InsertBenchmarks (插入基准测试)

测试各种插入操作的性能：

Tests performance of various insert operations:

- `SqlSugar_SingleInsert` - 单条插入
- `SqlSugar_BatchInsert_100` - 批量插入 100 条
- `SqlSugar_InsertReturnIdentity` - 插入并返回自增 ID
- `SqlSugar_BatchInsertReturnPKs` - 批量插入并返回主键列表
- `SqlSugar_InsertIgnoreColumns` - 插入时忽略指定列

### 3. UpdateBenchmarks (更新基准测试)

测试各种更新操作的性能：

Tests performance of various update operations:

- `SqlSugar_SingleUpdate` - 单条更新
- `SqlSugar_UpdateColumns` - 更新指定列
- `SqlSugar_BatchUpdate` - 批量更新
- `SqlSugar_UpdateWithCondition` - 条件更新
- `SqlSugar_UpdateIgnoreColumns` - 更新时忽略指定列

### 4. DeleteBenchmarks (删除基准测试)

测试各种删除操作的性能：

Tests performance of various delete operations:

- `SqlSugar_DeleteByEntity` - 按实体删除
- `SqlSugar_DeleteById` - 按 ID 删除
- `SqlSugar_DeleteWithCondition` - 条件删除
- `SqlSugar_DeleteByIdList` - 按 ID 列表删除

### 5. BulkOperationBenchmarks (批量操作基准测试)

测试 SqlSugar 的高性能批量操作：

Tests SqlSugar's high-performance bulk operations:

- `SqlSugar_BulkCopy_1000` - BulkCopy 插入 1000 条
- `SqlSugar_BatchInsert_1000` - 常规批量插入 1000 条
- `SqlSugar_BulkUpdate_1000` - 批量更新 1000 条
- `SqlSugar_Storageable_1000` - 存储（插入或更新）1000 条
- `SqlSugar_BulkMerge_1000` - 批量合并 1000 条
- `SqlSugar_BulkCopy_DataTable` - 使用 DataTable 的 BulkCopy

### 6. JoinQueryBenchmarks (联接查询基准测试)

测试各种联接查询的性能：

Tests performance of various join queries:

- `SqlSugar_InnerJoin` - 内联接
- `SqlSugar_LeftJoin` - 左联接
- `SqlSugar_MultipleJoins` - 多表联接
- `SqlSugar_JoinWithWhere` - 带条件的联接
- `SqlSugar_JoinWithGroupBy` - 带分组的联接

### 7. ComplexQueryBenchmarks (复杂查询基准测试)

测试复杂查询场景的性能：

Tests performance of complex query scenarios:

- `SqlSugar_Subquery` - 子查询
- `SqlSugar_GroupByWithHaving` - 分组查询带 Having
- `SqlSugar_UnionQuery` - Union 查询
- `SqlSugar_IncludeNavigation` - 包含导航属性
- `SqlSugar_MultipleIncludes` - 多个导航属性
- `SqlSugar_DynamicExpression` - 动态表达式查询
- `SqlSugar_DistinctQuery` - 去重查询
- `SqlSugar_AggregateFunctions` - 聚合函数
- `SqlSugar_InQuery` - In 查询

## 测试数据 / Test Data

基准测试使用以下实体和数据量：

Benchmarks use the following entities and data volumes:

- **BenchmarkCustomer** (客户): 100-1000 条记录
- **BenchmarkProduct** (产品): 50-1000 条记录
- **BenchmarkOrder** (订单): 100-200 条记录
- **BenchmarkOrderItem** (订单项): 200-600 条记录

## 结果解读 / Understanding Results

BenchmarkDotNet 会生成详细的性能报告，包括：

BenchmarkDotNet generates detailed performance reports including:

- **Mean** (平均值): 平均执行时间
- **Error** (误差): 标准误差
- **StdDev** (标准差): 标准偏差
- **Median** (中位数): 中位执行时间
- **Allocated** (内存分配): 分配的内存量
- **Rank** (排名): 性能排名

### 示例输出 / Sample Output

```
|                    Method |      Mean |     Error |    StdDev | Allocated |
|-------------------------- |----------:|----------:|----------:|----------:|
| SqlSugar_SimpleQuery      |  1.234 ms | 0.0123 ms | 0.0115 ms |   12.5 KB |
| Dapper_SimpleQuery        |  1.456 ms | 0.0145 ms | 0.0136 ms |   13.2 KB |
```

## 性能优化建议 / Performance Tips

基于基准测试结果，以下是一些性能优化建议：

Based on benchmark results, here are some performance optimization tips:

1. **批量操作** / Bulk Operations
   - 对于大量数据插入，使用 `BulkCopy` 而不是循环插入
   - Use `BulkCopy` instead of loop inserts for large data volumes

2. **查询优化** / Query Optimization
   - 使用 `Select` 指定需要的列，避免查询所有列
   - Use `Select` to specify needed columns, avoid selecting all columns

3. **分页查询** / Pagination
   - 使用 `ToPageList` 进行分页，性能优于手动分页
   - Use `ToPageList` for pagination, better performance than manual pagination

4. **导航属性** / Navigation Properties
   - 谨慎使用 `Includes`，只加载需要的导航属性
   - Use `Includes` carefully, only load needed navigation properties

5. **缓存** / Caching
   - 对于频繁查询的数据，考虑使用缓存
   - Consider using cache for frequently queried data

## 贡献 / Contributing

欢迎提交新的基准测试场景或改进现有测试！

Welcome to submit new benchmark scenarios or improve existing tests!

1. Fork 本仓库
2. 创建特性分支 (`git checkout -b feature/new-benchmark`)
3. 提交更改 (`git commit -am 'Add new benchmark'`)
4. 推送到分支 (`git push origin feature/new-benchmark`)
5. 创建 Pull Request

## 🔧 Troubleshooting / 故障排除

### Common Issues / 常见问题

#### Cannot Connect to Database / 无法连接到数据库

**Error / 错误**: `A network-related or instance-specific error occurred`

**Solutions / 解决方案**:
1. Check connection string in `BenchmarkConfig.cs`
2. Verify SQL Server is running
3. Check firewall settings
4. Confirm user permissions

#### Build Errors / 构建错误

**Error / 错误**: `The type or namespace name could not be found`

**Solutions / 解决方案**:
```bash
dotnet restore
dotnet clean
dotnet build -c Release
```

#### Tests Running Slowly / 测试运行缓慢

This is normal. Full benchmark suite takes 30-60 minutes.

这是正常的。完整的基准测试套件需要 30-60 分钟。

Use `--filter` to run specific tests:
```bash
dotnet run -c Release -- --filter *QueryBenchmarks*
```

---

## 📊 Project Statistics / 项目统计

- **Total Files / 文件总数**: 20+
- **Lines of Code / 代码行数**: 2,800+
- **Test Methods / 测试方法**: 104
- **Test Categories / 测试类别**: 7
- **Documentation / 文档**: Comprehensive

---

## 许可证 / License

本项目遵循 SqlSugar 项目的 MIT 许可证。

This project follows SqlSugar's MIT License.

## 相关链接 / Related Links

- [SqlSugar 官方文档](https://www.donet5.com/Home/Doc)
- [BenchmarkDotNet 文档](https://benchmarkdotnet.org/)
- [SqlSugar GitHub](https://github.com/donet5/SqlSugar)

## 联系方式 / Contact

如有问题或建议，请在 GitHub Issues 中提出。

For questions or suggestions, please submit them in GitHub Issues.

---

**Version / 版本**: 1.0.0  
**Status / 状态**: ✅ Production Ready  
**Last Updated / 最后更新**: 2025-11-08

**Happy Benchmarking! / 测试愉快！** 🚀

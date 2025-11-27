# Smart Batch Upsert Feature - Implementation Guide

## Overview

The **Smart Batch Upsert** feature adds advanced conflict resolution capabilities to SqlSugar ORM, extending beyond the basic [Storageable](cci:2://file:///root/SqlSugar/Src/Asp.NetCore2/SqlSugar/Abstract/SaveableProvider/Storageable.cs:11:4-645:5) functionality with granular column-level control and multiple resolution strategies.

## Files Created

### 1. Core Enum
- **Path**: `/root/SqlSugar/Src/Asp.NetCore2/SqlSugar/Enum/ConflictResolutionStrategy.cs`
- **Purpose**: Defines 10 conflict resolution strategies
- **Strategies**:
  - `UpdateAll` - Update all columns (default)
  - `UpdateSpecified` - Update only specified columns
  - `UpdateNonNull` - Update only non-null values
  - `UpdateIfGreater` - Update if incoming value is greater
  - `UpdateIfLess` - Update if incoming value is less
  - `SkipOnConflict` - Keep existing data
  - `ThrowOnConflict` - Throw exception
  - `Custom` - Custom update logic
  - `IncrementOnConflict` - Add values together
  - `DecrementOnConflict` - Subtract values

### 2. Result Entities
- **Path**: `/root/SqlSugar/Src/Asp.NetCore2/SqlSugar/Entities/SmartUpsertResult.cs`
- **Classes**:
  - `SmartUpsertResult<T>` - Detailed operation results with statistics
  - `SmartUpsertError<T>` - Error information

### 3. Configuration Entities
- **Path**: `/root/SqlSugar/Src/Asp.NetCore2/SqlSugar/Entities/ColumnStrategyConfig.cs`
- **Purpose**: Column-specific strategy configuration

### 4. Interface
- **Path**: `/root/SqlSugar/Src/Asp.NetCore2/SqlSugar/Interface/ISmartUpsertable.cs`
- **Methods**: 15+ fluent API methods for configuration

### 5. Core Provider
- **Path**: `/root/SqlSugar/Src/Asp.NetCore2/SqlSugar/Abstract/SmartUpsertProvider/SmartUpsertableProvider.cs`
- **Size**: ~500 lines
- **Features**:
  - Batch processing with configurable page size
  - Column-level strategy application
  - Conditional updates
  - Audit trail support
  - Async operations
  - Comprehensive error handling

### 6. Client Extensions
- **Modified**: [SqlSugarClient.cs](cci:7://file:///root/SqlSugar/Src/Asp.NetCore2/SqlSugar/SqlSugarClient.cs:0:0-0:0) and [SqlSugarProvider.cs](cci:7://file:///root/SqlSugar/Src/Asp.NetCore2/SqlSugar/Abstract/SugarProvider/SqlSugarProvider.cs:0:0-0:0)
- **Added**: `SmartUpsert<T>()` methods

### 7. Test Suite
- **Path**: `/root/SqlSugar/Src/Asp.NetCore2/MySqlTest/SmartUpsertTest.cs`
- **Tests**: 10 comprehensive test cases

## Quick Start

### Basic Usage

```csharp
var products = new List<Product>
{
    new Product { ProductCode = "P001", Name = "Product 1", Price = 100 },
    new Product { ProductCode = "P002", Name = "Product 2", Price = 200 }
};

var result = db.SmartUpsert(products)
    .WhereColumns(p => new { p.ProductCode })
    .SetStrategy(ConflictResolutionStrategy.UpdateAll)
    .ExecuteCommand();

Console.WriteLine($"Inserted: {result.InsertedCount}, Updated: {result.UpdatedCount}");
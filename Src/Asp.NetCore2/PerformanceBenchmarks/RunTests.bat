@echo off
REM ============================================
REM SqlSugar Performance Benchmarks Test Runner
REM SqlSugar 性能基准测试运行器
REM ============================================

echo.
echo ============================================
echo SqlSugar Performance Benchmarks
echo SqlSugar 性能基准测试
echo ============================================
echo.

REM Check if .NET SDK is installed
REM 检查是否安装了 .NET SDK
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: .NET SDK is not installed or not in PATH
    echo 错误: 未安装 .NET SDK 或不在 PATH 中
    pause
    exit /b 1
)

echo .NET SDK Version:
dotnet --version
echo.

REM Step 1: Restore packages
REM 步骤 1: 还原包
echo ============================================
echo Step 1: Restoring NuGet packages...
echo 步骤 1: 还原 NuGet 包...
echo ============================================
dotnet restore
if errorlevel 1 (
    echo ERROR: Failed to restore packages
    echo 错误: 还原包失败
    pause
    exit /b 1
)
echo.

REM Step 2: Build project
REM 步骤 2: 构建项目
echo ============================================
echo Step 2: Building project...
echo 步骤 2: 构建项目...
echo ============================================
dotnet build -c Release
if errorlevel 1 (
    echo ERROR: Build failed
    echo 错误: 构建失败
    pause
    exit /b 1
)
echo.

REM Step 3: Run validation tests
REM 步骤 3: 运行验证测试
echo ============================================
echo Step 3: Running validation tests...
echo 步骤 3: 运行验证测试...
echo ============================================
dotnet run -c Release -- --test
if errorlevel 1 (
    echo.
    echo ERROR: Validation tests failed!
    echo 错误: 验证测试失败！
    echo.
    echo Please check:
    echo 请检查:
    echo 1. Database connection string in BenchmarkConfig.cs
    echo    BenchmarkConfig.cs 中的数据库连接字符串
    echo 2. Database server is running
    echo    数据库服务器正在运行
    echo 3. User has necessary permissions
    echo    用户具有必要的权限
    echo.
    pause
    exit /b 1
)

echo.
echo ============================================
echo SUCCESS: All validation tests passed!
echo 成功: 所有验证测试通过！
echo ============================================
echo.
echo You can now run the full benchmark suite with:
echo 您现在可以使用以下命令运行完整的基准测试套件:
echo.
echo     dotnet run -c Release
echo.
echo Or run specific benchmarks:
echo 或运行特定的基准测试:
echo.
echo     dotnet run -c Release -- --filter *QueryBenchmarks*
echo     dotnet run -c Release -- --filter *BulkOperationBenchmarks*
echo.
pause

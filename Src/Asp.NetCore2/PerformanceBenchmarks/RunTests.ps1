<#
.SYNOPSIS
    SqlSugar Performance Benchmarks Test Runner
    SqlSugar 性能基准测试运行器

.DESCRIPTION
    This script builds the project, restores packages, and runs validation tests
    to ensure everything is configured correctly before running benchmarks.
    此脚本构建项目、还原包并运行验证测试，以确保在运行基准测试之前所有配置正确。

.PARAMETER SkipBuild
    Skip the build step (useful if already built)
    跳过构建步骤（如果已构建则有用）

.PARAMETER RunFullBenchmarks
    Run full benchmark suite after validation
    验证后运行完整的基准测试套件

.EXAMPLE
    .\RunTests.ps1
    Run validation tests only
    仅运行验证测试

.EXAMPLE
    .\RunTests.ps1 -RunFullBenchmarks
    Run validation tests and then full benchmarks
    运行验证测试然后运行完整基准测试

.NOTES
    Author: SqlSugar Community
    Version: 1.0.0
#>

param(
    [switch]$SkipBuild,
    [switch]$RunFullBenchmarks
)

# Set error action preference
# 设置错误操作首选项
$ErrorActionPreference = "Stop"

# Function to write colored output
# 用于写入彩色输出的函数
function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = "White"
    )
    Write-Host $Message -ForegroundColor $Color
}

# Function to write section header
# 用于写入节标题的函数
function Write-SectionHeader {
    param([string]$Title)
    Write-Host ""
    Write-ColorOutput "============================================" "Cyan"
    Write-ColorOutput $Title "Cyan"
    Write-ColorOutput "============================================" "Cyan"
    Write-Host ""
}

# Main script
# 主脚本
try {
    Write-SectionHeader "SqlSugar Performance Benchmarks Test Runner"
    Write-ColorOutput "SqlSugar 性能基准测试运行器" "Cyan"

    # Check .NET SDK
    # 检查 .NET SDK
    Write-Host "Checking .NET SDK..."
    Write-Host "检查 .NET SDK..."
    try {
        $dotnetVersion = dotnet --version
        Write-ColorOutput "✓ .NET SDK Version: $dotnetVersion" "Green"
    }
    catch {
        Write-ColorOutput "✗ ERROR: .NET SDK is not installed or not in PATH" "Red"
        Write-ColorOutput "✗ 错误: 未安装 .NET SDK 或不在 PATH 中" "Red"
        exit 1
    }

    if (-not $SkipBuild) {
        # Step 1: Restore packages
        # 步骤 1: 还原包
        Write-SectionHeader "Step 1: Restoring NuGet Packages"
        Write-Host "步骤 1: 还原 NuGet 包"
        dotnet restore
        if ($LASTEXITCODE -ne 0) {
            Write-ColorOutput "✗ ERROR: Failed to restore packages" "Red"
            Write-ColorOutput "✗ 错误: 还原包失败" "Red"
            exit 1
        }
        Write-ColorOutput "✓ Packages restored successfully" "Green"

        # Step 2: Build project
        # 步骤 2: 构建项目
        Write-SectionHeader "Step 2: Building Project"
        Write-Host "步骤 2: 构建项目"
        dotnet build -c Release
        if ($LASTEXITCODE -ne 0) {
            Write-ColorOutput "✗ ERROR: Build failed" "Red"
            Write-ColorOutput "✗ 错误: 构建失败" "Red"
            exit 1
        }
        Write-ColorOutput "✓ Build successful" "Green"
    }
    else {
        Write-ColorOutput "Skipping build step..." "Yellow"
        Write-ColorOutput "跳过构建步骤..." "Yellow"
    }

    # Step 3: Run validation tests
    # 步骤 3: 运行验证测试
    Write-SectionHeader "Step 3: Running Validation Tests"
    Write-Host "步骤 3: 运行验证测试"
    Write-Host ""
    
    dotnet run -c Release -- --test
    if ($LASTEXITCODE -ne 0) {
        Write-Host ""
        Write-ColorOutput "✗ ERROR: Validation tests failed!" "Red"
        Write-ColorOutput "✗ 错误: 验证测试失败！" "Red"
        Write-Host ""
        Write-ColorOutput "Please check:" "Yellow"
        Write-ColorOutput "请检查:" "Yellow"
        Write-ColorOutput "1. Database connection string in BenchmarkConfig.cs" "Yellow"
        Write-ColorOutput "   BenchmarkConfig.cs 中的数据库连接字符串" "Yellow"
        Write-ColorOutput "2. Database server is running" "Yellow"
        Write-ColorOutput "   数据库服务器正在运行" "Yellow"
        Write-ColorOutput "3. User has necessary permissions" "Yellow"
        Write-ColorOutput "   用户具有必要的权限" "Yellow"
        exit 1
    }

    # Success message
    # 成功消息
    Write-SectionHeader "SUCCESS: All Validation Tests Passed!"
    Write-ColorOutput "成功: 所有验证测试通过！" "Green"

    if ($RunFullBenchmarks) {
        Write-Host ""
        Write-ColorOutput "Starting full benchmark suite..." "Cyan"
        Write-ColorOutput "开始完整的基准测试套件..." "Cyan"
        Write-Host ""
        Write-ColorOutput "⚠️  This may take 30-60 minutes..." "Yellow"
        Write-ColorOutput "⚠️  这可能需要 30-60 分钟..." "Yellow"
        Write-Host ""
        
        dotnet run -c Release
    }
    else {
        Write-Host ""
        Write-ColorOutput "You can now run the full benchmark suite with:" "Cyan"
        Write-ColorOutput "您现在可以使用以下命令运行完整的基准测试套件:" "Cyan"
        Write-Host ""
        Write-ColorOutput "    dotnet run -c Release" "White"
        Write-Host ""
        Write-ColorOutput "Or run specific benchmarks:" "Cyan"
        Write-ColorOutput "或运行特定的基准测试:" "Cyan"
        Write-Host ""
        Write-ColorOutput "    dotnet run -c Release -- --filter *QueryBenchmarks*" "White"
        Write-ColorOutput "    dotnet run -c Release -- --filter *BulkOperationBenchmarks*" "White"
        Write-Host ""
    }
}
catch {
    Write-Host ""
    Write-ColorOutput "✗ An error occurred: $_" "Red"
    Write-ColorOutput "✗ 发生错误: $_" "Red"
    Write-Host ""
    Write-ColorOutput "Stack Trace:" "Red"
    Write-Host $_.ScriptStackTrace
    exit 1
}

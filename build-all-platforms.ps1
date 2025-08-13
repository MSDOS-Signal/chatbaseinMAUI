Write-Host "========================================" -ForegroundColor Green
Write-Host "炘灏AI 多平台构建脚本" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""

# 构建 Windows 版本
Write-Host "正在构建 Windows 版本..." -ForegroundColor Yellow
dotnet build -f net9.0-windows10.0.19041.0 -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "Windows 构建失败！" -ForegroundColor Red
    Read-Host "按回车键退出"
    exit 1
}
Write-Host "Windows 构建成功！" -ForegroundColor Green
Write-Host ""

# 构建 Android 版本
Write-Host "正在构建 Android 版本..." -ForegroundColor Yellow
dotnet build -f net9.0-android -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "Android 构建失败！" -ForegroundColor Red
    Read-Host "按回车键退出"
    exit 1
}
Write-Host "Android 构建成功！" -ForegroundColor Green
Write-Host ""

# 发布 Windows 版本
Write-Host "正在发布 Windows 版本..." -ForegroundColor Yellow
dotnet publish -f net9.0-windows10.0.19041.0 -c Release -r win-x64 --self-contained -o "bin\Release\Windows"
if ($LASTEXITCODE -ne 0) {
    Write-Host "Windows 发布失败！" -ForegroundColor Red
    Read-Host "按回车键退出"
    exit 1
}
Write-Host "Windows 发布成功！输出目录: bin\Release\Windows" -ForegroundColor Green
Write-Host ""

# 发布 Android APK
Write-Host "正在发布 Android APK..." -ForegroundColor Yellow
dotnet publish -f net9.0-android -c Release -p:AndroidPackageFormat=apk -o "bin\Release\Android"
if ($LASTEXITCODE -ne 0) {
    Write-Host "Android 发布失败！" -ForegroundColor Red
    Read-Host "按回车键退出"
    exit 1
}
Write-Host "Android 发布成功！输出目录: bin\Release\Android" -ForegroundColor Green
Write-Host ""

Write-Host "========================================" -ForegroundColor Green
Write-Host "所有平台构建完成！" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "输出目录:" -ForegroundColor Cyan
Write-Host "- Windows: bin\Release\Windows" -ForegroundColor White
Write-Host "- Android: bin\Release\Android" -ForegroundColor White
Write-Host ""
Read-Host "按回车键退出"

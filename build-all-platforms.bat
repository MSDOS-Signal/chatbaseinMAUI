@echo off
echo ========================================
echo 炘灏AI 多平台构建脚本
echo ========================================
echo.

echo 正在构建 Windows 版本...
dotnet build -f net9.0-windows10.0.19041.0 -c Release
if %ERRORLEVEL% NEQ 0 (
    echo Windows 构建失败！
    pause
    exit /b 1
)
echo Windows 构建成功！
echo.

echo 正在构建 Android 版本...
dotnet build -f net9.0-android -c Release
if %ERRORLEVEL% NEQ 0 (
    echo Android 构建失败！
    pause
    exit /b 1
)
echo Android 构建成功！
echo.

echo 正在发布 Windows 版本...
dotnet publish -f net9.0-windows10.0.19041.0 -c Release -r win-x64 --self-contained -o "bin\Release\Windows"
if %ERRORLEVEL% NEQ 0 (
    echo Windows 发布失败！
    pause
    exit /b 1
)
echo Windows 发布成功！输出目录: bin\Release\Windows
echo.

echo 正在发布 Android APK...
dotnet publish -f net9.0-android -c Release -p:AndroidPackageFormat=apk -o "bin\Release\Android"
if %ERRORLEVEL% NEQ 0 (
    echo Android 发布失败！
    pause
    exit /b 1
)
echo Android 发布成功！输出目录: bin\Release\Android
echo.

echo ========================================
echo 所有平台构建完成！
echo ========================================
echo.
echo 输出目录:
echo - Windows: bin\Release\Windows
echo - Android: bin\Release\Android
echo.
pause

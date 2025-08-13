# 炘灏AI - MAUI聊天应用  

一个基于 **.NET MAUI** 开发的跨平台 AI 聊天应用，支持 **Android** 和 **iOS** 平台。  


## 🌟 功能特性  
- 🤖 **AI对话**：基于 Qwen2.5(72B) 大模型的智能对话  
- 💬 **对话历史**：支持多会话管理，每个对话以用户第一句话命名  
- 🌓 **主题切换**：支持浅色模式和深色模式  
- 📱 **跨平台**：支持 Android 和 iOS 平台  
- 🎨 **现代UI**：简洁高级的界面设计，带有科技感元素  
- ⚡ **实时响应**：流畅的聊天体验  


## 🛠 技术栈  
| 类别       | 详情                              |  
|------------|-----------------------------------|  
| 框架       | .NET MAUI 9.0                     |  
| 语言       | C#                                |  
| AI 模型    | Qwen2.5(72B)                      |  
| API        | SiliconFlow API                   |  
| UI 架构    | XAML + MVVM 架构                  |  


## 📁 项目结构  
```
MauiApp1/
├── Models/
│ ├── ChatMessage.cs # 聊天消息模型
│ └── ChatSession.cs # 对话会话模型
├── Services/
│ ├── AIService.cs # AI 服务
│ ├── ThemeService.cs # 主题服务
│ └── ChatHistoryService.cs # 历史记录服务
├── ViewModels/
│ └── MainViewModel.cs # 主页面视图模型
├── Converters/
│ ├── BoolToThemeIconConverter.cs
│ ├── MessageTypeToVisibilityConverter.cs
│ └── StringToBoolConverter.cs
├── Resources/
│ ├── AppIcon/ # 应用图标
│ └── Splash/ # 启动画面
└── MainPage.xaml # 主页面
```

## 🚀 安装和运行  
### 前置要求  
- Visual Studio 2022 17.8+ 或 Visual Studio for Mac  
- .NET 9.0 SDK  
- Android SDK（用于 Android 开发）  
- Xcode（用于 iOS 开发，仅 macOS）  


### 构建步骤  
1. 克隆项目
```   
git clone git@github.com:MSDOS-Signal/chatbaseinMAUI.git
cd MauiApp1  
```
2. 还原 NuGet 包
```dotnet restore ``` 
3. 构建项目
```dotnet build  ```
4. 运行项目
# Windows  
```dotnet run -f net9.0-windows10.0.19041.0  ```

# Android  
```dotnet build -t:Run -f net9.0-android```  

# iOS（仅 macOS）  
```dotnet build -t:Run -f net9.0-ios ``` 
# API 密钥配置
在 Services/AIService.cs 文件中配置你的 API 密钥：
```private const string API_KEY = "your-api-key-here"; ```
## 🔍 主要功能
### 1. AI 对话
支持与 Qwen2.5 (72B) 模型进行自然语言对话
流式输出：AI 回复逐字显示，实时反馈
上下文记忆：AI 能记住对话历史，提供连贯的回复
自动滚动：AI 回复时自动滚动到底部，确保用户看到最新内容
实时响应，支持长文本输入
错误处理和重试机制
### 2. 对话历史管理
多会话支持：可以创建多个独立的对话会话
自动命名：每个对话以用户的第一句话自动命名
会话切换：点击侧边栏可以快速切换不同会话
会话删除：支持删除不需要的对话会话
新建对话：点击 + 按钮创建新对话
本地存储：对话历史自动保存到本地文件
持久化：重启应用后对话历史依然保留
### 3. 主题系统
自动检测系统主题
手动切换浅色 / 深色模式
平滑的主题切换动画
### 4. 用户界面
现代化的聊天界面设计
侧边栏历史记录管理
消息气泡样式区分用户和 AI
响应式布局适配不同屏幕尺寸
## 📖 使用说明
基本操作
发送消息：在底部输入框输入问题，点击发送或按回车键
切换主题：点击右上角的 🌙/☀️ 图标
查看历史：点击左上角的菜单按钮打开侧边栏
新建对话：点击底部的 + 按钮
切换会话：在侧边栏点击任意对话标题
删除会话：在侧边栏点击对话右侧的 × 按钮
界面布局
顶部栏：菜单按钮、应用标题、模型标识、主题切换
侧边栏：对话历史列表，显示对话标题和最后修改时间
主区域：当前对话的消息显示区域
底部栏：新建对话按钮、输入框、发送按钮
## 👨‍💻 开发说明
架构模式
项目采用 MVVM 架构模式：

Model：数据模型和业务逻辑
View：XAML 界面定义
ViewModel：视图模型，处理 UI 逻辑
依赖注入
使用内置的依赖注入容器
服务注册在 MauiProgram.cs 中
数据绑定
使用 XAML 数据绑定
支持双向绑定和命令绑定
自定义值转换器
## 📦 部署
Android
在 Visual Studio 中选择 Android 模拟器或连接设备
设置发布配置
构建 APK 或 AAB 文件
iOS
在 macOS 上使用 Xcode
配置开发者证书
构建并部署到设备或 App Store
## 📄 更新日志
本项目采用 MIT 许可证。
炘灏 AI - 让 AI 对话更简单、更智能！ 🚀

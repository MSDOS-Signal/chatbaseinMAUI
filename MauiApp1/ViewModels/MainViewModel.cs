using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using MauiApp1.Models;
using MauiApp1.Services;

namespace MauiApp1.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly AIService _aiService;
        private readonly ThemeService _themeService;
        private readonly ChatHistoryService _chatHistoryService;
        private readonly AuthService _authService;
        private string _inputText = string.Empty;
        private bool _isLoading;
        private bool _isDarkMode;
        private bool _isSidebarVisible;

        public ObservableCollection<ChatSession> Sessions => _chatHistoryService.Sessions;
        public ChatSession? CurrentSession => _chatHistoryService.CurrentSession;
        public ICommand SendMessageCommand { get; }
        public ICommand ToggleThemeCommand { get; }
        public ICommand NewChatCommand { get; }
        public ICommand SwitchSessionCommand { get; }
        public ICommand DeleteSessionCommand { get; }
        public ICommand ToggleSidebarCommand { get; }
        public ICommand LogoutCommand { get; }

        public User? CurrentUser => _authService.CurrentUser;
        public bool IsLoggedIn => _authService.IsLoggedIn;

        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                OnPropertyChanged(nameof(InputText));
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                _isDarkMode = value;
                OnPropertyChanged(nameof(IsDarkMode));
            }
        }

        public bool IsSidebarVisible
        {
            get => _isSidebarVisible;
            set
            {
                _isSidebarVisible = value;
                OnPropertyChanged(nameof(IsSidebarVisible));
            }
        }

        public MainViewModel()
        {
            _aiService = new AIService();
            _themeService = new ThemeService();
            _chatHistoryService = new ChatHistoryService();
            _authService = new AuthService();
            
            SendMessageCommand = new Command(async () => await SendMessage());
            ToggleThemeCommand = new Command(ToggleTheme);
            NewChatCommand = new Command(CreateNewChat);
            SwitchSessionCommand = new Command<ChatSession>(SwitchSession);
            DeleteSessionCommand = new Command<ChatSession>(DeleteSession);
            ToggleSidebarCommand = new Command(ToggleSidebar);
            LogoutCommand = new Command(async () => await LogoutAsync());

            // 监听主题变化
            _themeService.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(ThemeService.IsDarkMode))
                {
                    IsDarkMode = _themeService.IsDarkMode;
                    
                    // 主题变化时强制刷新所有会话，确保所有消息的颜色都更新
                    // 使用更直接的方法：强制重新创建所有会话对象
                    var currentSessions = Sessions.ToList();
                    var currentSession = CurrentSession;
                    
                    // 清空并重新添加所有会话，强制UI重新渲染
                    Sessions.Clear();
                    foreach (var session in currentSessions)
                    {
                        Sessions.Add(session);
                    }
                    
                    // 通知UI更新
                    OnPropertyChanged(nameof(Sessions));
                    OnPropertyChanged(nameof(CurrentSession));
                }
            };

            // 监听当前会话变化
            _chatHistoryService.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(ChatHistoryService.CurrentSession))
                {
                    OnPropertyChanged(nameof(CurrentSession));
                }
            };

            // 监听会话集合变化
            _chatHistoryService.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(ChatHistoryService.Sessions))
                {
                    OnPropertyChanged(nameof(Sessions));
                }
            };

            IsDarkMode = _themeService.IsDarkMode;

            // 立即尝试自动登录，确保对话记录正确加载
            _ = Task.Run(async () => await AutoLoginAsync());
        }

        private async Task AutoLoginAsync()
        {
            try
            {
                if (await _authService.AutoLoginAsync())
                {
                    // 设置用户特定的存储路径
                    var storageService = new StorageService();
                    storageService.SetCurrentUser(_authService.CurrentUser!.Id);
                    
                    // 重新初始化聊天历史服务
                    _chatHistoryService.InitializeWithStorage(storageService);
                    
                    // 等待一下确保会话加载完成
                    await Task.Delay(100);
                    
                    // 通知UI更新会话列表
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        OnPropertyChanged(nameof(Sessions));
                        OnPropertyChanged(nameof(CurrentSession));
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"自动登录失败: {ex.Message}");
            }
        }

        private async Task LogoutAsync()
        {
            await _authService.LogoutAsync();
            await Shell.Current.GoToAsync("///LoginPage");
        }

        private void CreateNewChat()
        {
            _chatHistoryService.CreateNewSession();
            IsSidebarVisible = false;
        }

        private void SwitchSession(ChatSession session)
        {
            _chatHistoryService.SwitchToSession(session);
            IsSidebarVisible = false;
        }

        private void DeleteSession(ChatSession session)
        {
            _chatHistoryService.DeleteSession(session);
        }

        private void ToggleSidebar()
        {
            IsSidebarVisible = !IsSidebarVisible;
        }

        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(InputText) || IsLoading || CurrentSession == null)
                return;

            var userMessage = new ChatMessage(InputText, MessageType.User);
            _chatHistoryService.AddMessageToCurrentSession(userMessage);

            var userInput = InputText;
            InputText = string.Empty;
            IsLoading = true;

            try
            {
                // 创建AI回复消息
                var assistantMessage = new ChatMessage("", MessageType.Assistant);
                assistantMessage.IsStreaming = true;
                _chatHistoryService.AddMessageToCurrentSession(assistantMessage);

                // 通知UI滚动到底部
                OnPropertyChanged(nameof(CurrentSession));

                // 使用流式输出，传递对话历史
                var conversationHistory = CurrentSession.Messages.ToList();
                var updateCount = 0;
                await foreach (var chunk in _aiService.GetStreamingResponseAsync(userInput, conversationHistory))
                {
                    assistantMessage.Content += chunk;
                    updateCount++;
                    
                    // 每累积一定字符数或每3次更新才通知UI刷新，减少频繁刷新
                    if (updateCount % 3 == 0 || chunk.Length > 10)
                    {
                        OnPropertyChanged(nameof(CurrentSession));
                        await Task.Delay(30); // 减少延迟，让输出更流畅
                    }
                }

                assistantMessage.IsStreaming = false;
                // 最终通知UI滚动到底部
                OnPropertyChanged(nameof(CurrentSession));
            }
            catch (Exception ex)
            {
                var errorMessage = new ChatMessage($"抱歉，发生了错误：{ex.Message}", MessageType.Assistant);
                _chatHistoryService.AddMessageToCurrentSession(errorMessage);
                OnPropertyChanged(nameof(CurrentSession));
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ToggleTheme()
        {
            _themeService.ToggleTheme();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

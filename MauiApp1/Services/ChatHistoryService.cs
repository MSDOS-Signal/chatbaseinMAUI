using System.Collections.ObjectModel;
using System.ComponentModel;
using MauiApp1.Models;

namespace MauiApp1.Services
{
    public class ChatHistoryService : INotifyPropertyChanged
    {
        private ObservableCollection<ChatSession> _sessions;
        private ChatSession? _currentSession;
        private StorageService _storageService;

        public ObservableCollection<ChatSession> Sessions
        {
            get => _sessions;
            set
            {
                _sessions = value;
                OnPropertyChanged(nameof(Sessions));
            }
        }

        public ChatSession? CurrentSession
        {
            get => _currentSession;
            set
            {
                _currentSession = value;
                OnPropertyChanged(nameof(CurrentSession));
            }
        }

        public ChatHistoryService()
        {
            _storageService = new StorageService();
            Sessions = new ObservableCollection<ChatSession>();
            // 立即创建第一个会话，确保有欢迎消息
            CreateNewSession();
            // 然后异步加载保存的会话
            _ = LoadSessionsAsync();
        }

        public void InitializeWithStorage(StorageService storageService)
        {
            _storageService = storageService;
            // 重新加载会话
            _ = LoadSessionsAsync();
        }

        private async Task LoadSessionsAsync()
        {
            try
            {
                var savedSessions = await _storageService.LoadSessionsAsync();
                if (savedSessions.Count > 0)
                {
                    // 清除当前会话，加载保存的会话
                    Sessions.Clear();
                    foreach (var session in savedSessions)
                    {
                        Sessions.Add(session);
                    }
                    CurrentSession = Sessions.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载会话失败: {ex.Message}");
                // 如果加载失败，确保至少有一个会话
                if (Sessions.Count == 0)
                {
                    CreateNewSession();
                }
            }
        }

        public ChatSession CreateNewSession()
        {
            var newSession = new ChatSession();
            // 从下往上添加，插入到列表开头
            Sessions.Insert(0, newSession);
            CurrentSession = newSession;
            
            // 添加欢迎消息（非流式）
            var welcomeMessage = new ChatMessage(
                "你好！我是炘灏AI，基于Qwen2.5(72B)大模型。我可以帮助你回答问题、进行创作、分析问题等。请告诉我你需要什么帮助？",
                MessageType.Assistant
            );
            welcomeMessage.IsStreaming = false; // 确保欢迎消息不是流式的
            newSession.AddMessage(welcomeMessage);
            
            return newSession;
        }

        public void SwitchToSession(ChatSession session)
        {
            CurrentSession = session;
        }

        public async void DeleteSession(ChatSession session)
        {
            if (Sessions.Contains(session))
            {
                Sessions.Remove(session);
                
                // 如果删除的是当前会话，切换到第一个会话或创建新会话
                if (CurrentSession == session)
                {
                    CurrentSession = Sessions.FirstOrDefault() ?? CreateNewSession();
                }
                
                // 保存到本地存储
                await _storageService.SaveSessionsAsync(Sessions);
                // 通知UI更新
                OnPropertyChanged(nameof(Sessions));
                OnPropertyChanged(nameof(CurrentSession));
            }
        }

        public async void AddMessageToCurrentSession(ChatMessage message)
        {
            if (CurrentSession != null)
            {
                CurrentSession.AddMessage(message);
                // 保存到本地存储
                await _storageService.SaveSessionsAsync(Sessions);
                // 通知UI更新
                OnPropertyChanged(nameof(Sessions));
                OnPropertyChanged(nameof(CurrentSession));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

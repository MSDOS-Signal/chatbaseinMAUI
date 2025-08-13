using System.ComponentModel;
using System.Collections.ObjectModel;

namespace MauiApp1.Models
{
    public class ChatSession : INotifyPropertyChanged
    {
        private string _id;
        private string _title;
        private DateTime _createdAt;
        private DateTime _lastModified;
        private ObservableCollection<ChatMessage> _messages;

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set
            {
                _createdAt = value;
                OnPropertyChanged(nameof(CreatedAt));
            }
        }

        public DateTime LastModified
        {
            get => _lastModified;
            set
            {
                _lastModified = value;
                OnPropertyChanged(nameof(LastModified));
            }
        }

        public ObservableCollection<ChatMessage> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        public ChatSession()
        {
            Id = Guid.NewGuid().ToString();
            Title = "新对话";
            CreatedAt = DateTime.Now;
            LastModified = DateTime.Now;
            Messages = new ObservableCollection<ChatMessage>();
        }

        public ChatSession(string title) : this()
        {
            Title = title;
        }

        public void AddMessage(ChatMessage message)
        {
            Messages.Add(message);
            LastModified = DateTime.Now;
            
            // 如果是第一条用户消息，用它来更新标题
            if (message.Type == MessageType.User && Messages.Count(m => m.Type == MessageType.User) == 1)
            {
                Title = message.Content.Length > 20 ? message.Content.Substring(0, 20) + "..." : message.Content;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

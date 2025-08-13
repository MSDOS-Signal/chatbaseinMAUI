using System.ComponentModel;

namespace MauiApp1.Models
{
    public enum MessageType
    {
        User,
        Assistant
    }

    public class ChatMessage : INotifyPropertyChanged
    {
        private string _content = string.Empty;
        private MessageType _type;
        private DateTime _timestamp;
        private bool _isLoading;
        private bool _isStreaming;

        public string Content
        {
            get => _content;
            set
            {
                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        public MessageType Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged(nameof(Type));
            }
        }

        public DateTime Timestamp
        {
            get => _timestamp;
            set
            {
                _timestamp = value;
                OnPropertyChanged(nameof(Timestamp));
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

        public bool IsStreaming
        {
            get => _isStreaming;
            set
            {
                _isStreaming = value;
                OnPropertyChanged(nameof(IsStreaming));
            }
        }

        public ChatMessage(string content, MessageType type)
        {
            Content = content;
            Type = type;
            Timestamp = DateTime.Now;
            IsLoading = false;
            IsStreaming = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

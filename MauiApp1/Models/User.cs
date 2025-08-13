using System.ComponentModel;

namespace MauiApp1.Models
{
    public class User : INotifyPropertyChanged
    {
        private string _id = string.Empty;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private DateTime _createdAt;
        private DateTime _lastLoginAt;

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
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

        public DateTime LastLoginAt
        {
            get => _lastLoginAt;
            set
            {
                _lastLoginAt = value;
                OnPropertyChanged(nameof(LastLoginAt));
            }
        }

        public User()
        {
            Id = Guid.NewGuid().ToString();
            CreatedAt = DateTime.Now;
            LastLoginAt = DateTime.Now;
        }

        public User(string username, string password) : this()
        {
            Username = username;
            Password = password;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

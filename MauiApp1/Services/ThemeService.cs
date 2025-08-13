using System.ComponentModel;

namespace MauiApp1.Services
{
    public class ThemeService : INotifyPropertyChanged
    {
        private bool _isDarkMode;

        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                _isDarkMode = value;
                OnPropertyChanged(nameof(IsDarkMode));
                ApplyTheme();
            }
        }

        public ThemeService()
        {
            // 默认使用系统主题
            IsDarkMode = Application.Current?.RequestedTheme == AppTheme.Dark;
        }

        public void ToggleTheme()
        {
            IsDarkMode = !IsDarkMode;
        }

        private void ApplyTheme()
        {
            if (Application.Current != null)
            {
                Application.Current.UserAppTheme = IsDarkMode ? AppTheme.Dark : AppTheme.Light;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

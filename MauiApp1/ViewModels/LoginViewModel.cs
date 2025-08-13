using System.ComponentModel;
using System.Windows.Input;
using MauiApp1.Services;

namespace MauiApp1.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _hasError;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
                OnPropertyChanged(nameof(CanLogin));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                OnPropertyChanged(nameof(CanLogin));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
                HasError = !string.IsNullOrEmpty(value);
            }
        }

        public bool HasError
        {
            get => _hasError;
            set
            {
                _hasError = value;
                OnPropertyChanged(nameof(HasError));
            }
        }

        public bool CanLogin => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);

        public ICommand LoginCommand { get; }
        public ICommand ShowRegisterCommand { get; }

        public LoginViewModel()
        {
            _authService = new AuthService();
            LoginCommand = new Command(async () => await LoginAsync());
            ShowRegisterCommand = new Command(async () => await ShowRegisterAsync());
        }

        private async Task LoginAsync()
        {
            try
            {
                ErrorMessage = string.Empty;
                
                if (await _authService.LoginAsync(Username, Password))
                {
                    // 登录成功，导航到主页面
                    await Shell.Current.GoToAsync("///MainPage");
                }
                else
                {
                    ErrorMessage = "用户名或密码错误";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"登录失败: {ex.Message}";
            }
        }

        private async Task ShowRegisterAsync()
        {
            await Shell.Current.GoToAsync("///RegisterPage");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

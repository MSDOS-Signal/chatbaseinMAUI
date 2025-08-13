using System.ComponentModel;
using System.Windows.Input;
using MauiApp1.Services;

namespace MauiApp1.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _errorMessage = string.Empty;
        private string _successMessage = string.Empty;
        private bool _hasError;
        private bool _hasSuccess;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
                OnPropertyChanged(nameof(CanRegister));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                OnPropertyChanged(nameof(CanRegister));
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
                OnPropertyChanged(nameof(CanRegister));
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

        public string SuccessMessage
        {
            get => _successMessage;
            set
            {
                _successMessage = value;
                OnPropertyChanged(nameof(SuccessMessage));
                HasSuccess = !string.IsNullOrEmpty(value);
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

        public bool HasSuccess
        {
            get => _hasSuccess;
            set
            {
                _hasSuccess = value;
                OnPropertyChanged(nameof(HasSuccess));
            }
        }

        public bool CanRegister => !string.IsNullOrWhiteSpace(Username) && 
                                  !string.IsNullOrWhiteSpace(Password) && 
                                  !string.IsNullOrWhiteSpace(ConfirmPassword) &&
                                  Password == ConfirmPassword &&
                                  Password.Length >= 6;

        public ICommand RegisterCommand { get; }
        public ICommand BackToLoginCommand { get; }

        public RegisterViewModel()
        {
            _authService = new AuthService();
            RegisterCommand = new Command(async () => await RegisterAsync());
            BackToLoginCommand = new Command(async () => await BackToLoginAsync());
        }

        private async Task RegisterAsync()
        {
            try
            {
                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

                if (Password != ConfirmPassword)
                {
                    ErrorMessage = "两次输入的密码不一致";
                    return;
                }

                if (Password.Length < 6)
                {
                    ErrorMessage = "密码长度至少6位";
                    return;
                }

                if (await _authService.RegisterAsync(Username, Password))
                {
                    SuccessMessage = "注册成功！正在跳转到登录页面...";
                    await Task.Delay(2000);
                    await BackToLoginAsync();
                }
                else
                {
                    ErrorMessage = "用户名已存在";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"注册失败: {ex.Message}";
            }
        }

        private async Task BackToLoginAsync()
        {
            await Shell.Current.GoToAsync("///LoginPage");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

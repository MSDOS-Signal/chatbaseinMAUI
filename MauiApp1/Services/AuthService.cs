using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json;
using MauiApp1.Models;

namespace MauiApp1.Services
{
    public class AuthService : INotifyPropertyChanged
    {
        private User? _currentUser;
        private readonly string _usersFilePath;
        private readonly string _currentUserFilePath;

        public User? CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
                OnPropertyChanged(nameof(IsLoggedIn));
            }
        }

        public bool IsLoggedIn => CurrentUser != null;

        public AuthService()
        {
            _usersFilePath = Path.Combine(FileSystem.AppDataDirectory, "users.json");
            _currentUserFilePath = Path.Combine(FileSystem.AppDataDirectory, "current_user.json");
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            try
            {
                var users = await LoadUsersAsync();
                
                // 检查用户名是否已存在
                if (users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                {
                    return false; // 用户名已存在
                }

                var newUser = new User(username, password);
                users.Add(newUser);
                
                await SaveUsersAsync(users);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"注册失败: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var users = await LoadUsersAsync();
                var user = users.FirstOrDefault(u => 
                    u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && 
                    u.Password == password);

                if (user != null)
                {
                    user.LastLoginAt = DateTime.Now;
                    await SaveUsersAsync(users);
                    CurrentUser = user;
                    await SaveCurrentUserAsync(user);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"登录失败: {ex.Message}");
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            CurrentUser = null;
            await DeleteCurrentUserFileAsync();
        }

        public async Task<bool> AutoLoginAsync()
        {
            try
            {
                if (File.Exists(_currentUserFilePath))
                {
                    var json = await File.ReadAllTextAsync(_currentUserFilePath);
                    var user = JsonSerializer.Deserialize<User>(json);
                    if (user != null)
                    {
                        CurrentUser = user;
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"自动登录失败: {ex.Message}");
                return false;
            }
        }

        private async Task<List<User>> LoadUsersAsync()
        {
            try
            {
                if (File.Exists(_usersFilePath))
                {
                    var json = await File.ReadAllTextAsync(_usersFilePath);
                    return JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
                }
                return new List<User>();
            }
            catch
            {
                return new List<User>();
            }
        }

        private async Task SaveUsersAsync(List<User> users)
        {
            var json = JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_usersFilePath, json);
        }

        private async Task SaveCurrentUserAsync(User user)
        {
            var json = JsonSerializer.Serialize(user, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_currentUserFilePath, json);
        }

        private async Task DeleteCurrentUserFileAsync()
        {
            if (File.Exists(_currentUserFilePath))
            {
                await Task.Run(() => File.Delete(_currentUserFilePath));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

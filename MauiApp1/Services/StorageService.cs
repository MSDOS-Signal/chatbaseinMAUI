using System.Text.Json;
using MauiApp1.Models;
using System.Collections.ObjectModel;

namespace MauiApp1.Services
{
    public class StorageService
    {
        private string _storagePath;
        private JsonSerializerOptions _jsonOptions;
        private string? _currentUserId;

        public StorageService()
        {
            _storagePath = Path.Combine(FileSystem.AppDataDirectory, "ChatHistory");
            Directory.CreateDirectory(_storagePath);
            
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public void SetCurrentUser(string userId)
        {
            _currentUserId = userId;
            _storagePath = Path.Combine(FileSystem.AppDataDirectory, "ChatHistory", userId);
            Directory.CreateDirectory(_storagePath);
        }

        public async Task SaveSessionsAsync(ObservableCollection<ChatSession> sessions)
        {
            try
            {
                var sessionsData = sessions.Select(session => new
                {
                    session.Id,
                    session.Title,
                    session.CreatedAt,
                    session.LastModified,
                    Messages = session.Messages.Select(msg => new
                    {
                        msg.Content,
                        msg.Type,
                        msg.Timestamp,
                        msg.IsStreaming
                    }).ToList()
                }).ToList();

                var json = JsonSerializer.Serialize(sessionsData, _jsonOptions);
                var filePath = Path.Combine(_storagePath, "sessions.json");
                await File.WriteAllTextAsync(filePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"保存会话失败: {ex.Message}");
            }
        }

        public async Task<ObservableCollection<ChatSession>> LoadSessionsAsync()
        {
            try
            {
                var filePath = Path.Combine(_storagePath, "sessions.json");
                if (!File.Exists(filePath))
                    return new ObservableCollection<ChatSession>();

                var json = await File.ReadAllTextAsync(filePath);
                var sessionsData = JsonSerializer.Deserialize<List<dynamic>>(json, _jsonOptions);

                var sessions = new ObservableCollection<ChatSession>();
                foreach (var sessionData in sessionsData)
                {
                    var session = new ChatSession
                    {
                        Id = sessionData.GetProperty("id").GetString(),
                        Title = sessionData.GetProperty("title").GetString(),
                        CreatedAt = sessionData.GetProperty("createdAt").GetDateTime(),
                        LastModified = sessionData.GetProperty("lastModified").GetDateTime()
                    };

                    var messages = sessionData.GetProperty("messages");
                    foreach (var msgData in messages.EnumerateArray())
                    {
                        var content = msgData.GetProperty("content").GetString();
                        var typeStr = msgData.GetProperty("type").GetString();
                        var timestamp = msgData.GetProperty("timestamp").GetDateTime();
                        var isStreaming = msgData.GetProperty("isStreaming").GetBoolean();

                        var messageType = typeStr == "User" ? MessageType.User : MessageType.Assistant;
                        var message = new ChatMessage(content, messageType)
                        {
                            Timestamp = timestamp,
                            IsStreaming = isStreaming
                        };
                        session.Messages.Add(message);
                    }

                    sessions.Add(session);
                }

                return sessions;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"加载会话失败: {ex.Message}");
                return new ObservableCollection<ChatSession>();
            }
        }

        public async Task DeleteSessionAsync(string sessionId)
        {
            try
            {
                var sessions = await LoadSessionsAsync();
                var sessionToRemove = sessions.FirstOrDefault(s => s.Id == sessionId);
                if (sessionToRemove != null)
                {
                    sessions.Remove(sessionToRemove);
                    await SaveSessionsAsync(sessions);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"删除会话失败: {ex.Message}");
            }
        }

        public async Task ClearAllSessionsAsync()
        {
            try
            {
                var filePath = Path.Combine(_storagePath, "sessions.json");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"清空会话失败: {ex.Message}");
            }
        }
    }
}

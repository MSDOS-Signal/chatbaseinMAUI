using Newtonsoft.Json;
using System.Text;
using System.IO;
using MauiApp1.Models;

namespace MauiApp1.Services
{
    public class AIService
    {
        private readonly HttpClient _httpClient;
        private const string API_KEY = "sk-jisussvbdngycoyhktajjvfqakobrrunazkucdbkjjrxyoyh";
        private const string API_URL = "https://api.siliconflow.cn/v1/chat/completions";

        public AIService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_KEY}");
        }

        public async Task<string> GetResponseAsync(string userMessage)
        {
            try
            {
                var requestBody = new
                {
                    model = "Qwen/Qwen2.5-72B-Instruct",
                    messages = new[]
                    {
                        new
                        {
                            role = "user",
                            content = userMessage
                        }
                    },
                    max_tokens = 2048,
                    temperature = 0.7,
                    top_p = 0.8
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(API_URL, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    return result?.choices?[0]?.message?.content?.ToString() ?? "抱歉，我无法生成回复。";
                }
                else
                {
                    return $"API请求失败: {response.StatusCode} - {responseContent}";
                }
            }
            catch (Exception ex)
            {
                return $"发生错误: {ex.Message}";
            }
        }

        public async IAsyncEnumerable<string> GetStreamingResponseAsync(string userMessage, List<ChatMessage> conversationHistory = null)
        {
            var messages = new List<object>();
            
            // 添加对话历史（最多保留最近10轮对话）
            if (conversationHistory != null && conversationHistory.Count > 0)
            {
                var recentHistory = conversationHistory.TakeLast(20).ToList(); // 保留最近20条消息
                foreach (var msg in recentHistory)
                {
                    messages.Add(new
                    {
                        role = msg.Type == MessageType.User ? "user" : "assistant",
                        content = msg.Content
                    });
                }
            }
            
            // 添加当前用户消息
            messages.Add(new
            {
                role = "user",
                content = userMessage
            });

            var requestBody = new
            {
                model = "Qwen/Qwen2.5-72B-Instruct",
                messages = messages.ToArray(),
                max_tokens = 2048,
                temperature = 0.7,
                top_p = 0.8,
                stream = true
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await GetHttpResponseAsync(content);
            if (response == null)
            {
                yield return "网络连接失败";
                yield break;
            }

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                yield return $"API请求失败: {response.StatusCode} - {responseContent}";
                yield break;
            }

            var stream = await response.Content.ReadAsStreamAsync();
            var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(line)) continue;
                
                if (line.StartsWith("data: "))
                {
                    var data = line.Substring(6);
                    if (data == "[DONE]") break;
                    
                    var deltaContent = ParseStreamData(data);
                    if (!string.IsNullOrEmpty(deltaContent))
                    {
                        yield return deltaContent;
                    }
                }
            }
        }

        private async Task<HttpResponseMessage?> GetHttpResponseAsync(StringContent content)
        {
            try
            {
                return await _httpClient.PostAsync(API_URL, content);
            }
            catch
            {
                return null;
            }
        }

        private string? ParseStreamData(string data)
        {
            try
            {
                var jsonData = JsonConvert.DeserializeObject<dynamic>(data);
                return jsonData?.choices?[0]?.delta?.content?.ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}

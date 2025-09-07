using Newtonsoft.Json;
using System;
using System.Text;
using System.Text.Json;

namespace BotFix
{
    internal class TelegramController
    {
        public delegate void GetMessageEvent(string messageText, long userId);

        public event GetMessageEvent? OnEventOccurred;

        private string _botToken;
        private HttpClient _httpClient;
        private long offset = 0;
        public TelegramController(string token) 
        {
            _botToken = token;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri($"https://api.telegram.org/bot{_botToken}/");
        }

        public async void MessageChecker()
        {
            try
            {
                while (true)
                {
                    var response = await _httpClient.GetAsync($"getUpdates?offset={offset}&timeout=1");
                    var strReader = response.Content.ReadAsStringAsync();

                    using (var document = JsonDocument.Parse(strReader.Result))
                    {
                        foreach (var update in document.RootElement.GetProperty("result").EnumerateArray())
                        {
                            offset = update.GetProperty("update_id").GetInt32() + 1;

                            OnEventOccurred.Invoke(update.GetProperty("message").GetProperty("text").GetString(), update.GetProperty("message").GetProperty("from").GetProperty("id").GetInt64());
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ошибка при отправке сообщения: {ex.Message}");
            }
        }

        public void SendMessage(string massage, long chatID)
        {
            var playload = new
            {
                chat_id = chatID,
                text = massage
            };

            var responge = _httpClient.PostAsync(
                $"sendMessage",
                new StringContent(JsonConvert.SerializeObject(playload), Encoding.UTF8, "application/json")
                );

            responge.Wait();
        }
    }
}

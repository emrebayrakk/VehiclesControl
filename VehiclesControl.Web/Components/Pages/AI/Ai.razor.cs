using Blazored.TextEditor;
using Newtonsoft.Json.Linq;

namespace VehiclesControl.Web.Components.Pages.AI
{
    public partial class Ai
    {
        private string message { get; set; } = "Merhaba, Nasıl yardımcı olabilirim?";
        private string response { get; set; } = string.Empty;
        private string userMessage { get; set; } = string.Empty;
        private string apiKey { get; set; } = string.Empty;
        private string aiResponse { get; set; } = string.Empty;
        private Dictionary<string, string> aiChats { get; set; } = new Dictionary<string, string>();
        private bool activeSpinner { get; set; } = false;
        private async Task SubmitMessageAsync()
        {
            activeSpinner = true;
            var req = userMessage;
            userMessage = "";
            //ToDo appsettings.json apikey
            apiKey = "";
            var path = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent?key={apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
            new
            {
                parts = new[]
                {
                    new { text = req }
                }
            }
        }
            };

            var res = await Http.PostAsJsonAsync(path, requestBody);

            if (res.IsSuccessStatusCode)
            {
                var jsonString = await res.Content.ReadAsStringAsync();

                var jsonResponse = JObject.Parse(jsonString);

                string modelResponseText = jsonResponse["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                response = !string.IsNullOrEmpty(modelResponseText) ? modelResponseText : "Yanıt alınamadı.";
            }
            else
            {
                response = "İstek başarısız oldu.";
            }
            aiChats.Add(req, response);
            activeSpinner = false;
        }



    }
}




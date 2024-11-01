using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;
using VehiclesControl.Web.Utils;

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
            string userText = ConstantsData.GEMINI_API_PROMPT;
            string modelText = ConstantsData.GEMINI_API_MODEL_TEXT;
            activeSpinner = true;
            var req = userMessage;
            userMessage = "";


            apiKey = ConstantsData.GEMINI_API_KEY;
            var path = ConstantsData.GEMINI_API_PATH + apiKey;

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new { text = userText }
                        }
                    },
                    new
                    {
                        role = "model",
                        parts = new[]
                        {
                            new { text = modelText }
                        }
                    },
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new { text = req }
                        }
                    }
                },
                generationConfig = new 
                {
                    temperature = 2,
                    topK = 40,
                    topP = 0.95,
                    maxOutputTokens = 8192,
                    responseMimeType = "application/json"
                }
            };

            var res = await Http.PostAsJsonAsync(path, requestBody);

            if (res.IsSuccessStatusCode)
            {
                var jsonString = await res.Content.ReadAsStringAsync();

                var jsonResponse = JObject.Parse(jsonString);

                string? contentText = jsonResponse["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
                if (!string.IsNullOrEmpty(contentText))
                {
                    JsonNode contentJson = JsonNode.Parse(contentText);
                    string? modelResponseText = contentJson?["response"]?.ToString();

                    if (!string.IsNullOrEmpty(modelResponseText))
                    {
                        response =  modelResponseText;
                    }
                    else
                    {
                        response = "Response bulunamadı.";
                    }
                }
                else
                {
                    response = "Content text bulunamadı.";
                }  
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




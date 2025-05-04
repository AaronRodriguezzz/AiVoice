using System;
using System.Linq; // Needed for FirstOrDefault
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json; // Make sure you have the Newtonsoft.Json NuGet package installed

namespace AiVoice
{
    public static class GeminiClass
    {

        private static readonly string apiKey = "AIzaSyBH_0HpxQsfClI-ovA3JSzKnOMJBUydIs8"; 
        private static readonly string modelName = "gemini-1.5-flash-latest"; 
        private static readonly string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/{modelName}:generateContent?key={apiKey}";
        private static readonly HttpClient client = new HttpClient();

        public static async Task<string> GetGeminiResponse(string userMessage, string selectedLangguage)
        {

            var client = new HttpClient();

            string context = "You are programer chatbot that will answer programming related questions only. " +
                "And you're only allowed to answer questions based on " + selectedLangguage + 
                "You will apologize if you're ask about other langguage and advise to change the selected langguage in the combo box. " ;

            string fullMessage = context + "\nNow, please respond to this: " + userMessage;

            var requestBody = new
            {
                contents = new[]
                {
                    new {
                        parts = new[]
                        {
                            new { text = userMessage }
                        }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpoint, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                dynamic result = JsonConvert.DeserializeObject(responseString);
                return result.candidates[0].content.parts[0].text;
            }
            else
            {
                return $"Error: {response.StatusCode}\n{responseString}";
            }
        }
    }
}


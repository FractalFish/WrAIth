using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Net.Http.Headers;
using Wraith.Constants;

namespace Wraith
{
    public class LLMService
    {
        private readonly AppSettings _settings;
        private readonly HttpClient _httpClient;

        public LLMService(AppSettings settings)
        {
            _settings = settings;
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(2);
        }

        public async Task<string> GetResponseAsync(string userMessage, string? base64Image = null)
        {
            // Get appropriate model based on whether we're processing an image
            ModelConfig? model;
            if (!string.IsNullOrEmpty(base64Image))
            {
                model = _settings.GetDefaultVisionModel();
                if (model == null)
                {
                    throw new InvalidOperationException("No vision model configured. Please add a model with vision support in settings.");
                }
            }
            else
            {
                model = _settings.GetDefaultTextModel();
                if (model == null)
                {
                    throw new InvalidOperationException("No text model configured. Please add a model in settings.");
                }
            }

            // Log which model is being used
            string keyPreview = string.IsNullOrEmpty(model.ApiKey) ? "EMPTY" : $"{model.ApiKey.Substring(0, Math.Min(8, model.ApiKey.Length))}...";
            Log($"Using model: {model.Name} (Provider={model.Provider}, Model={model.Model}, Key={keyPreview})");
            Log($"Endpoint: {model.Endpoint}");

            if (string.IsNullOrWhiteSpace(model.ApiKey))
            {
                throw new InvalidOperationException($"API Key is not configured for model '{model.Name}'.");
            }

            try
            {
                // Build request based on provider
                if (model.Provider.Equals(ProviderTypes.OpenAI, StringComparison.OrdinalIgnoreCase) ||
                    model.Provider.Equals(ProviderTypes.Groq, StringComparison.OrdinalIgnoreCase) ||
                    model.Provider.Equals(ProviderTypes.Custom, StringComparison.OrdinalIgnoreCase))
                {
                    return await GetOpenAIResponseAsync(userMessage, base64Image, model);
                }
                else if (model.Provider.Equals(ProviderTypes.Anthropic, StringComparison.OrdinalIgnoreCase))
                {
                    return await GetAnthropicResponseAsync(userMessage, base64Image, model);
                }
                else
                {
                    // Default to OpenAI-compatible API
                    return await GetOpenAIResponseAsync(userMessage, base64Image, model);
                }
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        private async Task<string> GetOpenAIResponseAsync(string userMessage, string? base64Image, ModelConfig model)
        {
            object requestBody;

            if (!string.IsNullOrEmpty(base64Image))
            {
                // Vision request with image
                requestBody = new
                {
                    model = model.Model,
                    messages = new object[]
                    {
                        new { role = "system", content = _settings.SystemPrompt },
                        new { 
                            role = "user", 
                            content = new object[]
                            {
                                new { type = "text", text = userMessage },
                                new { 
                                    type = "image_url",
                                    image_url = new { url = $"data:image/png;base64,{base64Image}" }
                                }
                            }
                        }
                    },
                    max_tokens = 2000
                };
            }
            else
            {
                // Regular text request
                requestBody = new
                {
                    model = model.Model,
                    messages = new[]
                    {
                        new { role = "system", content = _settings.SystemPrompt },
                        new { role = "user", content = userMessage }
                    },
                    temperature = 0.7,
                    max_tokens = 2000
                };
            }

            var request = new HttpRequestMessage(HttpMethod.Post, model.Endpoint);
            request.Headers.Add("Authorization", $"Bearer {model.ApiKey}");
            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                MediaTypeHeaderValue.Parse("application/json"));

            Log($"Sending request to {model.Endpoint}");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Log($"API Error: {response.StatusCode} - {responseContent}");
                throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
            }

            var jsonResponse = JsonNode.Parse(responseContent);
            if (jsonResponse?["choices"]?[0]?["message"]?["content"] != null)
            {
                string content = jsonResponse["choices"]![0]!["message"]!["content"]!.ToString();
                Log($"Response received: {content.Length} characters");
                return content;
            }

            throw new Exception("Invalid response format from API");
        }

        private async Task<string> GetAnthropicResponseAsync(string userMessage, string? base64Image, ModelConfig model)
        {
            object requestBody;

            if (!string.IsNullOrEmpty(base64Image))
            {
                // Vision request with image
                requestBody = new
                {
                    model = model.Model,
                    max_tokens = 2000,
                    system = _settings.SystemPrompt,
                    messages = new[]
                    {
                        new { 
                            role = "user", 
                            content = new object[]
                            {
                                new { type = "text", text = userMessage },
                                new { 
                                    type = "image",
                                    source = new {
                                        type = "base64",
                                        media_type = "image/png",
                                        data = base64Image
                                    }
                                }
                            }
                        }
                    }
                };
            }
            else
            {
                // Regular text request
                requestBody = new
                {
                    model = model.Model,
                    max_tokens = 2000,
                    system = _settings.SystemPrompt,
                    messages = new[]
                    {
                        new { role = "user", content = userMessage }
                    }
                };
            }

            var request = new HttpRequestMessage(HttpMethod.Post, model.Endpoint);
            request.Headers.Add("x-api-key", model.ApiKey);
            request.Headers.Add("anthropic-version", "2023-06-01");
            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                MediaTypeHeaderValue.Parse("application/json"));

            Log($"Sending request to {model.Endpoint}");

            var response = await _httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Log($"API Error: {response.StatusCode} - {responseContent}");
                throw new HttpRequestException($"API request failed: {response.StatusCode} - {responseContent}");
            }

            var jsonResponse = JsonNode.Parse(responseContent);
            if (jsonResponse?["content"]?[0]?["text"] != null)
            {
                string content = jsonResponse["content"]![0]!["text"]!.ToString();
                Log($"Response received: {content.Length} characters");
                return content;
            }

            throw new Exception("Invalid response format from API");
        }

        private void Log(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            System.Diagnostics.Debug.WriteLine($"[{timestamp}] [LLMService] {message}");
            Console.WriteLine($"[{timestamp}] [LLMService] {message}");
        }
    }
}

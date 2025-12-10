using System.Text.Json.Serialization;

namespace BLLMT
{
    /// <summary>
    /// Defines a custom API request format for providers not using OpenAI or Anthropic formats
    /// </summary>
    public class CustomApiFormat
    {
        /// <summary>
        /// HTTP method (GET, POST, etc.)
        /// </summary>
        [JsonPropertyName("method")]
        public string Method { get; set; } = "POST";

        /// <summary>
        /// Custom headers as key-value pairs
        /// Format: "HeaderName: HeaderValue" per line
        /// Example:
        /// Authorization: Bearer {API_KEY}
        /// Content-Type: application/json
        /// </summary>
        [JsonPropertyName("headers")]
        public List<string> Headers { get; set; } = new List<string> { "Authorization: Bearer {API_KEY}" };

        /// <summary>
        /// Request body template in JSON format
        /// Use placeholders:
        /// {API_KEY} - Will be replaced with model's API key
        /// {SYSTEM_PROMPT} - Will be replaced with system prompt
        /// {USER_MESSAGE} - Will be replaced with user's message
        /// {IMAGE_BASE64} - Will be replaced with base64 image (for vision)
        /// {MODEL_ID} - Will be replaced with model ID
        /// 
        /// Example:
        /// {
        ///   "messages": [{"role": "user", "content": "{USER_MESSAGE}"}],
        ///   "model": "{MODEL_ID}",
        ///   "temperature": 0.7
        /// }
        /// </summary>
        [JsonPropertyName("requestBodyTemplate")]
        public string RequestBodyTemplate { get; set; } = @"{
  ""messages"": [
    {
      ""role"": ""system"",
      ""content"": ""{SYSTEM_PROMPT}""
    },
    {
      ""role"": ""user"",
      ""content"": ""{USER_MESSAGE}""
    }
  ],
  ""model"": ""{MODEL_ID}"",
  ""temperature"": 0.7,
  ""max_tokens"": 2000
}";

        /// <summary>
        /// Request body template for vision requests (with images)
        /// Same placeholders as RequestBodyTemplate, plus {IMAGE_BASE64}
        /// </summary>
        [JsonPropertyName("visionRequestBodyTemplate")]
        public string VisionRequestBodyTemplate { get; set; } = @"{
  ""messages"": [
    {
      ""role"": ""system"",
      ""content"": ""{SYSTEM_PROMPT}""
    },
    {
      ""role"": ""user"",
      ""content"": [
        {
          ""type"": ""text"",
          ""text"": ""{USER_MESSAGE}""
        },
        {
          ""type"": ""image_url"",
          ""image_url"": {
            ""url"": ""data:image/png;base64,{IMAGE_BASE64}""
          }
        }
      ]
    }
  ],
  ""model"": ""{MODEL_ID}"",
  ""max_tokens"": 2000
}";

        /// <summary>
        /// JSON path to extract response text from API response
        /// Use dot notation for nested properties, [0] for arrays
        /// 
        /// Examples:
        /// - OpenAI: "choices[0].message.content"
        /// - Anthropic: "content[0].text"
        /// - Custom: "response.text" or "data.output"
        /// </summary>
        [JsonPropertyName("responseTextPath")]
        public string ResponseTextPath { get; set; } = "choices[0].message.content";
    }
}

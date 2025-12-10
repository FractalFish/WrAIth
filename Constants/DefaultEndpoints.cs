namespace BLLMT.Constants
{
    /// <summary>
    /// Default API endpoints for supported providers
    /// </summary>
    public static class DefaultEndpoints
    {
        /// <summary>
        /// OpenAI chat completions endpoint
        /// </summary>
        public const string OpenAI = "https://api.openai.com/v1/chat/completions";

        /// <summary>
        /// Anthropic messages endpoint
        /// </summary>
        public const string Anthropic = "https://api.anthropic.com/v1/messages";

        /// <summary>
        /// Groq chat completions endpoint (OpenAI-compatible)
        /// </summary>
        public const string Groq = "https://api.groq.com/openai/v1/chat/completions";

        /// <summary>
        /// Together AI endpoint (OpenAI-compatible)
        /// </summary>
        public const string TogetherAI = "https://api.together.xyz/v1/chat/completions";

        /// <summary>
        /// Perplexity AI endpoint (OpenAI-compatible)
        /// </summary>
        public const string Perplexity = "https://api.perplexity.ai/chat/completions";

        /// <summary>
        /// Local Ollama endpoint (OpenAI-compatible)
        /// </summary>
        public const string Ollama = "http://localhost:11434/v1/chat/completions";

        /// <summary>
        /// Local LM Studio endpoint (OpenAI-compatible)
        /// </summary>
        public const string LMStudio = "http://localhost:1234/v1/chat/completions";

        /// <summary>
        /// Get default endpoint for a provider type
        /// </summary>
        public static string GetFor(string provider) => provider switch
        {
            ProviderTypes.OpenAI => OpenAI,
            ProviderTypes.Anthropic => Anthropic,
            ProviderTypes.Groq => Groq,
            _ => OpenAI // Default fallback
        };
    }
}

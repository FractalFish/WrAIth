namespace BLLMT.Constants
{
    /// <summary>
    /// Provider type constants for API format identification
    /// </summary>
    public static class ProviderTypes
    {
        /// <summary>
        /// OpenAI-compatible API format (Bearer token authentication)
        /// </summary>
        public const string OpenAI = "OpenAI";

        /// <summary>
        /// Anthropic API format (x-api-key authentication)
        /// </summary>
        public const string Anthropic = "Anthropic";

        /// <summary>
        /// Groq API (OpenAI-compatible format)
        /// </summary>
        public const string Groq = "Groq";

        /// <summary>
        /// User-defined custom API format
        /// </summary>
        public const string Custom = "Custom";

        /// <summary>
        /// Get all available provider types
        /// </summary>
        public static readonly string[] All = { OpenAI, Anthropic, Groq, Custom };
    }
}

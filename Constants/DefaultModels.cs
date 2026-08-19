namespace Wraith.Constants
{
    /// <summary>
    /// Default model IDs for supported providers
    /// </summary>
    public static class DefaultModels
    {
        #region OpenAI Models
        /// <summary>
        /// OpenAI GPT-4o - Most capable model with vision
        /// </summary>
        public const string OpenAI_GPT4o = "gpt-4o";

        /// <summary>
        /// OpenAI GPT-4o-mini - Fast and cost-effective
        /// </summary>
        public const string OpenAI_GPT4oMini = "gpt-4o-mini";

        /// <summary>
        /// OpenAI GPT-4 Turbo - Previous generation flagship
        /// </summary>
        public const string OpenAI_GPT4Turbo = "gpt-4-turbo";

        /// <summary>
        /// OpenAI GPT-3.5 Turbo - Legacy fast model
        /// </summary>
        public const string OpenAI_GPT35Turbo = "gpt-3.5-turbo";
        #endregion

        #region Anthropic Models
        /// <summary>
        /// Anthropic Claude 3.5 Sonnet - Most capable (2024-10-22)
        /// </summary>
        public const string Anthropic_Claude35Sonnet = "claude-3-5-sonnet-20241022";

        /// <summary>
        /// Anthropic Claude 3.5 Haiku - Fast and efficient
        /// </summary>
        public const string Anthropic_Claude35Haiku = "claude-3-5-haiku-20241022";

        /// <summary>
        /// Anthropic Claude 3 Opus - Previous generation flagship
        /// </summary>
        public const string Anthropic_Claude3Opus = "claude-3-opus-20240229";
        #endregion

        #region Groq Models
        /// <summary>
        /// Groq Llama 3.1 70B - Capable open source model
        /// </summary>
        public const string Groq_Llama3170B = "llama-3.1-70b-versatile";

        /// <summary>
        /// Groq Llama 3.1 8B - Fast open source model
        /// </summary>
        public const string Groq_Llama318B = "llama-3.1-8b-instant";

        /// <summary>
        /// Groq Llama 3 70B - Previous generation
        /// </summary>
        public const string Groq_Llama370B = "llama3-70b-8192";
        #endregion

        #region Default Selections
        /// <summary>
        /// Default fast text model (balance of speed and capability)
        /// </summary>
        public const string DefaultFast = OpenAI_GPT4oMini;

        /// <summary>
        /// Default capable text model (best quality)
        /// </summary>
        public const string DefaultCapable = OpenAI_GPT4o;

        /// <summary>
        /// Default vision model
        /// </summary>
        public const string DefaultVision = OpenAI_GPT4o;
        #endregion

        /// <summary>
        /// Get default model for a provider type
        /// </summary>
        public static string GetDefaultFor(string provider) => provider switch
        {
            ProviderTypes.OpenAI => OpenAI_GPT4oMini,
            ProviderTypes.Anthropic => Anthropic_Claude35Haiku,
            ProviderTypes.Groq => Groq_Llama318B,
            _ => OpenAI_GPT4oMini
        };
    }
}

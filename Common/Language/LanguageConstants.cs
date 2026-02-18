namespace Common.Language
{
    public static class LanguageConstants
    {
        public const string English = "en";
        public const string Hindi = "hi";
        public const string Tamil = "ta";
        public const string Gujarati = "gu";
        
        public static readonly IReadOnlyList<string> SupportedLanguages = new[]
        {
            English, Hindi, Tamil, Gujarati
        };
        
        public static readonly IReadOnlyDictionary<string, string> LanguageNames = new Dictionary<string, string>
        {
            { English, "English" },
            { Hindi, "हिन्दी" },
            { Tamil, "தமிழ்" },
            { Gujarati, "ગુજરાતી" }
        };
        
        public const string DefaultLanguage = English;
        public const string AcceptLanguageHeader = "Accept-Language";
    }
}

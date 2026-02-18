namespace MasterService.Domain
{
    /// <summary>
    /// Allowed CMS content keys. Create/Update only accepts these exact keys - no typo or custom key.
    /// Add new key here when you need a new CMS page/section.
    /// </summary>
    public static class CmsContentKeys
    {
        public const string TermsAndConditions = "terms_and_conditions";
        public const string PrivacyPolicy = "privacy_policy";
        public const string AboutUs = "about_us";
        public const string Faq = "faq";
        public const string ContactUs = "contact_us";

        private static readonly Lazy<IReadOnlyList<string>> AllKeys = new(() => new[]
        {
            TermsAndConditions,
            PrivacyPolicy,
            AboutUs,
            Faq,
            ContactUs
        });

        /// <summary>All allowed key strings (exact match required).</summary>
        public static IReadOnlyList<string> All => AllKeys.Value;

        /// <summary>Check if key is allowed (exact, case-sensitive).</summary>
        public static bool IsValid(string? key)
        {
            if (string.IsNullOrWhiteSpace(key)) return false;
            return All.Contains(key.Trim());
        }

        /// <summary>Error message listing allowed keys.</summary>
        public static string InvalidKeyMessage(string? suppliedKey)
        {
            return $"Invalid CMS key: '{suppliedKey}'. Allowed keys (exact match): {string.Join(", ", All)}.";
        }

        /// <summary>Error message when key is already used.</summary>
        public static string AlreadyDefinedMessage(string key)
        {
            return $"CMS content with key '{key}' is already defined. Use Update for existing or choose a different allowed key.";
        }
    }
}

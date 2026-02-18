namespace MasterService.Application.Exceptions
{
    /// <summary>Invalid CMS key (not in allowed list). Return 400.</summary>
    public class CmsContentKeyInvalidException : ArgumentException
    {
        public CmsContentKeyInvalidException(string? key, string message) : base(message)
        {
            Key = key;
        }

        public string? Key { get; }
    }

    /// <summary>CMS key already exists. Return 409 Conflict.</summary>
    public class CmsContentKeyAlreadyDefinedException : InvalidOperationException
    {
        public CmsContentKeyAlreadyDefinedException(string key, string message) : base(message)
        {
            Key = key;
        }

        public string Key { get; }
    }
}

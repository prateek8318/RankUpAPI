using System.Security.Claims;

namespace QuestionService.API.Helpers
{
    internal static class AuthClaimsHelper
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            var candidates = new[]
            {
                user.FindFirstValue(ClaimTypes.NameIdentifier),
                user.FindFirstValue("nameid"),
                user.FindFirstValue("userId"),
                user.FindFirstValue("UserId"),
                user.FindFirstValue("AdminId"),
                user.FindFirstValue("sub")
            };

            foreach (var value in candidates)
            {
                if (!string.IsNullOrWhiteSpace(value) && int.TryParse(value, out var id) && id > 0)
                {
                    return id;
                }
            }

            return 0;
        }
    }
}


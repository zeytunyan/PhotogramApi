using System.ComponentModel;
using System.Security.Claims;

namespace Common.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static bool TryFindGuidValue(this ClaimsPrincipal user, string claimName, out Guid guidRes)
        {
            var claimValue = user.FindFirstValue(claimName);
            return Guid.TryParse(claimValue, out guidRes);
        }
    }
}

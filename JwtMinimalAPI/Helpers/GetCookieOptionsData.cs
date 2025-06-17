using Microsoft.AspNetCore.CookiePolicy;

namespace JwtMinimalAPI.Helpers
{
    // FIX USERID IN METHOD NAME§
    public class GetCookieOptionsData
    {
        public static CookieOptions AccessTokenCookie()
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
            };
        }
        public static CookieOptions RefreshTokenAndUserIdCookie()
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };
        }
    }
}

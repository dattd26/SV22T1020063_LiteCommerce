using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace SV22T1020063.Shop.Models
{
    /// <summary>
    /// Thông tin lưu trong phiên người dùng (claims)
    /// </summary>
    public class WebUserData
    {
        public string RequestFeedback { get; set; } = "";
        public string UserId { get; set; } = "";
        public string UserName { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Photo { get; set; } = "";
        public List<string> Roles { get; set; } = new List<string>();

        /// <summary>
        /// Tạo User identity từ thông tin user data
        /// </summary>
        public ClaimsPrincipal CreatePrincipal()
        {
            var claims = new List<Claim>
            {
                new Claim(nameof(UserId), UserId),
                new Claim(nameof(UserName), UserName),
                new Claim(nameof(DisplayName), DisplayName),
                new Claim(nameof(Email), Email),
                new Claim(nameof(Photo), Photo)
            };
            foreach (var role in Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }
    }
}

using Infrastructure.Entity.AppProvider;
using Infrastructure.Entity.AppUser;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BLL.Tools
{
    public static class ProviderTokenGenerator
    {
        const string KEY = "PROVIDERS_SECURITY_KEY!@#$!";
        const string ISSUER = "Identity";
        const int VALID_MIN = 60;

        const string KEY_PROVIDER_ID = "provider.id";
        const string KEY_USER_ID = "user.id";

        public static SymmetricSecurityKey GetKey() =>
            new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));

        public static string Generate(string providerId, string userId)
        {
            var claims = new List<Claim>
            {
                new Claim(KEY_PROVIDER_ID, providerId),
                new Claim(KEY_USER_ID, userId)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Token", KEY_PROVIDER_ID, KEY_USER_ID);
            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                issuer: ISSUER,
                audience: "ALL",
                notBefore: now,
                claims: claimsIdentity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(VALID_MIN)),
                signingCredentials: new SigningCredentials(GetKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        /// <summary>
        /// Decrypt providers token
        /// </summary>
        /// <param name="token">Provider`s access token</param>
        /// <returns>(ProviderId, UserId)</returns>
        public static (string, string) DecryptToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = GetKey(),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            var claims = handler.ValidateToken(token, validations, out var tokenSecure);

            return (claims.FindFirst(x => x.Type == KEY_PROVIDER_ID).Value, claims.FindFirst(x => x.Type == KEY_USER_ID).Value);
        }
    }
}

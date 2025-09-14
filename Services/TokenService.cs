using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using MyControllerApi.Models;

namespace MyControllerApi.Services;

public class TokenService
{
    private readonly IConfiguration _config;
    private readonly byte[] _keyBytes;

    public TokenService(IConfiguration config)
    {
        _config = config;
        var signingKeyBase64Url = _config["Jwt:SigningKey"]
                                  ?? throw new InvalidOperationException("JWT SigningKey missing");
        _keyBytes = Base64UrlDecode(signingKeyBase64Url).ToArray();

        if (_keyBytes.Length != 32)
            throw new InvalidOperationException($"Signing key length is {_keyBytes.Length} bytes - expected 32 bytes (256 bits).");
    }

    public string CreateToken(AppUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!)
        };

        var securityKey = new SymmetricSecurityKey(_keyBytes);
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static ReadOnlySpan<byte> Base64UrlDecode(string base64Url)
    {
        string s = base64Url.Trim().Replace('-', '+').Replace('_', '/');
        switch (s.Length % 4)
        {
            case 2: s += "=="; break;
            case 3: s += "="; break;
        }
        return Convert.FromBase64String(s);
    }
}
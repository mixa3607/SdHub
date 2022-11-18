using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;

namespace SdHub.Options;

public class WebSecurityOptions
{
    public bool EnableHttpsRedirections { get; set; } = true;
    public bool EnableForwardedHeaders { get; set; } = true;
    public JwtOptions Jwt { get; set; } = new();
    public CorsOptions Cors { get; set; } = new();

    public class CorsOptions
    {
        public string[] AllowedHosts { get; set; } = Array.Empty<string>();
    }
    public class JwtOptions
    {
        private X509SecurityKey? _publicKey;
        private X509SecurityKey? _privateKey;
        private X509SigningCredentials? _signingCredentials;

        public string[] Issuers { get; set; } = Array.Empty<string>();
        public string? Issuer { get; set; }
        public string[] Audiences { get; set; } = Array.Empty<string>();
        public bool ValidateLifetime { get; set; } = true;
        public TimeSpan JwtLifetime { get; set; } = TimeSpan.FromMinutes(5);
        public TimeSpan RefreshTokenLifetime { get; set; } = TimeSpan.FromDays(5);

        public string? PfxPassword { get; set; }
        public string? PfxFile { get; set; }
        public bool LogPii { get; set; }

        public X509SecurityKey? PublicKey
        {
            get
            {
                ReadPfx();
                return _publicKey;
            }
        }

        public X509SecurityKey? PrivateKey
        {
            get
            {
                ReadPfx();
                return _privateKey;
            }
        }

        public X509SigningCredentials? SigningCredentials
        {
            get
            {
                ReadPfx();
                return _signingCredentials;
            }
        }

        private void ReadPfx()
        {
            if (PfxFile == null || _publicKey != null)
                return;

            var pfxBytes = File.ReadAllBytes(PfxFile);
            var cert = new X509Certificate2(pfxBytes, PfxPassword);
            var secKey = new X509SecurityKey(cert);
            if (secKey.PrivateKeyStatus == PrivateKeyStatus.Exists)
            {
                _signingCredentials = new X509SigningCredentials(cert);
                _privateKey = secKey;
            }
            _publicKey = secKey;
        }
    }
}
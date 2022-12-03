using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;

namespace SdHub.Options;

/// <summary>
/// Web security options
/// </summary>
public class WebSecurityOptions
{
    /// <summary>
    /// Enable https redirection
    /// </summary>
    public bool EnableHttpsRedirections { get; set; } = true;

    /// <summary>
    /// Enable forwarded headers like X-Forwarded-For
    /// </summary>
    public bool EnableForwardedHeaders { get; set; } = true;

    /// <summary>
    /// Jwt auth options
    /// </summary>
    public JwtOptions Jwt { get; set; } = new();

    /// <summary>
    /// CORS options
    /// </summary>
    public CorsOptions Cors { get; set; } = new();

    public class CorsOptions
    {
        /// <summary>
        /// Allowed hosts
        /// </summary>
        [Required]
        public string[] AllowedHosts { get; set; } = Array.Empty<string>();
    }

    public class JwtOptions
    {
        private X509SecurityKey? _publicKey;
        private X509SecurityKey? _privateKey;
        private X509SigningCredentials? _signingCredentials;

        /// <summary>
        /// Issuers
        /// </summary>
        [Required]
        public string[] Issuers { get; set; } = { "SdHub" };

        /// <summary>
        /// Issuer
        /// </summary>
        [Required]
        public string? Issuer { get; set; } = "SdHub";

        /// <summary>
        /// Audiences
        /// </summary>
        [Required]
        public string[] Audiences { get; set; } = { "SdHub" };

        /// <summary>
        /// Validate lifetime
        /// </summary>
        public bool ValidateLifetime { get; set; } = true;

        /// <summary>
        /// Lifetime
        /// </summary>
        public TimeSpan JwtLifetime { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Refresh token lifetime
        /// </summary>
        public TimeSpan RefreshTokenLifetime { get; set; } = TimeSpan.FromDays(5);

        /// <summary>
        /// Password for pfx cert. Can be null if not required
        /// </summary>
        public string? PfxPassword { get; set; }

        /// <summary>
        /// Path to pfx cert file
        /// </summary>
        [Required]
        public string? PfxFile { get; set; }

        /// <summary>
        /// Log more info about authorization
        /// </summary>
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
using SdHub.Services.Mailing;
using System.ComponentModel.DataAnnotations;

namespace SdHub.Options;

/// <summary>
/// Mailing options
/// </summary>
public class MailingOptions
{
    /// <summary>
    /// Sender email
    /// </summary>
    [Required]
    public string? From { get; set; }

    /// <summary>
    /// Mail server host
    /// </summary>
    [Required]
    public string? Host { get; set; }

    /// <summary>
    /// Mail server port
    /// </summary>
    public int Port { get; set; } = 587;

    /// <summary>
    /// Login
    /// </summary>
    [Required]
    public string? Username { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    [Required]
    public string? Password { get; set; }

    /// <summary>
    /// Enable ssl
    /// </summary>
    public bool EnableSsl { get; set; } = true;

    /// <summary>
    /// Использовать <see href="https://ru.wikipedia.org/wiki/Maildir">Maildir</see> вместо отправки сообщений по сети
    /// </summary>
    /// <remarks>
    /// Для работы с такой структурой можно использовать mutt (<c>mutt -f /&lt;path_to_maildir_root&gt;/</c>)
    /// либо <see href="https://www.thunderbird.net/ru/">Thunderbird</see>
    /// </remarks>
    public bool UseMaildir { get; set; } = true;

    /// <summary>
    /// Путь до корневой папки <see href="https://ru.wikipedia.org/wiki/Maildir">Maildir</see>
    /// </summary>
    [Required]
    public string PathToMaildir { get; set; } = "./maildir/";

    /// <summary>
    /// Есть установить то email'ы не будут проверяться на однодневки/подозрительные
    /// </summary>
    public EmailTrustLevel MailTrustLevel { get; set; } = EmailTrustLevel.Allow;

    /// <summary>
    /// Templates directory
    /// </summary>
    [Required]
    public string? TemplatesDir { get; set; } = "./files/mailing/templates";
}
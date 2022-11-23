using SdHub.Services.Mailing;

namespace SdHub.Options;

public class MailingOptions
{
    public string? From { get; set; }
    public string? Host { get; set; }
    public int Port { get; set; } = 587;
    public string? Username { get; set; }
    public string? Password { get; set; }
    public bool EnableSsl { get; set; } = true;

    /// <summary>
    /// Использовать <see href="https://ru.wikipedia.org/wiki/Maildir">Maildir</see> вместо отправки сообщений по сети
    /// </summary>
    /// <remarks>
    /// Для работы с такой структурой можно использовать mutt (<c>mutt -f /&lt;path_to_maildir_root&gt;/</c>)
    /// либо <see href="https://www.thunderbird.net/ru/">Thunderbird</see>
    /// </remarks>
    public bool UseMaildir { get; set; } = false;

    /// <summary>
    /// Путь до корневой папки <see href="https://ru.wikipedia.org/wiki/Maildir">Maildir</see>
    /// </summary>
    public string PathToMaildir { get; set; } = "./maildir/";

    /// <summary>
    /// Есть установить то email'ы не будут проверяться на однодневки/подозрительные
    /// </summary>
    public EmailTrustLevel MailTrustLevel { get; set; } = EmailTrustLevel.Allow;

    public string? TemplatesDir { get; set; } = "./files/mailing/templates";
}
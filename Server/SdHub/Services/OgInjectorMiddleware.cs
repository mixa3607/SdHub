using System;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SdHub.Database;
using SdHub.Options;
using Microsoft.AspNetCore.SpaServices.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Flurl;
using System.Text;

namespace SdHub.Services;

public class OgInjectorMiddleware
{
    private readonly ILogger<OgInjectorMiddleware> _logger;
    private readonly ISpaStaticFileProvider? _spaStaticFileProvider;
    private readonly RequestDelegate _next;
    private readonly AppInfoOptions _options;

    private readonly string? _indexHtml;

    public OgInjectorMiddleware(RequestDelegate next, ILogger<OgInjectorMiddleware> logger,
        IOptions<AppInfoOptions> options, ISpaStaticFileProvider? spaStaticFileProvider)
    {
        _next = next;
        _logger = logger;
        _spaStaticFileProvider = spaStaticFileProvider;
        _options = options.Value;
        if (_spaStaticFileProvider?.FileProvider != null)
        {
            try
            {
                var fileInfo = _spaStaticFileProvider.FileProvider.GetFileInfo("./index.html");
                using var fileStream = fileInfo.CreateReadStream();
                using var streamReader = new StreamReader(fileStream);
                _indexHtml = streamReader.ReadToEnd();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Cant load index.html");
            }
        }
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string? imageShortCode = null;
        if (context.Request.Path.Value?.StartsWith("/i/") == true && context.Request.Path.Value.Length == 8 + 3)
            imageShortCode = context.Request.Path.Value!["/i/".Length..];

        if (!string.IsNullOrWhiteSpace(_options.FrontDevServer)
            || context.Request.Method != "GET"
            || imageShortCode == null
            || _spaStaticFileProvider?.FileProvider == null
            || _indexHtml == null)
        {
            await _next(context);
            return;
        }

        try
        {
            var sp = context.RequestServices;
            var db = sp.GetRequiredService<SdHubDbContext>();
            var image = await db.Images
                .Include(x => x.OriginalImage).ThenInclude(x => x!.Storage)
                .Include(x => x.ThumbImage).ThenInclude(x => x!.Storage)
                .Include(x => x.ParsedMetadata).ThenInclude(x => x!.Tags)
                .FirstOrDefaultAsync(x => x.DeletedAt == null && x.ShortToken == imageShortCode);
            var content = _indexHtml;
            if (image != null)
            {
                var sb = new StringBuilder();
                foreach (var metadataTag in image.ParsedMetadata!.Tags!)
                {
                    sb.Append($"{metadataTag.Software} => {metadataTag.Name}: \n{metadataTag.Value}\n");
                }

                var twSite = "@SdHub";

                var title = "SdHub: ";
                if (!string.IsNullOrWhiteSpace(image.Name))
                    title += image.Name;
                else if (!string.IsNullOrWhiteSpace(image.OriginalImage!.Name))
                    title += image.OriginalImage!.Name;
                else
                    title += "Image name not set 😖";

                var description = sb.ToString();
                var url = _options.BaseUrl;
                var imageFile = image.ThumbImage ?? image.OriginalImage;
                var imageUrl = imageFile!.Storage!.BaseUrl.AppendPathSegment(imageFile.PathOnStorage).ToString();

                var injection = @$"
  <meta name=""twitter:card"" content=""summary_large_image"" />
  <meta name=""twitter:site"" content=""{twSite}"" />
  <meta name=""twitter:title"" content=""{title}"" />
  <meta name=""twitter:description"" content=""{description}"" />
  <meta name=""twitter:image"" content=""{imageUrl}"" />
  <meta property=""og:type"" content=""article"" />
  <meta property=""og:title"" content=""{title}"" />
  <meta property=""og:description"" content=""{description}"" />
  <meta property=""og:url"" content=""{url}"" />
  <meta property=""og:image"" content=""{imageUrl}"" />
";
                content = InsertAfter(_indexHtml, "<head>", injection);
            }

            context.Response.ContentType = "text/html";
            context.Response.StatusCode = 200;
            await context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(content));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Cant inject {code} to html", imageShortCode);
            await _next(context);
        }
    }

    static string InsertAfter(string text, string search, string insert)
    {
        var pos = text.IndexOf(search, StringComparison.InvariantCultureIgnoreCase);
        if (pos < 0)
        {
            return text;
        }

        return text.Substring(0, pos + search.Length) + insert + text.Substring(pos + search.Length);
    }
}
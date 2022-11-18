using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace SdHub.Models.Upload;


public class UploadRequest
{
    public List<IFormFile> Files { get; set; } = new List<IFormFile>();
}
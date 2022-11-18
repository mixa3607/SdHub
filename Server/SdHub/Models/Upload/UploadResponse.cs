using System;
using System.Collections.Generic;

namespace SdHub.Models.Upload;

public class UploadResponse
{

    public IReadOnlyList<UploadedFileModel> Files { get; set; } = Array.Empty<UploadedFileModel>();
}
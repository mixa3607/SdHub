using System.Collections.Generic;

namespace SdHub.Models;

public class PaginationResponse<T>
{
    public int Skip { get; set; }
    public int Take { get; set; }
    public int Total { get; set; }
    public required IReadOnlyList<T> Values { get; set; }
}
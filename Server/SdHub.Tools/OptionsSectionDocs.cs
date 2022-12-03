public class OptionsSectionDocs
{
    public string? Name { get; set; }
    public string? Summary { get; set; }
    public IReadOnlyList<PropDoc> Props { get; set; } = Array.Empty<PropDoc>();

    public class PropDoc
    {
        public int NestingLevel { get; set; }
        public string? Name { get; set; }
        public string? Summary { get; set; }
        public string DefaultValue { get; set; } = "";
        public string? Example { get; set; }
        public bool Required { get; set; }
    }
}
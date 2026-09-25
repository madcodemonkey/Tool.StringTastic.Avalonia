namespace Avalonia.StringTastic.Models;

public enum ToolType
{
    Compare,
    Base64Encoder,
    ColorPicker,
    GenerateGuid,
    JwtDecoder,
    JsonFormatter,
    Sorter,
    UrlEncoder
}

public class ToolItem
{
    public string DisplayName { get; set; } = string.Empty;
    public ToolType Type { get; set; }
    public string IconKey { get; set; } = string.Empty;
}
namespace AspireYouTubeSummariser.Shared.Configurations;

public class PromptSettings
{
    public const string Name = "Prompt";

    public virtual string? System { get; set; }
    public virtual int? MaxTokens { get; set; }
    public virtual float? Temperature { get; set; }
}

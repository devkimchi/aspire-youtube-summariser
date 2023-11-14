namespace AspireYouTubeSummariser.Shared.Configurations;

public class OpenAISettings
{
    public const string Name = "OpenAI";

    public virtual string? Endpoint { get; set; }
    public virtual string? ApiKey { get; set; }
    public virtual string? DeploymentId { get; set; }
}

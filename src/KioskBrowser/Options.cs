using CommandLine;

namespace KioskBrowser;

public class Options
{
    [Value(0, MetaName = "url", Required = false, HelpText = "URL to open in the browser")]
    public string? Url { get; set; }
    
    [Option('t', "enable-titlebar", Required = false, Default = false, HelpText = "Enable Title bar")]
    public bool EnableTitlebar { get; set; }
        
    [Option('r', "enable-content-refresh", Required = false, Default = false, HelpText = "(default: 60 seconds) Enable automatic refresh of content")]
    public bool EnableAutomaticContentRefresh { get; set; }
        
    [Option("content-refresh-interval", Required = false, Default = 60, HelpText = "(min: 10, max: 3600) Content refresh interval in seconds")]
    public int ContentRefreshIntervalInSeconds { get; set; }
}
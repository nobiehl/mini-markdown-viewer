namespace MarkdownViewer.Models
{
    /// <summary>
    /// Application settings stored in JSON format.
    /// Location: %APPDATA%/MarkdownViewer/settings.json
    /// </summary>
    public class AppSettings
    {
        public string Version { get; set; } = "1.5.1";
        public string Language { get; set; } = "system";
        public string Theme { get; set; } = "standard";

        public UiSettings UI { get; set; } = new();
        public UpdateSettings Updates { get; set; } = new();
        public ExplorerSettings Explorer { get; set; } = new();
        public ShortcutSettings Shortcuts { get; set; } = new();
        public NavigationSettings Navigation { get; set; } = new();
    }

    /// <summary>
    /// User interface visibility and behavior settings
    /// </summary>
    public class UiSettings
    {
        public StatusBarSettings StatusBar { get; set; } = new();
        public NavigationBarSettings NavigationBar { get; set; } = new();
        public SearchSettings Search { get; set; } = new();
    }

    /// <summary>
    /// StatusBar visibility and icon settings
    /// Default: All hidden/disabled
    /// </summary>
    public class StatusBarSettings
    {
        public bool Visible { get; set; } = false;
        public bool ShowUpdateStatus { get; set; } = true;
        public bool ShowExplorerStatus { get; set; } = true;
        public bool ShowLanguage { get; set; } = true;
        public bool ShowInfo { get; set; } = true;
        public bool ShowHelp { get; set; } = true;
    }

    /// <summary>
    /// Navigation bar visibility and behavior
    /// Default: Hidden
    /// </summary>
    public class NavigationBarSettings
    {
        public bool Visible { get; set; } = false;
        public bool EnableMouseGestures { get; set; } = false;
    }

    /// <summary>
    /// Search panel settings
    /// Default: Hidden until Ctrl+F pressed
    /// </summary>
    public class SearchSettings
    {
        public bool CaseSensitive { get; set; } = false;
        public bool WholeWords { get; set; } = false;
    }

    /// <summary>
    /// Update check settings
    /// </summary>
    public class UpdateSettings
    {
        public bool CheckOnStartup { get; set; } = true;
        public int CheckIntervalDays { get; set; } = 7;
        public bool AutoDownload { get; set; } = false;
        public bool IncludePrereleases { get; set; } = false;
    }

    /// <summary>
    /// Windows Explorer integration settings
    /// </summary>
    public class ExplorerSettings
    {
        public bool RegisterFileAssociation { get; set; } = false;
        public bool RegisterContextMenu { get; set; } = false;
    }

    /// <summary>
    /// Keyboard shortcuts configuration
    /// Note: Currently hard-coded, this is for future extensibility
    /// </summary>
    public class ShortcutSettings
    {
        public string ToggleStatusBar { get; set; } = "Ctrl+B";
        public string ToggleNavigationBar { get; set; } = "Ctrl+N";
        public string Search { get; set; } = "Ctrl+F";
        public string Refresh { get; set; } = "F5";
        public string NavigateBack { get; set; } = "Alt+Left";
        public string NavigateForward { get; set; } = "Alt+Right";
        public string Help { get; set; } = "F1";
    }

    /// <summary>
    /// Navigation history settings
    /// </summary>
    public class NavigationSettings
    {
        public int MaxHistorySize { get; set; } = 50;
        public bool PreserveHistoryOnRestart { get; set; } = false;
    }
}

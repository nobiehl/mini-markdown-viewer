using System;

namespace MarkdownViewer.Configuration
{
    /// <summary>
    /// Configuration for the update checker.
    /// Supports both production mode (GitHub API) and test mode (mock server).
    /// Uses singleton pattern for global access.
    /// </summary>
    public class UpdateConfiguration
    {
        private static UpdateConfiguration _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static UpdateConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new UpdateConfiguration();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Whether test mode is enabled (uses mock server instead of GitHub API)
        /// </summary>
        public bool IsTestMode { get; private set; }

        /// <summary>
        /// Base URL for GitHub API
        /// Production: "https://api.github.com"
        /// Test: "http://localhost:8080" (or custom)
        /// </summary>
        public string ApiBaseUrl { get; private set; }

        /// <summary>
        /// GitHub repository owner
        /// </summary>
        public string RepoOwner { get; private set; }

        /// <summary>
        /// GitHub repository name
        /// </summary>
        public string RepoName { get; private set; }

        /// <summary>
        /// Path to test data directory (only used in test mode)
        /// </summary>
        public string TestDataPath { get; private set; }

        /// <summary>
        /// Test scenario name (only used in test mode)
        /// </summary>
        public string TestScenario { get; private set; }

        /// <summary>
        /// Private constructor - loads configuration from environment variables
        /// </summary>
        private UpdateConfiguration()
        {
            LoadFromEnvironment();
        }

        /// <summary>
        /// Loads configuration from environment variables.
        /// Environment variables:
        /// - MDVIEWER_TEST_MODE=1 : Enables test mode
        /// - MDVIEWER_API_URL : Custom API base URL (default: localhost:8080 for test, api.github.com for prod)
        /// - MDVIEWER_TEST_DATA : Path to test data directory
        /// - MDVIEWER_TEST_SCENARIO : Test scenario name (e.g., "update-available")
        /// </summary>
        private void LoadFromEnvironment()
        {
            IsTestMode = Environment.GetEnvironmentVariable("MDVIEWER_TEST_MODE") == "1";

            if (IsTestMode)
            {
                // Test mode configuration
                ApiBaseUrl = Environment.GetEnvironmentVariable("MDVIEWER_API_URL")
                    ?? "http://localhost:8080";

                TestDataPath = Environment.GetEnvironmentVariable("MDVIEWER_TEST_DATA")
                    ?? "./test-data";

                TestScenario = Environment.GetEnvironmentVariable("MDVIEWER_TEST_SCENARIO")
                    ?? "update-available";

                // In test mode, repo owner/name can be overridden
                RepoOwner = Environment.GetEnvironmentVariable("MDVIEWER_REPO_OWNER")
                    ?? "test-owner";

                RepoName = Environment.GetEnvironmentVariable("MDVIEWER_REPO_NAME")
                    ?? "test-repo";
            }
            else
            {
                // Production mode configuration
                ApiBaseUrl = "https://api.github.com";
                RepoOwner = "nobiehl";
                RepoName = "mini-markdown-viewer";
                TestDataPath = null;
                TestScenario = null;
            }
        }

        /// <summary>
        /// Manually enables test mode (useful for unit tests).
        /// </summary>
        /// <param name="mockServerUrl">Mock server URL</param>
        /// <param name="testDataPath">Path to test data</param>
        /// <param name="scenario">Test scenario name</param>
        public void EnableTestMode(string mockServerUrl, string testDataPath, string scenario = "update-available")
        {
            IsTestMode = true;
            ApiBaseUrl = mockServerUrl;
            TestDataPath = testDataPath;
            TestScenario = scenario;
            RepoOwner = "test-owner";
            RepoName = "test-repo";
        }

        /// <summary>
        /// Resets configuration to production mode
        /// </summary>
        public void ResetToProductionMode()
        {
            IsTestMode = false;
            ApiBaseUrl = "https://api.github.com";
            RepoOwner = "nobiehl";
            RepoName = "mini-markdown-viewer";
            TestDataPath = null;
            TestScenario = null;
        }

        /// <summary>
        /// Gets the full GitHub API URL for latest release
        /// </summary>
        public string GetLatestReleaseUrl()
        {
            if (IsTestMode)
            {
                // In test mode, include scenario as query parameter
                return $"{ApiBaseUrl}/repos/{RepoOwner}/{RepoName}/releases/latest?scenario={TestScenario}";
            }
            else
            {
                // Production GitHub API URL
                return $"{ApiBaseUrl}/repos/{RepoOwner}/{RepoName}/releases/latest";
            }
        }

        /// <summary>
        /// Returns a summary of current configuration (for logging/debugging)
        /// </summary>
        public override string ToString()
        {
            return $"UpdateConfiguration(" +
                   $"Mode={{{(IsTestMode ? "Test" : "Production")}}}, " +
                   $"ApiUrl={{{ApiBaseUrl}}}, " +
                   $"Repo={{{RepoOwner}/{RepoName}}}" +
                   (IsTestMode ? $", Scenario={{{TestScenario}}}" : "") +
                   ")";
        }
    }
}

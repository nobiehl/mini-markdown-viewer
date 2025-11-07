using System;
using System.IO;
using Xunit;
using MarkdownViewer.Core;

namespace MarkdownViewer.Tests
{
    public class LinkNavigationHelperTests
    {
        private readonly string _testDirectory;
        private readonly string _testFilePath;

        public LinkNavigationHelperTests()
        {
            // Setup test directory structure
            _testDirectory = Path.Combine(Path.GetTempPath(), "MarkdownViewerTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);

            _testFilePath = Path.Combine(_testDirectory, "test.md");
            File.WriteAllText(_testFilePath, "# Test");
        }

        #region ResolveRelativePath Tests

        [Fact]
        public void ResolveRelativePath_WithAbsolutePath_ReturnsNormalizedPath()
        {
            // Arrange
            string absolutePath = @"C:\Users\Test\Documents\file.md";
            string currentFile = @"C:\Users\Test\Downloads\current.md";

            // Act
            string result = LinkNavigationHelper.ResolveRelativePath(absolutePath, currentFile);

            // Assert
            Assert.Equal(@"C:\Users\Test\Documents\file.md", result);
        }

        [Fact]
        public void ResolveRelativePath_WithRelativePath_ResolvesRelativeToCurrentFileDirectory()
        {
            // Arrange
            string relativePath = "README.md";
            string currentFile = Path.Combine(_testDirectory, "subfolder", "current.md");

            // Create actual directory structure
            Directory.CreateDirectory(Path.Combine(_testDirectory, "subfolder"));

            // Act
            string result = LinkNavigationHelper.ResolveRelativePath(relativePath, currentFile);

            // Assert
            string expected = Path.Combine(_testDirectory, "subfolder", "README.md");
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ResolveRelativePath_WithRelativePathGoingUp_ResolvesCorrectly()
        {
            // Arrange
            string relativePath = "../README.md";
            string currentFile = Path.Combine(_testDirectory, "subfolder", "current.md");

            // Create actual directory structure
            Directory.CreateDirectory(Path.Combine(_testDirectory, "subfolder"));

            // Act
            string result = LinkNavigationHelper.ResolveRelativePath(relativePath, currentFile);

            // Assert
            string expected = Path.Combine(_testDirectory, "README.md");
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ResolveRelativePath_WithDeepRelativePath_ResolvesCorrectly()
        {
            // Arrange
            string relativePath = "docs/API/endpoints.md";
            string currentFile = Path.Combine(_testDirectory, "current.md");

            // Act
            string result = LinkNavigationHelper.ResolveRelativePath(relativePath, currentFile);

            // Assert
            string expected = Path.Combine(_testDirectory, "docs", "API", "endpoints.md");
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ResolveRelativePath_WithNullLinkPath_ThrowsArgumentException()
        {
            // Arrange
            string currentFile = @"C:\Users\Test\current.md";

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                LinkNavigationHelper.ResolveRelativePath(null!, currentFile));
        }

        [Fact]
        public void ResolveRelativePath_WithEmptyLinkPath_ThrowsArgumentException()
        {
            // Arrange
            string currentFile = @"C:\Users\Test\current.md";

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                LinkNavigationHelper.ResolveRelativePath("", currentFile));
        }

        [Fact]
        public void ResolveRelativePath_WithNullCurrentFile_ThrowsArgumentException()
        {
            // Arrange
            string linkPath = "README.md";

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                LinkNavigationHelper.ResolveRelativePath(linkPath, null!));
        }

        #endregion

        #region GetLinkType Tests

        [Fact]
        public void GetLinkType_WithHttpUrl_ReturnsExternalHttp()
        {
            // Arrange
            string link = "http://www.example.com";

            // Act
            var result = LinkNavigationHelper.GetLinkType(link);

            // Assert
            Assert.Equal(LinkNavigationHelper.LinkType.ExternalHttp, result);
        }

        [Fact]
        public void GetLinkType_WithHttpsUrl_ReturnsExternalHttp()
        {
            // Arrange
            string link = "https://www.example.com";

            // Act
            var result = LinkNavigationHelper.GetLinkType(link);

            // Assert
            Assert.Equal(LinkNavigationHelper.LinkType.ExternalHttp, result);
        }

        [Fact]
        public void GetLinkType_WithAnchorLink_ReturnsAnchor()
        {
            // Arrange
            string link = "#section-title";

            // Act
            var result = LinkNavigationHelper.GetLinkType(link);

            // Assert
            Assert.Equal(LinkNavigationHelper.LinkType.Anchor, result);
        }

        [Fact]
        public void GetLinkType_WithMarkdownFile_ReturnsLocalMarkdown()
        {
            // Arrange
            string link = "README.md";

            // Act
            var result = LinkNavigationHelper.GetLinkType(link);

            // Assert
            Assert.Equal(LinkNavigationHelper.LinkType.LocalMarkdown, result);
        }

        [Fact]
        public void GetLinkType_WithMarkdownExtension_ReturnsLocalMarkdown()
        {
            // Arrange
            string link = "docs/API.markdown";

            // Act
            var result = LinkNavigationHelper.GetLinkType(link);

            // Assert
            Assert.Equal(LinkNavigationHelper.LinkType.LocalMarkdown, result);
        }

        [Fact]
        public void GetLinkType_WithUnknownLink_ReturnsUnknown()
        {
            // Arrange
            string link = "document.pdf";

            // Act
            var result = LinkNavigationHelper.GetLinkType(link);

            // Assert
            Assert.Equal(LinkNavigationHelper.LinkType.Unknown, result);
        }

        [Fact]
        public void GetLinkType_WithNullLink_ReturnsUnknown()
        {
            // Act
            var result = LinkNavigationHelper.GetLinkType(null!);

            // Assert
            Assert.Equal(LinkNavigationHelper.LinkType.Unknown, result);
        }

        [Fact]
        public void GetLinkType_WithEmptyLink_ReturnsUnknown()
        {
            // Act
            var result = LinkNavigationHelper.GetLinkType("");

            // Assert
            Assert.Equal(LinkNavigationHelper.LinkType.Unknown, result);
        }

        #endregion

        #region ValidateFileExists Tests

        [Fact]
        public void ValidateFileExists_WithExistingFile_ReturnsTrue()
        {
            // Arrange
            string existingFile = _testFilePath;

            // Act
            bool result = LinkNavigationHelper.ValidateFileExists(existingFile, "test.md");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateFileExists_WithNonExistentFile_ReturnsFalse()
        {
            // Arrange
            string nonExistentFile = Path.Combine(_testDirectory, "nonexistent.md");

            // Act
            bool result = LinkNavigationHelper.ValidateFileExists(nonExistentFile, "nonexistent.md");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateFileExists_WithNullPath_ReturnsFalse()
        {
            // Act
            bool result = LinkNavigationHelper.ValidateFileExists(null!, "test.md");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateFileExists_WithEmptyPath_ReturnsFalse()
        {
            // Act
            bool result = LinkNavigationHelper.ValidateFileExists("", "test.md");

            // Assert
            Assert.False(result);
        }

        #endregion

        #region IsInlineResource Tests

        [Fact]
        public void IsInlineResource_WithPlantUmlUrl_ReturnsTrue()
        {
            // Arrange
            string url = "https://www.plantuml.com/plantuml/png/SyfFKj2rKt3CoKnELR1Io4ZDoSa70000";

            // Act
            bool result = LinkNavigationHelper.IsInlineResource(url);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsInlineResource_WithCdnUrl_ReturnsTrue()
        {
            // Arrange
            string url = "https://cdn.jsdelivr.net/npm/mermaid/dist/mermaid.min.js";

            // Act
            bool result = LinkNavigationHelper.IsInlineResource(url);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsInlineResource_WithImageUrl_ReturnsTrue()
        {
            // Arrange
            string url = "https://example.com/image.png";

            // Act
            bool result = LinkNavigationHelper.IsInlineResource(url);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsInlineResource_WithJpgUrl_ReturnsTrue()
        {
            // Arrange
            string url = "https://example.com/photo.jpg";

            // Act
            bool result = LinkNavigationHelper.IsInlineResource(url);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsInlineResource_WithSvgUrl_ReturnsTrue()
        {
            // Arrange
            string url = "https://example.com/icon.svg";

            // Act
            bool result = LinkNavigationHelper.IsInlineResource(url);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsInlineResource_WithRegularWebPage_ReturnsFalse()
        {
            // Arrange
            string url = "https://www.google.com";

            // Act
            bool result = LinkNavigationHelper.IsInlineResource(url);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsInlineResource_WithNullUrl_ReturnsFalse()
        {
            // Act
            bool result = LinkNavigationHelper.IsInlineResource(null!);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsInlineResource_WithEmptyUrl_ReturnsFalse()
        {
            // Act
            bool result = LinkNavigationHelper.IsInlineResource("");

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void Integration_ResolveAndValidateLocalMarkdownLink_Success()
        {
            // Arrange
            string currentFile = Path.Combine(_testDirectory, "subfolder", "current.md");
            Directory.CreateDirectory(Path.GetDirectoryName(currentFile)!);

            string targetFile = Path.Combine(_testDirectory, "README.md");
            File.WriteAllText(targetFile, "# README");

            string relativePath = "../README.md";

            // Act
            var linkType = LinkNavigationHelper.GetLinkType(relativePath);
            string resolvedPath = LinkNavigationHelper.ResolveRelativePath(relativePath, currentFile);
            bool exists = LinkNavigationHelper.ValidateFileExists(resolvedPath, relativePath);

            // Assert
            Assert.Equal(LinkNavigationHelper.LinkType.LocalMarkdown, linkType);
            Assert.Equal(targetFile, resolvedPath);
            Assert.True(exists);
        }

        [Fact]
        public void Integration_ResolveAndValidateMissingFile_Fails()
        {
            // Arrange
            string currentFile = Path.Combine(_testDirectory, "current.md");
            string relativePath = "missing.md";

            // Act
            var linkType = LinkNavigationHelper.GetLinkType(relativePath);
            string resolvedPath = LinkNavigationHelper.ResolveRelativePath(relativePath, currentFile);
            bool exists = LinkNavigationHelper.ValidateFileExists(resolvedPath, relativePath);

            // Assert
            Assert.Equal(LinkNavigationHelper.LinkType.LocalMarkdown, linkType);
            Assert.False(exists);
        }

        #endregion
    }
}

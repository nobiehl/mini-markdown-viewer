using System;
using FluentAssertions;
using Microsoft.Web.WebView2.WinForms;
using Xunit;
using MarkdownViewer.Core;

namespace MarkdownViewer.Tests
{
    /// <summary>
    /// Unit tests for NavigationManager functionality.
    /// Tests navigation history (Back/Forward) management for WebView2.
    /// Note: WebView2 cannot be mocked easily, so we test basic initialization and properties.
    /// </summary>
    public class NavigationManagerTests : IDisposable
    {
        private readonly WebView2 _webView;
        private readonly NavigationManager _navigationManager;

        public NavigationManagerTests()
        {
            // Create real WebView2 instance (not initialized)
            _webView = new WebView2();
            _navigationManager = new NavigationManager(_webView);
        }

        public void Dispose()
        {
            _webView?.Dispose();
        }

        [Fact]
        public void Constructor_WithNullWebView_ThrowsArgumentNullException()
        {
            // Act & Assert
            Action act = () => new NavigationManager(null!);
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("webView");
        }

        [Fact]
        public void Constructor_WithValidWebView_DoesNotThrow()
        {
            // Act & Assert
            Action act = () => new NavigationManager(_webView);
            act.Should().NotThrow();
        }

        [Fact]
        public void CanGoBack_BeforeInitialization_ReturnsFalse()
        {
            // Act
            var canGoBack = _navigationManager.CanGoBack;

            // Assert
            canGoBack.Should().BeFalse();
        }

        [Fact]
        public void CanGoForward_BeforeInitialization_ReturnsFalse()
        {
            // Act
            var canGoForward = _navigationManager.CanGoForward;

            // Assert
            canGoForward.Should().BeFalse();
        }

        [Fact]
        public void GoBack_BeforeInitialization_DoesNotThrow()
        {
            // Act & Assert
            Action act = () => _navigationManager.GoBack();
            act.Should().NotThrow();
        }

        [Fact]
        public void GoForward_BeforeInitialization_DoesNotThrow()
        {
            // Act & Assert
            Action act = () => _navigationManager.GoForward();
            act.Should().NotThrow();
        }

        [Fact]
        public void ClearHistory_DoesNotThrow()
        {
            // Act & Assert
            Action act = () => _navigationManager.ClearHistory();
            act.Should().NotThrow();
        }

        [Fact]
        public void NavigationChanged_EventCanBeSubscribed()
        {
            // Arrange
            EventHandler handler = (sender, args) => { };

            // Act & Assert
            Action act = () => _navigationManager.NavigationChanged += handler;
            act.Should().NotThrow();
        }

        [Fact]
        public void NavigationChanged_EventCanBeUnsubscribed()
        {
            // Arrange
            EventHandler handler = (sender, args) => { };
            _navigationManager.NavigationChanged += handler;

            // Act & Assert
            Action act = () => _navigationManager.NavigationChanged -= handler;
            act.Should().NotThrow();
        }

        [Fact]
        public void CanGoBack_Property_IsAccessible()
        {
            // Act
            var result = _navigationManager.CanGoBack;

            // Assert
            result.Should().BeFalse(); // Not initialized, so false
        }

        [Fact]
        public void CanGoForward_Property_IsAccessible()
        {
            // Act
            var result = _navigationManager.CanGoForward;

            // Assert
            result.Should().BeFalse(); // Not initialized, so false
        }
    }
}

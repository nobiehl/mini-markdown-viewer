using System;
using System.Globalization;
using FluentAssertions;
using Moq;
using MarkdownViewer.Tests.Mocks;
using Xunit;

namespace MarkdownViewer.Tests.Services
{
    public class LocalizationServiceTests
    {
        [Fact]
        public void Translate_WhenCalled_ReturnsTranslatedString()
        {
            // Arrange
            var mockCulture = new CultureInfo("en-US");
            var localizationService = new MockLocalizationService(mockCulture);

            // Act
            var translatedText = localizationService.Translate("TestKey");

            // Assert
            translatedText.Should().Be("Translated_TestKey");
            localizationService.CurrentCulture.Should().Be(mockCulture);
        }

        [Fact]
        public void ChangeCulture_UpdatesCultureCorrectly()
        {
            // Arrange
            var initialCulture = new CultureInfo("en-US");
            var newCulture = new CultureInfo("de-DE");
            var localizationService = new MockLocalizationService(initialCulture);

            // Act
            localizationService.ChangeCulture(newCulture);

            // Assert
            localizationService.CurrentCulture.Should().Be(newCulture);
        }
    }
}
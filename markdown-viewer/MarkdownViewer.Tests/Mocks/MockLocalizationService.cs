using System;
using System.Globalization;

namespace MarkdownViewer.Tests.Mocks
{
    public class MockLocalizationService : ILocalizationService
    {
        public CultureInfo CurrentCulture { get; private set; }

        public MockLocalizationService(CultureInfo culture)
        {
            CurrentCulture = culture;
        }

        public string Translate(string key)
        {
            // Simple mock translation logic
            return $"Translated_{key}";
        }

        public void ChangeCulture(CultureInfo culture)
        {
            CurrentCulture = culture;
        }
    }

    public interface ILocalizationService
    {
        CultureInfo CurrentCulture { get; }
        string Translate(string key);
        void ChangeCulture(CultureInfo culture);
    }
}
# MarkdownViewer Test Project

## Overview
This project contains unit tests for the MarkdownViewer application using xUnit, Moq, and FluentAssertions.

## Prerequisites
- .NET 8 SDK
- Visual Studio 2022 (recommended) or VS Code

## Running Tests

### Via Command Line
```bash
dotnet test
```

### Via Visual Studio
1. Open Test Explorer
2. Click "Run All Tests"

## Test Conventions

### Naming
- Test classes should end with `Tests`
- Test methods should be named `MethodName_Scenario_ExpectedBehavior`
  - Example: `Translate_WhenCalledWithKey_ReturnsTranslatedString`

### Mocking
- Use Moq for creating mock objects
- Prefer constructor injection for dependencies
- Create mock interfaces in the `Mocks/` directory

### Assertions
- Use FluentAssertions for readable, expressive assertions
- Avoid using raw Assert methods

## Test Structure
- `Tests/Services/`: Service layer tests
- `Tests/Core/`: Core component tests
- `Tests/Presenters/`: Presenter/UI logic tests
- `Mocks/`: Mock implementations of interfaces

## Example Test
```csharp
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
}
```

## Troubleshooting
- Ensure all NuGet packages are restored
- Check .NET SDK version compatibility
- Run tests in a clean environment
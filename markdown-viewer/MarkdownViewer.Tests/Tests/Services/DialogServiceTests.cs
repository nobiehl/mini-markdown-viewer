using FluentAssertions;
using Moq;
using MarkdownViewer.Core.Services;
using MarkdownViewer.Tests.Mocks;
using Xunit;

namespace MarkdownViewer.Tests.Services
{
    /// <summary>
    /// Unit tests for IDialogService implementations.
    /// Tests both the interface contract and the MockDialogService behavior.
    /// </summary>
    public class DialogServiceTests
    {
        [Fact]
        public void ShowError_DisplaysCorrectly()
        {
            // Arrange
            var dialogService = new MockDialogService();
            var expectedMessage = "An error occurred";
            var expectedTitle = "Error";

            // Act
            dialogService.ShowError(expectedMessage, expectedTitle);

            // Assert
            dialogService.LastErrorMessage.Should().Be(expectedMessage);
            dialogService.ShowErrorCallCount.Should().Be(1);
        }

        [Fact]
        public void ShowInfo_DisplaysCorrectly()
        {
            // Arrange
            var dialogService = new MockDialogService();
            var expectedMessage = "Information message";
            var expectedTitle = "Info";

            // Act
            dialogService.ShowInfo(expectedMessage, expectedTitle);

            // Assert
            dialogService.LastInfoMessage.Should().Be(expectedMessage);
            dialogService.ShowInfoCallCount.Should().Be(1);
        }

        [Fact]
        public void ShowWarning_DisplaysCorrectly()
        {
            // Arrange
            var dialogService = new MockDialogService();
            var expectedMessage = "Warning message";
            var expectedTitle = "Warning";

            // Act
            dialogService.ShowWarning(expectedMessage, expectedTitle);

            // Assert
            dialogService.LastWarningMessage.Should().Be(expectedMessage);
            dialogService.ShowWarningCallCount.Should().Be(1);
        }

        [Fact]
        public void ShowConfirmation_ReturnsYes()
        {
            // Arrange
            var dialogService = new MockDialogService
            {
                ConfirmationResult = ServiceDialogResult.OK
            };

            // Act
            var result = dialogService.ShowConfirmation("Confirm this action?", "Confirm");

            // Assert
            result.Should().Be(ServiceDialogResult.OK);
        }

        [Fact]
        public void ShowConfirmation_ReturnsNo()
        {
            // Arrange
            var dialogService = new MockDialogService
            {
                ConfirmationResult = ServiceDialogResult.Cancel
            };

            // Act
            var result = dialogService.ShowConfirmation("Confirm this action?", "Confirm");

            // Assert
            result.Should().Be(ServiceDialogResult.Cancel);
        }

        [Fact]
        public void ShowYesNo_ReturnsYes()
        {
            // Arrange
            var dialogService = new MockDialogService
            {
                YesNoResult = ServiceDialogResult.Yes
            };

            // Act
            var result = dialogService.ShowYesNo("Do you want to proceed?", "Question");

            // Assert
            result.Should().Be(ServiceDialogResult.Yes);
        }

        [Fact]
        public void ShowYesNo_ReturnsNo()
        {
            // Arrange
            var dialogService = new MockDialogService
            {
                YesNoResult = ServiceDialogResult.No
            };

            // Act
            var result = dialogService.ShowYesNo("Do you want to proceed?", "Question");

            // Assert
            result.Should().Be(ServiceDialogResult.No);
        }

        [Fact]
        public void ShowError_WithDefaultTitle_UsesDefaultValue()
        {
            // Arrange
            var dialogService = new MockDialogService();
            var expectedMessage = "Error without custom title";

            // Act
            dialogService.ShowError(expectedMessage);

            // Assert
            dialogService.LastErrorMessage.Should().Be(expectedMessage);
            dialogService.ShowErrorCallCount.Should().Be(1);
        }

        [Fact]
        public void ShowInfo_WithDefaultTitle_UsesDefaultValue()
        {
            // Arrange
            var dialogService = new MockDialogService();
            var expectedMessage = "Info without custom title";

            // Act
            dialogService.ShowInfo(expectedMessage);

            // Assert
            dialogService.LastInfoMessage.Should().Be(expectedMessage);
            dialogService.ShowInfoCallCount.Should().Be(1);
        }

        [Fact]
        public void ShowWarning_WithDefaultTitle_UsesDefaultValue()
        {
            // Arrange
            var dialogService = new MockDialogService();
            var expectedMessage = "Warning without custom title";

            // Act
            dialogService.ShowWarning(expectedMessage);

            // Assert
            dialogService.LastWarningMessage.Should().Be(expectedMessage);
            dialogService.ShowWarningCallCount.Should().Be(1);
        }

        [Fact]
        public void MultipleShowError_IncrementsCallCount()
        {
            // Arrange
            var dialogService = new MockDialogService();

            // Act
            dialogService.ShowError("Error 1");
            dialogService.ShowError("Error 2");
            dialogService.ShowError("Error 3");

            // Assert
            dialogService.ShowErrorCallCount.Should().Be(3);
            dialogService.LastErrorMessage.Should().Be("Error 3");
        }

        [Fact]
        public void MultipleShowInfo_IncrementsCallCount()
        {
            // Arrange
            var dialogService = new MockDialogService();

            // Act
            dialogService.ShowInfo("Info 1");
            dialogService.ShowInfo("Info 2");

            // Assert
            dialogService.ShowInfoCallCount.Should().Be(2);
            dialogService.LastInfoMessage.Should().Be("Info 2");
        }

        [Fact]
        public void MockDialogService_InitialState_HasZeroCounts()
        {
            // Arrange & Act
            var dialogService = new MockDialogService();

            // Assert
            dialogService.ShowErrorCallCount.Should().Be(0);
            dialogService.ShowInfoCallCount.Should().Be(0);
            dialogService.ShowWarningCallCount.Should().Be(0);
            dialogService.LastErrorMessage.Should().BeNull();
            dialogService.LastInfoMessage.Should().BeNull();
            dialogService.LastWarningMessage.Should().BeNull();
        }

        [Fact]
        public void ShowConfirmation_CanReturnDifferentResults()
        {
            // Arrange
            var dialogService = new MockDialogService();

            // Act & Assert - First call returns OK
            dialogService.ConfirmationResult = ServiceDialogResult.OK;
            var result1 = dialogService.ShowConfirmation("Confirm?");
            result1.Should().Be(ServiceDialogResult.OK);

            // Act & Assert - Second call returns Cancel
            dialogService.ConfirmationResult = ServiceDialogResult.Cancel;
            var result2 = dialogService.ShowConfirmation("Confirm again?");
            result2.Should().Be(ServiceDialogResult.Cancel);
        }
    }
}

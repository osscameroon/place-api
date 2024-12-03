using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Place.Notification;
using Place.Notification.Email;
using Place.Notification.Email.SMTP;

namespace Core.Identity.UnitTests;

[Trait("Category", "Unit")]
[Trait("Category", "IdentitySendEmail")]
public class IdentityEmailSenderTests
{
    private readonly ILogger<IdentityEmailSender<TestUser>> _logger = Substitute.For<
        ILogger<IdentityEmailSender<TestUser>>
    >();
    private readonly IEmailService _emailService = Substitute.For<IEmailService>();
    private readonly TestUser _testUser = new() { Email = TestEmail };
    private const string TestEmail = "test@example.com";

    [Fact]
    public async Task SendConfirmationLinkAsync_Should_SendEmailWithCorrectContent()
    {
        // Arrange
        IdentityEmailSender<TestUser> sender = new(_logger, _emailService);
        const string confirmationLink = "https://example.com/confirm";
        EmailMessage? capturedEmail = null;

        _emailService
            .SendAsync(Arg.Do<EmailMessage>(email => capturedEmail = email))
            .Returns(Task.CompletedTask);

        // Act
        await sender.SendConfirmationLinkAsync(_testUser, TestEmail, confirmationLink);

        // Assert
        await _emailService.Received(1).SendAsync(Arg.Any<EmailMessage>());

        capturedEmail.Should().NotBeNull();
        capturedEmail?.To.Should().Be(TestEmail);
        capturedEmail?.Subject.Should().Be("Confirmez votre adresse email");
        capturedEmail?.Body.Should().Contain(confirmationLink);
    }

    [Fact]
    public async Task SendPasswordResetLinkAsync_Should_SendEmailWithCorrectContent()
    {
        // Arrange
        IdentityEmailSender<TestUser> sender = new(_logger, _emailService);
        const string resetLink = "https://example.com/reset";
        EmailMessage? capturedEmail = null;

        _emailService
            .SendAsync(Arg.Do<EmailMessage>(email => capturedEmail = email))
            .Returns(Task.CompletedTask);

        // Act
        await sender.SendPasswordResetLinkAsync(_testUser, TestEmail, resetLink);

        // Assert
        await _emailService.Received(1).SendAsync(Arg.Any<EmailMessage>());

        capturedEmail.Should().NotBeNull();
        capturedEmail?.To.Should().Be(TestEmail);
        capturedEmail?.Subject.Should().Be("Réinitialisation de votre mot de passe");
        capturedEmail?.Body.Should().Contain(resetLink);
    }

    [Fact]
    public async Task SendPasswordResetCodeAsync_Should_SendEmailWithCorrectContent()
    {
        // Arrange
        IdentityEmailSender<TestUser> sender = new(_logger, _emailService);
        const string resetCode = "123456";
        EmailMessage? capturedEmail = null;

        _emailService
            .SendAsync(Arg.Do<EmailMessage>(email => capturedEmail = email))
            .Returns(Task.CompletedTask);

        // Act
        await sender.SendPasswordResetCodeAsync(_testUser, TestEmail, resetCode);

        // Assert
        await _emailService.Received(1).SendAsync(Arg.Any<EmailMessage>());

        capturedEmail.Should().NotBeNull();
        capturedEmail?.To.Should().Be(TestEmail);
        capturedEmail?.Subject.Should().Be("Votre code de réinitialisation de mot de passe");
        capturedEmail?.Body.Should().Contain(resetCode);
    }

    [Theory]
    [InlineData(EmailProvider.Smtp)]
    [InlineData(EmailProvider.SendGrid)]
    public async Task SendEmail_Should_Work_WithDifferentProviders(EmailProvider provider)
    {
        // Arrange

        IdentityEmailSender<TestUser> sender = new(_logger, _emailService);

        // Act
        await sender.SendConfirmationLinkAsync(_testUser, TestEmail, "https://example.com");

        // Assert
        await _emailService
            .Received(1)
            .SendAsync(
                Arg.Is<EmailMessage>(email =>
                    email.To == TestEmail
                    && email.Subject == "Confirmez votre adresse email"
                    && email.Body.Contains("https://example.com")
                )
            );
    }
}

public class TestUser
{
    public string Email { get; set; } = null!;
}

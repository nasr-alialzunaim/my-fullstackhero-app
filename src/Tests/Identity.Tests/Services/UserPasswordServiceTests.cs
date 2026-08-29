using System.Linq.Expressions;
using FSH.Framework.Jobs.Services;
using FSH.Framework.Mailing;
using FSH.Framework.Mailing.Services;
using FSH.Modules.Identity.Domain;
using FSH.Modules.Identity.Services;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

namespace Identity.Tests.Services;

/// <summary>
/// Tests for UserPasswordService.ForgotPasswordAsync — focuses on the reset-link format
/// and verifies the single-installation flow does not carry tenant-selection data.
/// </summary>
public sealed class UserPasswordServiceTests
{
    private readonly UserManager<FshUser> _userManager;
    private readonly IJobService _jobService;
    private readonly IMailService _mailService;

    public UserPasswordServiceTests()
    {
        _userManager = Substitute.For<UserManager<FshUser>>(
            Substitute.For<IUserStore<FshUser>>(), null, null, null, null, null, null, null, null);
        _jobService = Substitute.For<IJobService>();
        _mailService = Substitute.For<IMailService>();

        // The mail job is enqueued as an expression; compile + invoke it so the captured MailRequest
        // reaches the (mocked) mail service exactly as production would build it.
        _jobService.Enqueue(Arg.Any<Expression<Func<Task>>>())
            .Returns(ci =>
            {
                ci.Arg<Expression<Func<Task>>>().Compile().Invoke();
                return "job-1";
            });
        _mailService.SendAsync(Arg.Any<MailRequest>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
    }

    private UserPasswordService CreateSut() =>
        new(_userManager, null!, _jobService, _mailService, null!, null!);

    private MailRequest CaptureSentMail()
    {
        var call = _mailService.ReceivedCalls().Single();
        return (MailRequest)call.GetArguments()[0]!;
    }

    [Fact]
    public async Task ForgotPasswordAsync_Should_BuildResetLink_WithSingleSlash_NoTenant_AndEncodedEmail()
    {
        // Arrange — trailing slash on the origin (as Uri.ToString() produces for a host-only URL) and an
        // email with reserved characters ('+', '@') exercise URL normalization and encoding.
        const string email = "marcelo+reset@codefi.com.br";
        var user = new FshUser { Email = email, UserName = email };
        _userManager.FindByEmailAsync(email).Returns(user);
        _userManager.GeneratePasswordResetTokenAsync(user).Returns("raw-token");

        var sut = CreateSut();

        // Act
        await sut.ForgotPasswordAsync(email, "https://appbase.codefi.com.br/", CancellationToken.None);

        // Assert
        var body = CaptureSentMail().Body!;
        body.ShouldContain("https://appbase.codefi.com.br/reset-password?");
        body.ShouldNotContain("//reset-password");
        body.ShouldNotContain("tenant=");
        body.ShouldContain("email=marcelo%2Breset");
        body.ShouldNotContain("email=marcelo+reset");
    }

    [Fact]
    public async Task ForgotPasswordAsync_Should_NotEnqueueMail_When_UserIsUnknown()
    {
        // Arrange — anti-enumeration: unknown user silently no-ops (no mail), still a 200 upstream.
        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns((FshUser?)null);
        var sut = CreateSut();

        // Act
        await sut.ForgotPasswordAsync("ghost@codefi.com.br", "https://appbase.codefi.com.br/", CancellationToken.None);

        // Assert
        _jobService.DidNotReceive().Enqueue(Arg.Any<Expression<Func<Task>>>());
    }
}

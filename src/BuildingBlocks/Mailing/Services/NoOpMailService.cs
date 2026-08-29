namespace FSH.Framework.Mailing.Services;

/// <summary>
/// Offline/local fallback used when outbound mailing is disabled.
/// Keeps mail-dependent workflows resolvable without performing network I/O.
/// </summary>
public sealed class NoOpMailService : IMailService
{
    public Task SendAsync(MailRequest request, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(request);
        return Task.CompletedTask;
    }
}

using FSH.Framework.Shared.Installation;
using FSH.Modules.Billing.Contracts;
using FSH.Modules.Billing.Contracts.v1.Usage.CaptureUsageSnapshots;
using FSH.Modules.Billing.Services;

namespace FSH.Modules.Billing.Features.v1.Usage.CaptureUsageSnapshots;

public sealed class CaptureUsageSnapshotsCommandHandler(IUsageReporter reporter)
    : ICommandHandler<CaptureUsageSnapshotsCommand, IReadOnlyList<UsageSnapshotDto>>
{
    public async ValueTask<IReadOnlyList<UsageSnapshotDto>> Handle(
        CaptureUsageSnapshotsCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        return await reporter
            .CaptureForPeriodAsync(
                InstallationConstants.Id,
                command.PeriodYear,
                command.PeriodMonth,
                cancellationToken)
            .ConfigureAwait(false);
    }
}

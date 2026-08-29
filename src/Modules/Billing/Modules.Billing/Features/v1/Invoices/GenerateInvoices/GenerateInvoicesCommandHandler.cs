using FSH.Modules.Billing.Contracts.v1.Invoices.GenerateInvoices;
using FSH.Modules.Billing.Services;

namespace FSH.Modules.Billing.Features.v1.Invoices.GenerateInvoices;

public sealed class GenerateInvoicesCommandHandler(IBillingService billing)
    : ICommandHandler<GenerateInvoicesCommand, int>
{
    public ValueTask<int> Handle(GenerateInvoicesCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        return new ValueTask<int>(billing.GenerateInvoicesForAllTenantsAsync(
            command.PeriodYear,
            command.PeriodMonth,
            cancellationToken));
    }
}

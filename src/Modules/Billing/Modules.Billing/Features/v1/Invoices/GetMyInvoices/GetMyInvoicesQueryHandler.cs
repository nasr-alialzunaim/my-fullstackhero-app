using FSH.Framework.Shared.Installation;
using FSH.Modules.Billing.Contracts;
using FSH.Modules.Billing.Contracts.v1.Invoices.GetMyInvoices;
using FSH.Modules.Billing.Data;
using Microsoft.EntityFrameworkCore;

namespace FSH.Modules.Billing.Features.v1.Invoices.GetMyInvoices;

public sealed class GetMyInvoicesQueryHandler(BillingDbContext db)
    : IQueryHandler<GetMyInvoicesQuery, IReadOnlyList<InvoiceDto>>
{
    public async ValueTask<IReadOnlyList<InvoiceDto>> Handle(
        GetMyInvoicesQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        return await db.Invoices
            .AsNoTracking()
            .Where(i => i.TenantId == InstallationConstants.Id)
            .OrderByDescending(i => i.CreatedAtUtc)
            .Select(i => new InvoiceDto(
                i.Id,
                i.TenantId,
                i.InvoiceNumber,
                i.PeriodYear,
                i.PeriodMonth,
                i.Purpose,
                i.Status,
                i.Currency,
                i.Subtotal,
                i.Tax,
                i.Total,
                i.IssuedAtUtc,
                i.PaidAtUtc,
                i.CreatedAtUtc))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}

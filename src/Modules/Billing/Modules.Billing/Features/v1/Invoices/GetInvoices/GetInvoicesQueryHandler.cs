using FSH.Framework.Shared.Installation;
using FSH.Modules.Billing.Contracts.Dtos;
using FSH.Modules.Billing.Contracts.v1.Invoices;
using FSH.Modules.Billing.Data;
using Microsoft.EntityFrameworkCore;

using Mediator;

namespace FSH.Modules.Billing.Features.v1.Invoices.GetInvoices;

public sealed class GetInvoicesQueryHandler(BillingDbContext db)
    : IQueryHandler<GetInvoicesQuery, IReadOnlyList<InvoiceDto>>
{
    public async ValueTask<IReadOnlyList<InvoiceDto>> Handle(
        GetInvoicesQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var q = db.Invoices
            .AsNoTracking()
            .Where(i => i.TenantId == InstallationConstants.Id);

        if (query.Status is not null)
        {
            q = q.Where(i => i.Status == query.Status);
        }

        return await q
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

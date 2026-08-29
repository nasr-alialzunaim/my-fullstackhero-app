using FSH.Framework.Core.Exceptions;
using FSH.Framework.Shared.Installation;
using FSH.Modules.Billing.Contracts.Dtos;
using FSH.Modules.Billing.Contracts.v1.Invoices;
using FSH.Modules.Billing.Data;
using Microsoft.EntityFrameworkCore;

using Mediator;

namespace FSH.Modules.Billing.Features.v1.Invoices.GetInvoiceById;

public sealed class GetInvoiceByIdQueryHandler(BillingDbContext db)
    : IQueryHandler<GetInvoiceByIdQuery, InvoiceDto>
{
    public async ValueTask<InvoiceDto> Handle(GetInvoiceByIdQuery query, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        var invoice = await db.Invoices
            .AsNoTracking()
            .FirstOrDefaultAsync(
                i => i.Id == query.InvoiceId && i.TenantId == InstallationConstants.Id,
                cancellationToken)
            .ConfigureAwait(false)
            ?? throw new NotFoundException($"Invoice {query.InvoiceId} not found.");

        return new InvoiceDto(
            invoice.Id,
            invoice.TenantId,
            invoice.InvoiceNumber,
            invoice.PeriodYear,
            invoice.PeriodMonth,
            invoice.Purpose,
            invoice.Status,
            invoice.Currency,
            invoice.Subtotal,
            invoice.Tax,
            invoice.Total,
            invoice.IssuedAtUtc,
            invoice.PaidAtUtc,
            invoice.CreatedAtUtc);
    }
}
